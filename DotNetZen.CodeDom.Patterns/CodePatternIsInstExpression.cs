using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the closest expression to IL's isinst opcode (C#'s is keyword).
	/// </summary>
	/// <example>Output example:<code>myVariable.GetType().IsInstanceOfType(typeof(System.IComparable))</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternIsInstExpression : CodeMethodInvokeExpression
	{
		/// <summary>
		/// Initializes a new instance of the CodePatternIsExpression class.
		/// </summary>
		/// <param name="instance">A reference to the instance being tested.</param>
		/// <param name="type">The type being tested against.</param>
		public CodePatternIsInstExpression(CodeExpression instance, CodeTypeReference type)
			: base(new CodeMethodInvokeExpression(instance, typeof(object).GetMethod("GetType").Name), typeof(Type).GetMethod("IsInstanceOfType").Name, new CodeTypeOfExpression(type))
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
		}
	}
}
