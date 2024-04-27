using System;
using System.CodeDom;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of a field with a get/set accessor and an event that is raised when the value is changed.
	/// </summary>
	/// <example>Output example:<code>
	///	/// &lt;summary&gt;
	///	/// Value for the property &lt;see cref="MyValue" /&gt;.
	///	/// &lt;/summary&gt;
	///	private int m_MyValue;
	///
	///	public int MyValue
	///	{
	///		get
	///		{
	///			return this.m_MyValue;
	///		}
	///		set
	///		{
	///			if ((this.m_MyValue != value))
	///			{
	///				int oldValue = this.m_MyValue;
	///				this.m_MyValue = value;
	///				this.OnMyValueChanged(new MyValueChangedEventArgs(oldValue, this.m_MyValue));
	///			}
	///		}
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Occurs when the &lt;see cref="MyValue" /&gt; property is changed.
	///	/// &lt;/summary&gt;
	///	public event MyValueChangedEventHandler MyValueChanged;
	///
	///	/// &lt;summary&gt;
	///	/// Raises the &lt;see cref="MyValueChanged" /&gt; event.
	///	/// &lt;/summary&gt;
	///	/// &lt;param name="e"&gt;The value passed for the event's e parameter.&lt;/param&gt;
	///	protected virtual void OnMyValueChanged(MyValueChangedEventArgs e)
	///	{
	///		if ((this.MyValueChanged != null))
	///		{
	///			this.MyValueChanged(this, e);
	///		}
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Represents a method that takes a &lt;see cref="System.Object" /&gt; and &lt;see cref="MyValueChangedEventArgs" /&gt;.
	///	/// &lt;/summary&gt;
	///	/// &lt;param name="sender"&gt;The event's originating object.&lt;/param&gt;
	///	/// &lt;param name="e"&gt;The event's arguments.&lt;/param&gt;
	///	public delegate void MyValueChangedEventHandler(object sender, MyValueChangedEventArgs e);
	///
	///	/// &lt;summary&gt;
	///	/// Contains the arguments for events based on the &lt;see cref="MyValueChangedEventHandler" /&gt; delegate.
	///	/// &lt;/summary&gt;
	///	public class MyValueChangedEventArgs : System.EventArgs
	///	{
	///		/// &lt;summary&gt;
	///		/// Value for the property &lt;see cref="OldValue" /&gt;.
	///		/// &lt;/summary&gt;
	///		private int m_OldValue;
	///
	///		/// &lt;summary&gt;
	///		/// Value for the property &lt;see cref="NewValue" /&gt;.
	///		/// &lt;/summary&gt;
	///		private int m_NewValue;
	///
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="MyValueChangedEventArgs" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="OldValue"&gt;The value before the change.&lt;/param&gt;
	///		/// &lt;param name="NewValue"&gt;The current value.&lt;/param&gt;
	///		public MyValueChangedEventArgs(int OldValue, int NewValue)
	///		{
	///			this.m_OldValue = OldValue;
	///			this.m_NewValue = NewValue;
	///		}
	///
	///		/// &lt;summary&gt;
	///		/// Gets the value before the change.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;The value before the change.&lt;/value&gt;
	///		public virtual int OldValue
	///		{
	///			get
	///			{
	///				return this.m_OldValue;
	///			}
	///		}
	///
	///		/// &lt;summary&gt;
	///		/// Gets the current value.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;The current value.&lt;/value&gt;
	///		public virtual int NewValue
	///		{
	///			get
	///			{
	///				return this.m_NewValue;
	///			}
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternObserver : CodeTypeMemberCollection
	{
		private const string Changed = "Changed";
		private const string NewValue = "NewValue";
		private const string OldValue = "OldValue";
		private const string OldValueVariable = "oldValue";

		private CodePatternDelegate delegatePattern;
		private CodePatternGetField getFieldPattern;
		private CodePatternEvent eventPattern;

		/// <summary>
		/// Initializes a new instance of the CodePatternObserver class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="fieldType">The type of the field.</param>
		/// <param name="scope">The scope of the event.</param>
		public CodePatternObserver(string name, CodeTypeReference fieldType, Scope scope)
		{
			if (name == null || name == string.Empty)
			{
				throw new ArgumentNullException("name");
			}

			if (fieldType == null)
			{
				throw new ArgumentNullException("fieldType");
			}

			// Create the delegate and event args class.
			delegatePattern = new CodePatternDelegate(name + Changed, 
				new CodeParameterDeclarationExpression(fieldType, OldValue),
				new CodeParameterDeclarationExpression(fieldType, NewValue));

			// Create the field and get property.
			getFieldPattern = new CodePatternGetField(name, fieldType, scope);

			// Create the event and invocation method.
			eventPattern = new CodePatternEvent(name + Changed, scope, CodeTypeReferenceStore.Get(delegatePattern.Delegate.Name), delegatePattern.Delegate);

			CodeFieldReferenceExpression fieldReference = new CodeFieldReferenceExpression((scope == Scope.Static ? null : new CodeThisReferenceExpression()), getFieldPattern.Field.Name);
			CodeMethodReferenceExpression invokerReference = new CodeMethodReferenceExpression((scope == Scope.Static ? null : new CodeThisReferenceExpression()), eventPattern.Invoker.Name);
			
			// Create the parameters for calling the invocation method.
			CodeExpression[] invokerParameters = new CodeExpression[(scope == Scope.Static ? 2 : 1)];

			if (scope == Scope.Static)
			{
				invokerParameters[0] = new CodePrimitiveExpression(null);
			}

			invokerParameters[invokerParameters.Length - 1] = 
				new CodeObjectCreateExpression(
					CodeTypeReferenceStore.Get(delegatePattern.EventArgs.Name),
					new CodeVariableReferenceExpression(OldValueVariable),
					fieldReference
				);

			// Add set accessor to the field's property.
			getFieldPattern.Property.HasSet = true;
			getFieldPattern.Property.SetStatements.Add(
				new CodeConditionStatement(
					new CodeBinaryOperatorExpression(
						fieldReference,
						CodeBinaryOperatorType.IdentityInequality,
						new CodePropertySetValueReferenceExpression()
					),
					new CodeStatement[] {
											new CodeVariableDeclarationStatement(fieldType, OldValueVariable, fieldReference),
											new CodeAssignStatement(fieldReference, new CodePropertySetValueReferenceExpression()),
											new CodeExpressionStatement(new CodeMethodInvokeExpression(invokerReference, invokerParameters))
										}
				)
			);

			this.Add(delegatePattern.Delegate);
			this.Add(delegatePattern.EventArgs);
			this.AddRange(getFieldPattern);
			this.AddRange(eventPattern);

			this.HasComments = true;
		}

		/// <summary>
		/// Gets the delegate pattern used in this pattern.
		/// </summary>
		public CodePatternDelegate DelegatePattern
		{
			get
			{
				return this.delegatePattern;
			}
		}

		/// <summary>
		/// Gets the field/property pattern used in this pattern.
		/// </summary>
		public CodePatternGetField GetFieldPattern
		{
			get
			{
				return this.getFieldPattern;
			}
		}

		/// <summary>
		/// Gets the event pattern used in this pattern.
		/// </summary>
		public CodePatternEvent EventPattern
		{
			get
			{
				return this.eventPattern;
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
				return (this.delegatePattern.HasComments);
			}
			set
			{
				this.delegatePattern.HasComments = value;
				this.eventPattern.HasComments = value;
				this.getFieldPattern.Field.Comments.Clear();

				if (value)
				{
					this.delegatePattern.SetComment(OldValue, "The value before the change.");
					this.delegatePattern.SetComment(NewValue, "The current value.");

					this.eventPattern.Event.Comments.AddRange(new SummaryStatements(
						"Occurs when the " + new SeeExpression(this.getFieldPattern.Property.Name) + " property is changed."));

					this.getFieldPattern.Field.Comments.AddRange(new SummaryStatements(
						"Value for the property " + new SeeExpression(this.getFieldPattern.Property.Name)));
				}
			}
		}
	}
}
