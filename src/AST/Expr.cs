using System;
using System.Collections.Generic;
using TypeScriptNative.Scan;

namespace TypeScriptNative.AST
{
	public abstract class Expr
	{
		public abstract R accept<R>(Visitor<R> visitor);

		public interface Visitor<R>
		{
			R visitAssignExpr(Assign expr);

			R visitBinaryExpr(Binary expr);

			R visitCallExpr(Call expr);

			R visitGetExpr(Get expr);

			R visitGroupingExpr(Grouping expr);

			R visitLiteralExpr(Literal expr);

			R visitLogicalExpr(Logical expr);

			R visitSetExpr(Set expr);

			R visitSuperExpr(Super expr);

			R visitThisExpr(This expr);

			R visitUnaryExpr(Unary expr);

			R visitVariableExpr(Variable expr);
		}
	}

	public class Assign : Expr
	{

		public Token name;
		public Expr value;

		public Assign(Token name, Expr value)
		{
			this.name = name;
			this.value = value;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitAssignExpr(this);
		}
	}

	public class Binary : Expr
	{

		public Expr left;
		public Token myOperator;
		public Expr right;

		public Binary(Expr left, Token myOperator, Expr right)
		{
			this.left = left;
			this.myOperator = myOperator;
			this.right = right;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitBinaryExpr(this);
		}
	}

	public class Call : Expr
	{

		public Expr callee;
		public Token paren;
		public List<Expr> arguments;

		public Call(Expr callee, Token paren, List<Expr> arguments)
		{
			this.callee = callee;
			this.paren = paren;
			this.arguments = arguments;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitCallExpr(this);
		}
	}

	public class Get : Expr
	{

		public Expr myObject;
		public Token name;

		public Get(Expr myObject, Token name)
		{
			this.myObject = myObject;
			this.name = name;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitGetExpr(this);
		}
	}

	public class Grouping : Expr
	{

		public Expr expression;

		public Grouping(Expr expression)
		{
			this.expression = expression;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitGroupingExpr(this);
		}
	}

	public class Literal : Expr
	{

		public Object value;

		public Literal(Object value)
		{
			this.value = value;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitLiteralExpr(this);
		}
	}

	public class Logical : Expr
	{

		public Expr left;
		public Token myOperator;
		public Expr right;

		public Logical(Expr left, Token myOperator, Expr right)
		{
			this.left = left;
			this.myOperator = myOperator;
			this.right = right;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitLogicalExpr(this);
		}
	}

	public class Set : Expr
	{

		public Expr myObject;
		public Token name;
		public Expr value;

		public Set(Expr myObject, Token name, Expr value)
		{
			this.myObject = myObject;
			this.name = name;
			this.value = value;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitSetExpr(this);
		}
	}

	public class Super : Expr
	{

		public Token keyword;
		public Token method;

		public Super(Token keyword, Token method)
		{
			this.keyword = keyword;
			this.method = method;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitSuperExpr(this);
		}
	}

	public class This : Expr
	{

		public Token keyword;

		public This(Token keyword)
		{
			this.keyword = keyword;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitThisExpr(this);
		}
	}

	public class Unary : Expr
	{

		public Token myOperator;
		public Expr right;

		public Unary(Token myOperator, Expr right)
		{
			this.myOperator = myOperator;
			this.right = right;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitUnaryExpr(this);
		}
	}

	public class Variable : Expr
	{

		public Token name;

		public Variable(Token name)
		{
			this.name = name;
		}

		override public R accept<R>(Visitor<R> visitor)
		{
			return visitor.visitVariableExpr(this);
		}

		public override string ToString()
		{
			return base.ToString() + " :: name => " + this.name;
		}
	}
}
