using System;
using System.Collections.Generic;
using TypeScriptNative.Scan;

namespace TypeScriptNative.AST
{
	public abstract class Stmt
	{
		public abstract R accept<R>(Visitor<R> visitor);

		public interface Visitor<R>
		{
			R visitBlockStmt(Block stmt);

			R visitClassStmt(Class stmt);

			R visitExpressionStmt(Expression stmt);

			R visitFunctionStmt(Function stmt);

			R visitIfStmt(If stmt);

			R visitReturnStmt(Return stmt);

			R visitVarStmt(MyVar stmt);

			R visitWhileStmt(While stmt);
		}
	}

	public class Expression : Stmt
	{

		public Expr expression;

		public Expression(Expr expression)
		{
			this.expression = expression;
		}

		public override R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitExpressionStmt(this);
		}
	}

	public class Block : Stmt
	{

		public List<Stmt> statements;

		public Block(List<Stmt> statements)
		{
			this.statements = statements;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitBlockStmt(this);
		}
	}

	public class Class : Stmt
	{

		public Token name;
		public Variable superclass;
		public List<Function> methods;

		public Class(Token name, Variable superclass, List<Function> methods)
		{
			this.name = name;
			this.superclass = superclass;
			this.methods = methods;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitClassStmt(this);
		}
	}

	public class Function : Stmt
	{

		public Token name;
		public List<Token> myParams;
		public List<Stmt> body;

		public Function(Token name, List<Token> myParams, List<Stmt> body)
		{
			this.name = name;
			this.myParams = myParams;
			this.body = body;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitFunctionStmt(this);
		}
	}

	public class If : Stmt
	{

		public Expr condition;
		public Stmt thenBranch;
		public Stmt elseBranch;

		public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
		{
			this.condition = condition;
			this.thenBranch = thenBranch;
			this.elseBranch = elseBranch;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitIfStmt(this);
		}
	}

	public class Return : Stmt
	{

		public Token keyword;
		public Expr value;

		public Return(Token keyword, Expr value)
		{
			this.keyword = keyword;
			this.value = value;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitReturnStmt(this);
		}
	}

	public class MyVar : Stmt
	{

		public Token name;
		public Expr initializer;

		public MyVar(Token name, Expr initializer)
		{
			this.name = name;
			this.initializer = initializer;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitVarStmt(this);
		}
	}

	public class While : Stmt
	{

		public Expr condition;
		public Stmt body;

		public While(Expr condition, Stmt body)
		{
			this.condition = condition;
			this.body = body;
		}

		override public
		 R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitWhileStmt(this);
		}
	}
}
