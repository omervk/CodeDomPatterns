using System;
using System.CodeDom;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNetZen.CodeDom.Patterns
{
    /// <summary>
    /// Provides a container for a CodeDOM program graph.
    /// </summary>
    [Serializable, CLSCompliant(true)]
    public class CodePatternCompileUnit : CodeCompileUnit
    {
        /// <summary>
        /// Initializes a new instance of the CodePatternCompileUnit class.
        /// </summary>
        public CodePatternCompileUnit()
        {
        }

        #region AssemblyTitle
        private CodeAttributeDeclaration assemblyTitle;

        /// <summary>
        /// Gets or sets an assembly title for the assembly manifest.
        /// </summary>
        /// <value>An assembly title for the assembly manifest.</value>
        public string AssemblyTitle
        {
            get
            {
                return (this.assemblyTitle != null ? ((string)((CodePrimitiveExpression)(this.assemblyTitle.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyTitle != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyTitle);
                        this.assemblyTitle = null;
                    }
                }
                else
                {
                    if (this.assemblyTitle != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyTitle.Arguments[0].Value)).Value = value;
                    }
                    else
                    {
                        this.assemblyTitle = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyTitleAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value)));

                        this.AssemblyCustomAttributes.Add(this.assemblyTitle);
                    }
                }
            }
        }
        #endregion

        #region AssemblyDescription
        private CodeAttributeDeclaration assemblyDescription;

        /// <summary>
        /// Gets or sets an assembly description for the assembly manifest.
        /// </summary>
        /// <value>An assembly description for the assembly manifest.</value>
        public string AssemblyDescription
        {
            get
            {
                return (this.assemblyDescription != null ? ((string)((CodePrimitiveExpression)(this.assemblyDescription.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyDescription != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyDescription);
                        this.assemblyDescription = null;
                    }
                }
                else
                {
                    if (this.assemblyDescription != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyDescription.Arguments[0].Value)).Value = value;
                    }
                    else
                    {
                        this.assemblyDescription = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyDescriptionAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value)));

                        this.AssemblyCustomAttributes.Add(this.assemblyDescription);
                    }
                }
            }
        }
        #endregion

        #region AssemblyConfiguration
        private CodeAttributeDeclaration assemblyConfiguration;

        /// <summary>
        /// Gets or sets the build configuration, such as retail or debug, for an assembly.
        /// </summary>
        /// <value>The build configuration, such as retail or debug, for an assembly.</value>
        public string AssemblyConfiguration
        {
            get
            {
                return (this.assemblyConfiguration != null ? ((string)((CodePrimitiveExpression)(this.assemblyConfiguration.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyConfiguration != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyConfiguration);
                        this.assemblyConfiguration = null;
                    }
                }
                else
                {
                    if (this.assemblyConfiguration != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyConfiguration.Arguments[0].Value)).Value = value;
                    }
                    else
                    {
                        this.assemblyConfiguration = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyConfigurationAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value)));

                        this.AssemblyCustomAttributes.Add(this.assemblyConfiguration);
                    }
                }
            }
        }
        #endregion

        #region AssemblyCompany
        private CodeAttributeDeclaration assemblyCompany;

        /// <summary>
        /// Gets or sets a company name for the assembly manifest.
        /// </summary>
        /// <value>A company name for the assembly manifest.</value>
        public string AssemblyCompany
        {
            get
            {
                return (this.assemblyCompany != null ? ((string)((CodePrimitiveExpression)(this.assemblyCompany.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyCompany != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyCompany);
                        this.assemblyCompany = null;
                    }
                }
                else
                {
                    if (this.assemblyCompany != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyCompany.Arguments[0].Value)).Value = value;
                    }
                    else
                    {
                        this.assemblyCompany = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyCompanyAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value)));

                        this.AssemblyCustomAttributes.Add(this.assemblyCompany);
                    }
                }
            }
        }
        #endregion

        #region AssemblyProduct
        private CodeAttributeDeclaration assemblyProduct;

        /// <summary>
        /// Gets or sets a product name for the assembly manifest.
        /// </summary>
        /// <value>A product name for the assembly manifest.</value>
        public string AssemblyProduct
        {
            get
            {
                return (this.assemblyProduct != null ? ((string)((CodePrimitiveExpression)(this.assemblyProduct.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyProduct != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyProduct);
                        this.assemblyProduct = null;
                    }
                }
                else
                {
                    if (this.assemblyProduct != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyProduct.Arguments[0].Value)).Value = value;
                    }
                    else
                    {
                        this.assemblyProduct = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyProductAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value)));

                        this.AssemblyCustomAttributes.Add(this.assemblyProduct);
                    }
                }
            }
        }
        #endregion

        #region AssemblyCopyright
        private CodeAttributeDeclaration assemblyCopyright;

        /// <summary>
        /// Gets or sets a copyright for the assembly manifest.
        /// </summary>
        /// <value>A copyright for the assembly manifest.</value>
        public string AssemblyCopyright
        {
            get
            {
                return (this.assemblyCopyright != null ? ((string)((CodePrimitiveExpression)(this.assemblyCopyright.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyCopyright != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyCopyright);
                        this.assemblyCopyright = null;
                    }
                }
                else
                {
                    if (this.assemblyCopyright != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyCopyright.Arguments[0].Value)).Value = value;
                    }
                    else
                    {
                        this.assemblyCopyright = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyCopyrightAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value)));

                        this.AssemblyCustomAttributes.Add(this.assemblyCopyright);
                    }
                }
            }
        }
        #endregion

        #region AssemblyTrademark
        private CodeAttributeDeclaration assemblyTrademark;

        /// <summary>
        /// Gets or sets a trademark for the assembly manifest.
        /// </summary>
        /// <value>A trademark for the assembly manifest.</value>
        public string AssemblyTrademark
        {
            get
            {
                return (this.assemblyTrademark != null ? ((string)((CodePrimitiveExpression)(this.assemblyTrademark.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyTrademark != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyTrademark);
                        this.assemblyTrademark = null;
                    }
                }
                else
                {
                    if (this.assemblyTrademark != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyTrademark.Arguments[0].Value)).Value = value;
                    }
                    else
                    {
                        this.assemblyTrademark = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyTrademarkAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value)));

                        this.AssemblyCustomAttributes.Add(this.assemblyTrademark);
                    }
                }
            }
        }
        #endregion

        #region AssemblyCulture
        private CodeAttributeDeclaration assemblyCulture;

        /// <summary>
        /// Gets or sets which culture the assembly supports.
        /// </summary>
        /// <value>The culture the assembly supports.</value>
        public CultureInfo AssemblyCulture
        {
            get
            {
                return (this.assemblyCulture != null ? new CultureInfo((string)((CodePrimitiveExpression)(this.assemblyCulture.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyCulture != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyCulture);
                        this.assemblyCulture = null;
                    }
                }
                else
                {
                    if (this.assemblyCulture != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyCulture.Arguments[0].Value)).Value = value.ToString();
                    }
                    else
                    {
                        this.assemblyCulture = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyCultureAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value.ToString())));

                        this.AssemblyCustomAttributes.Add(this.assemblyCulture);
                    }
                }
            }
        }
        #endregion

        #region AssemblyVersion
        private CodeAttributeDeclaration assemblyVersion;
        private CodeAttributeDeclaration assemblyFileVersion;

        /// <summary>
        /// Gets or sets the version of the assembly.
        /// </summary>
        /// <value>The version of the assembly.</value>
        public Version AssemblyVersion
        {
            get
            {
                return (this.assemblyVersion != null ? new Version((string)((CodePrimitiveExpression)(this.assemblyVersion.Arguments[0].Value)).Value) : null);
            }
            set
            {
                if (value == null)
                {
                    if (this.assemblyVersion != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.assemblyVersion);
                        this.assemblyVersion = null;
                    }
                }
                else
                {
                    if (this.assemblyVersion != null)
                    {
                        ((CodePrimitiveExpression)(this.assemblyVersion.Arguments[0].Value)).Value = value.ToString();
                        ((CodePrimitiveExpression)(this.assemblyFileVersion.Arguments[0].Value)).Value = value.ToString();
                    }
                    else
                    {
                        this.assemblyVersion = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyVersionAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value.ToString())));
                        this.assemblyFileVersion = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(AssemblyFileVersionAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value.ToString())));

                        this.AssemblyCustomAttributes.Add(this.assemblyVersion);
                        this.AssemblyCustomAttributes.Add(this.assemblyFileVersion);
                    }
                }
            }
        }
        #endregion

        #region ComVisible
        private CodeAttributeDeclaration comVisible;

        /// <summary>
        /// Gets or sets whether types in the assembly are accessibile to COM.
        /// </summary>
        /// <value>true if types in the assembly are accessibile to COM; otherwise, false.</value>
        public bool ComVisible
        {
            get
            {
                return (this.comVisible == null);
            }
            set
            {
                if (value)
                {
                    if (this.comVisible != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.comVisible);
                        this.comVisible = null;
                    }
                }
                else
                {
                    if (this.comVisible != null)
                    {
                        ((CodePrimitiveExpression)(this.comVisible.Arguments[0].Value)).Value = false;
                    }
                    else
                    {
                        this.comVisible = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(ComVisibleAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(false)));

                        this.AssemblyCustomAttributes.Add(this.comVisible);
                    }
                }
            }
        }
        #endregion

        #region CLSCompliant
        private CodeAttributeDeclaration clsCompliant;

        /// <summary>
        /// Gets or sets whether the assembly is compliant with the Common Language Specification (CLS).
        /// </summary>
        /// <value>true if the assembly is compliant with the Common Language Specification (CLS); otherwise, false.</value>
        public bool CLSCompliant
        {
            get
            {
                return (this.clsCompliant != null);
            }
            set
            {
                if (!value)
                {
                    if (this.clsCompliant != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.clsCompliant);
                        this.clsCompliant = null;
                    }
                }
                else
                {
                    if (this.clsCompliant != null)
                    {
                        ((CodePrimitiveExpression)(this.clsCompliant.Arguments[0].Value)).Value = true;
                    }
                    else
                    {
                        this.clsCompliant = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(CLSCompliantAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(true)));

                        this.AssemblyCustomAttributes.Add(this.clsCompliant);
                    }
                }
            }
        }
        #endregion

        #region Guid
        private CodeAttributeDeclaration guid;

        /// <summary>
        /// Gets or sets an explicit <see cref="Guid"/> when an automatic GUID is undesirable.
        /// </summary>
        /// <value>The value of the explicit <see cref="Guid"/>.</value>
        public Guid Guid
        {
            get
            {
                return (this.guid != null ? new Guid((string)((CodePrimitiveExpression)(this.guid.Arguments[0].Value)).Value) : Guid.Empty);
            }
            set
            {
                if (value == Guid.Empty)
                {
                    if (this.guid != null)
                    {
                        this.AssemblyCustomAttributes.Remove(this.guid);
                        this.guid = null;
                    }
                }
                else
                {
                    if (this.guid != null)
                    {
                        ((CodePrimitiveExpression)(this.guid.Arguments[0].Value)).Value = value.ToString();
                    }
                    else
                    {
                        this.guid = new CodeAttributeDeclaration(CodeTypeReferenceStore.Get(typeof(GuidAttribute)),
                            new CodeAttributeArgument(new CodePrimitiveExpression(value.ToString())));

                        this.AssemblyCustomAttributes.Add(this.guid);
                    }
                }
            }
        }
        #endregion
    }
}
