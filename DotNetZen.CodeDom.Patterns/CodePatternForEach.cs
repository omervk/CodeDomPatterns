using System;
using System.CodeDom;
using System.Collections;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the expansion of C#'s foreach keyword.
	/// </summary>
	/// <example>Output example:<code>
	///	System.Collections.IEnumerator enumerator0 = ((System.Collections.IEnumerator)(myCollection)).GetEnumerator();
	///
	///	try
	///	{
	///		for (; enumerator0.MoveNext();)
	///		{
	///			int element0 = ((int)(enumerator0.Current));
	///			// Contained statements ...
	///		}
	///	}
	///	finally
	///	{
	///		if (((enumerator0 != null) &amp;&amp; enumerator0.GetType().IsInstanceOfType(typeof(System.IDisposable))))
	///		{
	///			((System.IDisposable)(enumerator0)).Dispose();
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternForEach : CodeStatementCollection
	{
		private static readonly string GetEnumeratorName = typeof(IEnumerable).GetMethod("GetEnumerator").Name;
		private static readonly string CurrentName = typeof(IEnumerator).GetProperty("Current").Name;
		private static readonly string MoveNextName = typeof(IEnumerator).GetMethod("MoveNext").Name;
		private static readonly string DisposeName = typeof(IDisposable).GetMethod("Dispose").Name;

		private const string EnumeratorName = "enumerator";
		private const string ElementName = "element";

		private static int objectCounter = 0;

		private CodeStatementCollection statements;
		private string currentElementName;

		/// <summary>
		/// Initializes a new instance of the CodePatternIsExpression class with an implicit collection.
		/// </summary>
		/// <param name="elementType">The type of element to expect.</param>
		/// <param name="collection">The collection to be iterated on.</param>
		/// <param name="enumeratorType">The type of the enumerating object.</param>
		/// <param name="statements">Statements inside the block.</param>
		public CodePatternForEach(CodeTypeReference elementType, CodeExpression collection, Type enumeratorType, params CodeStatement[] statements)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException("elementType");
			}

			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			if (enumeratorType == null)
			{
				throw new ArgumentNullException("enumeratorType");
			}

			if (!(enumeratorType == typeof(IEnumerator) || enumeratorType.GetInterface(typeof(IEnumerator).Name, false) != null ))
			{
				throw new ArgumentException("Supplied enumerator type must implement IEnumerator.", "enumeratorType");
			}

			int thisCounter = objectCounter++;
			this.currentElementName = ElementName + thisCounter.ToString();

			CodeVariableDeclarationStatement enumerator = new CodeVariableDeclarationStatement(enumeratorType, EnumeratorName + thisCounter.ToString());
			enumerator.InitExpression = new CodeMethodInvokeExpression(collection, GetEnumeratorName);

			CodeVariableDeclarationStatement element = new CodeVariableDeclarationStatement(elementType, this.currentElementName);
			element.InitExpression = new CodeCastExpression(elementType, 
				new CodePropertyReferenceExpression(
					new CodeVariableReferenceExpression(enumerator.Name),
					CurrentName));

			this.Add(enumerator);

			CodeTryCatchFinallyStatement tryFinally = new CodeTryCatchFinallyStatement();

			tryFinally.TryStatements.Add(
				new CodeIterationStatement(
					new CodeSnippetStatement(string.Empty), 
					new CodePatternUnaryOperatorExpression(
						CodePatternUnaryOperatorType.BooleanIsTrue,
						new CodeMethodInvokeExpression(
							new CodeVariableReferenceExpression(enumerator.Name),
							MoveNextName)
					),
					new CodeSnippetStatement(string.Empty),
					element
				)
			);

			// If the type is IEnumerator or any other interface, we need to check for both nulls and IDisposability.
			if (enumeratorType.IsInterface)
			{
				tryFinally.FinallyStatements.Add(
					new CodeConditionStatement(
						new CodeBinaryOperatorExpression(
							new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, 
								new CodeVariableReferenceExpression(enumerator.Name)
							),
							CodeBinaryOperatorType.BooleanAnd, 
							new CodePatternIsInstExpression(
								new CodeVariableReferenceExpression(enumerator.Name),
								CodeTypeReferenceStore.Get(typeof(IDisposable))
							)
						),
						new CodeExpressionStatement(
							new CodeMethodInvokeExpression(
								new CodeCastExpression(typeof(IDisposable), new CodeVariableReferenceExpression(enumerator.Name)),
								DisposeName
							)
						)
					));

				this.Add(tryFinally);
			}
			else if (enumeratorType.GetInterface(typeof(IDisposable).Name, false) != null)
			{
				// Value types can not be null and the type is IDisposable
				if (enumeratorType.IsValueType)
				{
					tryFinally.FinallyStatements.Add(
						new CodeExpressionStatement(
							new CodeMethodInvokeExpression(
								new CodeCastExpression(typeof(IDisposable), new CodeVariableReferenceExpression(enumerator.Name)),
								DisposeName
						)));
				}
				else // Check only for null
				{
					tryFinally.FinallyStatements.Add(
						new CodeConditionStatement(
							new CodePatternUnaryOperatorExpression(
								CodePatternUnaryOperatorType.BooleanNotNull, 
								new CodeVariableReferenceExpression(enumerator.Name)
							),
							new CodeExpressionStatement(
								new CodeMethodInvokeExpression(
									new CodeCastExpression(typeof(IDisposable), new CodeVariableReferenceExpression(enumerator.Name)),
									DisposeName
								)
							)
						));
				}

				this.Add(tryFinally);
			}
			else
			{
				// No need for try/catch.
				this.AddRange(tryFinally.TryStatements);
			}

			this.statements = ((CodeIterationStatement)(tryFinally.TryStatements[0])).Statements;
			this.statements.AddRange(statements);
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternIsExpression class with an explicit collection.
		/// </summary>
		/// <param name="elementType">The type of element to expect.</param>
		/// <param name="collection">The collection to be iterated on.</param>
		/// <param name="statements">Statements inside the block.</param>
		public CodePatternForEach(CodeTypeReference elementType, CodeExpression collection, params CodeStatement[] statements)
			: this(elementType, new CodeCastExpression(typeof(IEnumerable), collection), typeof(IEnumerator), statements)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
		}
		
		/// <summary>
		/// Gets the statements in the foreach block.
		/// </summary>
		public CodeStatementCollection Statements
		{
			get
			{
				return this.statements;
			}
		}

		/// <summary>
		/// Gets the name of the current element in the iteration.
		/// </summary>
		public string CurrentElementName
		{
			get
			{
				return this.currentElementName;
			}
		}
	}
}
