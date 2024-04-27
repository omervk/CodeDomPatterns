using System;
using System.CodeDom;

namespace DotNetZen.CodeDom
{
	/// <summary>
	/// Represents a reference to an instance with the indication of whether it's a reference or a value type.
	/// </summary>
	public class CodeInstanceReferenceExpression : CodeExpression
	{
		private CodeExpression instance;
		private ResourceType objectType;

		/// <summary>
		/// Initializes a new instance of the CodeInstanceReferenceExpression class.
		/// </summary>
		/// <param name="instance">The expression representing the instance.</param>
		/// <param name="objectType">The type of the object (reference/value).</param>
		public CodeInstanceReferenceExpression(CodeExpression instance, ResourceType objectType)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}

			this.instance = instance;
			this.objectType = objectType;
		}

		/// <summary>
		/// Initializes a new instance of the CodeInstanceReferenceExpression class.
		/// </summary>
		/// <param name="instance">The expression representing the instance.</param>
		/// <param name="objectType">The <see cref="Type"/> of the object.</param>
		public CodeInstanceReferenceExpression(CodeExpression instance, Type objectType)
			: this(instance, (objectType.IsValueType ? ResourceType.ValueType : ResourceType.ReferenceType))
		{
		}

		/// <summary>
		/// The expression representing the instance.
		/// </summary>
		public CodeExpression Instance
		{
			get
			{
				return this.instance;
			}
		}

		/// <summary>
		/// The type of the instance.
		/// </summary>
		public ResourceType ObjectType
		{
			get
			{
				return this.objectType;
			}
		}
	}
}
