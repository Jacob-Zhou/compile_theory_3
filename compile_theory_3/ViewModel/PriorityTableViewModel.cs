using compile_theory_3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_3.ViewModel
{
	enum CompareType
	{
		GT = 1,
		LT = 2,
		EQU = 4,
		NULL = 0
	}
	class PriorityTableViewModel
	{
		static public CompareType Compare(TokenKind inStack, TokenKind inBuffer)
		{
			switch (inStack)
			{
				case TokenKind.ADD:
					switch (inBuffer)
					{
						case TokenKind.RPAR:
						case TokenKind.END:
						case TokenKind.ADD:
						case TokenKind.SUB:
							return CompareType.GT;
						case TokenKind.MULT:
						case TokenKind.DIV:
						case TokenKind.ID:
						case TokenKind.LPAR:
							return CompareType.LT;
						default:
							return CompareType.NULL;
					}
				case TokenKind.SUB:
					switch (inBuffer)
					{
						case TokenKind.RPAR:
						case TokenKind.END:
						case TokenKind.ADD:
						case TokenKind.SUB:
							return CompareType.GT;
						case TokenKind.MULT:
						case TokenKind.DIV:
						case TokenKind.ID:
						case TokenKind.LPAR:
							return CompareType.LT;
						default:
							return CompareType.NULL;
					}
				case TokenKind.MULT:
					switch (inBuffer)
					{
						case TokenKind.ADD:
						case TokenKind.SUB:
						case TokenKind.MULT:
						case TokenKind.DIV:
						case TokenKind.RPAR:
						case TokenKind.END:
							return CompareType.GT;
						case TokenKind.ID:
						case TokenKind.LPAR:
							return CompareType.LT;
						default:
							return CompareType.NULL;
					}
				case TokenKind.DIV:
					switch (inBuffer)
					{
						case TokenKind.ADD:
						case TokenKind.SUB:
						case TokenKind.MULT:
						case TokenKind.DIV:
						case TokenKind.RPAR:
						case TokenKind.END:
							return CompareType.GT;
						case TokenKind.ID:
						case TokenKind.LPAR:
							return CompareType.LT;
						default:
							return CompareType.NULL;
					}
				case TokenKind.ID:
					switch (inBuffer)
					{
						case TokenKind.ADD:
						case TokenKind.SUB:
						case TokenKind.MULT:
						case TokenKind.DIV:
						case TokenKind.RPAR:
						case TokenKind.END:
							return CompareType.GT;
						default:
							return CompareType.NULL;
					}
				case TokenKind.LPAR:
					switch (inBuffer)
					{
						case TokenKind.ADD:
						case TokenKind.SUB:
						case TokenKind.MULT:
						case TokenKind.DIV:
						case TokenKind.ID:
						case TokenKind.LPAR:
							return CompareType.LT;
						case TokenKind.RPAR:
							return CompareType.EQU;
						default:
							return CompareType.NULL;
					}
				case TokenKind.RPAR:
					switch (inBuffer)
					{
						case TokenKind.ADD:
						case TokenKind.SUB:
						case TokenKind.MULT:
						case TokenKind.DIV:
						case TokenKind.RPAR:
						case TokenKind.END:
							return CompareType.GT;
						default:
							return CompareType.NULL;
					}
				case TokenKind.END:
					switch (inBuffer)
					{
						case TokenKind.ADD:
						case TokenKind.SUB:
						case TokenKind.MULT:
						case TokenKind.DIV:
						case TokenKind.ID:
						case TokenKind.LPAR:
							return CompareType.LT;
						default:
							return CompareType.NULL;
					}

				default:
					return CompareType.NULL;
			}
		}
	}
}
