using System;
using System.CodeDom;
using DotNetZen.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents an assertion which checks that the argument is not null.
	/// </summary>
	/// <example>Output example:<code>
	///	if ((myArgument == null))
	///	{
	///		throw new System.ArgumentNullException("myArgument");
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternArgumentAssertNotNullStatement : CodeConditionStatement
	{
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertNotNullStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		public CodePatternArgumentAssertNotNullStatement(CodeParameterDeclarationExpression parameter)
			: this(parameter.Name)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertNotNullStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		public CodePatternArgumentAssertNotNullStatement(string argumentName)
			: base(new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanIsNull, new CodeVariableReferenceExpression(argumentName)),
			new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(ArgumentNullException), new CodePrimitiveExpression(argumentName))))
		{
			if (argumentName == null)
			{
				throw new ArgumentNullException("argumentName");
			}
		}
	}

	/// <summary>
	/// Represents an assertion which checks that the argument is in a range.
	/// (upperBound >= argument >= lowerBound) ==> true
	/// </summary>
	/// <example>Output example:<code>
	///	if (((myArgument &gt; MyType.MaxValue) 
	///		|| (myArgument &lt; MyType.MinValue)))
	///	{
	///		throw new System.ArgumentOutOfRangeException("myArgument");
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternArgumentAssertInRangeStatement : CodeConditionStatement
	{
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertInRangeStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		/// <param name="lowerBound">The lower bound value.</param>
		/// <param name="upperBound">The upper bound value.</param>
		public CodePatternArgumentAssertInRangeStatement(CodeParameterDeclarationExpression parameter, CodeExpression lowerBound, CodeExpression upperBound)
			: this(parameter.Name, lowerBound, upperBound)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertInRangeStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		/// <param name="lowerBound">The lower bound value.</param>
		/// <param name="upperBound">The upper bound value.</param>
		public CodePatternArgumentAssertInRangeStatement(string argumentName, CodeExpression lowerBound, CodeExpression upperBound)
			: base(new CodeBinaryOperatorExpression(
			new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(argumentName), CodeBinaryOperatorType.GreaterThan, upperBound),
			CodeBinaryOperatorType.BooleanOr,
			new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(argumentName), CodeBinaryOperatorType.LessThan, lowerBound)),
			new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(ArgumentOutOfRangeException), new CodePrimitiveExpression(argumentName))))
		{
			if (argumentName == null)
			{
				throw new ArgumentNullException("argumentName");
			}

			if (lowerBound == null)
			{
				throw new ArgumentNullException("lowerBound");
			}

			if (upperBound == null)
			{
				throw new ArgumentNullException("upperBound");
			}
		}
	}

	/// <summary>
	/// Represents an assertion which checks that the argument is in a range.
	/// (parameter >= lowerBound) ==> true
	/// </summary>
	/// <example>Output example:<code>
	///	if ((myArgument &lt; MyType.MinValue))
	///	{
	///		throw new System.ArgumentOutOfRangeException("myArgument");
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternArgumentAssertInLowerBoundStatement : CodeConditionStatement
	{
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertInLowerBoundStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		/// <param name="lowerBound">The lower bound value.</param>
		public CodePatternArgumentAssertInLowerBoundStatement(CodeParameterDeclarationExpression parameter, CodeExpression lowerBound)
			: this(parameter.Name, lowerBound)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertInLowerBoundStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		/// <param name="lowerBound">The lower bound value.</param>
		public CodePatternArgumentAssertInLowerBoundStatement(string argumentName, CodeExpression lowerBound)
			: base(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(argumentName), CodeBinaryOperatorType.LessThan, lowerBound),
			new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(ArgumentOutOfRangeException), new CodePrimitiveExpression(argumentName))))
		{
			if (argumentName == null)
			{
				throw new ArgumentNullException("argumentName");
			}

			if (lowerBound == null)
			{
				throw new ArgumentNullException("lowerBound");
			}
		}
	}

	/// <summary>
	/// Represents an assertion which checks that the argument is in a range.
	/// (upperBound >= argument) ==> true
	/// </summary>
	/// <example>Output example:<code>
	///	if ((myArgument &gt; MyType.MaxValue))
	///	{
	///		throw new System.ArgumentOutOfRangeException("myArgument");
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternArgumentAssertInUpperBoundStatement : CodeConditionStatement
	{
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertInUpperBoundStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		/// <param name="upperBound">The lower bound value.</param>
		public CodePatternArgumentAssertInUpperBoundStatement(CodeParameterDeclarationExpression parameter, CodeExpression upperBound)
			: this(parameter.Name, upperBound)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertInUpperBoundStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		/// <param name="upperBound">The lower bound value.</param>
		public CodePatternArgumentAssertInUpperBoundStatement(string argumentName, CodeExpression upperBound)
			: base(new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(argumentName), CodeBinaryOperatorType.GreaterThan, upperBound),
			new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(ArgumentOutOfRangeException), new CodePrimitiveExpression(argumentName))))
		{
			if (argumentName == null)
			{
				throw new ArgumentNullException("argumentName");
			}

			if (upperBound == null)
			{
				throw new ArgumentNullException("upperBound");
			}
		}
	}

	/// <summary>
	/// Represents an assertion which checks that the argument is of a certain type.
	/// </summary>
	/// <example>Output example:<code>
	///	if ((myArgument.GetType().IsInstanceOfType(typeof(MyType)) == false))
	///	{
	///		throw new System.ArgumentException(string.Format("The argument myArgument must be of type {0}.", typeof(MyType).FullName), "myArgument");
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternArgumentAssertIsInstanceOfStatement : CodeConditionStatement
	{
		private static readonly string StringFormatMethodName = typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object) }).Name;
		private static readonly string TypeFullNamePropertyName = typeof(Type).GetProperty("FullName").Name;
		private const string Message = "The argument {0} must be of type {1}.";
		private const string Argument0 = "{0}";

		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertIsInstanceOfStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		/// <param name="type">The expected type of argument.</param>
		public CodePatternArgumentAssertIsInstanceOfStatement(CodeParameterDeclarationExpression parameter, CodeTypeReference type)
			: this(parameter.Name, type)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertIsInstanceOfStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		/// <param name="type">The expected type of argument.</param>
		public CodePatternArgumentAssertIsInstanceOfStatement(CodeParameterDeclarationExpression parameter, Type type)
			: this(parameter.Name, CodeTypeReferenceStore.Get(type))
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertIsInstanceOfStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		/// <param name="type">The expected type of argument.</param>
		public CodePatternArgumentAssertIsInstanceOfStatement(string argumentName, Type type)
			: this(argumentName, CodeTypeReferenceStore.Get(type))
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertIsInstanceOfStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		/// <param name="type">The expected type of argument.</param>
		public CodePatternArgumentAssertIsInstanceOfStatement(string argumentName, CodeTypeReference type)
			: base(new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNot, new CodePatternIsInstExpression(new CodeVariableReferenceExpression(argumentName), type)),
				new CodeThrowExceptionStatement(
					new CodeObjectCreateExpression(typeof(ArgumentException), 
						new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(string)), StringFormatMethodName, new CodePrimitiveExpression(string.Format(Message, argumentName, Argument0)), new CodePropertyReferenceExpression(new CodeTypeOfExpression(type), TypeFullNamePropertyName)),
						new CodePrimitiveExpression(argumentName))))
		{
			if (argumentName == null)
			{
				throw new ArgumentNullException("argumentName");
			}

			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
		}
	}

	/// <summary>
	/// Represents an assertion which checks that the argument is a valid enum value.
	/// </summary>
	/// <example>Output example:<code>
	///	if (Enum.IsDefined(Type.GetType("MyEnum"), myArgument) == false))
	///	{
	///		throw new System.ArgumentException("The argument myArgument is not a member in the enumeration MyEnum.", "myArgument");
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternArgumentAssertEnumIsDefinedStatement : CodeConditionStatement
	{
		private static readonly string TypeGetTypeMethodName = typeof(Type).GetMethod("GetType", new Type[] { typeof(string) }).Name;
		private static readonly string EnumIsDefinedPropertyName = typeof(Enum).GetMethod("IsDefined").Name;
		private const string Message = "The argument {0} is not a member in the enumeration {1}.";

		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertEnumIsDefinedStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		/// <param name="type">The expected type of enum.</param>
		public CodePatternArgumentAssertEnumIsDefinedStatement(CodeParameterDeclarationExpression parameter, CodeTypeReference type)
			: this(parameter.Name, type)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertEnumIsDefinedStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		/// <param name="type">The expected type of enum.</param>
		public CodePatternArgumentAssertEnumIsDefinedStatement(CodeParameterDeclarationExpression parameter, Type type)
			: this(parameter.Name, CodeTypeReferenceStore.Get(type))
		{
		}

		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertEnumIsDefinedStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		/// <param name="type">The expected type of enum.</param>
		public CodePatternArgumentAssertEnumIsDefinedStatement(string argumentName, Type type)
			: this(argumentName, CodeTypeReferenceStore.Get(type))
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertEnumIsDefinedStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		/// <param name="type">The expected type of enum.</param>
		public CodePatternArgumentAssertEnumIsDefinedStatement(string argumentName, CodeTypeReference type)
			: base(new CodePatternUnaryOperatorExpression(CodePatternUnaryOperatorType.BooleanNot, 
			new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Enum)), EnumIsDefinedPropertyName, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Type)), TypeGetTypeMethodName, new CodePrimitiveExpression(type.BaseType)), new CodeVariableReferenceExpression(argumentName))),
			new CodeThrowExceptionStatement(
			new CodeObjectCreateExpression(typeof(ArgumentException), new CodePrimitiveExpression(string.Format(Message, argumentName, type.BaseType)))))
		{
			if (argumentName == null)
			{
				throw new ArgumentNullException("argumentName");
			}

			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
		}
	}

	/// <summary>
	/// Represents an assertion which checks that the string argument is neither null nor empty.
	/// </summary>
	/// <example>Output example:<code>
	///	if ((myArgument == null) || (myArgument == string.Empty))
	///	{
	///		throw new System.ArgumentNullException("myArgument");
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternArgumentAssertStringNotNullOrEmptyStatement : CodePatternArgumentAssertNotNullStatement
	{
		private static readonly string StringEmptyFieldName = typeof(string).GetField("Empty").Name;

		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertStringNotNullOrEmptyStatement class.
		/// </summary>
		/// <param name="parameter">The parameter to check.</param>
		public CodePatternArgumentAssertStringNotNullOrEmptyStatement(CodeParameterDeclarationExpression parameter)
			: this(parameter.Name)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the CodePatternArgumentAssertStringNotNullOrEmptyStatement class.
		/// </summary>
		/// <param name="argumentName">The name argument of the argument to validate.</param>
		public CodePatternArgumentAssertStringNotNullOrEmptyStatement(string argumentName)
			: base(argumentName)
		{
			this.Condition = new CodeBinaryOperatorExpression(this.Condition, CodeBinaryOperatorType.BooleanOr,
				new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(argumentName), CodeBinaryOperatorType.ValueEquality, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(string)), StringEmptyFieldName)));
		}
	}

}
