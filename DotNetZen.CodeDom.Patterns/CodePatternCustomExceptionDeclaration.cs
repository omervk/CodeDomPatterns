using System;
using System.CodeDom;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of a custom exception.
	/// </summary>
	/// <example>Output example:<code>
	///	[System.SerializableAttribute()]
	///	public class FooException : System.Exception
	///	{
	///		/// &lt;summary&gt;
	///		/// Value for the property &lt;see cref="Bar" /&gt;.
	///		/// &lt;/summary&gt;
	///		private int m_Bar;
	///    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="FooException" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="Bar"&gt;A healthy snack-bar.&lt;/param&gt;
	///		public FooException(int Bar)
	///		{
	///			this.m_Bar = Bar;
	///		}
	///    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="FooException" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="Bar"&gt;A healthy snack-bar.&lt;/param&gt;
	///		/// &lt;param name="message"&gt;The message in the exception.&lt;/param&gt;
	///		public FooException(int Bar, string message) : 
	///			base(message)
	///		{
	///			this.m_Bar = Bar;
	///		}
	///    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="FooException" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="info"&gt;The data needed to serialize or deserialize an object.&lt;/param&gt;
	///		/// &lt;param name="context"&gt;The source and destination of a given serialized stream.&lt;/param&gt;
	///		/// &lt;remarks&gt;This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.&lt;/remarks&gt;
	///		protected FooException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
	///			base(info, context)
	///		{
	///			this.m_Bar = ((int)(info.GetValue("m_Bar", typeof(int))));
	///		}
	///    
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of the &lt;see cref="FooException" /&gt; class.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="Bar"&gt;A healthy snack-bar.&lt;/param&gt;
	///		/// &lt;param name="message"&gt;The message in the exception.&lt;/param&gt;
	///		/// &lt;param name="innerException"&gt;An exception encapsulated in the new exception.&lt;/param&gt;
	///		public FooException(int Bar, string message, System.Exception innerException) : 
	///			base(message, innerException)
	///		{
	///			this.m_Bar = Bar;
	///		}
	///    
	///		/// &lt;summary&gt;
	///		/// Gets a healthy snack-bar.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;A healthy snack-bar.&lt;/value&gt;
	///		public int Bar
	///		{
	///			get
	///			{
	///				return this.m_Bar;
	///			}
	///		}
	///    
	///		/// &lt;summary&gt;
	///		/// Populates a &lt;see cref="System.Runtime.Serialization.SerializationInfo" /&gt; with the data needed to serialize the target object.
	///		/// &lt;/summary&gt;
	///		/// &lt;param name="info"&gt;The &lt;see cref="System.Runtime.Serialization.SerializationInfo" /&gt; to populate with data.&lt;/param&gt;
	///		/// &lt;param name="context"&gt;The destination (see &lt;see cref="System.Runtime.Serialization.StreamingContext" /&gt;) for this serialization.&lt;/param&gt;
	///		/// &lt;exception cref="System.ArgumentNullException"&gt;Thrown when the &lt;paramref name="info" /&gt; parameter is a null reference (Nothing in Visual Basic).&lt;/exception&gt;
    ///		[System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
	///		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
	///		{
	///			base.GetObjectData(info, context);
	///			info.AddValue("m_Bar", this.m_Bar, typeof(int));
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternCustomExceptionDeclaration : CodePatternTypeDeclaration
	{
		private const string ExceptionSuffix = "Exception";
		private const string MessageParameterName = "message";
		private const string InnerExceptionParameterName = "innerException";

		private System.Collections.Specialized.NameValueCollection fieldComments = new System.Collections.Specialized.NameValueCollection();
		private CodeConstructor parameterlessConstructor;
		private CodeConstructor messageConstructor;
		private CodeConstructor innerExceptionConstructor;
		private CodePatternGetField[] getFields;
		private CodeCommentStatement[] serializationConstructorComments, getObjectDataComments;

		/// <summary>
		/// Initializes a new instance of the CodePatternCustomExceptionDeclaration class.
		/// </summary>
		/// <param name="name">The name of the exception sans the Exception suffix.</param>
		/// <param name="fields">The custom fields in the exception.</param>
		public CodePatternCustomExceptionDeclaration(string name, params System.CodeDom.CodeParameterDeclarationExpression[] fields) : base(name + ExceptionSuffix)
		{
			if (name == null || name == string.Empty)
			{
				throw new ArgumentNullException("name");
			}

			this.BaseTypes.Add(typeof(Exception));

			// Prepare parameters and initialization
			this.getFields = new CodePatternGetField[fields.Length];
			CodeParameterDeclarationExpression[] parameters = new CodeParameterDeclarationExpression[fields.Length];
			CodeAssignStatement[] assignStatements = new CodeAssignStatement[fields.Length];

			for (int i = 0; i < fields.Length; i++)
			{
				getFields[i] = new CodePatternGetField(fields[i].Name, fields[i].Type, Scope.Instance);

				parameters[i] = new CodeParameterDeclarationExpression(getFields[i].Field.Type, getFields[i].Property.Name);
				assignStatements[i] = new CodeAssignStatement(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), getFields[i].Field.Name),
					new CodeVariableReferenceExpression(getFields[i].Property.Name));
			}

			// Prepare parameterless (unless fields are appended) constructor.
			this.parameterlessConstructor = new CodeConstructor();
			this.parameterlessConstructor.Attributes &= ~MemberAttributes.AccessMask;
			this.parameterlessConstructor.Attributes |= MemberAttributes.Public;
			this.parameterlessConstructor.Parameters.AddRange(parameters);
			this.parameterlessConstructor.Statements.AddRange(assignStatements);
			this.Members.Add(this.parameterlessConstructor);

			// Prepare the constructor only with message (unless fields are appended).
			this.messageConstructor = new CodeConstructor();
			this.messageConstructor.Attributes &= ~MemberAttributes.AccessMask;
			this.messageConstructor.Attributes |= MemberAttributes.Public;
			this.messageConstructor.Parameters.AddRange(parameters);
			this.messageConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), MessageParameterName));
			this.messageConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(MessageParameterName));
			this.messageConstructor.Statements.AddRange(assignStatements);
			this.Members.Add(this.messageConstructor);
			
			// Prepare the constructor only with message and inner exception (unless fields are appended).
			this.innerExceptionConstructor = new CodeConstructor();
			this.innerExceptionConstructor.Attributes &= ~MemberAttributes.AccessMask;
			this.innerExceptionConstructor.Attributes |= MemberAttributes.Public;
			this.innerExceptionConstructor.Parameters.AddRange(parameters);
			this.innerExceptionConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), MessageParameterName));
			this.innerExceptionConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Exception), InnerExceptionParameterName));
			this.innerExceptionConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(MessageParameterName));
			this.innerExceptionConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(InnerExceptionParameterName));
			this.innerExceptionConstructor.Statements.AddRange(assignStatements);
			this.Members.Add(this.innerExceptionConstructor);

			// Add field members.
			foreach (CodePatternGetField field in getFields)
			{
				this.Members.AddRange(field);
			}

			// Apply serialization pattern.
			CodeMemberField[] serializedFields = new CodeMemberField[getFields.Length];

			for (int index = 0; index < getFields.Length; index++)
			{
				serializedFields[index] = getFields[index].Field;
			}

			this.ApplySerializablePattern(SerializationType.InheritedCustom, serializedFields);

			this.serializationConstructorComments = new CodeCommentStatement[this.SerializableConstructor.Comments.Count];
			this.SerializableConstructor.Comments.CopyTo(this.serializationConstructorComments, 0);
			this.getObjectDataComments = new CodeCommentStatement[this.GetSerializableObjectData.Comments.Count];
			this.GetSerializableObjectData.Comments.CopyTo(this.getObjectDataComments, 0);

			this.HasComments = true;
		}

		/// <summary>
		/// Gets the parameterless (unless fields are appended) constructor.
		/// </summary>
		public CodeConstructor ParameterlessConstructor
		{
			get
			{
				return this.parameterlessConstructor;
			}
		}

		/// <summary>
		/// Gets the constructor only with message (unless fields are appended).
		/// </summary>
		public CodeConstructor MessageConstructor
		{
			get
			{
				return this.messageConstructor;
			}
		}

		/// <summary>
		/// Gets the constructor only with message and inner exception (unless fields are appended).
		/// </summary>
		public CodeConstructor InnerExceptionConstructor
		{
			get
			{
				return this.innerExceptionConstructor;
			}
		}

		/// <summary>
		/// Gets the fields in the exception.
		/// </summary>
		public CodePatternGetField[] GetFields
		{
			get
			{
				return this.getFields;
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
				return (this.parameterlessConstructor.Comments.Count > 0);
			}
			set
			{
				this.parameterlessConstructor.Comments.Clear();
				this.messageConstructor.Comments.Clear();
				this.SerializableConstructor.Comments.Clear();
				this.innerExceptionConstructor.Comments.Clear();
				this.GetSerializableObjectData.Comments.Clear();

				if (value)
				{
					ArrayList list = new ArrayList(this.fieldComments.Count);

					foreach (string entry in this.fieldComments)
					{
						if (this.fieldComments[entry] != null)
						{
							list.Add(new ParameterStatements(entry, this.fieldComments[entry]));
						}
					}

					this.parameterlessConstructor.Comments.AddRange(new CommentsForConstructor(this.Name, (ParameterStatements[])list.ToArray(typeof(ParameterStatements))));

					list.Add(new ParameterStatements(MessageParameterName, "The message in the exception"));
					this.messageConstructor.Comments.AddRange(new CommentsForConstructor(this.Name, (ParameterStatements[])list.ToArray(typeof(ParameterStatements))));

					list.Add(new ParameterStatements(InnerExceptionParameterName, "An exception encapsulated in the new exception"));
					this.innerExceptionConstructor.Comments.AddRange(new CommentsForConstructor(this.Name, (ParameterStatements[])list.ToArray(typeof(ParameterStatements))));

					this.SerializableConstructor.Comments.AddRange(this.serializationConstructorComments);
					this.GetSerializableObjectData.Comments.AddRange(this.getObjectDataComments);
				}

				foreach (CodePatternGetField field in this.getFields)
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
		/// Sets the comment for a property of the exception.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="comment">The comment for the parameter.</param>
		/// <exception cref="NotSupportedException">Thrown when the property does not exist.</exception>
		public void SetComment(string propertyName, string comment)
		{
			foreach (CodePatternGetField field in this.getFields)
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
