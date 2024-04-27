using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents an expression that consists of an unary operation on one expression.
	/// </summary>
	/// <example>Output example:<code>
	/// (bool1 == true)
	/// (bool1 == false)
	/// </code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternUnaryOperatorExpression : CodeBinaryOperatorExpression
	{
		/// <summary>
		/// Initializes a new instance of the CodePatternUnaryOperatorExpression class using the specified parameters.
		/// </summary>
		/// <param name="operator">A CodePatternUnaryOperatorType indicating the type of operator.</param>
		/// <param name="operand">The CodeExpression on which the operator will work.</param>
		public CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType @operator, CodeExpression operand)
		{
			if (operand == null)
			{
				throw new ArgumentNullException("operand");
			}

			switch (@operator)
			{
				// expr -> expr == true
				case CodePatternUnaryOperatorType.BooleanIsTrue:
					this.Left = operand;
					this.Operator = CodeBinaryOperatorType.ValueEquality;
					this.Right = new CodePrimitiveExpression(true);
					break;
				// !expr -> expr == false
				case CodePatternUnaryOperatorType.BooleanNot:
					this.Left = operand;
					this.Operator = CodeBinaryOperatorType.ValueEquality;
					this.Right = new CodePrimitiveExpression(false);
					break;
				// expr == null
				case CodePatternUnaryOperatorType.BooleanIsNull:
					this.Left = operand;
					this.Operator = CodeBinaryOperatorType.IdentityEquality;
					this.Right = new CodePrimitiveExpression(null);
					break;
				// expr != null
				case CodePatternUnaryOperatorType.BooleanNotNull:
					this.Left = operand;
					this.Operator = CodeBinaryOperatorType.IdentityInequality;
					this.Right = new CodePrimitiveExpression(null);
					break;
			}
		}
	}
}
