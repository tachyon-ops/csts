using System;
using System.Collections.Generic;

namespace TypeScriptNative.src.AST
{
	// interface Node
	public interface INode { }

	//
	// TypeScript specific part
	//

	// data class SandyFile(val statements : List<Statement>) : Node
	public class Program : INode
	{
		public List<Statement> statements = new List<Statement>();
		public Program()
		{

		}

		public void AddStatement(Statement element)
		{
			this.statements.Add(element);
		}

		public void AddStatements(List<Statement> elements)
		{
			this.statements.AddRange(elements);
		}
	}

	public class FunctionDeclaration : INode
	{

		public string functionName;
		public List<Parameter> parameters;
		public string type;
		public FunctionDeclaration(string functionName, List<Parameter> parameters, string type = null)
		{
			this.functionName = functionName;
			this.parameters = parameters;
			this.type = type;
			Console.WriteLine("TODO: FunctionDeclaration | identifier: " + this.functionName + " parameters: " + this.parameters.Count + " type: " + this.type);
		}
	}

	public class Argument : INode
	{
		ArgumentType type;
		string val;
		public Argument(ArgumentType type, string val)
		{
			this.type = type;
			this.val = val;
		}
	}

	public enum ArgumentType
	{
		numericLiteral
	}

	public enum StatementType
	{
		RETURN,
		SINGLE_EXPRESSION
	}

	// interface Statement : Node { }
	public class Statement : INode
	{
		public INode child;
		public StatementType type;
		public Statement() { }
		public Statement(INode child, StatementType type = StatementType.SINGLE_EXPRESSION)
		{
			this.child = child;
			this.type = type;
		}
	}

	public enum Operation
	{
		UNKNOWN,
		ADDITION
	}

	public interface Expression : INode { }
	//class Expression : INode
	//{
	//	INode left;
	//	INode right;
	//	List<Expression> rightExpressions;
	//	public Expression()
	//	{
	//		this.left = null;
	//		this.right = null;
	//	}
	//	public Expression(INode left, INode right)
	//	{
	//		this.left = left;
	//		this.right = right;
	//	}
	//	public Expression(INode left, List<Expression> rightExpressions)
	//	{
	//		this.left = left;
	//		this.right = null;
	//		this.rightExpressions = rightExpressions;
	//	}
	//}

	// interface Type : Node { }
	public interface IType : INode { }

	//
	// Types
	//

	// object DecimalType : Type
	public class NumberAST : IType
	{

	}

	// object IntType : Type
	public class IntegerAST : IType
	{

	}

	//
	// Expressions
	//

	//interface BinaryExpression : Expression
	//{
	//	val left: Expression
	//	val right: Expression
	//}

	public interface BinaryExpression : Expression
	{
		Expression left { get; }
		Expression right { get; }
	}

	// data class SumExpression(override val left: Expression, override val right: Expression) : BinaryExpression
	public class SumExpression : BinaryExpression
	{
		Expression _left;
		Expression _right;
		public SumExpression(Expression left, Expression right)
		{
			this._left = left;
			this._right = right;
		}

		public Expression left => this._left;
		public Expression right => this._right;
	}

	// data class SubtractionExpression(override val left: Expression, override val right: Expression) : BinaryExpression

	// data class MultiplicationExpression(override val left: Expression, override val right: Expression) : BinaryExpression

	// data class DivisionExpression(override val left: Expression, override val right: Expression) : BinaryExpression

	// data class UnaryMinusExpression(val value: Expression) : Expression

	// data class TypeConversion(val value: Expression, val targetType: Type) : Expression

	// data class VarReference(val varName: String) : Expression
	public class VarReference : Expression
	{
		string varName;
		public VarReference(string varName)
		{
			this.varName = varName;
		}
	}

	// data class IntLit(val value: String) : Expression
	public class IntegerLiteral : Expression
	{
		public string value;
		public IntegerLiteral(string value)
		{
			this.value = value;
		}
	}

	// data class DecLit(val value: String) : Expression
	public class DecimalLiteral : Expression
	{
		string value;
		public DecimalLiteral(string value)
		{
			this.value = value;
		}
	}

	/**
	 * Statements
	 */

	// data class VarDeclaration(val varName: String, val value: Expression) : Statement
	public class VarDeclaration : Statement
	{
		string varName;
		Expression value;
		VarDeclaration(string varName, Expression value)
		{
			this.varName = varName;
			this.value = value;
		}
	}

	// data class Assignment(val varName: String, val value: Expression) : Statement
	public class Assignment : Statement
	{
		string varName;
		Expression value;
		Assignment(string varName, Expression value)
		{
			this.varName = varName;
			this.value = value;
		}
	}

	// data class Print(val value: Expression) : Statement
	public class Print : Statement
	{
		Expression value;
		Print(Expression value)
		{
			this.value = value;
		}
	}

	public class Parameter : Statement
	{
		public string varName;
		public string type;
		public bool optional;
		public Parameter(string varName, string type, bool optional = false)
		{
			this.varName = varName;
			this.type = type;
			this.optional = optional;
		}
	}
}
