using System;
using System.CodeDom;
using System.Collections;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of a custom attribute.
	/// </summary>
	/// <example>Output example:<code>
	///	[System.AttributeUsageAttribute(((System.AttributeTargets.Enum | System.AttributeTargets.Struct) 
	///		 | System.AttributeTargets.Class), AllowMultiple=false, Inherited=true)]
	///	public sealed class CoolMetaDataAttribute : System.Attribute
	///	{
	///		/// &lt;summary&gt;
	///		/// Value for the property &lt;see cref="MetaData" /&gt;.
	///		/// &lt;/summary&gt;
	///		private int m_MetaData;
	///    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="CoolMetaDataAttribute" /&gt; class.
	///		/// &lt;/summary&gt;
	///		public CoolMetaDataAttribute()
	///		{
	///		}
	///    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="CoolMetaDataAttribute" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="MetaData"&gt;The metadata for the attribute.&lt;/param&gt;
	///		public CoolMetaDataAttribute(int MetaData)
	///		{
	///			this.m_MetaData = MetaData;
	///		}
	///    
	///		/// &lt;summary&gt;
	///		/// Gets the metadata for the attribute.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;The metadata for the attribute.&lt;/value&gt;
	///		public int MetaData
	///		{
	///			get
	///			{
	///				return this.m_MetaData;
	///			}
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternCustomAttributeDeclaration : CodeTypeDeclaration
	{
		private const string AttributeSuffix = "Attribute";
		private static readonly string AllowMultipleName = typeof(AttributeUsageAttribute).GetProperty("AllowMultiple").Name;
		private static readonly string InheritedName = typeof(AttributeUsageAttribute).GetProperty("Inherited").Name;

		private System.Collections.Specialized.NameValueCollection fieldComments = new System.Collections.Specialized.NameValueCollection();
		private CodeAttributeDeclaration attributeUsage;
		private CodeConstructor defaultConstructor;
		private CodeConstructor parameterizedConstructor;
		private CodePatternGetField[] fields;

		/// <summary>
		/// Initializes a new instance of the CodePatternCustomAttributeDeclaration class.
		/// </summary>
		/// <param name="name">The name of the attribute sans the Attribute suffix.</param>
		/// <param name="attributeUsage">Specifies the application elements on which it is valid to apply the attribute.</param>
		/// <param name="parameters">The parameters to the attribute's constructor.</param>
		public CodePatternCustomAttributeDeclaration(string name,
			AttributeTargets attributeUsage,
			params CodeParameterDeclarationExpression[] parameters)
			: base(name + AttributeSuffix)
		{
			if (name == null || name == string.Empty)
			{
				throw new ArgumentNullException("name");
			}

			// Attributes are sealed, based on a design guideline from Microsoft.
			this.TypeAttributes |= System.Reflection.TypeAttributes.Sealed;

			// This type is a custom attribute
			this.BaseTypes.Add(typeof(Attribute));

			this.attributeUsage = new CodeAttributeDeclaration(
				typeof(AttributeUsageAttribute).FullName, 
				new CodeAttributeArgument(ReflectionUtilities.GetFlagsValueExpression((int)attributeUsage, typeof(AttributeTargets))));

			// Add the attribute usage
			this.CustomAttributes.Add(this.attributeUsage);

			// Add default constructor
			this.defaultConstructor = new CodeConstructor();
			this.defaultConstructor.Attributes &= ~MemberAttributes.AccessMask;
			this.defaultConstructor.Attributes |= MemberAttributes.Public;

			this.Members.Add(this.defaultConstructor);

			// Handle attribute parameters.
			if (parameters.Length > 0)
			{
				this.fields = new CodePatternGetField[parameters.Length];

				// Adds the parameterized constructor
				this.parameterizedConstructor = new CodeConstructor();
				this.parameterizedConstructor.Attributes &= ~MemberAttributes.AccessMask;
				this.parameterizedConstructor.Attributes |= MemberAttributes.Public;

				for (int i = 0; i < parameters.Length; i++)
				{
					// Add the field and property to the type.
					this.fields[i] = new CodePatternGetField(parameters[i].Name, parameters[i].Type, Scope.Instance);
					this.Members.AddRange(this.fields[i]);

					// Add parameter and initialization to the constructor.
					this.parameterizedConstructor.Parameters.Add(new CodeParameterDeclarationExpression(parameters[i].Type, parameters[i].Name));
					this.parameterizedConstructor.Statements.Add(
						new CodeAssignStatement(
							new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.fields[i].Field.Name),
							new CodeVariableReferenceExpression(parameters[i].Name)));
				}

				this.Members.Add(this.parameterizedConstructor);
			}

			this.HasComments = true;
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternCustomAttributeDeclaration class.
		/// </summary>
		/// <param name="name">The name of the attribute sans the Attribute suffix.</param>
		/// <param name="attributeUsage">Specifies the application elements on which it is valid to apply the attribute.</param>
		/// <param name="allowMultiple">true if the indicated attribute can be specified more than once for a given program element; otherwise, false.</param>
		/// <param name="inherited">true if the indicated attribute can be inherited by derived classes and overriding members; otherwise, false.</param>
		/// <param name="parameters">The parameters to the attribute's constructor.</param>
		public CodePatternCustomAttributeDeclaration(string name, 
			AttributeTargets attributeUsage, 
			bool allowMultiple, 
			bool inherited,
			params CodeParameterDeclarationExpression[] parameters)
			: this(name, attributeUsage, parameters)
		{
			// Add the attribute usage
			this.attributeUsage.Arguments.Add(new CodeAttributeArgument(AllowMultipleName, new CodePrimitiveExpression(allowMultiple)));
			this.attributeUsage.Arguments.Add(new CodeAttributeArgument(InheritedName, new CodePrimitiveExpression(inherited)));
		}
		
		/// <summary>
		/// Gets or sets the attribute's usage targets.
		/// </summary>
		public AttributeTargets AttributeUsage
		{
			get
			{
				return (AttributeTargets)ReflectionUtilities.GetFlagsExpressionValue(this.attributeUsage.Arguments[0].Value);
			}
			set
			{
				this.attributeUsage.Arguments[0].Value = ReflectionUtilities.GetFlagsValueExpression((int)value, typeof(AttributeTargets));
			}
		}

		/// <summary>
		/// Gets the default, parameterless, public constructor.
		/// </summary>
		public CodeConstructor DefaultConstructor
		{
			get
			{
				return this.defaultConstructor;
			}
		}

		/// <summary>
		/// Gets the public constructor that initializes all parameters.
		/// </summary>
		public CodeConstructor ParameterizedConstructor
		{
			get
			{
				return this.parameterizedConstructor;
			}
		}

		/// <summary>
		/// Gets the list of fields of this attribute.
		/// </summary>
		public CodePatternGetField[] Fields
		{
			get
			{
				return this.fields;
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
				return (this.defaultConstructor.Comments.Count > 0);
			}
			set
			{
				this.defaultConstructor.Comments.Clear();
				this.parameterizedConstructor.Comments.Clear();

				if (value)
				{
					this.defaultConstructor.Comments.AddRange(new CommentsForConstructor(this.Name));

					ArrayList list = new ArrayList(this.fieldComments.Count);

					foreach (string entry in this.fieldComments)
					{
						if (this.fieldComments[entry] != null)
						{
							list.Add(new ParameterStatements(entry, this.fieldComments[entry]));
						}
					}

					this.parameterizedConstructor.Comments.AddRange(new CommentsForConstructor(this.Name, (ParameterStatements[])list.ToArray(typeof(ParameterStatements))));
				}

				foreach (CodePatternGetField field in this.fields)
				{
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
		/// Sets the comment for a property of the attribute.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="comment">The comment for the parameter.</param>
		/// <exception cref="NotSupportedException">Thrown when the property does not exist.</exception>
		public void SetComment(string propertyName, string comment)
		{
			foreach (CodePatternGetField field in this.fields)
			{
				if (field.Property.Name == propertyName)
				{
					this.fieldComments[propertyName] = comment;
					this.HasComments = this.HasComments; // Refresh.
					return;
				}
			}

			throw new NotSupportedException("The property name " + propertyName + " is not a property in this type.");
		}
	}
}
