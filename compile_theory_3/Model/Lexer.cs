using compile_theory_3.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_3.Model
{
	enum TokenKind
	{
		E,
		ADD,
		SUB,
		MULT,
		DIV,
		ID,
		LPAR,
		RPAR,
		NUM,
		ERROR,
		END
	};

	enum ErrorKind
	{
		UNNEEDBRA,
		INVALIDCHAR,
		NUMBERERROR,
		NESTBRA,
		ANNONOTCLOSED,
		DEFAULT
	};

	class Lexer
	{
		static HashSet<char> symbols = new HashSet<char>
		{ '+', '-', '*', '/', '(', ')', '_' };
		
		static public Token LexNext()
		{
			int state = 0;
			string value = string.Empty;
			int startOffset = 0;
			ErrorKind errorKind = ErrorKind.DEFAULT;
			char? cQuestion;
			char c;
			while (true)
			{
				cQuestion = SourceViewModel.NextChar();
				if(cQuestion.HasValue)
				{
					c = cQuestion.Value;
					switch (state)
					{
						case 0:
							startOffset = SourceViewModel.GetOffset();
							if (char.IsLetter(c) || c == '_')
							{
								state = 1;
								value += c;
								break;
							}

							if (char.IsWhiteSpace(c))
							{
								break;
							}

							if (char.IsDigit(c))
							{
								state = 2;
								value += c;
								break;
							}

							if (c == '+')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.ADD); ;
							}

							if (c == '-')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.SUB);
							}

							if (c == '*')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.MULT);
							}

							if (c == '/')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.DIV);
							}

							if (c == '(')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.LPAR);
							}

							if (c == ')')
							{
								value = c.ToString();
								return new Token(startOffset, value, TokenKind.RPAR);
							}

							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = ErrorKind.NUMBERERROR;
								break;
							}

							//ERROR
							state = 4;
							value += c;
							errorKind = ErrorKind.INVALIDCHAR;
							break;

						case 1:
							if (char.IsLetterOrDigit(c))
							{
								value += c;
								break;
							}

							SourceViewModel.putBack();
							return new Token(startOffset, value, TokenKind.ID);

						case 2:
							if (char.IsDigit(c))
							{
								value += c;
								break;
							}

							if (c == '.')
							{
								state = 5;
								value += c;
								break;
							}

							SourceViewModel.putBack();
							return new Token(startOffset, value, TokenKind.NUM);

						case 4:
							switch (errorKind)
							{
								case ErrorKind.NUMBERERROR:
									if (char.IsDigit(c) || c == '.')
									{
										value += c;
									}
									else
									{
										SourceViewModel.putBack();
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									break;
								case ErrorKind.INVALIDCHAR:
									if (char.IsLetterOrDigit(c) || symbols.Contains(c) || char.IsWhiteSpace(c))
									{
										SourceViewModel.putBack();
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									else
									{
										value += c;
									}
									break;
								default:
									if (char.IsWhiteSpace(c))
									{
										HandleError(startOffset, value, errorKind);
										return new Token(startOffset, value, TokenKind.ERROR);
									}
									else
									{
										value += c;
									}
									break;
							}
							break;

						case 5:
							if (char.IsDigit(c))
							{
								state = 6;
								value += c;
								break;
							}

							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = ErrorKind.NUMBERERROR;
								break;
							}

							SourceViewModel.putBack();

							HandleError(startOffset, value, ErrorKind.NUMBERERROR);
							return new Token(startOffset, value, TokenKind.ERROR);

						case 6:
							if (char.IsDigit(c))
							{
								state = 6;
								value += c;
								break;
							}

							if (c == '.')
							{
								state = 4;
								value += c;
								errorKind = ErrorKind.NUMBERERROR;
								break;
							}

							SourceViewModel.putBack();
							return new Token(startOffset, value, TokenKind.NUM);

					}
				}
				else
				{
					switch (state)
					{
						case 0:
							return new Token(startOffset, value, TokenKind.END);
						case 1:
							return new Token(startOffset, value, TokenKind.ID);
						case 2:
						case 6:
							return new Token(startOffset, value, TokenKind.NUM);
						case 4:
							HandleError(startOffset, value, errorKind);
							return new Token(startOffset, value, TokenKind.ERROR);
						case 5:
							HandleError(startOffset, value, ErrorKind.NUMBERERROR);
							return new Token(startOffset, value, TokenKind.ERROR);
					}
				}
			}
		}

		static private void HandleError(int eOffset, string eValue, ErrorKind ekind)
		{
			Error e = new Error();
			e.line = SourceViewModel.GetLine(eOffset);
			e.lineOffset = SourceViewModel.GetLineOffset(eOffset);
			e.length = eValue.Length;
			e.kind = ekind;
			switch (ekind)
			{
				case ErrorKind.ANNONOTCLOSED:
					e.infomation = "注释未闭合";
					break;
				case ErrorKind.NUMBERERROR:
					e.infomation = string.Format("数字 {0} 格式错误", eValue);
					break;
				case ErrorKind.INVALIDCHAR:
					e.infomation = string.Format("无法识别的字符: {0}", eValue);
					break;
				case ErrorKind.NESTBRA:
					e.infomation = "出现嵌套的注释";
					break;
				default:
					e.infomation = string.Format("未知类型错误: {0}", eValue);
					break;
			}
		}

		static public void Reset()
		{
			SourceViewModel.Reset();
		}

		static public void SetLexPosition(int offset)
		{
			SourceViewModel.SetOffset(offset - 1);
		}

		static public void preLex()
		{
			Reset();
			var t = LexNext();

			HashSet<string> varset = new HashSet<string>();

			while (t.kind != TokenKind.END)
			{
				if(t.kind == TokenKind.ID)
				{
					varset.Add(t.value);
				}
				t = LexNext();
			}

			VariableSetViewModel.Reset();

			foreach (var n in varset)
			{
				VariableSetViewModel.AddVariable(n);
			}
			VariableSetViewModel.SetValue();
			Reset();
		}

		static public void Test()
		{
			ProcessViewModel.Clear();
			Reset();
			var t = LexNext();
			while (t.kind != TokenKind.END)
			{
				ProcessViewModel.Add(new Process(t.kind.ToString(), "值 " , t.value));
				t = LexNext();
			}
		}
	}
}
