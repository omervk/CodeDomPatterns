using System;

namespace DotNetZen.CodeDom
{
	/// <summary>
	/// Designates the scope of a code element.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum Scope
	{
		/// <summary>
		/// The element is defined on the type.
		/// </summary>
		Static		= 1,
		/// <summary>
		/// The element is defined on an instance.
		/// </summary>
		Instance	= 2,
	}

	/// <summary>
	/// Designates the type category of a resource in the using and disposable type patterns.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum ResourceType
	{
		/// <summary>
		/// The resource is a reference type.
		/// </summary>
		ReferenceType	= 1,
		/// <summary>
		/// The resource is a value type.
		/// </summary>
		ValueType		= 2,
	}

	/// <summary>
	/// Designates the object's load type in the singleton pattern.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum LoadType
	{
		/// <summary>
		/// The singleton object will load on first call.
		/// </summary>
		LazyLoad	= 1,
		/// <summary>
		/// The singleton object will load on first type reference.
		/// </summary>
		PreLoad		= 2,
	}

	/// <summary>
	/// Designates unary operators.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum CodePatternUnaryOperatorType
	{
		/// <summary>
		/// Boolean not (== false) operator.
		/// </summary>
		BooleanNot		= 1,
		/// <summary>
		/// Boolean is (== true) operator.
		/// </summary>
		BooleanIsTrue	= 2,
		/// <summary>
		/// Boolean is null (== null) operator.
		/// </summary>
		BooleanIsNull	= 3,
		/// <summary>
		/// Boolean is not null (!= null) operator.
		/// </summary>
		BooleanNotNull	= 4,
	}

	/// <summary>
	/// Designates binary operators.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum CodePatternBinaryOperatorType
	{
		/// <summary>
		/// Boolean exclusive or (xor) operator.
		/// </summary>
		BooleanExclusiveOr	= 1,
	}

	/// <summary>
	/// Designates compound assignment operators.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum CodePatternCompoundAssignmentOperatorType
	{
		/// <summary>
		/// Addition operator.
		/// </summary>
		Add = System.CodeDom.CodeBinaryOperatorType.Add,
		/// <summary>
		/// Subtraction operator.
		/// </summary>
		Subtract = System.CodeDom.CodeBinaryOperatorType.Subtract,
		/// <summary>
		/// Multiplication operator.
		/// </summary>
		Multiply = System.CodeDom.CodeBinaryOperatorType.Multiply,
		/// <summary>
		/// Division operator.
		/// </summary>
		Divide = System.CodeDom.CodeBinaryOperatorType.Divide,
		/// <summary>
		/// Modulus operator.
		/// </summary>
		Modulus = System.CodeDom.CodeBinaryOperatorType.Modulus,
		/// <summary>
		/// Bitwise and operator.
		/// </summary>
		BitwiseAnd = System.CodeDom.CodeBinaryOperatorType.BitwiseAnd,
		/// <summary>
		/// Bitwise or operator.
		/// </summary>
		BitwiseOr = System.CodeDom.CodeBinaryOperatorType.BitwiseOr,
	}

	/// <summary>
	/// Designates types of events in a collection.
	/// </summary>
	[Flags, Serializable, CLSCompliant(true)]
	public enum CollectionEvents
	{
		/// <summary>
		/// No events.
		/// </summary>
		None			= 0,
		
		/// <summary>
		/// Pre-Clearing event.
		/// </summary>
		Clearing		= 1,
		/// <summary>
		/// Post-Clearing event.
		/// </summary>
		Cleared			= Clearing << 1,
		/// <summary>
		/// Both the Clearing and Cleared events.
		/// </summary>
		ClearEvents		= Clearing | Cleared,

		/// <summary>
		/// Pre-Inserting event.
		/// </summary>
		Inserting		= Cleared << 1,
		/// <summary>
		/// Post-Inserting event.
		/// </summary>
		Inserted		= Inserting << 1,
		/// <summary>
		/// Both the Inserting and Inserted events.
		/// </summary>
		InsertionEvents	= Inserting | Inserted,
		
		/// <summary>
		/// Pre-Removing event.
		/// </summary>
		Removing		= Inserted << 1,
		/// <summary>
		/// Post-Removing event.
		/// </summary>
		Removed			= Removing << 1,
		/// <summary>
		/// Both the Removing and Removed events.
		/// </summary>
		RemovalEvents	= Removing | Removed,

		/// <summary>
		/// Pre-Setting event.
		/// </summary>
		Setting			= Removed << 1,
		/// <summary>
		/// Post-Setting event.
		/// </summary>
		Set				= Setting << 1,
		/// <summary>
		/// Both the Setting and Set events.
		/// </summary>
		SettingEvents	= Setting | Set,

		/// <summary>
		/// Validation event.
		/// </summary>
		Validating		= Set << 1,

		/// <summary>
		/// Pre-Action events.
		/// </summary>
		PreEvents		= Clearing | Inserting | Removing | Setting | Validating,
		/// <summary>
		/// Post-Action events.
		/// </summary>
		PostEvents		= Cleared  | Inserted  | Removed  | Set,

		/// <summary>
		/// All events that can be exposed by a collection.
		/// </summary>
		All				= ClearEvents | InsertionEvents | RemovalEvents | SettingEvents | Validating,
	}

	/// <summary>
	/// Designates types of interface implementation.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum InterfaceImplementationType
	{
		/// <summary>
		/// Implicit implementation of an interface.
		/// </summary>
		Implicit	= 0,

		/// <summary>
		/// Explicit implementation of an interface.
		/// </summary>
		Explicit	= 1,
	}

	/// <summary>
	/// Designates types of xml comment list types.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public enum XmlCommentListType
	{
		/// <summary>
		/// The default list type (depends on the documentation generator).
		/// </summary>
		Default		= 0,

		/// <summary>
		/// Bulleted list.
		/// </summary>
		Bullets		= 1,

		/// <summary>
		/// Numbered list.
		/// </summary>
		Numbered	= 2,

		/// <summary>
		/// A table of either one or two columns.
		/// </summary>
		Table		= 3,
	}

	/// <summary>
	/// Designates accessors of a property.
	/// </summary>
	[Flags, Serializable, CLSCompliant(true)]
	public enum PropertyAccessors
	{
		/// <summary>
		/// Only the get accessor.
		/// </summary>
		Get		= 0x01,

		/// <summary>
		/// Only the set accessor.
		/// </summary>
		Set		= Get << 1,

		/// <summary>
		/// Both get and set accessor.
		/// </summary>
		Both	= Get | Set,
	}
}
