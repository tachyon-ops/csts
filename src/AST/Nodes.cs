using System;
using System.Collections.Generic;

namespace TypeScriptNative.src.AST
{
	// interface Node
	interface INode { }

	//
	// TypeScript specific part
	//

	// data class SandyFile(val statements : List<Statement>) : Node
	class Program : INode
	{
		List<Statement> elements;
		public Program(List<Statement> elements)
		{
			this.elements = elements;
		}
	}

	class FunctionDeclaration : INode
	{

		string functionName;
		List<Parameter> parameters;
		string type;
		public FunctionDeclaration(string functionName, List<Parameter> parameters, string type = null)
		{
			this.functionName = functionName;
			this.parameters = parameters;
			this.type = type;
			Console.WriteLine("TODO: FunctionDeclaration | identifier: " + this.functionName + " parameters: " + this.parameters.Count + " type: " + this.type);
		}
	}

	enum StatementType
	{
		RETURN,
		SINGLE_EXPRESSION
	}

	// interface Statement : Node { }
	class Statement : INode
	{
		INode child;
		StatementType type;
		public Statement() { }
		public Statement(INode child, StatementType type = StatementType.SINGLE_EXPRESSION)
		{
			this.child = child;
			this.type = type;
		}
	}

	// interface Expression : Node { }
	class Expression : INode
	{
		public Expression() { }
	}

	// interface Type : Node { }
	interface IType : INode { }

	//
	// Types
	//

	// object DecimalType : Type
	class NumberAST : IType
	{

	}

	// object IntType : Type
	class IntegerAST : IType
	{

	}

	//
	// Expressions
	//

	// interface BinaryExpression : Expression
	//{
	//	val left: Expression
	//	val right: Expression
	//}

	// data class SumExpression(override val left: Expression, override val right: Expression) : BinaryExpression
	class SumExpression : INode
	{
		Expression left;
		Expression right;
		SumExpression(Expression left, Expression right)
		{
			this.left = left;
			this.right = right;
		}
	}

	// data class SubtractionExpression(override val left: Expression, override val right: Expression) : BinaryExpression

	// data class MultiplicationExpression(override val left: Expression, override val right: Expression) : BinaryExpression

	// data class DivisionExpression(override val left: Expression, override val right: Expression) : BinaryExpression

	// data class UnaryMinusExpression(val value: Expression) : Expression

	// data class TypeConversion(val value: Expression, val targetType: Type) : Expression

	// data class VarReference(val varName: String) : Expression
	class VarReference : Expression
	{
		string varName;
		VarReference(string varName)
		{
			this.varName = varName;
		}
	}

	// data class IntLit(val value: String) : Expression
	class IntegerLiteral : Expression
	{
		string value;
		IntegerLiteral(string value)
		{
			this.value = value;
		}
	}

	// data class DecLit(val value: String) : Expression
	class DecimalLiteral : Expression
	{
		string value;
		DecimalLiteral(string value)
		{
			this.value = value;
		}
	}

	/**
	 * Statements
	 */

	// data class VarDeclaration(val varName: String, val value: Expression) : Statement
	class VarDeclaration : Statement
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
	class Assignment : Statement
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
	class Print : Statement
	{
		Expression value;
		Print(Expression value)
		{
			this.value = value;
		}
	}

	class Parameter : Statement
	{
		string varName;
		string type;
		bool optional;
		public Parameter(string varName, string type, bool optional = false)
		{
			this.varName = varName;
			this.type = type;
			this.optional = optional;
		}
	}
}
