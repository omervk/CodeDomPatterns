using System;
using System.CodeDom;
using System.Reflection;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of an event with an invoker method.
	/// </summary>
	/// <remarks>Please note: A bug in the .net frameworks up to 2.0 does not allow for creation of static events,
	/// which means that the pattern is incomplete. Please vote on this issue:
	/// http://lab.msdn.microsoft.com/ProductFeedback/viewFeedback.aspx?feedbackId=FDBK39456 </remarks>
	///	<example>Instance event output example:<code>
	///	public event System.EventHandler EventHappened;
	///
	///	/// &lt;summary&gt;
	///	/// Raises the &lt;see cref="EventHappened" /&gt; event.
	///	/// &lt;/summary&gt;
	///	/// &lt;param name="e"&gt;The value passed for the event's e parameter.&lt;/param&gt;
	///	protected virtual void OnEventHappened(System.EventArgs e)
	///	{
	///		if ((this.EventHappened != null))
	///		{
	///			this.EventHappened(this, e);
	///		}
	///	}
	///	</code>
	///	Static event output example:<code>
	///	public static event System.EventHandler EventHappened;
	///
	///	/// &lt;summary&gt;
	///	/// Raises the &lt;see cref="EventHappened" /&gt; event.
	///	/// &lt;/summary&gt;
	///	/// &lt;param name="sender"&gt;The originating object of the event call.&lt;/param&gt;
	///	/// &lt;param name="e"&gt;The value passed for the event's e parameter.&lt;/param&gt;
	///	protected static void OnEventHappened(object sender, System.EventArgs e)
	///	{
	///		if ((MyClass.EventHappened != null))
	///		{
	///			MyClass.EventHappened(sender, e);
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternEvent : CodeTypeMemberCollection
	{
		private const string InvokerPrefix = "On";
		private static readonly string InvokeName = typeof(EventHandler).GetMethod("Invoke").Name;
		private static readonly string SenderName = typeof(EventHandler).GetMethod("Invoke").GetParameters()[0].Name;

		private CodeMemberEvent @event;
		private CodeMemberMethod invoker;

		/// <summary>
		/// Initializes a new instance of the CodePatternEvent class.
		/// </summary>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="scope">The scope of the event.</param>
		/// <param name="delegateReference">A reference to the delegate type.</param>
		/// <param name="returnType">The delegate's return type.</param>
		/// <param name="parameters">The delegate's parameters.</param>
		public CodePatternEvent(string eventName, Scope scope, CodeTypeReference delegateReference, CodeTypeReference returnType, params CodeParameterDeclarationExpression[] parameters)
		{
			if (delegateReference == null)
			{
				throw new ArgumentNullException("delegateReference");
			}

			CodeEventReferenceExpression eventReference = new CodeEventReferenceExpression((scope == Scope.Instance ? new CodeThisReferenceExpression() : null), eventName);

			// Create the event.
			this.@event = new CodeMemberEvent();
			this.@event.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.@event.Attributes |= MemberAttributes.Public | (scope == Scope.Static ? MemberAttributes.Static : 0);
			this.@event.Name = eventName;
			this.@event.Type = delegateReference;

			// Create the invoking method.
			this.invoker = new CodeMemberMethod();
			this.invoker.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.invoker.Attributes |= MemberAttributes.Family | (scope == Scope.Static ? MemberAttributes.Static : 0);
			this.invoker.Name = InvokerPrefix + eventName;
			this.invoker.ReturnType = returnType;

			CodeExpression[] paramsList = new CodeExpression[parameters.Length];

			int parameterCount = parameters.Length;

			CodeExpression selfReference = (scope == Scope.Instance ? new CodeThisReferenceExpression() : null);

			// Instance events that have a sender/object parameter will be invoked with 'this' as the parameter value.
			if (scope == Scope.Instance && 
				parameterCount > 0 && 
				parameters[0].Name == SenderName &&
				parameters[0].Type.BaseType == CodeTypeReferenceStore.Get(typeof(object)).BaseType)
			{
				parameterCount--;

				paramsList[0] = selfReference;
			}

			// Add the rest of the parameters to the call and the list of parameters for the invoking method.
			for (int i = (parameters.Length > parameterCount ? 1 : 0); i < parameters.Length; i++)
			{
				paramsList[i] = new CodeVariableReferenceExpression(parameters[i].Name);
				this.invoker.Parameters.Add(parameters[i]);
			}

			// The event invocation expression.
			CodeStatement delegateInvocationExpression =
				new CodeExpressionStatement(new CodeDelegateInvokeExpression(
					eventReference,
					paramsList));

			// If the event has a return type, we must return the value from the event invokation.
			if (returnType != null && returnType.BaseType != CodeTypeReferenceStore.Get(typeof(void)).BaseType)
			{
				delegateInvocationExpression = new CodeMethodReturnStatement(((CodeExpressionStatement)(delegateInvocationExpression)).Expression);
			}
			
			// Add the statements to the method.
			this.invoker.Statements.Add(
					new CodeConditionStatement(
						new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNotNull, eventReference),
						delegateInvocationExpression
					)
				);

			// Add members to the collection.
			this.Add(this.@event);
			this.Add(this.invoker);

			this.HasComments = true;
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternEvent class.
		/// </summary>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="scope">The scope of the event.</param>
		/// <param name="delegateReference">A reference to the delegate type.</param>
		/// <param name="delegate">The declaration of the delegate to use.</param>
		public CodePatternEvent(string eventName, Scope scope, CodeTypeReference delegateReference, CodeTypeDelegate @delegate)
			: this(eventName, scope, delegateReference, GetReturnType(@delegate), GetArray(@delegate.Parameters))
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternEvent class.
		/// </summary>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="scope">The scope of the event.</param>
		/// <param name="delegate">The delegate type to use for the event.</param>
		public CodePatternEvent(string eventName, Scope scope, Type @delegate)
			: this(eventName, scope, CodeTypeReferenceStore.Get(@delegate),
				CodeTypeReferenceStore.Get(@delegate.GetMethod(InvokeName).ReturnType),
				GetArray(@delegate.GetMethod(InvokeName).GetParameters()))
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternEvent class.
		/// </summary>
		/// <param name="eventName">The name of the event.</param>
		/// <param name="scope">The scope of the event.</param>
		/// <param name="delegateReference">A reference to the delegate type.</param>
		/// <param name="parameters">The delegate's parameters.</param>
		public CodePatternEvent(string eventName, Scope scope, CodeTypeReference delegateReference, params CodeParameterDeclarationExpression[] parameters)
			: this(eventName, scope, delegateReference, null, parameters)
		{
		}

		private static CodeTypeReference GetReturnType(CodeTypeDelegate @delegate)
		{
			if (@delegate == null)
			{
				throw new ArgumentNullException("delegate");
			}

			return @delegate.ReturnType;
		}

		private static CodeParameterDeclarationExpression[] GetArray(CodeParameterDeclarationExpressionCollection collection)
		{
			CodeParameterDeclarationExpression[] array = new CodeParameterDeclarationExpression[collection.Count];

			for (int i = 0; i < array.Length; i++)
			{
				array[i] = collection[i];
			}

			return array;
		}

		private static CodeParameterDeclarationExpression[] GetArray(ParameterInfo[] parameters)
		{
			CodeParameterDeclarationExpression[] array = new CodeParameterDeclarationExpression[parameters.Length];

			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new CodeParameterDeclarationExpression(parameters[i].ParameterType, parameters[i].Name);
			}

			return array;
		}
		
		/// <summary>
		/// Gets the event declaration.
		/// </summary>
		public CodeMemberEvent Event
		{
			get
			{
				return this.@event;
			}
		}

		/// <summary>
		/// Gets the event invoking method.
		/// </summary>
		public CodeMemberMethod Invoker
		{
			get
			{
				return this.invoker;
			}
		}
		
		/// <summary>
		/// Gets or sets whether the members generated by the pattern have comments.
		/// </summary>
		/// <value>Whether the members generated by the pattern have comments.</value>
		public bool HasComments
		{
			get
			{
				return (this.invoker.Comments.Count > 0);
			}
			set
			{
				this.invoker.Comments.Clear();

				if (value)
				{
					ParameterStatements[] parameters = new ParameterStatements[this.invoker.Parameters.Count];
					int index = 0;

					if ((this.@event.Attributes & MemberAttributes.Static) == MemberAttributes.Static)
					{
						parameters[0] = new ParameterStatements(SenderName, "The originating object of the event call.");
						index++;
					}

					for (; index < parameters.Length; index++)
					{
						parameters[index] = new ParameterStatements(this.invoker.Parameters[index].Name, 
							"The value passed for the event's " + this.invoker.Parameters[index].Name + " parameter.");
					}

					this.invoker.Comments.AddRange(new CommentsForMethod("Raises the " + new SeeExpression(this.@event.Name) + " event.",
						parameters));
				}
			}
		}
	}
}
