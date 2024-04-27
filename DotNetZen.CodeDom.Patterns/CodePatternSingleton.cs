using System;
using System.CodeDom;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the base declaration of a class as a Singleton.
	/// </summary>
	/// <example>Output example (Lazy Load):<code>
	///	public class Foo
	///	{
	///		private Foo()
	///		{
	///		}
	///
	///		/// &lt;summary&gt;
	///		/// Gets the single instance of type &lt;see cref="Foo" /&gt;.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;The single instance of type &lt;see cref="Foo" /&gt;.&lt;/value&gt;
	///		public static Foo Instance
	///		{
	///			get
	///			{
	///				return InstanceContainer.Instance;
	///			}
	///		}
	///
	///		class InstanceContainer
	///		{
	///			private static Foo m_Instance = new Foo();
	///
	///			static InstanceContainer()
	///			{
	///			}
	///
	///			private InstanceContainer()
	///			{
	///			}
	///
	///			public static Foo Instance
	///			{
	///				get
	///				{
	///					return InstanceContainer.m_Instance;
	///				}
	///			}
	///		}
	///	}
	///	</code></example>
	///	<example>Output example (Pre-Load):<code>
	///	public class Bar
	///	{
	///		private static Bar m_Instance = new Bar();
	///
	///		static Bar()
	///		{
	///		}
	///
	///		private Bar()
	///		{
	///		}
	///
	///		/// &lt;summary&gt;
	///		/// Gets the single instance of type &lt;see cref="Foo" /&gt;.
	///		/// &lt;/summary&gt;
	///		/// &lt;value&gt;The single instance of type &lt;see cref="Foo" /&gt;.&lt;/value&gt;
	///		public static Bar Instance
	///		{
	///			get
	///			{
	///				return Bar.m_Instance;
	///			}
	///		}
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternSingleton : CodeTypeDeclaration
	{
		private const string InstanceFieldName = "m_Instance";
		private const string InstancePropertyName = "Instance";
		private const string InnerClassName = "InstanceContainer";

		private CodeMemberField instanceField;
		private CodeMemberProperty instanceProperty;
		private CodeConstructor privateConstructor;
		private CodeTypeConstructor staticConstructor;
		private CodeTypeReference selfReference;

		/// <summary>
		/// Initializes a new instance of the CodePatternSingleton class.
		/// </summary>
		/// <param name="name">The name for the new type.</param>
		/// <param name="loadType">Whether the object should load on first call or first reference.</param>
		public CodePatternSingleton(string name, LoadType loadType)
			: base(name)
		{
			if (name == null || name == string.Empty)
			{
				throw new ArgumentNullException("name");
			}

			// Create a self-reference for re-use.
			this.selfReference = CodeTypeReferenceStore.Get(name);

			// Initialize the static containing field.
			this.instanceField = new CodeMemberField(this.selfReference, InstanceFieldName);
			this.instanceField.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.instanceField.Attributes |= MemberAttributes.Private | MemberAttributes.Static;

			// Initialize the static access property.
			this.instanceProperty = new CodeMemberProperty();
			this.instanceProperty.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.instanceProperty.Attributes |= MemberAttributes.Public | MemberAttributes.Static;
			this.instanceProperty.HasGet = true;
			this.instanceProperty.HasSet = false;
			this.instanceProperty.Name = InstancePropertyName;
			this.instanceProperty.Type = this.selfReference;

			CodeObjectCreateExpression initializationExpression = new CodeObjectCreateExpression(this.selfReference);

			this.instanceField.InitExpression = initializationExpression;

			// If we lazy-load, we should use a nested class.
			// Otherwise, we should just assign it to the field.
			if (loadType == LoadType.LazyLoad)
			{
				CodeTypeDeclaration nestedClass = new CodeTypeDeclaration(InnerClassName);
				nestedClass.Attributes &= ~MemberAttributes.AccessMask;
				nestedClass.Attributes |= MemberAttributes.Private;
				nestedClass.TypeAttributes &= ~System.Reflection.TypeAttributes.VisibilityMask;
				nestedClass.TypeAttributes |= System.Reflection.TypeAttributes.NotPublic;

				// Create a private constructor so no one could initialize an instance of the nested class.
				CodeConstructor nestedPrivateConstructor = new CodeConstructor();
				nestedPrivateConstructor.Attributes &= ~MemberAttributes.AccessMask;
				nestedPrivateConstructor.Attributes |= MemberAttributes.Private;

				// Create a static private constructor so that the nested class is not marked with beforefieldinit.
				CodeTypeConstructor nestedStaticConstructor = new CodeTypeConstructor();

				// Initialize the static access nested property.
				CodeMemberProperty nestedInstanceProperty = new CodeMemberProperty();
				nestedInstanceProperty.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
				nestedInstanceProperty.Attributes |= MemberAttributes.Public | MemberAttributes.Static;
				nestedInstanceProperty.HasGet = true;
				nestedInstanceProperty.HasSet = false;
				nestedInstanceProperty.Name = InstancePropertyName;
				nestedInstanceProperty.Type = this.selfReference;

				nestedInstanceProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(InnerClassName), InstanceFieldName)));

				nestedClass.Members.Add(this.instanceField);
				nestedClass.Members.Add(nestedPrivateConstructor);
				nestedClass.Members.Add(nestedStaticConstructor);
				nestedClass.Members.Add(nestedInstanceProperty);

				this.instanceProperty.GetStatements.Add(
					new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(InnerClassName), InstancePropertyName))
					);

				this.Members.Add(nestedClass);
			}
			else
			{
				this.instanceProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(name), InstanceFieldName)));

				// Create a static private constructor so that the class is not marked with beforefieldinit.
				this.staticConstructor = new CodeTypeConstructor();

				this.Members.Add(this.instanceField);
				this.Members.Add(this.staticConstructor);
			}

			// Create a private constructor so no one could initialize an instance of the class.
			this.privateConstructor = new CodeConstructor();
			this.privateConstructor.Attributes &= ~MemberAttributes.AccessMask;
			this.privateConstructor.Attributes |= MemberAttributes.Private;

			this.Members.Add(this.privateConstructor);
			this.Members.Add(this.instanceProperty);

			this.HasComments = true;
		}

		/// <summary>
		/// Gets or sets the name of the type.
		/// </summary>
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;

				// If the name has changed, so should the singleton.
				this.selfReference.BaseType = value;
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
				return (this.instanceProperty.Comments.Count > 0);
			}
			set
			{
				this.instanceProperty.Comments.Clear();

				if (value)
				{
					this.instanceProperty.Comments.AddRange(new CommentsForProperty(PropertyAccessors.Get,
						"The single instance of type " + new SeeExpression(this.Name)));
				}
			}
		}
	}
}