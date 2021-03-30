using System;
using System.Collections.Generic;
using TypeScriptNative.AST;
using TypeScriptNative.globals;
using TypeScriptNative.Scan;
using TypeScriptNative.language;

namespace TypeScriptNative.Interpret
{
	public class Interpreter : Expr.Visitor<Object>, Stmt.Visitor<Object>
	{
		MyEnvironment globals = new MyEnvironment();
		// private Dictionary<Expr, int> locals = new Dictionary<Expr, int>();
		private Dictionary<Expr, int> locals = new Dictionary<Expr, int>();
		private MyEnvironment environment;

		public Interpreter()
		{
			this.globals.define("clock", new Clock());
			this.globals.define("print", new Print());
			this.globals.define("println", new Println());

			this.environment = this.globals;
		}

		public void interpret(List<Stmt> statements)
		{
			try
			{
				foreach (Stmt statement in statements)
				{
					execute(statement);
				}
			}
			catch (RuntimeError error)
			{
				ErrorReport.runtimeError(error);
			}
		}

		// Expressions Visitor
		object Expr.Visitor<object>.visitAssignExpr(Assign expr)
		{
			Object value = evaluate(expr.value);

			int distance = locals[expr];
			if (!distance.Equals(null))
			{
				environment.assignAt(distance, expr.name, value);
			}
			else
			{
				globals.assign(expr.name, value);
			}

			return value;
		}

		object Expr.Visitor<object>.visitBinaryExpr(Binary expr)
		{
			Object left = evaluate(expr.left);
			Object right = evaluate(expr.right);

			switch (expr.myOperator.type)
			{
				case TokenType.GREATER:
					checkNumberOperands(expr.myOperator, left, right);
					return (double)left > (double)right;
				case TokenType.GREATER_EQUAL:
					checkNumberOperands(expr.myOperator, left, right);
					return (double)left >= (double)right;
				case TokenType.LESS:
					checkNumberOperands(expr.myOperator, left, right);
					return (double)left < (double)right;
				case TokenType.LESS_EQUAL:
					checkNumberOperands(expr.myOperator, left, right);
					return (double)left <= (double)right;
				case TokenType.MINUS:
					checkNumberOperands(expr.myOperator, left, right);
					return (double)left - (double)right;
				case TokenType.PLUS:
					if (left is Double && right is Double)
					{
						return (double)left + (double)right;
					}

					if (left is String && right is String)
					{
						return (String)left + (String)right;
					}

					throw new RuntimeError(expr.myOperator,
							"Operands must be two numbers or two strings.");
				case TokenType.SLASH:
					checkNumberOperands(expr.myOperator, left, right);
					return (double)left / (double)right;
				case TokenType.STAR:
					checkNumberOperands(expr.myOperator, left, right);
					return (double)left * (double)right;
				case TokenType.BANG_EQUAL:
					return !isEqual(left, right);
				case TokenType.EQUAL_EQUAL:
					return isEqual(left, right);
			}

			// Unreachable.
			return null;
		}

		object Expr.Visitor<object>.visitCallExpr(Call expr)
		{
			Object callee = evaluate(expr.callee);

			List<Object> arguments = new List<Object>();
			foreach (Expr argument in expr.arguments)
			{
				arguments.Add(evaluate(argument));
			}

			if (!(callee is TypeScriptNativeCallable))
			{
				throw new RuntimeError(expr.paren,
						"Can only call functions and classes.");
			}
			TypeScriptNativeCallable function = (TypeScriptNativeCallable)callee;
			if (arguments.Count != function.arity())
			{
				throw new RuntimeError(expr.paren, "Expected " +
						function.arity() + " arguments but got " +
						arguments.Count + ".");
			}
			return function.call(this, arguments);
		}

		object Expr.Visitor<object>.visitGetExpr(Get expr)
		{
			Object myObject = evaluate(expr.myObject);
			if (myObject is TypeScriptNativeInstance)
			{
				return ((TypeScriptNativeInstance)myObject).get(expr.name);
			}

			throw new RuntimeError(expr.name,
					"Only instances have properties.");
		}

		object Expr.Visitor<object>.visitGroupingExpr(Grouping expr)
		{
			return evaluate(expr.expression);
		}

		object Expr.Visitor<object>.visitLiteralExpr(Literal expr)
		{
			return expr.value;
		}

		object Expr.Visitor<object>.visitLogicalExpr(Logical expr)
		{
			Object left = evaluate(expr.left);

			if (expr.myOperator.type == TokenType.OR)
			{
				if (isTruthy(left)) return left;
			}
			else
			{
				if (!isTruthy(left)) return left;
			}

			return evaluate(expr.right);
		}

		object Expr.Visitor<object>.visitSetExpr(Set expr)
		{
			Object myObject = evaluate(expr.myObject);

			if (!(myObject is TypeScriptNativeInstance))
			{
				throw new RuntimeError(expr.name,
						"Only instances have fields.");
			}

			Object value = evaluate(expr.value);
			((TypeScriptNativeInstance)myObject).set(expr.name, value);
			return value;
		}

		object Expr.Visitor<object>.visitSuperExpr(Super expr)
		{
			int distance = locals[expr];
			TypeScriptNativeClass superclass =
				(TypeScriptNativeClass)environment.getAt(distance, "super");
			TypeScriptNativeInstance myObject =
				(TypeScriptNativeInstance)environment.getAt(distance - 1, "this");
			TypeScriptNativeFunction method =
				superclass.findMethod(expr.method.lexeme);
			if (method == null)
			{
				throw new RuntimeError(expr.method,
						"Undefined property '" + expr.method.lexeme + "'.");
			}
			return method.bind(myObject);
		}

		object Expr.Visitor<object>.visitThisExpr(This expr)
		{
			return lookUpVariable(expr.keyword, expr);
		}

		object Expr.Visitor<object>.visitUnaryExpr(Unary expr)
		{
			Object right = evaluate(expr.right);
			switch (expr.myOperator.type)
			{
				case TokenType.BANG:
					return !isTruthy(right);
				case TokenType.MINUS:
					checkNumberOperand(expr.myOperator, right);
					return -(double)right;
			}
			// Unreachable.
			return null;
		}

		object Expr.Visitor<object>.visitVariableExpr(Variable expr)
		{
			return lookUpVariable(expr.name, expr);
		}

		// Statements Visitor
		object Stmt.Visitor<object>.visitBlockStmt(Block stmt)
		{
			executeBlock(stmt.statements, new MyEnvironment(environment));
			return null;
		}

		object Stmt.Visitor<object>.visitClassStmt(Class stmt)
		{
			Object superclass = null;
			if (stmt.superclass != null)
			{
				superclass = evaluate(stmt.superclass);
				if (!(superclass is TypeScriptNativeClass))
				{
					throw new RuntimeError(stmt.superclass.name,
							"Superclass must be a class.");
				}
			}

			environment.define(stmt.name.lexeme, null);

			if (stmt.superclass != null)
			{
				environment = new MyEnvironment(environment);
				environment.define("super", superclass);
			}

			Dictionary<String, TypeScriptNativeFunction> methods =
				new Dictionary<String, TypeScriptNativeFunction>();
			foreach (Function method in stmt.methods)
			{
				TypeScriptNativeFunction function =
					new TypeScriptNativeFunction(
						method,
						environment,
						method.name.lexeme.Equals("init")
					);
				methods.Add(method.name.lexeme, function);
			}

			TypeScriptNativeClass klass =
				new TypeScriptNativeClass(
					stmt.name.lexeme,
					(TypeScriptNativeClass)superclass,
					methods
				);

			if (superclass != null)
			{
				environment = environment.enclosing;
			}

			environment.assign(stmt.name, klass);
			return null;
		}

		object Stmt.Visitor<object>.visitExpressionStmt(Expression stmt)
		{
			evaluate(stmt.expression);
			return null;
		}

		object Stmt.Visitor<object>.visitFunctionStmt(Function stmt)
		{
			TypeScriptNativeFunction function =
				new TypeScriptNativeFunction(stmt, environment, false);
			environment.define(stmt.name.lexeme, function);
			return null;
		}

		object Stmt.Visitor<object>.visitIfStmt(If stmt)
		{
			if (isTruthy(evaluate(stmt.condition)))
			{
				execute(stmt.thenBranch);
			}
			else if (stmt.elseBranch != null)
			{
				execute(stmt.elseBranch);
			}
			return null;
		}

		object Stmt.Visitor<object>.visitReturnStmt(Return stmt)
		{
			Object value = null;
			if (stmt.value != null) value = evaluate(stmt.value);
			throw new ReturnException(value);
		}

		object Stmt.Visitor<object>.visitVarStmt(MyVar stmt)
		{
			Object value = null;
			if (stmt.initializer != null)
			{
				value = evaluate(stmt.initializer);
			}

			environment.define(stmt.name.lexeme, value);
			return null;
		}

		object Stmt.Visitor<object>.visitWhileStmt(While stmt)
		{
			while (isTruthy(evaluate(stmt.condition)))
			{
				execute(stmt.body);
			}
			return null;
		}

		// Helpers

		private Object lookUpVariable(Token name, Expr expr)
		{
			Console.WriteLine(name.lexeme + " " + expr.ToString());
			int distance = locals[expr];
			if (!distance.Equals(null))
			{
				return environment.getAt(distance, name.lexeme);
			}
			else
			{
				return globals.get(name);
			}
		}

		public void resolve(Expr expr, int depth)
		{
			locals.Add(expr, depth);
		}

		public void executeBlock(List<Stmt> statements,
					  MyEnvironment environment)
		{
			MyEnvironment previous = this.environment;
			try
			{
				this.environment = environment;

				foreach (Stmt statement in statements)
				{
					execute(statement);
				}
			}
			finally
			{
				this.environment = previous;
			}
		}

		private void execute(Stmt stmt)
		{
			stmt.accept(this);
		}

		private Object evaluate(Expr expr)
		{
			return expr.accept(this);
		}

		private void checkNumberOperand(Token myOperator, Object operand)
		{
			if (operand is Double) return;
			throw new RuntimeError(myOperator, "Operand must be a number.");
		}

		private void checkNumberOperands(Token op, Object left, Object right)
		{
			if (left is Double && right is Double) return;
			throw new RuntimeError(op, "Operands must be numbers.");
		}

		private bool isTruthy(Object myObject)
		{
			if (myObject == null) return false;
			if (myObject is bool) return (bool)myObject;
			return true;
		}

		private bool isEqual(Object a, Object b)
		{
			if (a == null && b == null) return true;
			if (a == null) return false;

			return a.Equals(b);
		}
	}
}
