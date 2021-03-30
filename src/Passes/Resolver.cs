using System;
using System.Collections.Generic;
using TypeScriptNative.Scan;
using TypeScriptNative.AST;
using TypeScriptNative.Interpret;

namespace TypeScriptNative.Passes
{
	public class Resolver : Expr.Visitor<Object>, Stmt.Visitor<Object>
	{
		private Interpreter interpreter;
		private Stack<Dictionary<String, Boolean>> scopes =
			new Stack<Dictionary<String, Boolean>>();
		private FunctionType currentFunction = FunctionType.NONE;
		private ClassType currentClass = ClassType.NONE;

		public Resolver(Interpreter interpreter)
		{
			this.interpreter = interpreter;
		}


		public Object visitBlockStmt(Block stmt)
		{
			beginScope();
			resolve(stmt.statements);
			endScope();
			return null;
		}

		//	@Override
		public Object visitClassStmt(Class stmt)
		{
			ClassType enclosingClass = currentClass;
			currentClass = ClassType.CLASS;

			declare(stmt.name);
			define(stmt.name);

			if (stmt.superclass != null &&
					stmt.name.lexeme.Equals(stmt.superclass.name.lexeme))
			{
				ErrorReport.error(stmt.superclass.name,
						"A class can't inherit from itself.");
			}

			if (stmt.superclass != null)
			{
				currentClass = ClassType.SUBCLASS;
				resolve(stmt.superclass);
			}

			if (stmt.superclass != null)
			{
				beginScope();
				scopes.Peek().Add("super", true);
			}

			beginScope();
			scopes.Peek().Add("this", true);

			foreach (Function method in stmt.methods)
			{
				FunctionType declaration = FunctionType.METHOD;
				if (method.name.lexeme.Equals("init"))
				{
					declaration = FunctionType.INITIALIZER;
				}
				resolveFunction(method, declaration);
			}

			endScope();

			if (stmt.superclass != null) endScope();
			currentClass = enclosingClass;
			return null;
		}

		//	@Override
		public Object visitExpressionStmt(Expression stmt)
		{
			resolve(stmt.expression);
			return null;
		}

		//	@Override
		public Object visitFunctionStmt(Function stmt)
		{
			declare(stmt.name);
			define(stmt.name);

			resolveFunction(stmt, FunctionType.FUNCTION);
			return null;
		}

		//	@Override
		public Object visitIfStmt(If stmt)
		{
			resolve(stmt.condition);
			resolve(stmt.thenBranch);
			if (stmt.elseBranch != null) resolve(stmt.elseBranch);
			return null;
		}

		//	@Override
		public Object visitReturnStmt(Return stmt)
		{
			if (currentFunction == FunctionType.NONE)
			{
				ErrorReport.error(stmt.keyword, "Can't return from top-level code.");
			}

			if (stmt.value != null)
			{
				if (currentFunction == FunctionType.INITIALIZER)
				{
					ErrorReport.error(stmt.keyword,
							"Can't return a value from an initializer.");
				}
				resolve(stmt.value);
			}

			return null;
		}

		//	@Override
		public Object visitVarStmt(MyVar stmt)
		{
			declare(stmt.name);
			if (stmt.initializer != null)
			{
				resolve(stmt.initializer);
			}
			define(stmt.name);
			return null;
		}

		//	//	@Override
		//	//	public Void visitPrintStmt(Stmt.Print stmt) {
		//	//		resolve(stmt.expression);
		//	//		return null;
		//	//	}

		//	@Override
		public Object visitWhileStmt(While stmt)
		{
			resolve(stmt.condition);
			resolve(stmt.body);
			return null;
		}

		//	@Override
		public Object visitAssignExpr(Assign expr)
		{
			resolve(expr.value);
			resolveLocal(expr, expr.name);
			return null;
		}

		//	@Override
		public Object visitBinaryExpr(Binary expr)
		{
			resolve(expr.left);
			resolve(expr.right);
			return null;
		}

		//	@Override
		public Object visitCallExpr(Call expr)
		{
			resolve(expr.callee);

			foreach (Expr argument in expr.arguments)
			{
				resolve(argument);
			}

			return null;
		}

		//	@Override
		public Object visitGetExpr(Get expr)
		{
			resolve(expr.myObject);
			return null;
		}

		//	@Override
		public Object visitGroupingExpr(Grouping expr)
		{
			resolve(expr.expression);
			return null;
		}

		//	@Override
		public Object visitLiteralExpr(Literal expr)
		{
			return null;
		}

		//	@Override
		public Object visitLogicalExpr(Logical expr)
		{
			resolve(expr.left);
			resolve(expr.right);
			return null;
		}

		//	@Override
		public Object visitSetExpr(Set expr)
		{
			resolve(expr.value);
			resolve(expr.myObject);
			return null;
		}

		//	@Override
		public Object visitSuperExpr(Super expr)
		{
			if (currentClass == ClassType.NONE)
			{
				ErrorReport.error(expr.keyword,
						"Can't use 'super' outside of a class.");
			}
			else if (currentClass != ClassType.SUBCLASS)
			{
				ErrorReport.error(expr.keyword,
						"Can't use 'super' in a class with no superclass.");
			}
			resolveLocal(expr, expr.keyword);
			return null;
		}

		//	@Override
		public Object visitThisExpr(This expr)
		{
			if (currentClass == ClassType.NONE)
			{
				ErrorReport.error(expr.keyword,
						"Can't use 'this' outside of a class.");
				return null;
			}

			resolveLocal(expr, expr.keyword);
			return null;
		}

		//	@Override
		public Object visitUnaryExpr(Unary expr)
		{
			resolve(expr.right);
			return null;
		}

		//	@Override
		public Object visitVariableExpr(Variable expr)
		{
			if (scopes.Count != 0 && scopes.Peek()[expr.name.lexeme] == false)
			{
				ErrorReport.error(expr.name,
						"Can't read local variable in its own initializer.");
			}

			resolveLocal(expr, expr.name);
			return null;
		}

		// Helper methods
		public void resolve(List<Stmt> statements)
		{
			foreach (Stmt statement in statements)
			{
				resolve(statement);
			}
		}

		private void resolve(Stmt stmt)
		{
			stmt.accept(this as Stmt.Visitor<Object>);
		}

		private void resolve(Expr expr)
		{
			expr.accept(this);
		}

		private void resolveFunction(Function function, FunctionType type)
		{
			FunctionType enclosingFunction = currentFunction;
			currentFunction = type;
			beginScope();
			foreach (Token param in function.myParams)
			{
				declare(param);
				define(param);
			}
			resolve(function.body);
			endScope();
			currentFunction = enclosingFunction;
		}

		private void beginScope()
		{
			scopes.Push(new Dictionary<String, Boolean>());
		}

		private void endScope()
		{
			scopes.Pop();
		}

		private void declare(Token name)
		{
			if (scopes.Count == 0) return;

			Dictionary<String, Boolean> scope = scopes.Peek();
			if (scope.ContainsKey(name.lexeme))
			{
				ErrorReport.error(name,
						"Already variable with this name in this scope.");
			}
			scope.Add(name.lexeme, false);
		}

		private void define(Token name)
		{
			if (scopes.Count == 0) return;
			scopes.Peek()[name.lexeme] = true; // was peek().put()
		}

		private void resolveLocal(Expr expr, Token name)
		{
			for (int i = scopes.Count - 1; i >= 0; i--)
			{
				if (scopes.ToArray()[i].ContainsKey(name.lexeme))
				{
					interpreter.resolve(expr, scopes.Count - 1 - i);
					return;
				}
			}
		}


		private enum FunctionType
		{
			NONE,
			FUNCTION,
			INITIALIZER,
			METHOD
		}

		private enum ClassType
		{
			NONE,
			CLASS,
			SUBCLASS
		}

	}
}
