using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents an expression that consists of a binary operation between two expressions.
	/// </summary>
	///	<example>Output example:<code>((bool1 == true) &amp;&amp; (bool2 == false)) || ((bool1 == false) &amp;&amp; (bool2 == true))</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternBinaryOperatorExpression : System.CodeDom.CodeBinaryOperatorExpression
	{
		/// <summary>
		/// Initializes a new instance of the CodePatternBinaryOperatorExpression class.
		/// </summary>
		/// <param name="left">The CodeExpression on the left of the operator.</param>
		/// <param name="op">A CodeBinaryOperatorType indicating the type of operator.</param>
		/// <param name="right">The CodeExpression on the right of the operator.</param>
		public CodePatternBinaryOperatorExpression(CodeExpression left, CodePatternBinaryOperatorType op, CodeExpression right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}

			switch (op)
			{
				// left ^ right -> (left and not right) or (not left and right)
				case CodePatternBinaryOperatorType.BooleanExclusiveOr:
					this.Left = new System.CodeDom.CodeBinaryOperatorExpression(
						new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanIsTrue, left),
						System.CodeDom.CodeBinaryOperatorType.BooleanAnd,
						new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNot, right));
					this.Operator = System.CodeDom.CodeBinaryOperatorType.BooleanOr;
					this.Right = new System.CodeDom.CodeBinaryOperatorExpression(
						new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNot, left),
						System.CodeDom.CodeBinaryOperatorType.BooleanAnd,
						new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanIsTrue, right));
					break;
			}
		}
	}
}
