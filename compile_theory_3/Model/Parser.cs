using compile_theory_3.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace compile_theory_3.Model
{
	enum SymbolKind
	{
		E,
		ADD,
		SUB,
		MULT,
		DIV,
		LPAR,
		RPAR,
		ID,
		ALL
	}

	class Parser
	{
		static private Stack<TokenKind> parseStack = new Stack<TokenKind>();
		static private Stack<Token> tokenStack = new Stack<Token>();
		static private Stack<Process> processStack = new Stack<Process>();
		static private Token token;
		static private bool enableNumbers = false;

		public static bool EnableNumbers
		{
			get
			{
				return enableNumbers;
			}

			set
			{
				enableNumbers = value;
			}
		}

		static private double? Calculate(double? a, double? b, TokenKind t)
		{
			if(a.HasValue && b.HasValue)
			{
				switch (t)
				{
					case TokenKind.ADD:
						return a + b;
					case TokenKind.SUB:
						return a - b;
					case TokenKind.MULT:
						return a * b;
					case TokenKind.DIV:
						if(b == 0)
						{
							return null;
						}
						return a / b;
					default:
						return null;
				}
			}
			return null;
		}

		static private bool Reduce()
		{
			Token temp;
			Token temp1;
			Token temp2;
			Process proc;

			switch (tokenStack.Peek().kind)
			{
				case TokenKind.ID:
					temp = tokenStack.Pop();

					proc = new Process("E", "E -> ID",  temp.value);

					temp.kind = TokenKind.E;
					temp.dvalue = VariableSetViewModel.getValue(temp.value);

					proc.addDetail(new Process("ID", temp.value));
					processStack.Push(proc);

					tokenStack.Push(temp);
					parseStack.Pop();
					break;
				case TokenKind.E:
					temp = tokenStack.Pop();
					temp1 = tokenStack.Pop();
					temp2 = temp1;
					if(tokenStack.Count > 0)
					{
						temp2 = tokenStack.Pop();
					}
					switch (temp1.kind)
					{
						case TokenKind.ADD:
						case TokenKind.MULT:
						case TokenKind.DIV:
							switch(temp2.kind)
							{
								case TokenKind.E:
									temp2.value += temp1.value + temp.value;
									temp2.length = temp.offset - temp2.offset + temp2.length;
									
									if(temp1.kind == TokenKind.DIV && temp.dvalue == 0)
									{
										ErrorHandle(temp, "除数为零");
										return false;
									}
									else
									{
										temp2.dvalue = Calculate(temp2.dvalue, temp.dvalue, temp1.kind);
										if (!temp2.dvalue.HasValue)
										{
											ErrorHandle(temp2, "计算出错");
											return false;
										}
									}

									proc = new Process("E", "E -> E " + TokenKindToString(temp1.kind) + " E", temp2.dvalue.ToString());
									var t = processStack.Pop();
									proc.addDetail(processStack.Pop());
									proc.addDetail(TokenKindToString(temp1.kind));
									proc.addDetail(t);
									processStack.Push(proc);

									tokenStack.Push(temp2);
									parseStack.Pop();
									break;
								default:
									ErrorHandle(temp2, "未知错误");
									return false;
							}
							break;
						case TokenKind.SUB:
							switch (temp2.kind)
							{
								case TokenKind.E:
									temp2.value += temp1.value + temp.value;
									temp2.length = temp.offset - temp2.offset + temp.length;

									temp2.dvalue = Calculate(temp2.dvalue, temp.dvalue, temp1.kind);
									if (!temp2.dvalue.HasValue)
									{
										ErrorHandle(temp2, "计算出错");
										return false;
									}

									proc = new Process("E", "E -> E " + TokenKindToString(temp1.kind) + " E", temp2.dvalue.ToString());
									var t = processStack.Pop();
									proc.addDetail(processStack.Pop());
									proc.addDetail(TokenKindToString(temp1.kind));
									proc.addDetail(t);
									processStack.Push(proc);

									tokenStack.Push(temp2);
									break;
								case TokenKind.ADD:
								case TokenKind.SUB:
								case TokenKind.MULT:
								case TokenKind.DIV:
								case TokenKind.LPAR:
									tokenStack.Push(temp2);

									

									temp1.value += temp.value;
									temp1.length = temp.offset - temp1.offset + temp.length;

									temp1.dvalue = Calculate(0.0, temp.dvalue, temp1.kind);
									if (!temp1.dvalue.HasValue)
									{
										ErrorHandle(temp1, "计算出错");
										return false;
									}

									temp1.kind = TokenKind.E;

									proc = new Process("E", "E -> - E", temp1.dvalue.ToString());
									proc.addDetail("-");
									proc.addDetail(processStack.Pop());
									processStack.Push(proc);

									tokenStack.Push(temp1);
									break;
								
								default:
									ErrorHandle(temp2, "未知错误");
									return false;
							}
							parseStack.Pop();
							break;
					}
					break;

				case TokenKind.RPAR:
					temp = tokenStack.Pop();
					temp1 = tokenStack.Pop();
					temp2 = tokenStack.Pop();
					
					temp2.value += temp1.value + temp.value;
					temp2.length = temp.offset - temp2.offset + temp2.length;
					temp2.kind = TokenKind.E;

					temp2.dvalue = temp1.dvalue;

					proc = new Process("E", "E -> ( E )", temp2.dvalue.ToString());
					proc.addDetail("(");
					proc.addDetail(processStack.Pop());
					proc.addDetail(")");
					processStack.Push(proc);

					tokenStack.Push(temp2);
					parseStack.Pop();
					parseStack.Pop();
					break;

				case TokenKind.ADD:
				case TokenKind.SUB:
				case TokenKind.MULT:
				case TokenKind.DIV:
					if(token.kind == TokenKind.SUB)
					{
						parseStack.Push(token.kind);
						tokenStack.Push(token);
						readToken();
					}
					else
					{
						if(token.kind != TokenKind.END)
						{
							ErrorHandle(token, "重复的运算符");
						}
						else
						{
							ErrorHandle(tokenStack.Peek(), "异常的结尾");
						}
						return false;
					}
					break;

				default:
					ErrorHandle(tokenStack.Peek(), "异常的结尾");
					return false;
			}
			return true;
		}



		static public void parse()
		{
			if (!VariableSetViewModel.IsSetVariable)
			{
				var res = MessageBox.Show("您还没有设置每一个 ID 的初值, 是否要进行设置", "警告", MessageBoxButton.YesNo);
				if(res == MessageBoxResult.Yes)
				{
					Lexer.preLex();
					parse();
					return;
				}
			}

			SourceViewModel.KeepOnlyRead();
			Reset();

			parseStack.Push(TokenKind.END);

			bool result = false;
			Lexer.Reset();
			readToken();
			while (true)
			{
				if (token != null)
				{
					if (token.kind == TokenKind.END && parseStack.Peek() == TokenKind.END)
					{
						result = true;
						break;
					}
					else if ((PriorityTableViewModel.Compare(parseStack.Peek(), token.kind) & (CompareType.LT | CompareType.EQU)) != CompareType.NULL)
					{
						parseStack.Push(token.kind);
						tokenStack.Push(token);
						readToken();
					}
					else if (PriorityTableViewModel.Compare(parseStack.Peek(), token.kind) == CompareType.GT)
					{
						if (!Reduce())
						{
							break;
						}
					}
					else if (PriorityTableViewModel.Compare(parseStack.Peek(), token.kind) == CompareType.NULL)
					{
						if(token.kind == TokenKind.NUM)
						{
							ErrorHandle(token, "未开启允许数字选项");
						}
						else
						{
							ErrorHandle(token, string.Format("意外的字符 {0}", token.value));
						}
						break;
					}
				}
				else
				{
					break;
				}
			}

			if (result)
			{
				if (tokenStack.Count > 0)
				{
					StateViewModel.Display("成功	| 结果: " + tokenStack.Peek().dvalue.ToString());
					ProcessViewModel.Add(processStack.Peek());
				}
			}
			else
			{
				StateViewModel.Display("失败");
			}

			SourceViewModel.UnkeepOnlyRead();
		}

		private static void readToken()
		{
			token = Lexer.LexNext();
			if (enableNumbers && token.kind == TokenKind.NUM)
			{
				token.kind = TokenKind.ID;
			}
		}

		static private void Reset()
		{
			ProcessViewModel.Clear();
			ErrorViewModel.getInstance().clear();
			processStack.Clear();
			parseStack.Clear();
			tokenStack.Clear();
		}

		static private string TokenKindToString(TokenKind s)
		{
			switch (s)
			{
				case TokenKind.ADD:
					return "+ ";
				case TokenKind.SUB:
					return "- ";
				case TokenKind.MULT:
					return "* ";
				case TokenKind.DIV:
					return "/ ";
				case TokenKind.LPAR:
					return "( ";
				case TokenKind.RPAR:
					return ") ";
				default:
					return s.ToString() + " ";
			}
		}
		
		static private void ErrorHandle(Token t, string info)
		{
			Error err = new Error();
			err.line = SourceViewModel.GetLine(t.offset);
			err.lineOffset = SourceViewModel.GetLineOffset(t.offset);
			err.length = t.length;
			err.infomation = info;
			ErrorViewModel.getInstance().addError(err);
		}
	}
}
