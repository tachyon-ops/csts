using System;
namespace TypeScriptNative.Scan
{
	public enum TokenType
	{
		// Single-character tokens.
		LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
		COMMA, DOT, MINUS, PLUS, SEMICOLON, COLON, SLASH, STAR,

		// One or two character tokens.
		BANG, BANG_EQUAL,
		EQUAL, EQUAL_EQUAL,
		GREATER, GREATER_EQUAL,
		LESS, LESS_EQUAL,

		// Keywords.
		AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR, EXTENDS,
		RETURN, SUPER, THIS, TRUE, VAR, CONST, LET, WHILE,
		//PRINT,

		// Literals.
		IDENTIFIER, STRING, NUMBER,

		// Types
		INTERFACE, TYPE, IMPLEMENTS,

		EOF,
	}
}
