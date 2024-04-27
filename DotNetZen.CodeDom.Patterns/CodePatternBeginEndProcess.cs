using System;
using System.CodeDom;
using System.Runtime.InteropServices;

using DotNetZen.CodeDom.Patterns.XmlComments;

namespace DotNetZen.CodeDom.Patterns
{
	/// <summary>
	/// Represents the declaration of begin/end methods for a process, such as load, init, etc.
	/// </summary>
	/// <example>Output example:<code>
	///	/// &lt;summary&gt;
	///	/// See &lt;see cref="IsInInit" /&gt; for information about this field.
	///	/// &lt;/summary&gt;
	///	private int m_IsInInit;
	///
	///	/// &lt;summary&gt;
	///	/// Begins the Init process.
	///	/// &lt;/summary&gt;
	///	public virtual void BeginInit()
	///	{
	///		this.m_IsInInit = (this.m_IsInInit + 1);
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Ends the Init process.
	///	/// &lt;/summary&gt;
	///	public virtual void EndInit()
	///	{
	///		if ((this.m_IsInInit != 0))
	///		{
	///			this.m_IsInInit = (this.m_IsInInit - 1);
	///		}
	///	}
	///
	///	/// &lt;summary&gt;
	///	/// Gets whether the Init process has begun.
	///	/// &lt;/summary&gt;
	///	/// &lt;value&gt;Whether the init process has begun.&lt;/value&gt;
	///	protected bool IsInInit()
	///	{
	///		return (this.m_IsInInit != 0);
	///	}
	///	</code></example>
	[Serializable, CLSCompliant(true)]
	public class CodePatternBeginEndProcess : CodeTypeMemberCollection
	{
		private const string IsIn = "IsIn";
		private const string IsInField = "m_IsIn";
		private const string BeginName = "Begin";
		private const string EndName = "End";

		private CodeMemberMethod begin;
		private CodeMemberMethod end;
		private CodeMemberMethod isInProcess;
		private CodeMemberField isInProcessFlag;
		private string processName;

		/// <summary>
		/// Initializes a new instance of the CodePatternBeginEndProcess class.
		/// </summary>
		/// <param name="processName">The name of the process.</param>
		public CodePatternBeginEndProcess(string processName)
		{
			if (processName == null || processName == string.Empty)
			{
				throw new ArgumentNullException("processName");
			}

			processName = Char.ToUpper(processName[0]) + processName.Substring(1);
			this.processName = processName;
			
			// Create the flag.
			this.isInProcessFlag = new CodeMemberField(typeof(int), IsInField + processName);
			
			// Create the method that would begin the process.
			this.begin = new CodeMemberMethod();
			this.begin.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.begin.Attributes |= MemberAttributes.Public;
			this.begin.Name = BeginName + processName;
			this.begin.Statements.Add(new CodePatternCompoundAssignStatement(
				new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.isInProcessFlag.Name),
				CodePatternCompoundAssignmentOperatorType.Add,
				new CodePrimitiveExpression(1)));

			// Create the method that would end the process (too many calls are ignored).
			this.end = new CodeMemberMethod();
			this.end.Attributes &= ~MemberAttributes.AccessMask & ~MemberAttributes.ScopeMask;
			this.end.Attributes |= MemberAttributes.Public;
			this.end.Name = EndName + processName;
			this.end.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.isInProcessFlag.Name), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0)),
				new CodePatternCompoundAssignStatement(
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.isInProcessFlag.Name),
					CodePatternCompoundAssignmentOperatorType.Subtract,
					new CodePrimitiveExpression(1))));
			
			// Create a method for internal use to find out if we're in the process.
			this.isInProcess = new CodeMemberMethod();
			this.isInProcess.Attributes &= ~MemberAttributes.AccessMask;
			this.isInProcess.Attributes |= MemberAttributes.Family;
			this.isInProcess.Name = IsIn + processName;
			this.isInProcess.Statements.Add(new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), this.isInProcessFlag.Name), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0))));
			this.isInProcess.ReturnType = CodeTypeReferenceStore.Get(typeof(bool));

			this.Add(this.isInProcessFlag);
			this.Add(this.begin);
			this.Add(this.end);
			this.Add(this.isInProcess);

			this.HasComments = true;
		}

		/// <summary>
		/// Gets the Begin[Process] method.
		/// </summary>
		public CodeMemberMethod Begin
		{
			get 
			{
				return this.begin;
			}
		}

		/// <summary>
		/// Gets the End[Process] method.
		/// </summary>
		public CodeMemberMethod End
		{
			get 
			{
				return this.end;
			}
		}

		/// <summary>
		/// Gets the IsIn[Process] method.
		/// </summary>
		public CodeMemberMethod IsInProcess
		{
			get 
			{
				return this.isInProcess;
			}
		}

		/// <summary>
		/// Gets the flag that shows if we're in the process.
		/// </summary>
		public CodeMemberField IsInProcessFlag
		{
			get 
			{
				return this.isInProcessFlag;
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
				return (this.begin.Comments.Count > 0);
			}
			set
			{
				this.begin.Comments.Clear();
				this.end.Comments.Clear();
				this.isInProcess.Comments.Clear();
				this.isInProcessFlag.Comments.Clear();

				if (value)
				{
					this.begin.Comments.AddRange(new SummaryStatements("Begins the " + this.processName + " process."));
					this.end.Comments.AddRange(new SummaryStatements("Ends the " + this.processName + " process."));
					this.isInProcess.Comments.AddRange(new CommentsForProperty(PropertyAccessors.Get, "whether the " + this.processName + " process has begun."));
					this.isInProcessFlag.Comments.AddRange(new SummaryStatements("See " + new SeeExpression(this.isInProcess.Name) + " for information about this field."));
				}
			}
		}
	}
}
