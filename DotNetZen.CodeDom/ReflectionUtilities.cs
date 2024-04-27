using System;
using System.CodeDom;

namespace DotNetZen.CodeDom
{
	/// <summary>
	/// Provides utilities to create CodeDOM objects from runtime objects.
	/// </summary>
	public sealed class ReflectionUtilities
	{
		private ReflectionUtilities()
		{
		}

		/// <summary>
		/// Gets the value of a flags enum value as a list of enum values.
		/// </summary>
		/// <param name="value">The value of the enum instance.</param>
		/// <param name="enumType">The type of the enum.</param>
		/// <returns>An expression holding a list of enum values.</returns>
		public static CodeExpression GetFlagsValueExpression(int value, Type enumType)
		{
			CodeExpression expression = null;

			int[] values = (int[])Enum.GetValues(enumType);

			Array.Sort(values);

			for (int i = values.Length - 1; (i >= 0) && (value != 0); i--)
			{
				// If this value is in the list.
				if ((value & values[i]) == values[i])
				{
					CodeFieldReferenceExpression flagValue = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(enumType), Enum.GetName(enumType, values[i]));

					// Add value to the expression.
					if (expression == null)
					{
						expression = flagValue;
					}
					else
					{
						expression = new CodeBinaryOperatorExpression(expression, CodeBinaryOperatorType.BitwiseOr, flagValue);
					}

					// Remove it from the list.
					value &= ~values[i];
				}
			}

			if (value != 0)
			{
				throw new ArgumentOutOfRangeException("value", value, "The value given does not conform with the range of values dictated by the type " + enumType.FullName);
			}
			else if (expression == null)
			{
				expression = new CodeCastExpression(typeof(int), new CodePrimitiveExpression(0));
			}

			return expression;
		}

		/// <summary>
		/// Gets the value of a flags enum value from a list of enum values.
		/// </summary>
		/// <param name="expr">The expression that is the list of enum values.</param>
		/// <returns>The integer value of the enum.</returns>
		/// <remarks>This method only supports expressions created with the <see cref="GetFlagsValueExpression"/> method.</remarks>
		public static int GetFlagsExpressionValue(CodeExpression expr)
		{
			if (expr is CodeCastExpression)
			{
				return 0;
			}
			else if (expr is CodeBinaryOperatorExpression)
			{
				return GetFlagsExpressionValue(((CodeBinaryOperatorExpression)(expr)).Left) + GetFlagsExpressionValue(((CodeBinaryOperatorExpression)(expr)).Right);
			}
			else if (expr is CodeFieldReferenceExpression)
			{
				CodeFieldReferenceExpression fieldRef = (CodeFieldReferenceExpression)expr;

				return (int)Enum.Parse(Type.GetType(((CodeTypeReferenceExpression)(fieldRef.TargetObject)).Type.BaseType), fieldRef.FieldName, false);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
	}
}
