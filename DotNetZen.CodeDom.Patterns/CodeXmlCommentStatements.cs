using System;
using System.CodeDom;
using System.Runtime.InteropServices;

// Not implemented: <permission>, <include>.
namespace DotNetZen.CodeDom.Patterns.XmlComments
{
	#region Xml Comment Types
	#region MultilineStatement
	/// <summary>
	/// The base class for all Xml Comment classes, which supports multiline comments.
	/// </summary>
	/// <remarks>
	/// Please note that in frameworks up to 2.0, xml comments with line breaks would not render well:
	/// In the framework 2.0 this was fixed, but comments from the second line on do not have a space between them and the comment marker.
	/// <list type="table">
	///		<listheader>
	///			<term>
	///				Version
	///			</term>
	///			<description>
	///				Result
	///			</description>
	///		</listheader>
	///		<item>
	///			<term>
	///				Expected
	///			</term>
	///			<description>
	///				<code>/// Line one.
	///				/// Line two.</code>
	///			</description>
	///		</item>
	///		<item>
	///			<term>
	///				1.x
	///			</term>
	///			<description>
	///				<code>/// Line one.
	///				// Line two.</code>
	///			</description>
	///		</item>
	///		<item>
	///			<term>
	///				2.0
	///			</term>
	///			<description>
	///				<code>/// Line one.
	///				///Line two.</code>
	///			</description>
	///		</item>
	/// </list>
	/// </remarks>
	public abstract class MultilineStatement : CodeCommentStatementCollection
	{
		/// <summary>
		/// Initializes a new instance of the MultilineStatement class.
		/// </summary>
		protected MultilineStatement()
		{
		}

		/// <summary>
		/// Initializes a new instance of the MultilineStatement class.
		/// </summary>
		/// <param name="text">The text in the comment.</param>
		protected MultilineStatement(string text)
		{
			this.SetText(text);
		}

		/// <summary>
		/// Sets the text in the comment.
		/// </summary>
		/// <param name="text">The text in the comment.</param>
		protected void SetText(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			this.Clear();
			
			foreach (string line in text.Split(Environment.NewLine.ToCharArray()))
			{
				if (line.Length != 0)
				{
					this.Add(new CodeCommentStatement(line, true));
				}
			}
		}
	}
	#endregion

	/// <summary>
	/// Represents the &lt;summary&gt; tag: the summary of a member.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class SummaryStatements : MultilineStatement
	{
		private const string tag = "<summary>\n{0}.\n</summary>";

		/// <summary>
		/// Initializes a new instance of the SummaryStatements class.
		/// </summary>
		/// <param name="text">The text that would appear inside the tag.</param>
		public SummaryStatements(string text)
			: base(string.Format(tag, text.TrimEnd('.', ' ')))
		{
		}
	}

	/// <summary>
	/// Represents the &lt;c&gt; tag: inline text marked as code.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public struct TextAsCodeExpression
	{
		private const string tag = "<c>{0}</c>";

		private string text;

		/// <summary>
		/// Initializes a new instance of the TextAsCodeExpression class.
		/// </summary>
		/// <param name="text">The text that would appear inside the tag.</param>
		public TextAsCodeExpression(string text)
		{
			this.text = string.Format(tag, text);
		}

		/// <summary>
		/// Converts the object to its string representation.
		/// </summary>
		/// <param name="c">The original object.</param>
		/// <returns>The text in the object.</returns>
		public static implicit operator string(TextAsCodeExpression c)
		{
			return c.text;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.text;
		}
	}

	/// <summary>
	/// Represents the &lt;code&gt; tag: a text block marked as code.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class MultilineTextAsCodeStatements : MultilineStatement
	{
		private const string tag = "<code>\n{0}\n</code>";

		/// <summary>
		/// Initializes a new instance of the MultilineTextAsCodeStatements class.
		/// </summary>
		/// <param name="text">The text that would appear inside the tag.</param>
		public MultilineTextAsCodeStatements(string text)
			: base(string.Format(tag, text))
		{
		}
	}

	/// <summary>
	/// Represents the &lt;example&gt; tag: a text block that is an example.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class ExampleStatements : MultilineStatement
	{
		private const string tag = "<example>{0}.\n{1}\n</example>";

		/// <summary>
		/// Initializes a new instance of the ExampleStatements class.
		/// </summary>
		/// <param name="explanation">The explanation of the example.</param>
		/// <param name="example">The code for the example.</param>
		public ExampleStatements(string explanation, MultilineTextAsCodeStatements example)
			: base(string.Format(tag, explanation.TrimEnd('.', ' '), example))
		{
		}
	}

	/// <summary>
	/// Represents the &lt;exception&gt; tag: an exception that might be thrown by the member.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class ExceptionStatements : MultilineStatement
	{
		private const string tag = "<exception cref=\"{0}\">{1}.</exception>";

		/// <summary>
		/// Initializes a new instance of the ExceptionStatements class.
		/// </summary>
		/// <param name="type">The type of exception.</param>
		/// <param name="whenIsItThrown">The condition which causes the exception to be thrown. Usually begins with "Thrown when".</param>
		private ExceptionStatements(string type, string whenIsItThrown)
			: base(string.Format(tag, type, whenIsItThrown.TrimEnd('.', ' ')))
		{
		}

		/// <summary>
		/// Initializes a new instance of the ExceptionStatements class.
		/// </summary>
		/// <param name="type">The type of exception.</param>
		/// <param name="whenIsItThrown">The condition which causes the exception to be thrown. Usually begins with "Thrown when".</param>
		public ExceptionStatements(Type type, string whenIsItThrown)
			: this(type.FullName, whenIsItThrown)
		{
		}

		/// <summary>
		/// Initializes a new instance of the ExceptionStatements class.
		/// </summary>
		/// <param name="type">The type of exception.</param>
		/// <param name="whenIsItThrown">The condition which causes the exception to be thrown. Usually begins with "Thrown when".</param>
		public ExceptionStatements(CodeTypeReference type, string whenIsItThrown)
			: this(type.BaseType, whenIsItThrown)
		{
		}
	}

	/// <summary>
	/// Represents the &lt;list&gt; tag: a list/table.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class ListStatements : MultilineStatement
	{
		private const string headerOpeningTag = "<listheader>";
		private const string headerClosingTag = "</listheader>";
		private const string itemOpeningTag = "<item>";
		private const string itemClosingTag = "</item>";
		private const string termOpeningTag = "<term>";
		private const string termClosingTag = "</term>";
		private const string descriptionOpeningTag = "<description>";
		private const string descriptionClosingTag = "</description>";

		/// <summary>
		/// Initializes a new instance of the ListStatements class.
		/// </summary>
		/// <param name="type">The type of list.</param>
		/// <param name="header">The list's header.</param>
		/// <param name="items">The items in the list.</param>
		public ListStatements(XmlCommentListType type, ListItem header, params ListItem[] items)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}

			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			string comment = this.StartList(type);
			comment += headerOpeningTag + termOpeningTag + header.Term + termClosingTag + descriptionOpeningTag + header.Description + descriptionClosingTag + headerClosingTag;
			comment += this.GenerateItems(items);
			comment += this.EndList();

			this.SetText(comment);
		}

		/// <summary>
		/// Initializes a new instance of the ListStatements class.
		/// </summary>
		/// <param name="type">The type of list.</param>
		/// <param name="header">The list's header.</param>
		/// <param name="items">The items in the list.</param>
		public ListStatements(XmlCommentListType type, string header, params string[] items)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}

			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			string comment = this.StartList(type);
			comment += headerOpeningTag + termOpeningTag + header + termClosingTag + headerClosingTag;
			comment += this.GenerateItems(items);
			comment += this.EndList();

			this.SetText(comment);
		}

		/// <summary>
		/// Initializes a new instance of the ListStatements class.
		/// </summary>
		/// <param name="type">The type of list.</param>
		/// <param name="items">The items in the list.</param>
		public ListStatements(XmlCommentListType type, params ListItem[] items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			string comment = this.StartList(type);
			comment += this.GenerateItems(items);
			comment += this.EndList();

			this.SetText(comment);
		}

		/// <summary>
		/// Initializes a new instance of the ListStatements class.
		/// </summary>
		/// <param name="type">The type of list.</param>
		/// <param name="items">The items in the list.</param>
		public ListStatements(XmlCommentListType type, params string[] items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}

			string comment = this.StartList(type);
			comment += this.GenerateItems(items);
			comment += this.EndList();

			this.SetText(comment);
		}

		/// <summary>
		/// Initializes a new instance of the ListStatements class.
		/// </summary>
		/// <param name="items">The items in the list.</param>
		public ListStatements(params string[] items)
			: this(XmlCommentListType.Default, items)
		{
		}

		private string StartList(XmlCommentListType type)
		{
			string comment = "<list";

			if (type != XmlCommentListType.Default)
			{
				comment += " type=\"";

				switch (type)
				{
					case XmlCommentListType.Bullets:
						comment += "bullet";
						break;
					case XmlCommentListType.Numbered:
						comment += "number";
						break;
					case XmlCommentListType.Table:
						comment += "table";
						break;
				}

				comment += "\"";
			}

			return comment + ">" + Environment.NewLine;
		}

		private string GenerateItems(string[] items)
		{
			string itemList = string.Empty;

			foreach (string item in items)
			{
				if (item == null)
				{
					throw new ArgumentNullException("items", "An item on the list is null.");
				}

				itemList += itemOpeningTag + termOpeningTag + item + termClosingTag + itemClosingTag;
			}
			
			return itemList;
		}

		private string GenerateItems(ListItem[] items)
		{
			string itemList = string.Empty;

			foreach (ListItem item in items)
			{
				if (item == null)
				{
					throw new ArgumentNullException("items", "An item on the list is null.");
				}

				itemList += itemOpeningTag + termOpeningTag + item.Term + termClosingTag + descriptionOpeningTag + item.Description + descriptionClosingTag + itemClosingTag;
			}
			
			return itemList;
		}

		private string EndList()
		{
			return "</list>";
		}
	}

	/// <summary>
	/// Represents an item in the &lt;list&gt; tag.
	/// </summary>
	public class ListItem
	{
		private string term;
		private string description;

		/// <summary>
		/// Initializes a new instance of the ListItem class.
		/// </summary>
		/// <param name="term">When the list is a table, the row's value in the first column. Otherwise, the term in the definition list.</param>
		/// <param name="description">When the list is a table, the row's value in the second column. Otherwise, the description in the definition list.</param>
		public ListItem(string term, string description)
		{
			if (term == null)
			{
				throw new ArgumentNullException("term");
			}

			if (description == null)
			{
				throw new ArgumentNullException("description");
			}

			this.term = term;
			this.description = description;
		}

		/// <summary>
		/// Gets this row's term.
		/// </summary>
		/// <value>When the list is a table, the row's value in the first column. Otherwise, the term in the definition list.</value>
		public string Term
		{
			get
			{
				return this.term;
			}
		}

		/// <summary>
		/// Gets this row's description.
		/// </summary>
		/// <value>When the list is a table, the row's value in the second column. Otherwise, the description in the definition list.</value>
		public string Description
		{
			get
			{
				return this.description;
			}
		}
	}

	/// <summary>
	/// Represents the &lt;para&gt; tag: a new paragraph in the text.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public struct ParagraphExpression
	{
		private const string tag = "<para>{0}</para>";

		private string text;

		/// <summary>
		/// Initializes a new instance of the ParagraphExpression class.
		/// </summary>
		/// <param name="text">The text that would appear inside the tag.</param>
		public ParagraphExpression(string text)
		{
			this.text = string.Format(tag, text);
		}

		/// <summary>
		/// Converts the object to its string representation.
		/// </summary>
		/// <param name="para">The original object.</param>
		/// <returns>The text in the object.</returns>
		public static implicit operator string(ParagraphExpression para)
		{
			return para.text;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.text;
		}
	}

	/// <summary>
	/// Represents the &lt;param&gt; tag: an explanation about a parameter that the member takes.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class ParameterStatements : MultilineStatement
	{
		private const string tag = "<param name=\"{0}\">{1}.</param>";
		private string name;

		/// <summary>
		/// Initializes a new instance of the ParameterStatements class.
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <param name="description">A description of this parameter.</param>
		public ParameterStatements(string name, string description)
			: base(string.Format(tag, name, description.TrimEnd('.', ' ')))
		{
			this.name = name;
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}
	}

	/// <summary>
	/// Represents the &lt;paramref&gt; tag: a reference to a parameter that the member takes.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public struct ParameterReferenceExpression
	{
		private const string tag = "<paramref name=\"{0}\" />";

		private string text;

		/// <summary>
		/// Initializes a new instance of the ParameterReferenceExpression class.
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		public ParameterReferenceExpression(string name)
		{
			this.text = string.Format(tag, name);
		}

		/// <summary>
		/// Converts the object to its string representation.
		/// </summary>
		/// <param name="paramRef">The original object.</param>
		/// <returns>The text in the object.</returns>
		public static implicit operator string(ParameterReferenceExpression paramRef)
		{
			return paramRef.text;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.text;
		}
	}

	/// <summary>
	/// Represents the &lt;remarks&gt; tag: special remarks about the member.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class RemarksStatements : MultilineStatement
	{
		private const string tag = "<remarks>{0}.</remarks>";

		/// <summary>
		/// Initializes a new instance of the RemarksStatements class.
		/// </summary>
		/// <param name="text">The text that would appear inside the tag.</param>
		public RemarksStatements(string text)
			: base(string.Format(tag, text.TrimEnd('.', ' ')))
		{
		}
	}

	/// <summary>
	/// Represents the &lt;returns&gt; tag: an explanation about the method's return value.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class ReturnsStatements : MultilineStatement
	{
		private const string tag = "<returns>{0}.</returns>";

		/// <summary>
		/// Initializes a new instance of the ReturnsStatements class.
		/// </summary>
		/// <param name="text">The text that would appear inside the tag.</param>
		public ReturnsStatements(string text)
			: base(string.Format(tag, text.TrimEnd('.', ' ')))
		{
		}
	}

	/// <summary>
	/// Represents the &lt;see&gt; tag: a reference to a type or member.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public struct SeeExpression
	{
		private const string tag = "<see cref=\"{0}\" />";

		private string text;

		/// <summary>
		/// Initializes a new instance of the SeeExpression class.
		/// </summary>
		/// <param name="type">The referenced type.</param>
		public SeeExpression(Type type)
		{
			this.text = string.Format(tag, type.FullName);
		}

		/// <summary>
		/// Initializes a new instance of the SeeExpression class.
		/// </summary>
		/// <param name="member">The referenced member or type.</param>
		public SeeExpression(string member)
		{
			this.text = string.Format(tag, member);
		}

		/// <summary>
		/// Converts the object to its string representation.
		/// </summary>
		/// <param name="see">The original object.</param>
		/// <returns>The text in the object.</returns>
		public static implicit operator string(SeeExpression see)
		{
			return see.text;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.text;
		}
	}

	/// <summary>
	/// Represents the &lt;seealso&gt; tag: a reference to a type or member that would appear in the See Also section.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class SeeAlsoStatements : MultilineStatement
	{
		private const string tag = "<seealso cref=\"{0}\" />";

		/// <summary>
		/// Initializes a new instance of the SeeAlsoStatements class.
		/// </summary>
		/// <param name="member">The referenced member or type.</param>
		public SeeAlsoStatements(string member)
			: base(string.Format(tag, member))
		{
		}
	}

	/// <summary>
	/// Represents the &lt;value&gt; tag: an explanation about the value of the member.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class ValueStatements : MultilineStatement
	{
		private const string tag = "<value>{0}.</value>";

		/// <summary>
		/// Initializes a new instance of the ValueStatements class.
		/// </summary>
		/// <param name="description">The value's description.</param>
		public ValueStatements(string description)
			: base(string.Format(tag, description.TrimEnd('.', ' ')))
		{
		}
	}
	#endregion

	#region Member-Specific Xml Comment Templates
	#region For Properties
	/// <summary>
	/// The recommended Xml Comment representation for properties.
	/// </summary>
	public class CommentsForProperty : CodeCommentStatementCollection
	{
		private SummaryStatements summary;
		private ValueStatements value;

		/// <summary>
		/// Initializes a new instance of the ForPropertyStatements class.
		/// </summary>
		/// <param name="accessors">The property's accessors.</param>
		/// <param name="indicates">What the property indicates. For instance, <c>"whether the light is on."</c>.</param>
		public CommentsForProperty(PropertyAccessors accessors, string indicates)
		{
			if (indicates == null)
			{
				throw new ArgumentNullException("indicates");
			}

			string summary;

			if ((accessors & PropertyAccessors.Get) == PropertyAccessors.Get)
			{
				if ((accessors & PropertyAccessors.Set) == PropertyAccessors.Set)
				{
					summary = "Gets or sets ";
				}
				else
				{
					summary = "Gets ";
				}
			}
			else
			{
				summary = "Sets ";
			}

			summary += Char.ToLower(indicates[0]) + indicates.Substring(1);

			this.summary = new SummaryStatements(summary);
			this.value = new ValueStatements(Char.ToUpper(indicates[0]) + indicates.Substring(1));

			this.AddRange(this.summary);
			this.AddRange(this.value);
		}

		/// <summary>
		/// Gets the property's summary.
		/// </summary>
		/// <value>The property's summary.</value>
		public SummaryStatements Summary
		{
			get
			{
				return this.summary;
			}
		}

		/// <summary>
		/// Gets the property's return value.
		/// </summary>
		/// <value>The property's return value.</value>
		public ValueStatements Value
		{
			get
			{
				return this.value;
			}
		}
	}
	#endregion

	#region For Indexers
	/// <summary>
	/// The recommended Xml Comment representation for indexed properties.
	/// </summary>
	public class CommentsForIndexer : CommentsForProperty
	{
		private System.Collections.Hashtable parameters = new System.Collections.Hashtable();

		/// <summary>
		/// Initializes a new instance of the ForIndexerStatements class.
		/// </summary>
		/// <param name="accessors">The indexer's accessors.</param>
		/// <param name="indicates">What the indexer indicates. For instance, <c>"whether the light is on for the bulb at index."</c>.</param>
		/// <param name="indexerParameters">Comments on the indexer's parameters.</param>
		public CommentsForIndexer(PropertyAccessors accessors, string indicates, params ParameterStatements[] indexerParameters)
			: base(accessors, indicates)
		{
			if (indexerParameters == null)
			{
				throw new ArgumentNullException("indexerParameters");
			}
			
			foreach (ParameterStatements indexerParameter in indexerParameters)
			{
				this.parameters.Add(indexerParameter.Name, indexerParameter);
				this.AddRange(indexerParameter);
			}
		}

		/// <summary>
		/// Gets the comment on a specific parameter.
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <returns>The comment set for the specified parameter.</returns>
		/// <exception cref="NotSupportedException">Thrown when <paramref name="name" /> is not the name of a parameter.</exception>
		public ParameterStatements GetParameterComment(string name)
		{
			if (!this.parameters.Contains(name))
			{
				throw new NotSupportedException("The name " + name + " is not the name of a parameter.");
			}

			return (ParameterStatements)this.parameters[name];
		}
	}
	#endregion

	#region For Instance Constructors
	/// <summary>
	/// The recommended Xml Comment representation for instance constructors.
	/// </summary>
	public class CommentsForConstructor : CodeCommentStatementCollection
	{
		private SummaryStatements summary;
		private System.Collections.Hashtable parameters = new System.Collections.Hashtable();

		/// <summary>
		/// Initializes a new instance of the ForInstanceConstructor class.
		/// </summary>
		/// <param name="type">The name of the type (DO: MyTypeName; AVOID: MyNamespace.MyTypeName).</param>
		/// <param name="parameters">The parameters supplied to the constructor.</param>
		public CommentsForConstructor(string type, params ParameterStatements[] parameters)
		{
			if (type == null || type == string.Empty)
			{
				throw new ArgumentNullException("type");
			}

			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}

			this.summary = new SummaryStatements("Initializes a new instance of the " + new SeeExpression(type) + " class.");
			this.AddRange(this.summary);
			
			foreach (ParameterStatements parameter in parameters)
			{
				this.parameters.Add(parameter.Name, parameter);
				this.AddRange(parameter);
			}
		}

		/// <summary>
		/// Gets the comment on a specific parameter.
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <returns>The comment set for the specified parameter.</returns>
		/// <exception cref="NotSupportedException">Thrown when <paramref name="name" /> is not the name of a parameter.</exception>
		public ParameterStatements GetParameterComment(string name)
		{
			if (!this.parameters.Contains(name))
			{
				throw new NotSupportedException("The name " + name + " is not the name of a parameter.");
			}

			return (ParameterStatements)this.parameters[name];
		}

		/// <summary>
		/// Gets the constructor's summary.
		/// </summary>
		/// <value>The constructor's summary.</value>
		public SummaryStatements Summary
		{
			get
			{
				return this.summary;
			}
		}
	}
	#endregion

	#region For Methods
	/// <summary>
	/// The recommended Xml Comment representation for methods.
	/// </summary>
	public class CommentsForMethod : CodeCommentStatementCollection
	{
		private SummaryStatements summary;
		private System.Collections.Hashtable parameters = new System.Collections.Hashtable();
		private ReturnsStatements returns;

		/// <summary>
		/// Initializes a new instance of the CommentsForMethod class.
		/// </summary>
		/// <param name="summary">What the method does.</param>
		/// <param name="parameters">The parameters supplied to the method.</param>
		public CommentsForMethod(string summary, params ParameterStatements[] parameters)
		{
			if (summary == null || summary == string.Empty)
			{
				throw new ArgumentNullException("summary");
			}

			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}

			this.summary = new SummaryStatements(summary);
			this.AddRange(this.summary);
			
			foreach (ParameterStatements parameter in parameters)
			{
				this.parameters.Add(parameter.Name, parameter);
				this.AddRange(parameter);
			}
		}

		/// <summary>
		/// Initializes a new instance of the CommentsForMethod class.
		/// </summary>
		/// <param name="summary">What the method does.</param>
		/// <param name="returns">What the method returns.</param>
		/// <param name="parameters">The parameters supplied to the method.</param>
		public CommentsForMethod(string summary, string returns, params ParameterStatements[] parameters)
			: this(summary, parameters)
		{
			if (returns == null || returns == string.Empty)
			{
				throw new ArgumentNullException("returns", "Please use the appropriate overload: CommentsForMethod.ctor(string, ParameterStatements[]).");
			}

			this.returns = new ReturnsStatements(returns);
			this.AddRange(this.returns);
		}

		/// <summary>
		/// Gets the comment on a specific parameter.
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <returns>The comment set for the specified parameter.</returns>
		/// <exception cref="NotSupportedException">Thrown when <paramref name="name" /> is not the name of a parameter.</exception>
		public ParameterStatements GetParameterComment(string name)
		{
			if (!this.parameters.Contains(name))
			{
				throw new NotSupportedException("The name " + name + " is not the name of a parameter.");
			}

			return (ParameterStatements)this.parameters[name];
		}

		/// <summary>
		/// Gets the method's summary.
		/// </summary>
		/// <value>The method's summary.</value>
		public SummaryStatements Summary
		{
			get
			{
				return this.summary;
			}
		}

		/// <summary>
		/// Gets the method's return value.
		/// </summary>
		/// <value>The method's return value.</value>
		public ReturnsStatements Returns
		{
			get
			{
				return this.returns;
			}
		}
	}
	#endregion

	#region For Delegates
	/// <summary>
	/// The recommended Xml Comment representation for delegates.
	/// </summary>
	public class CommentsForDelegate : CodeCommentStatementCollection
	{
		private SummaryStatements summary;
		private System.Collections.Hashtable parameters = new System.Collections.Hashtable();
		private ReturnsStatements returns;

		/// <summary>
		/// Initializes a new instance of the CommentsForDelegate class.
		/// </summary>
		/// <param name="summary">What the delegate represents.</param>
		/// <param name="parameters">The parameters supplied to the delegate.</param>
		public CommentsForDelegate(string summary, params ParameterStatements[] parameters)
		{
			if (summary == null || summary == string.Empty)
			{
				throw new ArgumentNullException("summary");
			}

			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}

			this.summary = new SummaryStatements(summary);
			this.AddRange(this.summary);
			
			foreach (ParameterStatements parameter in parameters)
			{
				this.parameters.Add(parameter.Name, parameter);
				this.AddRange(parameter);
			}
		}

		/// <summary>
		/// Initializes a new instance of the CommentsForDelegate class.
		/// </summary>
		/// <param name="summary">What the delegate represents.</param>
		/// <param name="returns">What the delegate's return value represents.</param>
		/// <param name="parameters">The parameters supplied to the delegate.</param>
		public CommentsForDelegate(string summary, string returns, params ParameterStatements[] parameters)
			: this(summary, parameters)
		{
			if (returns == null || returns == string.Empty)
			{
				throw new ArgumentNullException("returns", "Please use the appropriate overload: CommentsForDelegate.ctor(string, ParameterStatements[]).");
			}

			this.returns = new ReturnsStatements(returns);
		}

		/// <summary>
		/// Gets the comment on a specific parameter.
		/// </summary>
		/// <param name="name">The parameter's name.</param>
		/// <returns>The comment set for the specified parameter.</returns>
		/// <exception cref="NotSupportedException">Thrown when <paramref name="name" /> is not the name of a parameter.</exception>
		public ParameterStatements GetParameterComment(string name)
		{
			if (!this.parameters.Contains(name))
			{
				throw new NotSupportedException("The name " + name + " is not the name of a parameter.");
			}

			return (ParameterStatements)this.parameters[name];
		}

		/// <summary>
		/// Gets the delegate's summary.
		/// </summary>
		/// <value>The delegate's summary.</value>
		public SummaryStatements Summary
		{
			get
			{
				return this.summary;
			}
		}

		/// <summary>
		/// Gets the delegate's return value.
		/// </summary>
		/// <value>The delegate's return value.</value>
		public ReturnsStatements Returns
		{
			get
			{
				return this.returns;
			}
		}
	}
	#endregion
	#endregion
}