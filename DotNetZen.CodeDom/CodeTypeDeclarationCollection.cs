using System;

namespace DotNetZen.CodeDom
{
	/// <summary>
	/// Represents an extended collection of System.CodeDom.CodeTypeDeclaration objects.
	/// </summary>
	[Serializable, CLSCompliant(true)]
	public class CodeTypeDeclarationCollection : System.CodeDom.CodeTypeDeclarationCollection
	{
		/// <summary>
		/// Implicitly converts the CodeTypeDeclarationCollection to a CodeTypeMemberCollection.
		/// </summary>
		/// <param name="declarations">The declarations collection.</param>
		/// <returns>The members collection.</returns>
		public static implicit operator System.CodeDom.CodeTypeMemberCollection (CodeTypeDeclarationCollection declarations)
		{
			System.CodeDom.CodeTypeMemberCollection members = new System.CodeDom.CodeTypeMemberCollection();
			
			foreach (System.CodeDom.CodeTypeDeclaration declaration in declarations)
			{
				members.Add(declaration);
			}

			return members;
		}
	}
}
