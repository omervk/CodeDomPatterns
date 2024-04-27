using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the expansion of C#'s using keyword.
	/// </summary>
	/// <example>Output example:<code>
	/// System.IDisposable kek;
	///	
	///	try
	///	{
	///		// ...
	///	}
	///	finally
	///	{
	///		if ((kek != null))
	///		{
	///			((System.IDisposable)(kek)).Dispose();
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternUsing : CodeStatementCollection
	{
		private const string UsedResourceExpression = "usedResourceExpr";
		private static readonly string DisposeMethodName = typeof(System.IDisposable).GetMethod("Dispose").Name;

		private static int objectCounter = 0;

		private CodeStatementCollection statements;
		private string variableName;

		private CodePatternUsing(string variableName, ResourceType resourceType, params CodeStatement[] statements)
		{
			if (variableName == null || variableName == string.Empty)
			{
				throw new ArgumentNullException("variableName");
			}
			
			this.variableName = variableName;

			CodeTryCatchFinallyStatement tryCatch = new CodeTryCatchFinallyStatement();

			CodeMethodInvokeExpression dispose = new CodeMethodInvokeExpression(
				new CodeCastExpression(typeof(IDisposable), new CodeVariableReferenceExpression(variableName)),
				DisposeMethodName);

			if (resourceType == ResourceType.ValueType)
			{
				tryCatch.FinallyStatements.Add(dispose);
			}
			else
			{
				tryCatch.FinallyStatements.Add(
					new CodeConditionStatement(
						new CodePatternUnaryOperatorExpression(
							CodePatternUnaryOperatorType.BooleanNotNull, 
							new CodeVariableReferenceExpression(variableName)
						),
						new CodeExpressionStatement(dispose)
					)
				);
			}

			this.statements = tryCatch.TryStatements;

			this.Add(tryCatch);
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternUsing class.
		/// </summary>
		/// <param name="usedExpression">The expression to use.</param>
		/// <param name="resourceType">The type category of the resource used.</param>
		/// <param name="statements">The statements in the using block.</param>
		public CodePatternUsing(CodeExpression usedExpression, ResourceType resourceType, params CodeStatement[] statements)
			: this(UsedResourceExpression + objectCounter.ToString(), resourceType, statements)
		{
			CodeVariableDeclarationStatement resourceVariable = new CodeVariableDeclarationStatement(typeof(object), variableName, usedExpression);

			this.Insert(0, resourceVariable);
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternUsing class.
		/// </summary>
		/// <param name="usedVariable">The variable to use.</param>
		/// <param name="resourceType">The type category of the resource used.</param>
		/// <param name="statements">The statements in the using block.</param>
		public CodePatternUsing(CodeVariableDeclarationStatement usedVariable, ResourceType resourceType, params CodeStatement[] statements)
			: this(usedVariable.Name, resourceType, statements)
		{
			this.Insert(0, usedVariable);
		}

		/// <summary>
		/// The statements in the using block.
		/// </summary>
		public CodeStatementCollection Statements
		{
			get
			{
				return this.statements;
			}
		}
	}
}
