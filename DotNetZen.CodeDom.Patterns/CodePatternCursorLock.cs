using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the locking of a cursor to an hourglass until the end of the contained statements.
	/// </summary>
	/// <example>Output example:<code>
	///	System.Windows.Forms.Cursor cursor0 = this.Cursor;
	///
	///	try
	///	{
	///		this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
	///		
	///		// More code here...
	///	}
	///	finally
	///	{
	///		this.Cursor = cursor0;
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternCursorLock : CodeStatementCollection
	{
		private static readonly string CursorPropertyName = typeof(System.Windows.Forms.Form).GetProperty("Cursor").Name;
		private static readonly string WaitCursorPropertyName = typeof(System.Windows.Forms.Cursors).GetProperty("WaitCursor").Name;
		private static readonly Type CursorPropertyType = typeof(System.Windows.Forms.Form).GetProperty("Cursor").PropertyType;

		private const string CursorName = "cursor";

		private static int objectCounter = 0;

		private CodeStatementCollection statements;

		/// <summary>
		/// Initializes a new instance of the CodePatternCursorLock class.
		/// </summary>
		/// <param name="formReference">The reference to the form object whose cursor should be locked.</param>
		/// <param name="statements">The statements during which the cursor is locked.</param>
		public CodePatternCursorLock(CodeExpression formReference, params CodeStatement[] statements)
		{
			if (formReference == null)
			{
				throw new ArgumentNullException("formReference");
			}

			// Catch previous cursor.
			CodeVariableDeclarationStatement cursor = new CodeVariableDeclarationStatement(
				CursorPropertyType,
				CursorName + objectCounter++, 
				new CodePropertyReferenceExpression(formReference, CursorPropertyName));

			CodeTryCatchFinallyStatement tryFinally = new CodeTryCatchFinallyStatement();

			// Add cursor lock.
			tryFinally.TryStatements.Add(
				new CodeAssignStatement(
					new CodePropertyReferenceExpression(formReference, CursorPropertyName),
					new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(System.Windows.Forms.Cursors)), WaitCursorPropertyName)));
			
			// Add cursor release.
			tryFinally.FinallyStatements.Add(
				new CodeAssignStatement(
				new CodePropertyReferenceExpression(formReference, CursorPropertyName),
				new CodeVariableReferenceExpression(cursor.Name)));

			// Add contained statements.
			tryFinally.TryStatements.AddRange(statements);

			this.statements = tryFinally.TryStatements;

			this.Add(cursor);
			this.Add(tryFinally);
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternCursorLock class when the statements are in the form.
		/// </summary>
		/// <param name="statements">The statements during which the cursor is locked.</param>
		public CodePatternCursorLock(params CodeStatement[] statements) : this(new CodeThisReferenceExpression(), statements)
		{
		}

		/// <summary>
		/// The statements in the try/finally block.
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
