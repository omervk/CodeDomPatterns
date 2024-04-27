using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of a field with a get accessor.
	/// </summary>
	/// <example>Output example:<code>
	///	private int m_Value;
	///
	///	public int Value
	///	{
	///		get
	///		{
	///			return this.m_Value;
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternGetField : CodeTypeMemberCollection
	{
		private const string FieldPrefix = "m_";

		private CodeMemberField field;
		private CodeMemberProperty property;
		private Scope scope;

		/// <summary>
		/// Initializes a new instance of the CodePatternGetField class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="scope">The scope of the event.</param>
		public CodePatternGetField(string name, CodeTypeReference type, Scope scope)
		{
			if (name == null || name == string.Empty)
			{
				throw new ArgumentNullException("name");
			}

			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			this.scope = scope;

			// Create the field.
			this.field = new CodeMemberField();
			this.field.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.field.Attributes |= MemberAttributes.Private | (scope == Scope.Static ? MemberAttributes.Static : 0);
			this.field.Name = FieldPrefix + name;
			this.field.Type = type;

			// Create the containing property.
			this.property = new CodeMemberProperty();
			this.property.Attributes &= ~MemberAttributes.AccessMask & (scope == Scope.Static ? ~MemberAttributes.ScopeMask : (MemberAttributes)int.MaxValue);
			this.property.Attributes |= MemberAttributes.Public | (scope == Scope.Static ? MemberAttributes.Static : 0);
			this.property.Name = name;
			this.property.Type = type;
			this.property.HasGet = true;
			this.property.GetStatements.Add(
				new CodeMethodReturnStatement(
					new CodeFieldReferenceExpression(
						(scope == Scope.Instance ? new CodeThisReferenceExpression() : null),
						this.field.Name
					)
				)
			);
			
			this.Add(this.field);
			this.Add(this.property);
		}

		#region Constructor overloads
		/// <summary>
		/// Initializes a new instance of the CodePatternGetField class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <remarks><see cref="Scope"/> defaults to <see cref="DotNetZen.CodeDom.Scope.Instance"/></remarks>
		public CodePatternGetField(string name, CodeTypeReference type)
			: this(name, type, Scope.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternGetField class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="scope">The scope of the event.</param>
		public CodePatternGetField(string name, string type, Scope scope)
			: this(name, CodeTypeReferenceStore.Get(type), scope)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternGetField class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <remarks><see cref="Scope"/> defaults to <see cref="DotNetZen.CodeDom.Scope.Instance"/></remarks>
		public CodePatternGetField(string name, string type)
			: this(name, type, Scope.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternGetField class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="scope">The scope of the event.</param>
		public CodePatternGetField(string name, Type type, Scope scope)
			: this(name, CodeTypeReferenceStore.Get(type), scope)
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternGetField class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="type">The type of the field.</param>
		/// <remarks><see cref="Scope"/> defaults to <see cref="DotNetZen.CodeDom.Scope.Instance"/></remarks>
		public CodePatternGetField(string name, Type type)
			: this(name, type, Scope.Instance)
		{
		}
		#endregion

		/// <summary>
		/// Gets the field declaration.
		/// </summary>
		public CodeMemberField Field
		{
			get
			{
				return this.field;
			}
		}

		/// <summary>
		/// Gets the property declaration.
		/// </summary>
		public CodeMemberProperty Property
		{
			get
			{
				return this.property;
			}
		}

		/// <summary>
		/// Gets the scope of the field/property.
		/// </summary>
		public Scope Scope
		{
			get
			{
				return this.scope;
			}
		}
	}
}
