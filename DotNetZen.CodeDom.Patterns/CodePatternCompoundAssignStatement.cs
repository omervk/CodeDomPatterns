using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents an assignment that's compound with an operator.
	/// </summary>
	/// <example>Output example:<code>foo = (foo + bar);</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternCompoundAssignStatement : CodeAssignStatement
	{
		/// <summary>
		/// Initializes a new instance of the CodePatternCompoundAssignStatement class.
		/// </summary>
		/// <param name="left">The variable to assign to.</param>
		/// <param name="op">A CodePatternCompoundAssignmentOperatorType indicating the type of operator.</param>
		/// <param name="right">The value to assign with.</param>
		public CodePatternCompoundAssignStatement(CodeExpression left, CodePatternCompoundAssignmentOperatorType op, CodeExpression right)
			: base(left, new CodeBinaryOperatorExpression(left, (System.CodeDom.CodeBinaryOperatorType)op, right))
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}

			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
		}
	}
}
