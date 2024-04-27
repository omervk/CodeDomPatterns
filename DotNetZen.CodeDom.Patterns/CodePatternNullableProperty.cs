using System;
using System.CodeDom;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of a nullable value type field with a get/set accessors and null control actions.
	/// </summary>
	/// <example>Output example:<code>
	///	private int m_Value;
	///
	///	/// &lt;summary&gt;
	///	/// See &lt;see cref="IsValueNull" /&gt; for information about this field.
	///	/// &lt;/summary&gt;
	///	private bool m_IsValueNull = true;
	///
	///	public int Value
	///	{
	///		get
	///		{
	///			if ((this.m_IsValueNull == true))
	///			{
	///				throw new System.InvalidOperationException("Can not get value when it is null. Check for nullability by calling IsValueNull.");
	///			}
	///			return this.m_Value;
	///		}
	///		set
	///		{
	///			this.m_Value = value;
	///			this.m_IsValueNull = false;
	///		}
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Gets whether the value of &lt;see cref="Value" /&gt; is null.
	///	/// &lt;/summary&gt;
	///	/// &lt;value&gt;Whether the value of &lt;see cref="Value" /&gt; is null.&lt;/value&gt;
	///	public bool IsValueNull
	///	{
	///		get
	///		{
	///			return this.m_IsValueNull;
	///		}
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Sets the value of &lt;see cref="Value" /&gt; to null.
	///	/// &lt;/summary&gt;
	///	public void SetValueNull()
	///	{
	///		this.m_IsValueNull = true;
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternNullableProperty : CodePatternGetField
	{
		private const string FieldPrefix = "m_";
		private const string IsPrefix = "Is";
		private const string SetPrefix = "Set";
		private const string NullSuffix = "Null";

		private CodeMemberField nullabilityField;
		private CodeMemberProperty isFieldNull;
		private CodeMemberMethod setFieldNull;

		/// <summary>
		/// Initializes a new instance of the CodePatternNullableProperty class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="scope">The scope of the event.</param>
		public CodePatternNullableProperty(string name, CodeTypeReference type, Scope scope)
			: base(name, type, scope)
		{
			// Create the nullability indication field.
			this.nullabilityField = new CodeMemberField();
			this.nullabilityField.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.nullabilityField.Attributes |= MemberAttributes.Private | (scope == Scope.Static ? MemberAttributes.Static : 0);
			this.nullabilityField.Name = FieldPrefix + IsPrefix + name + NullSuffix;
			this.nullabilityField.Type = CodeTypeReferenceStore.Get(typeof(bool));
			this.nullabilityField.InitExpression = new CodePrimitiveExpression(true);

			CodeFieldReferenceExpression nullFieldReference = new CodeFieldReferenceExpression((scope == Scope.Instance ? new CodeThisReferenceExpression() : null), this.nullabilityField.Name);

			// Create the nullability testing property.
			this.isFieldNull = new CodeMemberProperty();
			this.isFieldNull.Attributes &= ~MemberAttributes.AccessMask & (scope == Scope.Static ? ~MemberAttributes.ScopeMask : (MemberAttributes)int.MaxValue);
			this.isFieldNull.Attributes |= MemberAttributes.Public | (scope == Scope.Static ? MemberAttributes.Static : 0);
			this.isFieldNull.Name = IsPrefix + name + NullSuffix;
			this.isFieldNull.Type = CodeTypeReferenceStore.Get(typeof(bool));
			this.isFieldNull.HasGet = true;
			this.isFieldNull.GetStatements.Add(new CodeMethodReturnStatement(
				new CodeFieldReferenceExpression((scope == Scope.Instance ? new CodeThisReferenceExpression() : null), this.nullabilityField.Name)));

			// Create the nullability setter method.
			this.setFieldNull = new CodeMemberMethod();
			this.setFieldNull.Attributes &= ~MemberAttributes.AccessMask;
			this.setFieldNull.Attributes |= MemberAttributes.Public | (scope == Scope.Static ? MemberAttributes.Static : 0);
			this.setFieldNull.Name = SetPrefix + name + NullSuffix;
			this.setFieldNull.Statements.Add(new CodeAssignStatement(nullFieldReference, new CodePrimitiveExpression(true)));
			
			// Alter the base class's declaration of the property to include set and null checks.
			this.Property.HasSet = true;
			this.Property.SetStatements.Add(new CodeAssignStatement(
				new CodeFieldReferenceExpression((scope == Scope.Instance ? new CodeThisReferenceExpression() : null), this.Field.Name),
				new CodePropertySetValueReferenceExpression()));
			this.Property.SetStatements.Add(new CodeAssignStatement(nullFieldReference, new CodePrimitiveExpression(false)));
			this.Property.GetStatements.Insert(0, 
				new CodeConditionStatement(new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanIsTrue, nullFieldReference),
				new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(InvalidOperationException), new CodePrimitiveExpression("Can not get value when it is null. Check for nullability by calling " + this.isFieldNull.Name + ".")))));

			this.Add(this.nullabilityField);
			this.Add(this.isFieldNull);
			this.Add(this.setFieldNull);

			this.HasComments = true;
		}

		#region Constructor overloads
		/// <summary>
		/// Initializes a new instance of the CodePatternNullableProperty class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <remarks><see cref="Scope"/> defaults to <see cref="Scope.Instance"/></remarks>
		public CodePatternNullableProperty(string name, CodeTypeReference type)
			: this(name, type, Scope.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternNullableProperty class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="scope">The scope of the event.</param>
		public CodePatternNullableProperty(string name, string type, Scope scope)
			: this(name, CodeTypeReferenceStore.Get(type), scope)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternNullableProperty class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <remarks><see cref="Scope"/> defaults to <see cref="Scope.Instance"/></remarks>
		public CodePatternNullableProperty(string name, string type)
			: this(name, type, Scope.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternNullableProperty class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="scope">The scope of the event.</param>
		public CodePatternNullableProperty(string name, Type type, Scope scope)
			: this(name, CodeTypeReferenceStore.Get(type), scope)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternNullableProperty class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <remarks><see cref="Scope"/> defaults to <see cref="Scope.Instance"/></remarks>
		public CodePatternNullableProperty(string name, Type type)
			: this(name, type, Scope.Instance)
		{
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
				return (this.setFieldNull.Comments.Count > 0);
			}
			set
			{
				this.setFieldNull.Comments.Clear();
				this.isFieldNull.Comments.Clear();
				this.nullabilityField.Comments.Clear();

				if (value)
				{
					this.setFieldNull.Comments.AddRange(new CommentsForMethod("Sets the value of " + new SeeExpression(this.Property.Name) + " to null."));
					this.isFieldNull.Comments.AddRange(new CommentsForProperty(PropertyAccessors.Get, "whether the value of " + new SeeExpression(this.Property.Name) + " is null."));
					this.nullabilityField.Comments.AddRange(new SummaryStatements("See " + new SeeExpression(this.isFieldNull.Name) + " for information about this field."));
				}
			}
		}
		
		/// <summary>
		/// Gets the nullability indication field.
		/// </summary>
		/// <value>The nullability indication field.</value>
		public CodeMemberField NullabilityField
		{
			get
			{
				return this.nullabilityField;
			}
		}
	
		/// <summary>
		/// Gets the nullability tester property.
		/// </summary>
		/// <value>The nullability tester property.</value>
		public CodeMemberProperty IsFieldNull
		{
			get
			{
				return this.isFieldNull;
			}
		}
	
		/// <summary>
		/// Gets the nullability setter method.
		/// </summary>
		/// <value>The nullability setter method.</value>
		public CodeMemberMethod SetFieldNull
		{
			get
			{
				return this.setFieldNull;
			}
		}
	}
}
