using System;
using System.CodeDom;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of a flags enum.
	/// </summary>
	/// <example>Output example:<code>
	///	[System.FlagsAttribute()]
	///	public enum MyFlags : int
	///	{
	///		A = 1,
	///		B = 2,
	///		C = 4,
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternFlags : CodeTypeDeclaration
	{
		private System.Collections.Hashtable flags = new System.Collections.Hashtable();

		/// <summary>
		/// Initializes a new instance of the CodePatternFlags class.
		/// </summary>
		/// <param name="name">The name of the enum.</param>
		/// <param name="values">The names of the flags.</param>
		/// <remarks>A flags enumeration may have only as many as 63 flags in order to remain CLS compliant.</remarks>
		public CodePatternFlags(string name, params string[] values) : base(name)
		{
			if (name == null || name == string.Empty)
			{
				throw new ArgumentNullException("name");
			}

			if (values.Length == 0)
			{
				throw new ArgumentException("values should at least contain one value.", "values");
			}

			this.Attributes &= MemberAttributes.AccessMask;
			this.Attributes |= MemberAttributes.Public;

			// For more than 31 flags, the base type will be System.Int64.
			this.BaseTypes.Add(CodeTypeReferenceStore.Get(GetBaseType(values.Length)));

			this.CustomAttributes.Add(new CodeAttributeDeclaration(typeof(FlagsAttribute).FullName));

			this.IsEnum = true;

			long flagValue = 1;

			// Create the list of flags with values at power of two.
			foreach (string value in values)
			{
				CodeMemberField enumValue = new CodeMemberField();
				enumValue.Name = value;
				enumValue.InitExpression = new CodePrimitiveExpression(flagValue);

				this.Members.Add(enumValue);
				this.flags.Add(value, enumValue);

				flagValue *= 2;
			}
		}

		private static Type GetBaseType(int valueCount)
		{
			if (valueCount > Convert.ToInt32(Math.Ceiling(Math.Log(int.MaxValue, 2))))
				if (valueCount > Convert.ToInt32(Math.Ceiling(Math.Log(long.MaxValue, 2))))
					throw new ApplicationException("Too many flags defined. Maximum number of flags is 64. Defined: " + valueCount.ToString());
				else
					return typeof(long);
			else
				return typeof(int);
		}

		/// <summary>
		/// Gets a flag's <see cref="CodeMemberField"/> object by name.
		/// </summary>
		/// <value>A flag's <see cref="CodeMemberField"/> object.</value>
		[System.Runtime.CompilerServices.IndexerNameAttribute("Flags")]
		public CodeMemberField this[string flag]
		{
			get
			{
				if (!this.flags.ContainsKey(flag))
				{
					return null;
				}

				return this.flags[flag] as CodeMemberField;
			}
		}
	}
}
