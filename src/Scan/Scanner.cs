using System;
using System.Collections.Generic;

namespace TypeScriptNative.Scan
{
	public class Scanner
	{
		private String source;
		private int start = 0;
		private int current = 0;
		private int line = 1;
		private List<Token> tokens = new List<Token>();
		private static Dictionary<String, TokenType> keywords = new Dictionary<String, TokenType>(){
			{"new", TokenType.NEW},
			{"class", TokenType.CLASS},
			{"extends", TokenType.EXTENDS},
			{"else", TokenType.ELSE},
			{"false", TokenType.FALSE},
			{"for", TokenType.FOR},
			{"function", TokenType.FUN},
			{"if", TokenType.IF},
			{"null", TokenType.NIL},
			{"return", TokenType.RETURN},
			{"super", TokenType.SUPER},
			{"this", TokenType.THIS},
			{"true", TokenType.TRUE},
			{"var", TokenType.VAR},
			{"const", TokenType.VAR},
			{"let", TokenType.VAR},
			{"while", TokenType.WHILE},
			{"interface", TokenType.INTERFACE},
			{"type", TokenType. TYPE},
			{"implements", TokenType.IMPLEMENTS},
		};

		internal void debug()
		{
			foreach (Token token in tokens)
			{
				Console.WriteLine(token.ToString());
			}
		}

		public Scanner(String source)
		{
			this.source = source;
		}

		public List<Token> scanTokens()
		{
			while (!isAtEnd())
			{
				// We are at the beginning of the next lexeme.
				start = current;
				scanToken();
			}

			tokens.Add(new Token(TokenType.EOF, "", null, line));
			return tokens;
		}

		private char advance()
		{
			return source[current++];
		}

		private void addToken(TokenType type)
		{
			addToken(type, null);
		}

		private void addToken(TokenType type, Object literal)
		{
			String text = substring(source);
			tokens.Add(new Token(type, text, literal, line));
		}

		private bool isAtEnd()
		{
			return current >= source.Length;
		}

		private void scanToken()
		{
			char c = advance();
			switch (c)
			{
				case '(': addToken(TokenType.LEFT_PAREN); break;
				case ')': addToken(TokenType.RIGHT_PAREN); break;
				case '{': addToken(TokenType.LEFT_BRACE); break;
				case '}': addToken(TokenType.RIGHT_BRACE); break;
				case ',': addToken(TokenType.COMMA); break;
				case '.': addToken(TokenType.DOT); break;
				case '-': addToken(TokenType.MINUS); break;
				case '+': addToken(TokenType.PLUS); break;
				case ';': addToken(TokenType.SEMICOLON); break;
				case ':': addToken(TokenType.COLON); break;
				case '*': addToken(TokenType.STAR); break;
				case '!':
					addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
					break;
				case '=':
					addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
					break;
				case '<':
					addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
					break;
				case '>':
					addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
					break;
				case '&':
					if (peek() == '&')
					{
						addToken(TokenType.AND);
					}
					break;
				case '|':
					if (peek() == '|')
					{
						addToken(TokenType.OR);
					}
					break;
				case '/':
					if (match('/'))
					{
						// A comment goes until the end of the line.
						while (peek() != '\n' && !isAtEnd()) advance();
					}
					else
					{
						addToken(TokenType.SLASH);
					}
					break;
				case ' ':
				case '\r':
				case '\t':
					// Ignore whitespace.
					break;
				case '\n':
					line++;
					break;
				case '"': myString(); break;
				default:
					if (isDigit(c))
					{
						number();
					}
					else if (isAlpha(c))
					{
						identifier();
					}
					else
					{
						ErrorReport.error(line, "Unexpected character.");
					}
					break;
			}


		}

		private void identifier()
		{
			while (isAlphaNumeric(peek())) advance();
			String text = substring(source);
			TokenType token;
			bool gotValue = keywords.TryGetValue(text, out token);
			if (!gotValue) token = TokenType.IDENTIFIER;
			addToken(token);
		}

		private void number()
		{
			while (isDigit(peek())) advance();

			// Look for a fractional part.
			if (peek() == '.' && isDigit(peekNext()))
			{
				// Consume the "."
				advance();

				while (isDigit(peek())) advance();
			}

			addToken(TokenType.NUMBER,
					Double.Parse(substring(source)));
		}

		private String substring(String _str)
		{
			return source.Substring(start, current - start);
		}

		private void myString()
		{
			while (peek() != '"' && !isAtEnd())
			{
				if (peek() == '\n') line++;
				advance();
			}

			if (isAtEnd())
			{
				ErrorReport.error(line, "Unterminated string.");
				return;
			}

			// The closing ".
			advance();

			// Trim the surrounding quotes.
			String value = substring(source);
			addToken(TokenType.STRING, value);
		}

		private bool match(char expected)
		{
			if (isAtEnd()) return false;
			if (source[current] != expected) return false;

			current++;
			return true;
		}

		private char peek()
		{
			if (isAtEnd()) return '\0';
			return source[current];
		}

		private char peekNext()
		{
			if (current + 1 >= source.Length) return '\0';
			return source[current + 1];
		}

		private bool isAlpha(char c)
		{
			return (c >= 'a' && c <= 'z') ||
					(c >= 'A' && c <= 'Z') ||
					c == '_';
		}

		private bool isAlphaNumeric(char c)
		{
			return isAlpha(c) || isDigit(c);
		}

		private bool isDigit(char c)
		{
			return c >= '0' && c <= '9';
		}
	}
}
