using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the expansion of C#'s lock keyword.
	/// </summary>
	/// <example>Output example:<code>
	///	object lockCachedExpr0 = this.SyncRoot;
	///	System.Threading.Monitor.Enter(lockCachedExpr0);
	///
	///	try
	///	{
	///		// ...
	///	}
	///	finally
	///	{
	///		System.Threading.Monitor.Exit(lockCachedExpr0);
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternLock : CodeStatementCollection
	{
		private const string LockCachedExpression = "lockCachedExpr";
		private static readonly string EnterMethodName = typeof(System.Threading.Monitor).GetMethod("Enter").Name;
		private static readonly string ExitMethodName = typeof(System.Threading.Monitor).GetMethod("Exit").Name;

		private static int objectCounter = 0;

		private CodeStatementCollection statements;

		/// <summary>
		/// Initializes a new instance of the CodePatternLock class.
		/// </summary>
		/// <param name="lockedExpression">The expression to lock.</param>
		/// <param name="statements">The statements in the lock block.</param>
		public CodePatternLock(CodeExpression lockedExpression, params CodeStatement[] statements)
		{
			if (lockedExpression == null)
			{
				throw new ArgumentNullException("lockedExpression");
			}

			CodeVariableDeclarationStatement cachedExpression = new CodeVariableDeclarationStatement(typeof(object), LockCachedExpression + (objectCounter++).ToString(), lockedExpression);

			CodeMethodInvokeExpression enterMonitor = new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression(typeof(System.Threading.Monitor)),
				EnterMethodName,
				new CodeVariableReferenceExpression(cachedExpression.Name));

			CodeMethodInvokeExpression exitMonitor = new CodeMethodInvokeExpression(
				new CodeTypeReferenceExpression(typeof(System.Threading.Monitor)),
				ExitMethodName,
				new CodeVariableReferenceExpression(cachedExpression.Name));

			CodeTryCatchFinallyStatement tryCatch = new CodeTryCatchFinallyStatement();
			tryCatch.FinallyStatements.Add(exitMonitor);

			this.statements = tryCatch.TryStatements;

			this.Add(cachedExpression);
			this.Add(enterMonitor);
			this.Add(tryCatch);
		}

		/// <summary>
		/// The statements in the lock block.
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
