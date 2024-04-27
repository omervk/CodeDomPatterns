using System;
using System.CodeDom;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of methods for a asynchronous invocation of a method.
	/// </summary>
	/// <example>Output example:<code>
	///	/// &lt;summary&gt;
	///	/// Represents the delegate instance for asynchronous calls to MyMethod.
	///	/// &lt;/summary&gt;
	///	private MyMethodAsyncCallback m_MyMethodCallback;
	///
	///	/// &lt;summary&gt;
	///	/// Executes the MyMethod method asynchronously with a callback.
	///	/// &lt;/summary&gt;
	///	/// &lt;param name="foo"&gt;See original method, MyMethod, for more information about this parameter.&lt;/param&gt;
	///	/// &lt;param name="callback"&gt;A method to be called when the asynchronous action completes.&lt;/param&gt;
	///	/// &lt;returns&gt;An &lt;see cref="System.IAsyncResult" /&gt; object detailing the asynchronous action.&lt;/returns&gt;
	///	public System.IAsyncResult BeginMyMethod(int foo, System.AsyncCallback callback)
	///	{
	///		if ((this.m_MyMethodCallback == null))
	///		{
	///			this.m_MyMethodCallback = new MyMethodAsyncCallback(this.MyMethod);
	///		}
	///		return this.m_MyMethodCallback.BeginInvoke(foo, callback, null);
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Executes the MyMethod method asynchronously.
	///	/// &lt;/summary&gt;
	///	/// &lt;param name="foo"&gt;See original method, MyMethod, for more information about this parameter.&lt;/param&gt;
	///	/// &lt;returns&gt;An &lt;see cref="System.IAsyncResult" /&gt; object detailing the asynchronous action.&lt;/returns&gt;
	///	public System.IAsyncResult BeginMyMethod(int foo)
	///	{
	///		return this.BeginMyMethod(foo, null);
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Synchronously completes an asynchronous call to MyMethod.
	///	/// &lt;/summary&gt;
	///	/// &lt;param name="asyncResult"&gt;The &lt;see cref="System.IAsyncResult" /&gt; retrieved from the call to &lt;see cref="BeginMyMethod" /&gt;.&lt;/param&gt;
	///	/// &lt;exception cref="System.InvalidOperationException"&gt;Thrown when the method is called before the &lt;see cref="BeginMyMethod" /&gt; method.&lt;/exception&gt;
	///	public void EndMyMethod(System.IAsyncResult asyncResult)
	///	{
	///		if ((this.m_MyMethodCallback == null))
	///		{
	///			throw new System.InvalidOperationException("End of asynchronous operation attempted when one has not yet begun.");
	///		}
	///		this.m_MyMethodCallback.EndInvoke(asyncResult);
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Represents the delegate for asynchronous calls to MyMethod.
	///	/// &lt;/summary&gt;
	///	public delegate void MyMethodAsyncCallback(int foo);
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternAsyncOperation : CodeTypeMemberCollection
	{
		private const string AsyncCallbackDelegateSuffix = "AsyncCallback";
		private const string CallbackFieldName = "m_{0}Callback";
		private const string BeginMethodPrefix = "Begin";
		private const string EndMethodPrefix = "End";
		private const string BeginInvokeMethodName = "BeginInvoke";
		private const string EndInvokeMethodName = "EndInvoke";
		private const string CallbackVariableName = "callback";
		private const string AsyncResultVariableName = "asyncResult";
		private const string InvalidEndOperationExceptionMessage = "End of asynchronous operation attempted when one has not yet begun.";

		private CodeTypeDelegate callbackDelegate;
		private CodeMemberField callbackInstance;
		private CodeMemberMethod beginOperationWithCallback;
		private CodeMemberMethod beginOperation;
		private CodeMemberMethod endOperation;

		private string originalMethod;

		/// <summary>
		/// Initializes a new instance of the CodePatternAsyncOperation class.
		/// </summary>
		/// <param name="originalMethod">A reference to the method that is to be invoked.</param>
		/// <param name="returnType">The method's return type.</param>
		/// <param name="parameters">The parameters the method takes.</param>
		public CodePatternAsyncOperation(CodeMethodReferenceExpression originalMethod, CodeTypeReference returnType, params CodeParameterDeclarationExpression[] parameters)
		{
			if (originalMethod == null)
			{
				throw new ArgumentNullException("originalMethod");
			}

			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}

			this.originalMethod = originalMethod.MethodName;

			this.callbackDelegate = new CodeTypeDelegate(originalMethod.MethodName + AsyncCallbackDelegateSuffix);
			this.callbackDelegate.Attributes &= ~MemberAttributes.AccessMask;
			this.callbackDelegate.Attributes |= MemberAttributes.Private;
			this.callbackDelegate.ReturnType = returnType;
			this.callbackDelegate.Parameters.AddRange(parameters);

			this.Add(this.callbackDelegate);

			this.callbackInstance = new CodeMemberField();
			this.callbackInstance.Name = string.Format(CallbackFieldName, originalMethod.MethodName);
			this.callbackInstance.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.callbackInstance.Attributes |= MemberAttributes.Private | (originalMethod.TargetObject == null ? MemberAttributes.Static : 0);
			this.callbackInstance.Type = CodeTypeReferenceStore.Get(this.callbackDelegate.Name);

			this.Add(this.callbackInstance);

			CodeFieldReferenceExpression fieldReference = new CodeFieldReferenceExpression(((originalMethod.TargetObject == null) ? null : new CodeThisReferenceExpression()), this.callbackInstance.Name);

			this.beginOperationWithCallback = new CodeMemberMethod();
			this.beginOperationWithCallback.Name = BeginMethodPrefix + originalMethod.MethodName;
			this.beginOperationWithCallback.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.beginOperationWithCallback.Attributes |= MemberAttributes.Public | MemberAttributes.Overloaded | (originalMethod.TargetObject == null ? MemberAttributes.Static : 0) | MemberAttributes.Final;
			this.beginOperationWithCallback.Parameters.AddRange(parameters);
			this.beginOperationWithCallback.Parameters.Add(new CodeParameterDeclarationExpression(typeof(AsyncCallback), CallbackVariableName));
			this.beginOperationWithCallback.ReturnType = CodeTypeReferenceStore.Get(typeof(IAsyncResult));
			this.beginOperationWithCallback.Statements.Add(
				new CodeConditionStatement(
					new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanIsNull, fieldReference),
					new CodeAssignStatement(fieldReference, new CodeObjectCreateExpression(this.callbackDelegate.Name, originalMethod))));
			this.beginOperationWithCallback.Statements.Add(
				new CodeMethodReturnStatement(new CodeMethodInvokeExpression(fieldReference, BeginInvokeMethodName, GetVariableListWithCallback(parameters, CallbackVariableName))));
			
			this.Add(this.beginOperationWithCallback);

			this.beginOperation = new CodeMemberMethod();
			this.beginOperation.Name = this.beginOperationWithCallback.Name;
			this.beginOperation.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.beginOperation.Attributes |= MemberAttributes.Public | MemberAttributes.Overloaded | (originalMethod.TargetObject == null ? MemberAttributes.Static : 0) | MemberAttributes.Final;
			this.beginOperation.Parameters.AddRange(parameters);
			this.beginOperation.ReturnType = CodeTypeReferenceStore.Get(typeof(IAsyncResult));
			this.beginOperation.Statements.Add(
				new CodeMethodReturnStatement(new CodeMethodInvokeExpression(fieldReference.TargetObject, this.beginOperationWithCallback.Name, GetVariableList(parameters))));
			
			this.Add(this.beginOperation);

			this.endOperation = new CodeMemberMethod();
			this.endOperation.Name = EndMethodPrefix + originalMethod.MethodName;
			this.endOperation.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.endOperation.Attributes |= MemberAttributes.Public | MemberAttributes.Overloaded | (originalMethod.TargetObject == null ? MemberAttributes.Static : 0) | MemberAttributes.Final;
			this.endOperation.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IAsyncResult), AsyncResultVariableName));
			this.endOperation.ReturnType = returnType;
			this.endOperation.Statements.Add(
				new CodeConditionStatement(
				new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanIsNull, fieldReference),
				new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(InvalidOperationException), new CodePrimitiveExpression(InvalidEndOperationExceptionMessage)))));

			if (returnType != null &&
				returnType.BaseType != typeof(void).Name &&
				returnType.BaseType != typeof(void).FullName)
			{
				this.endOperation.Statements.Add(
					new CodeMethodReturnStatement(new CodeMethodInvokeExpression(fieldReference, EndInvokeMethodName, new CodeVariableReferenceExpression(AsyncResultVariableName))));
			}
			else
			{
				this.endOperation.Statements.Add(
					new CodeExpressionStatement(new CodeMethodInvokeExpression(fieldReference, EndInvokeMethodName, new CodeVariableReferenceExpression(AsyncResultVariableName))));
			}
			
			this.Add(this.endOperation);

			this.HasComments = true;
		}

		#region Constructor Overloads
		/// <summary>
		/// Initializes a new instance of the CodePatternAsyncOperation class.
		/// </summary>
		/// <param name="originalMethod">The declaration of the method that is to be invoked.</param>
		/// <remarks>
		/// Please note that the default target for the invocation is <code>this</code> when the method is not flagged as <see cref="MemberAttributes.Static"/>
		/// and the containing type when the method is flagged as <see cref="MemberAttributes.Static"/>.</remarks>
		public CodePatternAsyncOperation(CodeMemberMethod originalMethod)
			: this(((originalMethod.Attributes & MemberAttributes.Static) == MemberAttributes.Static ? null : new CodeThisReferenceExpression()), originalMethod)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternAsyncOperation class.
		/// </summary>
		/// <param name="target">The location of the invoked method.</param>
		/// <param name="originalMethod">The declaration of the method that is to be invoked.</param>
		public CodePatternAsyncOperation(CodeExpression target, CodeMemberMethod originalMethod)
			: this(new CodeMethodReferenceExpression(target, originalMethod.Name), originalMethod.ReturnType, ToParamArray(originalMethod.Parameters))
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternAsyncOperation class.
		/// </summary>
		/// <param name="originalMethod">A reference to the method that is to be invoked.</param>
		/// <param name="parameters">The parameters the method takes.</param>
		/// <remarks>The return value is inferred to be <see cref="System.Void"/>.</remarks>
		public CodePatternAsyncOperation(CodeMethodReferenceExpression originalMethod, params CodeParameterDeclarationExpression[] parameters)
			: this(originalMethod, (CodeTypeReference)null, parameters)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternAsyncOperation class.
		/// </summary>
		/// <param name="originalMethod">A reference to the method that is to be invoked.</param>
		/// <param name="returnType">The method's return type.</param>
		/// <param name="parameters">The parameters the method takes.</param>
		public CodePatternAsyncOperation(CodeMethodReferenceExpression originalMethod, Type returnType, params CodeParameterDeclarationExpression[] parameters)
			: this(originalMethod, CodeTypeReferenceStore.Get(returnType), parameters)
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the private delegate used to make the asynchronous call.
		/// </summary>
		public CodeTypeDelegate CallbackDelegate
		{
			get
			{
				return this.callbackDelegate;
			}
		}

		/// <summary>
		/// Gets the private instance of the delegate which points to the original method.
		/// </summary>
		public CodeMemberField CallbackInstance
		{
			get
			{
				return this.callbackInstance;
			}
		}

		/// <summary>
		/// Gets the method that calls BeginInvoke on <see cref="CallbackInstance"/> with a callback.
		/// </summary>
		public CodeMemberMethod BeginOperationWithCallback
		{
			get
			{
				return this.beginOperationWithCallback;
			}
		}

		/// <summary>
		/// Gets the method that calls BeginInvoke on <see cref="CallbackInstance"/> without a callback.
		/// </summary>
		public CodeMemberMethod BeginOperation
		{
			get
			{
				return this.beginOperation;
			}
		}

		/// <summary>
		/// Gets the method that calls EndInvoke on <see cref="CallbackInstance"/>.
		/// </summary>
		public CodeMemberMethod EndOperation
		{
			get
			{
				return this.endOperation;
			}
		}
		#endregion

		/// <summary>
		/// Gets or sets whether the members generated by the pattern have comments.
		/// </summary>
		/// <value>Whether the members generated by the pattern have comments.</value>
		public bool HasComments
		{
			get
			{
				return (this.beginOperationWithCallback.Comments.Count > 0);
			}
			set
			{
				this.beginOperation.Comments.Clear();
				this.beginOperationWithCallback.Comments.Clear();
				this.callbackDelegate.Comments.Clear();
				this.callbackInstance.Comments.Clear();
				this.endOperation.Comments.Clear();

				if (value)
				{
					this.beginOperation.Comments.AddRange(new SummaryStatements(
						"Executes the " + this.originalMethod + " method asynchronously."));

					this.beginOperationWithCallback.Comments.AddRange(new SummaryStatements(
						"Executes the " + this.originalMethod + " method asynchronously with a callback."));
					
					foreach (CodeParameterDeclarationExpression param in this.beginOperation.Parameters)
					{
						ParameterStatements paramComment = new ParameterStatements(
							param.Name, "See original method, " + this.originalMethod + ", for more information about this parameter.");
						
						this.beginOperation.Comments.AddRange(paramComment);
						this.beginOperationWithCallback.Comments.AddRange(paramComment);
					}

					this.beginOperationWithCallback.Comments.AddRange(new ParameterStatements(CallbackVariableName,
						"A method to be called when the asynchronous action completes."));

					this.beginOperation.Comments.AddRange(new ReturnsStatements(
						"An " + new SeeExpression(typeof(IAsyncResult)) + " object detailing the asynchronous action."));

					this.beginOperationWithCallback.Comments.AddRange(new ReturnsStatements(
						"An " + new SeeExpression(typeof(IAsyncResult)) + " object detailing the asynchronous action."));

					this.callbackDelegate.Comments.AddRange(new SummaryStatements(
						"Represents the delegate for asynchronous calls to " + this.originalMethod));

					this.callbackInstance.Comments.AddRange(new SummaryStatements(
						"Represents the delegate instance for asynchronous calls to " + this.originalMethod));

					this.endOperation.Comments.AddRange(new SummaryStatements("Synchronously completes an asynchronous call to " + this.originalMethod));

					this.endOperation.Comments.AddRange(new ParameterStatements(AsyncResultVariableName, 
						"The " + new SeeExpression(typeof(IAsyncResult)) + " retrieved from the call to " + new SeeExpression(this.beginOperation.Name)));

					this.endOperation.Comments.AddRange(new ExceptionStatements(typeof(InvalidOperationException),
						"Thrown when the method is called before the " + new SeeExpression(this.beginOperation.Name) + " method"));

					if (this.endOperation.ReturnType != null &&
						this.endOperation.ReturnType.BaseType != typeof(void).Name &&
						this.endOperation.ReturnType.BaseType != typeof(void).FullName)
					{
						this.endOperation.Comments.AddRange(new ReturnsStatements("The return value from the executed method."));
					}
				}
			}
		}

		#region Helper Methods
		private static CodeParameterDeclarationExpression[] ToParamArray(CodeParameterDeclarationExpressionCollection collection)
		{
			CodeParameterDeclarationExpression[] array = new CodeParameterDeclarationExpression[collection.Count];

			for (int i = 0; i < collection.Count; i++)
			{
				array[i] = collection[i];
			}

			return array;
		}

		private static CodeExpression[] GetVariableListWithCallback(CodeParameterDeclarationExpression[] parameters, string callbackVariableName)
		{
			CodeExpression[] variables = GetVariableListCore(parameters, parameters.Length + 2);

			variables[parameters.Length] = new CodeVariableReferenceExpression(callbackVariableName);

			return variables;
		}

		private static CodeExpression[] GetVariableList(CodeParameterDeclarationExpression[] parameters)
		{
			return GetVariableListCore(parameters, parameters.Length + 1);
		}

		private static CodeExpression[] GetVariableListCore(CodeParameterDeclarationExpression[] parameters, int length)
		{
			CodeExpression[] variables = new CodeExpression[length];

			for (int i = 0; i < parameters.Length; i++)
			{
				variables[i] = new CodeVariableReferenceExpression(parameters[i].Name);
			}

			variables[length - 1] = new CodePrimitiveExpression(null);

			return variables;
		}
		#endregion
	}
}
