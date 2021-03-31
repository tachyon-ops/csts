using System;
using System.Collections.Generic;

using TypeScriptNative.Scan;
using TypeScriptNative.AST;

namespace TypeScriptNative.Parse
{
	public class Parser
	{

		private List<Token> tokens;
		private int current = 0;
		public Parser(List<Token> tokens)
		{
			this.tokens = tokens;
		}

		public List<Stmt> parse()
		{
			List<Stmt> statements = new List<Stmt>();
			while (!isAtEnd())
			{
				statements.Add(declaration());
			}
			return statements;
		}

		// expression     → equality ;
		private Expr expression()
		{
			return assignment();
		}

		private Stmt declaration()
		{
			try
			{
				if (match(TokenType.CLASS)) return classDeclaration();
				if (match(TokenType.FUN)) return function("function");
				if (match(TokenType.VAR)) return varDeclaration();

				return statement();
			}
			catch (ParseError error)
			{
				synchronize();
				return null;
			}
		}

		private Stmt classDeclaration()
		{
			Token name = consume(TokenType.IDENTIFIER, "Expect class name.");

			Variable superclass = null;
			if (match(TokenType.EXTENDS))
			{
				consume(TokenType.IDENTIFIER, "Expect superclass name.");
				superclass = new Variable(previous());
			}

			consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");

			List<Function> methods = new List<Function>();
			while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
			{
				methods.Add(function("method"));
			}

			consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");

			return new Class(name, superclass, methods);
		}

		private Stmt statement()
		{
			if (match(TokenType.FOR)) return forStatement();
			if (match(TokenType.IF)) return ifStatement();
			// if (match(TokenType.PRINT)) return printStatement();
			if (match(TokenType.RETURN)) return returnStatement();
			if (match(TokenType.WHILE)) return whileStatement();
			if (match(TokenType.LEFT_BRACE)) return new Block(block());
			return expressionStatement();
		}

		// desugaring: a process where the front end takes code using syntax sugar and translates it to a more
		// primitive form that the back end already knows how to execute
		private Stmt forStatement()
		{
			consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");
			Stmt initializer;
			if (match(TokenType.SEMICOLON))
			{
				initializer = null;
			}
			else if (match(TokenType.VAR))
			{
				initializer = varDeclaration();
			}
			else
			{
				initializer = expressionStatement();
			}

			Expr condition = null;
			if (!check(TokenType.SEMICOLON))
			{
				condition = expression();
			}
			consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

			Expr increment = null;
			if (!check(TokenType.RIGHT_PAREN))
			{
				increment = expression();
			}
			consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

			Stmt body = statement();

			if (increment != null)
			{
				body = new Block(
						new List<Stmt>() { body, new Expression(increment) });
			}

			if (condition == null) condition = new Literal(true);
			body = new While(condition, body);

			if (initializer != null)
			{
				body = new Block(new List<Stmt>() { initializer, body });
			}

			return body;
		}

		private Stmt ifStatement()
		{
			consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
			Expr condition = expression();
			consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

			Stmt thenBranch = statement();
			Stmt elseBranch = null;
			if (match(TokenType.ELSE))
			{
				elseBranch = statement();
			}

			return new If(condition, thenBranch, elseBranch);
		}

		private Stmt expressionStatement()
		{
			Expr expr = expression();
			consume(TokenType.SEMICOLON, "Expect ';' after expression.");
			return new Expression(expr);
		}

		private Function function(String kind)
		{
			Token name = consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
			consume(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name.");
			List<Token> parameters = new List<Token>();

			String functionType = null; // Prepare type

			if (!check(TokenType.RIGHT_PAREN))
			{
				do
				{
					if (parameters.Count >= 255)
					{
						error(peek(), "Can't have more than 255 parameters.");
					}

					Token paramIdentifier = consume(TokenType.IDENTIFIER, "Expect parameter name.");

					// Parameters TYPE consumption
					if (check(TokenType.COLON))
					{
						advance(); // consume COLON
						String paramType = consume(TokenType.IDENTIFIER, "Parameter type expected.").lexeme;
						Console.WriteLine("Function parameter '" + paramIdentifier.lexeme + "' type: " + paramType.ToString());
						paramIdentifier.typeDefinition = paramType;
					}
					parameters.Add(paramIdentifier);

				} while (match(TokenType.COMMA));
			}
			consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

			// Function TYPE consumption
			if (check(TokenType.COLON))
			{
				advance(); // consume COLON
				functionType = consume(TokenType.IDENTIFIER, "Function type expected.").lexeme;
				Console.WriteLine("Function '" + name.lexeme + "' type: " + functionType.ToString());
			}

			consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
			List<Stmt> body = block();
			return new Function(name, parameters, body, functionType);
		}

		private List<Stmt> block()
		{
			List<Stmt> statements = new List<Stmt>();

			while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
			{
				statements.Add(declaration());
			}

			consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
			return statements;
		}

		private Stmt returnStatement()
		{
			Token keyword = previous();
			Expr value = null;
			if (!check(TokenType.SEMICOLON))
			{
				value = expression();
			}

			consume(TokenType.SEMICOLON, "Expect ';' after return value.");
			return new Return(keyword, value);
		}

		private Stmt varDeclaration()
		{
			Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");

			// Variable TYPE consumption
			String typeDefinition = null;
			if (match(TokenType.COLON))
			{
				// Flabergasted as to why the next token would be the IDENTIFIER
				//advance(); // consume COLON
				typeDefinition = consume(TokenType.IDENTIFIER, "Expect type after colon in variable declaration.").lexeme;
				Console.WriteLine("Variable '" + name.lexeme + "' type: " + typeDefinition);
			}

			Expr initializer = null;
			if (match(TokenType.EQUAL))
			{
				initializer = expression();
			}

			consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
			return new MyVar(name, initializer, typeDefinition);
		}

		private Stmt whileStatement()
		{
			consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
			Expr condition = expression();
			consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
			Stmt body = statement();

			return new While(condition, body);
		}

		private Expr assignment()
		{
			Expr expr = or();

			if (match(TokenType.EQUAL))
			{
				Token equals = previous();
				Expr value = assignment();

				if (expr is Variable)
				{
					Token name = ((Variable)expr).name;
					return new Assign(name, value);
				}
				else if (expr is Get)
				{
					Get get = (Get)expr;
					return new Set(get.myObject, get.name, value);
				}

				error(equals, "Invalid assignment target.");
			}

			return expr;
		}

		private Expr or()
		{
			Expr expr = and();

			while (match(TokenType.OR))
			{
				Token myOperator = previous();
				Expr right = and();
				expr = new Logical(expr, myOperator, right);
			}

			return expr;
		}

		private Expr and()
		{
			Expr expr = equality();

			while (match(TokenType.AND))
			{
				Token myOperator = previous();
				Expr right = equality();
				expr = new Logical(expr, myOperator, right);
			}

			return expr;
		}

		private Expr equality()
		{
			Expr expr = comparison();
			while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
			{
				Token myOperator = previous();
				Expr right = comparison();
				expr = new Binary(expr, myOperator, right);
			}
			return expr;
		}

		private Expr comparison()
		{
			Expr expr = term();
			while (match(new List<TokenType>() {
				TokenType.GREATER,
				TokenType.GREATER_EQUAL,
				TokenType.LESS,
				TokenType.LESS_EQUAL
			}))
			{
				Token myOperator = previous();
				Expr right = term();
				expr = new Binary(expr, myOperator, right);
			}
			return expr;
		}

		private Expr term()
		{
			Expr expr = factor();
			while (match(TokenType.MINUS, TokenType.PLUS))
			{
				Token myOperator = previous();
				Expr right = factor();
				expr = new Binary(expr, myOperator, right);
			}
			return expr;
		}

		private Expr factor()
		{
			Expr expr = unary();
			while (match(TokenType.SLASH, TokenType.STAR))
			{
				Token myOperator = previous();
				Expr right = unary();
				expr = new Binary(expr, myOperator, right);
			}
			return expr;
		}

		private Expr unary()
		{
			if (match(TokenType.BANG, TokenType.MINUS))
			{
				Token myOperator = previous();
				Expr right = unary();
				return new Unary(myOperator, right);
			}
			return call();
		}

		private Expr call()
		{
			// TODO: impose that the identifier is indeed followed by a class instantiation
			if (match(TokenType.NEW))
			{
				// Console.WriteLine("NEW matched!");
			}

			Expr expr = primary();

			while (true)
			{
				if (match(TokenType.LEFT_PAREN))
				{
					expr = finishCall(expr);
				}
				else if (match(TokenType.DOT))
				{
					Token name = consume(TokenType.IDENTIFIER,
							"Expect property name after '.'.");
					expr = new Get(expr, name);
				}
				else
				{
					break;
				}
			}

			return expr;
		}

		private Expr finishCall(Expr callee)
		{
			List<Expr> arguments = new List<Expr>();
			if (!check(TokenType.RIGHT_PAREN))
			{
				do
				{
					if (arguments.Count >= 255)
					{
						error(peek(), "Can't have more than 255 arguments.");
					}
					arguments.Add(expression());
				} while (match(TokenType.COMMA));
			}

			Token paren = consume(TokenType.RIGHT_PAREN,
					"Expect ')' after arguments.");

			return new Call(callee, paren, arguments);
		}

		private Expr primary()
		{
			if (match(TokenType.FALSE)) return new Literal(false);
			if (match(TokenType.TRUE)) return new Literal(true);
			if (match(TokenType.NIL)) return new Literal(null);
			if (match(TokenType.NUMBER, TokenType.STRING)) return new Literal(previous().literal);

			if (match(TokenType.SUPER))
			{
				Token keyword = previous();
				consume(TokenType.DOT, "Expect '.' after 'super'.");
				Token method = consume(TokenType.IDENTIFIER,
						"Expect superclass method name.");
				return new Super(keyword, method);
			}

			if (match(TokenType.THIS)) return new This(previous());

			if (match(TokenType.IDENTIFIER))
			{
				return new Variable(previous());
			}

			if (match(TokenType.LEFT_PAREN))
			{
				Expr expr = expression();
				consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
				return new Grouping(expr);
			}

			throw error(peek(), "Expect expression.");
		}

		/**
		 * Utils
		 */
		private bool match(List<TokenType> types)
		{
			foreach (TokenType type in types)
			{
				if (check(type))
				{
					advance();
					return true;
				}
			}
			return false;
		}

		private bool match(TokenType type)
		{
			if (check(type))
			{
				advance();
				return true;
			}
			return false;
		}

		private bool match(TokenType type1, TokenType type2)
		{
			if (check(type1) || check(type2))
			{
				advance();
				return true;
			}
			return false;
		}

		private Token consume(TokenType type, String message)
		{
			if (check(type)) return advance();

			throw error(peek(), message);
		}

		private bool check(TokenType type)
		{
			if (isAtEnd()) return false;
			return peek().type == type;
		}

		private Token advance()
		{
			if (!isAtEnd()) current++;
			return previous();
		}

		private bool isAtEnd()
		{
			return peek().type == TokenType.EOF;
		}

		private Token peek()
		{
			return tokens[current];
		}

		private Token previous()
		{
			return tokens[current - 1];
		}

		private ParseError error(Token token, String message)
		{
			ErrorReport.error(token, message);
			return new ParseError();
		}

		private void synchronize()
		{
			advance();
			while (!isAtEnd())
			{
				if (previous().type == TokenType.SEMICOLON) return;
				switch (peek().type)
				{
					case TokenType.CLASS:
					case TokenType.FUN:
					case TokenType.VAR:
					case TokenType.FOR:
					case TokenType.IF:
					case TokenType.WHILE:
					// case PRINT:
					case TokenType.RETURN:
						return;
				}
				advance();
			}
		}

		private class ParseError : SystemException
		{
		}
	}
}