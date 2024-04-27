using System;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Designates the patterns applied to a certain type.
	/// </summary>
	[Flags, Serializable, CLSCompliant(true)]
	public enum TypePatterns
	{
		/// <summary>
		/// Indicates no patterns applied.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates that the disposable type pattern is applied.
		/// </summary>
		Disposable = 1,

		/// <summary>
		/// Indicates that the serializable type pattern is applied.
		/// </summary>
		Serializable = Disposable << 1,
	}

	/// <summary>
	/// Designates the level of the dispose pattern to be applied to a type.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum DisposeImplementationType
	{
		/// <summary>
		/// Indicates that this is the first type in the hierarchy to implement the dispose pattern.
		/// </summary>
		New,

		/// <summary>
		/// Indicates that the dispose pattern has already been implemented higher up in the hierarchy.
		/// </summary>
		Inherited,
	}

	/// <summary>
	/// Designates the type of serialization applied to a type.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum SerializationType
	{
		/// <summary>
		/// Indicates basic serialization.
		/// All fields are serialized.
		/// </summary>
		Basic,

		/// <summary>
		/// Indicates selective serialization.
		/// When the <see cref="CodePatternTypeDeclaration.ApplySerializablePattern(SerializationType)"/> method is called, fields not in the fieldsToSerialize parameter
		/// are marked with the <see cref="NonSerializedAttribute"/> attribute.
		/// </summary>
		Selective,

		/// <summary>
		/// Indicates custom serialization that was not derived from a parent.
		/// </summary>
		NewCustom,

		/// <summary>
		/// Indicates custom serialization when custom serialization was inherited from a parent.
		/// </summary>
		InheritedCustom,
	}
}
