using System;
using System.CodeDom;
using System.Collections;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of an EventHandler delegate with a specialized EventArgs class.
	/// </summary>
	/// <example>Output example:<code>
	///	/// &lt;summary&gt;
	///	/// Represents a method that takes a &lt;see cref="System.Object" /&gt; and &lt;see cref="ItemChangedEventArgs" /&gt;.
	///	/// &lt;/summary&gt;
	///	/// &lt;param name="sender"&gt;The event's originating object.&lt;/param&gt;
	///	/// &lt;param name="e"&gt;The event's arguments.&lt;/param&gt;
	///	public delegate void ItemChangedEventHandler(object sender, ItemChangedEventArgs e);
	///    
	///	/// &lt;summary&gt;
	///	/// Contains the arguments for events based on the &lt;see cref="ItemChangedEventHandler" /&gt; delegate.
	///	/// &lt;/summary&gt;
	///	public class ItemChangedEventArgs : System.EventArgs
	///	{
	///        
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
	///		/// Initializes a new instance of the &lt;see cref="ItemChangedEventArgs" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="OldValue"&gt;The value before the change.&lt;/param&gt;
	///		/// &lt;param name="NewValue"&gt;The value after the change.&lt;/param&gt;
	///		public ItemChangedEventArgs(int OldValue, int NewValue)
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
	///		/// Gets the value after the change.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;The value after the change.&lt;/value&gt;
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
	public class CodePatternDelegate : DotNetZen.CodeDom.CodeTypeDeclarationCollection
	{
		[Serializable]
		private sealed class CodeEventArgsDeclaration : CodeTypeDeclaration
		{
			public CodeEventArgsDeclaration(string name) : base(name) {}

			public CodeConstructor constructor = new CodeConstructor();
			public Hashtable fields = new Hashtable();
		}

		private static readonly string SenderName = typeof(System.EventHandler).GetMethod("Invoke").GetParameters()[0].Name;
		private static readonly string EName = typeof(System.EventHandler).GetMethod("Invoke").GetParameters()[1].Name;
		
		private System.Collections.Specialized.NameValueCollection fieldComments = new System.Collections.Specialized.NameValueCollection();
		private CodeTypeDelegate @delegate;
		private CodeEventArgsDeclaration eventArgs;

		/// <summary>
		/// Initializes a new instance of the CodePatternDelegate class.
		/// </summary>
		/// <param name="name">The name of the delegate.</param>
		/// <param name="returnValue">The return type for the delegate.</param>
		/// <param name="parameters">The parameters that should be in the EventArgs.</param>
		/// <remarks>The EventHandler and EventArgs suffixes are appended automatically to the name.</remarks>
		public CodePatternDelegate(string name, CodeTypeReference returnValue, params CodeParameterDeclarationExpression[] parameters)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			// Create the delegate.
			this.@delegate = new CodeTypeDelegate(name + typeof(EventHandler).Name);
			this.@delegate.Attributes |= MemberAttributes.Public;
			this.@delegate.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), SenderName));
			this.@delegate.Parameters.Add(new CodeParameterDeclarationExpression(name + typeof(EventArgs).Name, EName));
			this.@delegate.ReturnType = returnValue;

			// Create the EventArgs.
			this.eventArgs = new CodeEventArgsDeclaration(name + typeof(EventArgs).Name);
			this.eventArgs.Attributes |= MemberAttributes.Public;
			this.eventArgs.BaseTypes.Add(typeof(System.EventArgs));

			// Create the constructor for the EventArgs.
			this.eventArgs.constructor.Attributes &= ~MemberAttributes.AccessMask;
			this.eventArgs.constructor.Attributes |= MemberAttributes.Public;
			this.eventArgs.constructor.Parameters.AddRange(parameters);

			foreach (CodeParameterDeclarationExpression parameter in parameters)
			{
				// Create the field/property for each parameter.
				CodePatternGetField getField = new CodePatternGetField(parameter.Name, parameter.Type, Scope.Instance);
				getField.Property.Attributes &= ~MemberAttributes.ScopeMask;

				this.eventArgs.Members.AddRange(getField);
				this.eventArgs.fields.Add(parameter.Name, getField);

				// Add initialization to the constructor.
				this.eventArgs.constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), getField.Field.Name), new CodeVariableReferenceExpression(parameter.Name)));
			}

			// Add the parameters to the constructor.
			this.eventArgs.Members.Add(this.eventArgs.constructor);

			this.Add(@delegate);
			this.Add(eventArgs);

			this.HasComments = true;
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternDelegate class.
		/// </summary>
		/// <param name="name">The name of the delegate.</param>
		/// <param name="returnValue">The return type for the delegate.</param>
		/// <param name="parameters">The parameters that should be in the EventArgs.</param>
		/// <remarks>The EventHandler and EventArgs suffixes are appended automatically to the name.</remarks>
		public CodePatternDelegate(string name, Type returnValue, params CodeParameterDeclarationExpression[] parameters)
			: this(name, CodeTypeReferenceStore.Get(returnValue), parameters)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternDelegate class with a void return value.
		/// </summary>
		/// <param name="name">The name of the delegate.</param>
		/// <param name="parameters">The parameters that should be in the EventArgs.</param>
		/// <remarks>The EventHandler and EventArgs suffixes are appended automatically to the name.</remarks>
		public CodePatternDelegate(string name, params CodeParameterDeclarationExpression[] parameters)
			: this(name, (CodeTypeReference)null, parameters)
		{
		}

		/// <summary>
		/// Gets the delegate.
		/// </summary>
		public CodeTypeDelegate Delegate
		{
			get
			{
				return this.@delegate;
			}
		}

		/// <summary>
		/// Gets the specialized EventArgs class.
		/// </summary>
		public CodeTypeDeclaration EventArgs
		{
			get
			{
				return this.eventArgs;
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
				return (this.@delegate.Comments.Count > 0);
			}
			set
			{
				this.@delegate.Comments.Clear();
				this.eventArgs.Comments.Clear();
				this.eventArgs.constructor.Comments.Clear();

				if (value)
				{
					CommentsForDelegate comment = new CommentsForDelegate("Represents a method that takes a " + new SeeExpression(typeof(object)) + " and " + new SeeExpression(eventArgs.Name),
						new ParameterStatements(SenderName, "The event's originating object."),
						new ParameterStatements(EName, "The event's arguments."));
					this.@delegate.Comments.AddRange(comment);

					this.eventArgs.Comments.AddRange(new SummaryStatements("Contains the arguments for events based on the " + new SeeExpression(this.@delegate.Name) + " delegate."));
						
					ArrayList list = new ArrayList(this.fieldComments.Count);

					foreach (string entry in this.fieldComments)
					{
						if (this.fieldComments[entry] != null)
						{
							list.Add(new ParameterStatements(entry, this.fieldComments[entry]));
						}
					}

					this.eventArgs.constructor.Comments.AddRange(new CommentsForConstructor(this.eventArgs.Name, (ParameterStatements[])list.ToArray(typeof(ParameterStatements))));
				}

				foreach (DictionaryEntry entry in this.eventArgs.fields)
				{
					CodePatternGetField field = (CodePatternGetField)entry.Value;
					field.Field.Comments.Clear();
					field.Property.Comments.Clear();

					if (value && this.fieldComments[field.Property.Name] != null)
					{
						field.Field.Comments.AddRange(new SummaryStatements("Value for the property " + new SeeExpression(field.Property.Name)));
						field.Property.Comments.AddRange(new CommentsForProperty(PropertyAccessors.Get, this.fieldComments[field.Property.Name]));
					}
				}
			}
		}

		/// <summary>
		/// Sets the comment for a property of the event arguments.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="comment">The comment for the parameter.</param>
		/// <exception cref="NotSupportedException">Thrown when the property does not exist.</exception>
		public void SetComment(string propertyName, string comment)
		{
			if (!this.eventArgs.fields.ContainsKey(propertyName))
			{
				throw new NotSupportedException("The property name " + propertyName + " is not a property in this type.");
			}

			this.fieldComments[propertyName] = comment;
			this.HasComments = this.HasComments; // Refresh.
		}
	}
}
