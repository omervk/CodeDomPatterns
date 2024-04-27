using System;
using System.CodeDom;
using System.Security.Permissions;
using System.Collections;

namespace DotNetZen.CodeDom.Patterns
{
    /// <summary>
    /// Represents the decoration of a member with a Code Access Security attribute.
    /// </summary>
    /// <example>Output example:<code>
    /// [System.Security.Permissions.UIPermissionAttribute(System.Security.Permissions.SecurityAction.Demand,
    ///     Clipboard=System.Security.Permissions.UIPermissionClipboard.AllClipboard)]
    /// private void MyMethod()
    /// {
    /// }
    ///	</code></example>
    [Serializable, CLSCompliant(true)]
    public sealed class CodePatternCasAttribute : CodeAttributeDeclaration
    {
        private static readonly string UnrestrictedArgumentName = typeof(SecurityAttribute).GetProperty("Unrestricted").Name;

        private CodeAttributeArgument action;
        private CodeAttributeArgument unrestricted;

        /// <summary>
        /// Initializes a new instance of the CodePatternCasAttribute class.
        /// </summary>
        /// <param name="permission">The type of permission demanded.</param>
        /// <remarks><see cref="Action"/> defaults to <see cref="SecurityAction.Demand"/>. <see cref="Unrestricted"/> defaults to false.</remarks>
        public CodePatternCasAttribute(Permissions.Permission permission)
            : this(SecurityAction.Demand, permission)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodePatternCasAttribute class.
        /// </summary>
        /// <param name="permission">The type of permission demanded.</param>
        /// <param name="action">The action taken.</param>
        /// <remarks><see cref="Unrestricted"/> defaults to false.</remarks>
        public CodePatternCasAttribute(SecurityAction action, Permissions.Permission permission)
            : this(action, false, permission)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CodePatternCasAttribute class.
        /// </summary>
        /// <param name="permission">The type of permission demanded.</param>
        /// <param name="action">The action taken.</param>
        /// <param name="unrestricted">A value indicating whether full (unrestricted) permission to the resource protected by the attribute is declared.</param>
        public CodePatternCasAttribute(SecurityAction action, bool unrestricted, Permissions.Permission permission)
            : base(CodeTypeReferenceStore.Get(permission.Type))
        {
            this.Action = action;
            this.Unrestricted = unrestricted;

            permission.Apply(this);
        }

        /// <summary>
        /// Gets a security action.
        /// </summary>
        /// <value>One of the <see cref="SecurityAction"/> values.</value>
        public SecurityAction Action
        {
            set
            {
                if (this.action == null)
                {
                    this.action = new CodeAttributeArgument(new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression(CodeTypeReferenceStore.Get(typeof(SecurityAction))), value.ToString()));

                    this.Arguments.Add(this.action);
                }
                else
                {
                    ((CodeFieldReferenceExpression)(this.action.Value)).FieldName = value.ToString();
                }
            }
        }

        /// <summary>
        /// Sets a value indicating whether full (unrestricted) permission to the resource protected by the attribute is declared.
        /// </summary>
        /// <value>true if full permission to the protected resource is declared; otherwise, false.</value>
        public bool Unrestricted
        {
            set
            {
                if (value)
                {
                    if (this.unrestricted == null)
                    {
                        this.unrestricted = new CodeAttributeArgument(UnrestrictedArgumentName, new CodePrimitiveExpression(true));
                        this.Arguments.Add(this.unrestricted);
                    }
                }
                else if (this.unrestricted != null)
                {
                    this.Arguments.Remove(this.unrestricted);
                    this.unrestricted = null;
                }
            }
        }
    }

    namespace Permissions
    {
        /// <summary>
        /// Base type for permission attribute builders.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public abstract class Permission
        {
            private Hashtable arguments = new Hashtable();

            /// <summary>
            /// Initializes a new instance of the Permission class.
            /// </summary>
            protected Permission()
            {
            }

            /// <summary>
            /// Gets the type of the attribute that will be generated.
            /// </summary>
            protected internal abstract Type Type
            {
                get;
            }

            /// <summary>
            /// Sets an argument of the attribute.
            /// </summary>
            /// <param name="name">The name of the argument.</param>
            /// <param name="value">The constant literal value (string literal, enum member, etc.) of the argument.</param>
            /// <remarks>This should be called by implementing builders to add named arguments.</remarks>
            protected void SetArgument(string name, object value)
            {
                if (value == null)
                {
                    if (this.arguments.ContainsKey(name))
                    {
                        this.arguments.Remove(name);
                    }
                }
                else
                {
                    if (value is Enum)
                    {
                        if (this.arguments.ContainsKey(name))
                        {
                            CodeAttributeArgument argument = ((CodeAttributeArgument)(this.arguments[name]));

                            ((CodeFieldReferenceExpression)(argument.Value)).FieldName = value.ToString();
                        }
                        else
                        {
                            CodeAttributeArgument argument = new CodeAttributeArgument(name, new CodeFieldReferenceExpression(
                                new CodeTypeReferenceExpression(CodeTypeReferenceStore.Get(value.GetType())), value.ToString()));

                            this.arguments.Add(name, argument);
                        }
                    }
                    else
                    {
                        if (this.arguments.ContainsKey(name))
                        {
                            CodeAttributeArgument argument = ((CodeAttributeArgument)(this.arguments[name]));

                            ((CodePrimitiveExpression)(argument.Value)).Value = value;
                        }
                        else
                        {
                            CodeAttributeArgument argument = new CodeAttributeArgument(name, new CodePrimitiveExpression(value));

                            this.arguments.Add(name, argument);
                        }
                    }
                }
            }

            /// <summary>
            /// Applies arguments to the attribute.
            /// </summary>
            /// <param name="attribute">An instance of the attribute when building.</param>
            internal void Apply(CodeAttributeDeclaration attribute)
            {
                if (attribute == null)
                    throw new ArgumentNullException("attribute");

                foreach (DictionaryEntry entry in this.arguments)
                {
                    attribute.Arguments.Add(((CodeAttributeArgument)(entry.Value)));
                }
            }
        }

        #region Actual permission attributes
        /// <summary>
        /// Builds a <see cref="System.Configuration.ConfigurationPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Configuration : Permission
        {
            /// <summary>
            /// An instance of <see cref="Configuration"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Configuration Empty = new Configuration();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Configuration.ConfigurationPermissionAttribute);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Net.DnsPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Dns : Permission
        {
            /// <summary>
            /// An instance of <see cref="Dns"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Dns Empty = new Dns();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Net.DnsPermissionAttribute);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.GacIdentityPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class GacIdentity : Permission
        {
            /// <summary>
            /// An instance of <see cref="GacIdentity"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly GacIdentity Empty = new GacIdentity();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.GacIdentityPermissionAttribute);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Transactions.DistributedTransactionPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class DistributedTransaction : Permission
        {
            /// <summary>
            /// An instance of <see cref="DistributedTransaction"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly DistributedTransaction Empty = new DistributedTransaction();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Transactions.DistributedTransactionPermissionAttribute);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Data.Common.DBDataPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class DBData : Permission
        {
            /// <summary>
            /// An instance of <see cref="DBData"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly DBData Empty = new DBData();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Data.Common.DBDataPermissionAttribute);
                }
            }

            private static readonly string AllowBlankPasswordArgumentName = typeof(System.Data.Common.DBDataPermissionAttribute).GetProperty("AllowBlankPassword").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Data.Common.DBDataPermissionAttribute.AllowBlankPassword" />.
            /// </summary>
            public System.Boolean AllowBlankPassword
            {
                set
                {
                    base.SetArgument(AllowBlankPasswordArgumentName, value);
                }
            }

            private static readonly string ConnectionStringArgumentName = typeof(System.Data.Common.DBDataPermissionAttribute).GetProperty("ConnectionString").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Data.Common.DBDataPermissionAttribute.ConnectionString" />.
            /// </summary>
            public System.String ConnectionString
            {
                set
                {
                    base.SetArgument(ConnectionStringArgumentName, value);
                }
            }

            private static readonly string KeyRestrictionBehaviorArgumentName = typeof(System.Data.Common.DBDataPermissionAttribute).GetProperty("KeyRestrictionBehavior").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Data.Common.DBDataPermissionAttribute.KeyRestrictionBehavior" />.
            /// </summary>
            public System.Data.KeyRestrictionBehavior KeyRestrictionBehavior
            {
                set
                {
                    base.SetArgument(KeyRestrictionBehaviorArgumentName, value);
                }
            }

            private static readonly string KeyRestrictionsArgumentName = typeof(System.Data.Common.DBDataPermissionAttribute).GetProperty("KeyRestrictions").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Data.Common.DBDataPermissionAttribute.KeyRestrictions" />.
            /// </summary>
            public System.String KeyRestrictions
            {
                set
                {
                    base.SetArgument(KeyRestrictionsArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Diagnostics.EventLogPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class EventLog : Permission
        {
            /// <summary>
            /// An instance of <see cref="EventLog"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly EventLog Empty = new EventLog();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Diagnostics.EventLogPermissionAttribute);
                }
            }

            private static readonly string MachineNameArgumentName = typeof(System.Diagnostics.EventLogPermissionAttribute).GetProperty("MachineName").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Diagnostics.EventLogPermissionAttribute.MachineName" />.
            /// </summary>
            public System.String MachineName
            {
                set
                {
                    base.SetArgument(MachineNameArgumentName, value);
                }
            }

            private static readonly string PermissionAccessArgumentName = typeof(System.Diagnostics.EventLogPermissionAttribute).GetProperty("PermissionAccess").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Diagnostics.EventLogPermissionAttribute.PermissionAccess" />.
            /// </summary>
            public System.Diagnostics.EventLogPermissionAccess PermissionAccess
            {
                set
                {
                    base.SetArgument(PermissionAccessArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Diagnostics.PerformanceCounterPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class PerformanceCounter : Permission
        {
            /// <summary>
            /// An instance of <see cref="PerformanceCounter"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly PerformanceCounter Empty = new PerformanceCounter();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Diagnostics.PerformanceCounterPermissionAttribute);
                }
            }

            private static readonly string CategoryNameArgumentName = typeof(System.Diagnostics.PerformanceCounterPermissionAttribute).GetProperty("CategoryName").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Diagnostics.PerformanceCounterPermissionAttribute.CategoryName" />.
            /// </summary>
            public System.String CategoryName
            {
                set
                {
                    base.SetArgument(CategoryNameArgumentName, value);
                }
            }

            private static readonly string MachineNameArgumentName = typeof(System.Diagnostics.PerformanceCounterPermissionAttribute).GetProperty("MachineName").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Diagnostics.PerformanceCounterPermissionAttribute.MachineName" />.
            /// </summary>
            public System.String MachineName
            {
                set
                {
                    base.SetArgument(MachineNameArgumentName, value);
                }
            }

            private static readonly string PermissionAccessArgumentName = typeof(System.Diagnostics.PerformanceCounterPermissionAttribute).GetProperty("PermissionAccess").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Diagnostics.PerformanceCounterPermissionAttribute.PermissionAccess" />.
            /// </summary>
            public System.Diagnostics.PerformanceCounterPermissionAccess PermissionAccess
            {
                set
                {
                    base.SetArgument(PermissionAccessArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.DirectoryServices.DirectoryServicesPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class DirectoryServices : Permission
        {
            /// <summary>
            /// An instance of <see cref="DirectoryServices"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly DirectoryServices Empty = new DirectoryServices();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.DirectoryServices.DirectoryServicesPermissionAttribute);
                }
            }

            private static readonly string PathArgumentName = typeof(System.DirectoryServices.DirectoryServicesPermissionAttribute).GetProperty("Path").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.DirectoryServices.DirectoryServicesPermissionAttribute.Path" />.
            /// </summary>
            public System.String Path
            {
                set
                {
                    base.SetArgument(PathArgumentName, value);
                }
            }

            private static readonly string PermissionAccessArgumentName = typeof(System.DirectoryServices.DirectoryServicesPermissionAttribute).GetProperty("PermissionAccess").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.DirectoryServices.DirectoryServicesPermissionAttribute.PermissionAccess" />.
            /// </summary>
            public System.DirectoryServices.DirectoryServicesPermissionAccess PermissionAccess
            {
                set
                {
                    base.SetArgument(PermissionAccessArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Drawing.Printing.PrintingPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Printing : Permission
        {
            /// <summary>
            /// An instance of <see cref="Printing"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Printing Empty = new Printing();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Drawing.Printing.PrintingPermissionAttribute);
                }
            }

            private static readonly string LevelArgumentName = typeof(System.Drawing.Printing.PrintingPermissionAttribute).GetProperty("Level").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Drawing.Printing.PrintingPermissionAttribute.Level" />.
            /// </summary>
            public System.Drawing.Printing.PrintingPermissionLevel Level
            {
                set
                {
                    base.SetArgument(LevelArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Messaging.MessageQueuePermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class MessageQueue : Permission
        {
            /// <summary>
            /// An instance of <see cref="MessageQueue"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly MessageQueue Empty = new MessageQueue();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Messaging.MessageQueuePermissionAttribute);
                }
            }

            private static readonly string CategoryArgumentName = typeof(System.Messaging.MessageQueuePermissionAttribute).GetProperty("Category").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Messaging.MessageQueuePermissionAttribute.Category" />.
            /// </summary>
            public System.String Category
            {
                set
                {
                    base.SetArgument(CategoryArgumentName, value);
                }
            }

            private static readonly string LabelArgumentName = typeof(System.Messaging.MessageQueuePermissionAttribute).GetProperty("Label").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Messaging.MessageQueuePermissionAttribute.Label" />.
            /// </summary>
            public System.String Label
            {
                set
                {
                    base.SetArgument(LabelArgumentName, value);
                }
            }

            private static readonly string MachineNameArgumentName = typeof(System.Messaging.MessageQueuePermissionAttribute).GetProperty("MachineName").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Messaging.MessageQueuePermissionAttribute.MachineName" />.
            /// </summary>
            public System.String MachineName
            {
                set
                {
                    base.SetArgument(MachineNameArgumentName, value);
                }
            }

            private static readonly string PathArgumentName = typeof(System.Messaging.MessageQueuePermissionAttribute).GetProperty("Path").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Messaging.MessageQueuePermissionAttribute.Path" />.
            /// </summary>
            public System.String Path
            {
                set
                {
                    base.SetArgument(PathArgumentName, value);
                }
            }

            private static readonly string PermissionAccessArgumentName = typeof(System.Messaging.MessageQueuePermissionAttribute).GetProperty("PermissionAccess").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Messaging.MessageQueuePermissionAttribute.PermissionAccess" />.
            /// </summary>
            public System.Messaging.MessageQueuePermissionAccess PermissionAccess
            {
                set
                {
                    base.SetArgument(PermissionAccessArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Net.Mail.SmtpPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Smtp : Permission
        {
            /// <summary>
            /// An instance of <see cref="Smtp"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Smtp Empty = new Smtp();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Net.Mail.SmtpPermissionAttribute);
                }
            }

            private static readonly string AccessArgumentName = typeof(System.Net.Mail.SmtpPermissionAttribute).GetProperty("Access").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.Mail.SmtpPermissionAttribute.Access" />.
            /// </summary>
            public System.String Access
            {
                set
                {
                    base.SetArgument(AccessArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Net.NetworkInformation.NetworkInformationPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class NetworkInformation : Permission
        {
            /// <summary>
            /// An instance of <see cref="NetworkInformation"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly NetworkInformation Empty = new NetworkInformation();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Net.NetworkInformation.NetworkInformationPermissionAttribute);
                }
            }

            private static readonly string AccessArgumentName = typeof(System.Net.NetworkInformation.NetworkInformationPermissionAttribute).GetProperty("Access").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.NetworkInformation.NetworkInformationPermissionAttribute.Access" />.
            /// </summary>
            public System.String Access
            {
                set
                {
                    base.SetArgument(AccessArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Net.SocketPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Socket : Permission
        {
            /// <summary>
            /// An instance of <see cref="Socket"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Socket Empty = new Socket();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Net.SocketPermissionAttribute);
                }
            }

            private static readonly string AccessArgumentName = typeof(System.Net.SocketPermissionAttribute).GetProperty("Access").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.SocketPermissionAttribute.Access" />.
            /// </summary>
            public System.String Access
            {
                set
                {
                    base.SetArgument(AccessArgumentName, value);
                }
            }

            private static readonly string HostArgumentName = typeof(System.Net.SocketPermissionAttribute).GetProperty("Host").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.SocketPermissionAttribute.Host" />.
            /// </summary>
            public System.String Host
            {
                set
                {
                    base.SetArgument(HostArgumentName, value);
                }
            }

            private static readonly string TransportArgumentName = typeof(System.Net.SocketPermissionAttribute).GetProperty("Transport").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.SocketPermissionAttribute.Transport" />.
            /// </summary>
            public System.String Transport
            {
                set
                {
                    base.SetArgument(TransportArgumentName, value);
                }
            }

            private static readonly string PortArgumentName = typeof(System.Net.SocketPermissionAttribute).GetProperty("Port").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.SocketPermissionAttribute.Port" />.
            /// </summary>
            public System.String Port
            {
                set
                {
                    base.SetArgument(PortArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Net.WebPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Web : Permission
        {
            /// <summary>
            /// An instance of <see cref="Web"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Web Empty = new Web();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Net.WebPermissionAttribute);
                }
            }

            private static readonly string ConnectArgumentName = typeof(System.Net.WebPermissionAttribute).GetProperty("Connect").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.WebPermissionAttribute.Connect" />.
            /// </summary>
            public System.String Connect
            {
                set
                {
                    base.SetArgument(ConnectArgumentName, value);
                }
            }

            private static readonly string AcceptArgumentName = typeof(System.Net.WebPermissionAttribute).GetProperty("Accept").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.WebPermissionAttribute.Accept" />.
            /// </summary>
            public System.String Accept
            {
                set
                {
                    base.SetArgument(AcceptArgumentName, value);
                }
            }

            private static readonly string ConnectPatternArgumentName = typeof(System.Net.WebPermissionAttribute).GetProperty("ConnectPattern").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.WebPermissionAttribute.ConnectPattern" />.
            /// </summary>
            public System.String ConnectPattern
            {
                set
                {
                    base.SetArgument(ConnectPatternArgumentName, value);
                }
            }

            private static readonly string AcceptPatternArgumentName = typeof(System.Net.WebPermissionAttribute).GetProperty("AcceptPattern").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Net.WebPermissionAttribute.AcceptPattern" />.
            /// </summary>
            public System.String AcceptPattern
            {
                set
                {
                    base.SetArgument(AcceptPatternArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.DataProtectionPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class DataProtection : Permission
        {
            /// <summary>
            /// An instance of <see cref="DataProtection"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly DataProtection Empty = new DataProtection();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.DataProtectionPermissionAttribute);
                }
            }

            private static readonly string FlagsArgumentName = typeof(System.Security.Permissions.DataProtectionPermissionAttribute).GetProperty("Flags").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.DataProtectionPermissionAttribute.Flags" />.
            /// </summary>
            public System.Security.Permissions.DataProtectionPermissionFlags Flags
            {
                set
                {
                    base.SetArgument(FlagsArgumentName, value);
                }
            }

            private static readonly string ProtectDataArgumentName = typeof(System.Security.Permissions.DataProtectionPermissionAttribute).GetProperty("ProtectData").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.DataProtectionPermissionAttribute.ProtectData" />.
            /// </summary>
            public System.Boolean ProtectData
            {
                set
                {
                    base.SetArgument(ProtectDataArgumentName, value);
                }
            }

            private static readonly string UnprotectDataArgumentName = typeof(System.Security.Permissions.DataProtectionPermissionAttribute).GetProperty("UnprotectData").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.DataProtectionPermissionAttribute.UnprotectData" />.
            /// </summary>
            public System.Boolean UnprotectData
            {
                set
                {
                    base.SetArgument(UnprotectDataArgumentName, value);
                }
            }

            private static readonly string ProtectMemoryArgumentName = typeof(System.Security.Permissions.DataProtectionPermissionAttribute).GetProperty("ProtectMemory").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.DataProtectionPermissionAttribute.ProtectMemory" />.
            /// </summary>
            public System.Boolean ProtectMemory
            {
                set
                {
                    base.SetArgument(ProtectMemoryArgumentName, value);
                }
            }

            private static readonly string UnprotectMemoryArgumentName = typeof(System.Security.Permissions.DataProtectionPermissionAttribute).GetProperty("UnprotectMemory").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.DataProtectionPermissionAttribute.UnprotectMemory" />.
            /// </summary>
            public System.Boolean UnprotectMemory
            {
                set
                {
                    base.SetArgument(UnprotectMemoryArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.EnvironmentPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Environment : Permission
        {
            /// <summary>
            /// An instance of <see cref="Environment"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Environment Empty = new Environment();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.EnvironmentPermissionAttribute);
                }
            }

            private static readonly string ReadArgumentName = typeof(System.Security.Permissions.EnvironmentPermissionAttribute).GetProperty("Read").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.EnvironmentPermissionAttribute.Read" />.
            /// </summary>
            public System.String Read
            {
                set
                {
                    base.SetArgument(ReadArgumentName, value);
                }
            }

            private static readonly string WriteArgumentName = typeof(System.Security.Permissions.EnvironmentPermissionAttribute).GetProperty("Write").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.EnvironmentPermissionAttribute.Write" />.
            /// </summary>
            public System.String Write
            {
                set
                {
                    base.SetArgument(WriteArgumentName, value);
                }
            }

            private static readonly string AllArgumentName = typeof(System.Security.Permissions.EnvironmentPermissionAttribute).GetProperty("All").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.EnvironmentPermissionAttribute.All" />.
            /// </summary>
            public System.String All
            {
                set
                {
                    base.SetArgument(AllArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.FileDialogPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class FileDialog : Permission
        {
            /// <summary>
            /// An instance of <see cref="FileDialog"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly FileDialog Empty = new FileDialog();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.FileDialogPermissionAttribute);
                }
            }

            private static readonly string OpenArgumentName = typeof(System.Security.Permissions.FileDialogPermissionAttribute).GetProperty("Open").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileDialogPermissionAttribute.Open" />.
            /// </summary>
            public System.Boolean Open
            {
                set
                {
                    base.SetArgument(OpenArgumentName, value);
                }
            }

            private static readonly string SaveArgumentName = typeof(System.Security.Permissions.FileDialogPermissionAttribute).GetProperty("Save").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileDialogPermissionAttribute.Save" />.
            /// </summary>
            public System.Boolean Save
            {
                set
                {
                    base.SetArgument(SaveArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.FileIOPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class FileIO : Permission
        {
            /// <summary>
            /// An instance of <see cref="FileIO"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly FileIO Empty = new FileIO();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.FileIOPermissionAttribute);
                }
            }

            private static readonly string ReadArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("Read").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.Read" />.
            /// </summary>
            public System.String Read
            {
                set
                {
                    base.SetArgument(ReadArgumentName, value);
                }
            }

            private static readonly string WriteArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("Write").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.Write" />.
            /// </summary>
            public System.String Write
            {
                set
                {
                    base.SetArgument(WriteArgumentName, value);
                }
            }

            private static readonly string AppendArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("Append").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.Append" />.
            /// </summary>
            public System.String Append
            {
                set
                {
                    base.SetArgument(AppendArgumentName, value);
                }
            }

            private static readonly string PathDiscoveryArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("PathDiscovery").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.PathDiscovery" />.
            /// </summary>
            public System.String PathDiscovery
            {
                set
                {
                    base.SetArgument(PathDiscoveryArgumentName, value);
                }
            }

            private static readonly string ViewAccessControlArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("ViewAccessControl").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.ViewAccessControl" />.
            /// </summary>
            public System.String ViewAccessControl
            {
                set
                {
                    base.SetArgument(ViewAccessControlArgumentName, value);
                }
            }

            private static readonly string ChangeAccessControlArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("ChangeAccessControl").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.ChangeAccessControl" />.
            /// </summary>
            public System.String ChangeAccessControl
            {
                set
                {
                    base.SetArgument(ChangeAccessControlArgumentName, value);
                }
            }

            private static readonly string AllArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("All").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.All" />.
            /// </summary>
            public System.String All
            {
                set
                {
                    base.SetArgument(AllArgumentName, value);
                }
            }

            private static readonly string ViewAndModifyArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("ViewAndModify").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.ViewAndModify" />.
            /// </summary>
            public System.String ViewAndModify
            {
                set
                {
                    base.SetArgument(ViewAndModifyArgumentName, value);
                }
            }

            private static readonly string AllFilesArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("AllFiles").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.AllFiles" />.
            /// </summary>
            public System.Security.Permissions.FileIOPermissionAccess AllFiles
            {
                set
                {
                    base.SetArgument(AllFilesArgumentName, value);
                }
            }

            private static readonly string AllLocalFilesArgumentName = typeof(System.Security.Permissions.FileIOPermissionAttribute).GetProperty("AllLocalFiles").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.FileIOPermissionAttribute.AllLocalFiles" />.
            /// </summary>
            public System.Security.Permissions.FileIOPermissionAccess AllLocalFiles
            {
                set
                {
                    base.SetArgument(AllLocalFilesArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.HostProtectionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class HostProtection : Permission
        {
            /// <summary>
            /// An instance of <see cref="HostProtection"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly HostProtection Empty = new HostProtection();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.HostProtectionAttribute);
                }
            }

            private static readonly string ResourcesArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("Resources").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.Resources" />.
            /// </summary>
            public System.Security.Permissions.HostProtectionResource Resources
            {
                set
                {
                    base.SetArgument(ResourcesArgumentName, value);
                }
            }

            private static readonly string SynchronizationArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("Synchronization").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.Synchronization" />.
            /// </summary>
            public System.Boolean Synchronization
            {
                set
                {
                    base.SetArgument(SynchronizationArgumentName, value);
                }
            }

            private static readonly string SharedStateArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("SharedState").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.SharedState" />.
            /// </summary>
            public System.Boolean SharedState
            {
                set
                {
                    base.SetArgument(SharedStateArgumentName, value);
                }
            }

            private static readonly string ExternalProcessMgmtArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("ExternalProcessMgmt").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.ExternalProcessMgmt" />.
            /// </summary>
            public System.Boolean ExternalProcessMgmt
            {
                set
                {
                    base.SetArgument(ExternalProcessMgmtArgumentName, value);
                }
            }

            private static readonly string SelfAffectingProcessMgmtArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("SelfAffectingProcessMgmt").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.SelfAffectingProcessMgmt" />.
            /// </summary>
            public System.Boolean SelfAffectingProcessMgmt
            {
                set
                {
                    base.SetArgument(SelfAffectingProcessMgmtArgumentName, value);
                }
            }

            private static readonly string ExternalThreadingArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("ExternalThreading").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.ExternalThreading" />.
            /// </summary>
            public System.Boolean ExternalThreading
            {
                set
                {
                    base.SetArgument(ExternalThreadingArgumentName, value);
                }
            }

            private static readonly string SelfAffectingThreadingArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("SelfAffectingThreading").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.SelfAffectingThreading" />.
            /// </summary>
            public System.Boolean SelfAffectingThreading
            {
                set
                {
                    base.SetArgument(SelfAffectingThreadingArgumentName, value);
                }
            }

            private static readonly string SecurityInfrastructureArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("SecurityInfrastructure").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.SecurityInfrastructure" />.
            /// </summary>
            public System.Boolean SecurityInfrastructure
            {
                set
                {
                    base.SetArgument(SecurityInfrastructureArgumentName, value);
                }
            }

            private static readonly string UIArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("UI").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.UI" />.
            /// </summary>
            public System.Boolean UI
            {
                set
                {
                    base.SetArgument(UIArgumentName, value);
                }
            }

            private static readonly string MayLeakOnAbortArgumentName = typeof(System.Security.Permissions.HostProtectionAttribute).GetProperty("MayLeakOnAbort").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.HostProtectionAttribute.MayLeakOnAbort" />.
            /// </summary>
            public System.Boolean MayLeakOnAbort
            {
                set
                {
                    base.SetArgument(MayLeakOnAbortArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.IsolatedStoragePermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class IsolatedStorage : Permission
        {
            /// <summary>
            /// An instance of <see cref="IsolatedStorage"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly IsolatedStorage Empty = new IsolatedStorage();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.IsolatedStoragePermissionAttribute);
                }
            }

            private static readonly string UserQuotaArgumentName = typeof(System.Security.Permissions.IsolatedStoragePermissionAttribute).GetProperty("UserQuota").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.IsolatedStoragePermissionAttribute.UserQuota" />.
            /// </summary>
            public System.Int64 UserQuota
            {
                set
                {
                    base.SetArgument(UserQuotaArgumentName, value);
                }
            }

            private static readonly string UsageAllowedArgumentName = typeof(System.Security.Permissions.IsolatedStoragePermissionAttribute).GetProperty("UsageAllowed").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.IsolatedStoragePermissionAttribute.UsageAllowed" />.
            /// </summary>
            public System.Security.Permissions.IsolatedStorageContainment UsageAllowed
            {
                set
                {
                    base.SetArgument(UsageAllowedArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.KeyContainerPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class KeyContainer : Permission
        {
            /// <summary>
            /// An instance of <see cref="KeyContainer"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly KeyContainer Empty = new KeyContainer();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.KeyContainerPermissionAttribute);
                }
            }

            private static readonly string KeyStoreArgumentName = typeof(System.Security.Permissions.KeyContainerPermissionAttribute).GetProperty("KeyStore").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.KeyContainerPermissionAttribute.KeyStore" />.
            /// </summary>
            public System.String KeyStore
            {
                set
                {
                    base.SetArgument(KeyStoreArgumentName, value);
                }
            }

            private static readonly string ProviderNameArgumentName = typeof(System.Security.Permissions.KeyContainerPermissionAttribute).GetProperty("ProviderName").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.KeyContainerPermissionAttribute.ProviderName" />.
            /// </summary>
            public System.String ProviderName
            {
                set
                {
                    base.SetArgument(ProviderNameArgumentName, value);
                }
            }

            private static readonly string ProviderTypeArgumentName = typeof(System.Security.Permissions.KeyContainerPermissionAttribute).GetProperty("ProviderType").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.KeyContainerPermissionAttribute.ProviderType" />.
            /// </summary>
            public System.Int32 ProviderType
            {
                set
                {
                    base.SetArgument(ProviderTypeArgumentName, value);
                }
            }

            private static readonly string KeyContainerNameArgumentName = typeof(System.Security.Permissions.KeyContainerPermissionAttribute).GetProperty("KeyContainerName").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.KeyContainerPermissionAttribute.KeyContainerName" />.
            /// </summary>
            public System.String KeyContainerName
            {
                set
                {
                    base.SetArgument(KeyContainerNameArgumentName, value);
                }
            }

            private static readonly string KeySpecArgumentName = typeof(System.Security.Permissions.KeyContainerPermissionAttribute).GetProperty("KeySpec").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.KeyContainerPermissionAttribute.KeySpec" />.
            /// </summary>
            public System.Int32 KeySpec
            {
                set
                {
                    base.SetArgument(KeySpecArgumentName, value);
                }
            }

            private static readonly string FlagsArgumentName = typeof(System.Security.Permissions.KeyContainerPermissionAttribute).GetProperty("Flags").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.KeyContainerPermissionAttribute.Flags" />.
            /// </summary>
            public System.Security.Permissions.KeyContainerPermissionFlags Flags
            {
                set
                {
                    base.SetArgument(FlagsArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.PermissionSetAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class PermissionSet : Permission
        {
            /// <summary>
            /// An instance of <see cref="PermissionSet"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly PermissionSet Empty = new PermissionSet();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.PermissionSetAttribute);
                }
            }

            private static readonly string FileArgumentName = typeof(System.Security.Permissions.PermissionSetAttribute).GetProperty("File").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PermissionSetAttribute.File" />.
            /// </summary>
            public System.String File
            {
                set
                {
                    base.SetArgument(FileArgumentName, value);
                }
            }

            private static readonly string UnicodeEncodedArgumentName = typeof(System.Security.Permissions.PermissionSetAttribute).GetProperty("UnicodeEncoded").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PermissionSetAttribute.UnicodeEncoded" />.
            /// </summary>
            public System.Boolean UnicodeEncoded
            {
                set
                {
                    base.SetArgument(UnicodeEncodedArgumentName, value);
                }
            }

            private static readonly string NameArgumentName = typeof(System.Security.Permissions.PermissionSetAttribute).GetProperty("Name").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PermissionSetAttribute.Name" />.
            /// </summary>
            public System.String Name
            {
                set
                {
                    base.SetArgument(NameArgumentName, value);
                }
            }

            private static readonly string XMLArgumentName = typeof(System.Security.Permissions.PermissionSetAttribute).GetProperty("XML").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PermissionSetAttribute.XML" />.
            /// </summary>
            public System.String XML
            {
                set
                {
                    base.SetArgument(XMLArgumentName, value);
                }
            }

            private static readonly string HexArgumentName = typeof(System.Security.Permissions.PermissionSetAttribute).GetProperty("Hex").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PermissionSetAttribute.Hex" />.
            /// </summary>
            public System.String Hex
            {
                set
                {
                    base.SetArgument(HexArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.PrincipalPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Principal : Permission
        {
            /// <summary>
            /// An instance of <see cref="Principal"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Principal Empty = new Principal();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.PrincipalPermissionAttribute);
                }
            }

            private static readonly string NameArgumentName = typeof(System.Security.Permissions.PrincipalPermissionAttribute).GetProperty("Name").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PrincipalPermissionAttribute.Name" />.
            /// </summary>
            public System.String Name
            {
                set
                {
                    base.SetArgument(NameArgumentName, value);
                }
            }

            private static readonly string RoleArgumentName = typeof(System.Security.Permissions.PrincipalPermissionAttribute).GetProperty("Role").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PrincipalPermissionAttribute.Role" />.
            /// </summary>
            public System.String Role
            {
                set
                {
                    base.SetArgument(RoleArgumentName, value);
                }
            }

            private static readonly string AuthenticatedArgumentName = typeof(System.Security.Permissions.PrincipalPermissionAttribute).GetProperty("Authenticated").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PrincipalPermissionAttribute.Authenticated" />.
            /// </summary>
            public System.Boolean Authenticated
            {
                set
                {
                    base.SetArgument(AuthenticatedArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.PublisherIdentityPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class PublisherIdentity : Permission
        {
            /// <summary>
            /// An instance of <see cref="PublisherIdentity"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly PublisherIdentity Empty = new PublisherIdentity();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.PublisherIdentityPermissionAttribute);
                }
            }

            private static readonly string X509CertificateArgumentName = typeof(System.Security.Permissions.PublisherIdentityPermissionAttribute).GetProperty("X509Certificate").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PublisherIdentityPermissionAttribute.X509Certificate" />.
            /// </summary>
            public System.String X509Certificate
            {
                set
                {
                    base.SetArgument(X509CertificateArgumentName, value);
                }
            }

            private static readonly string CertFileArgumentName = typeof(System.Security.Permissions.PublisherIdentityPermissionAttribute).GetProperty("CertFile").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PublisherIdentityPermissionAttribute.CertFile" />.
            /// </summary>
            public System.String CertFile
            {
                set
                {
                    base.SetArgument(CertFileArgumentName, value);
                }
            }

            private static readonly string SignedFileArgumentName = typeof(System.Security.Permissions.PublisherIdentityPermissionAttribute).GetProperty("SignedFile").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.PublisherIdentityPermissionAttribute.SignedFile" />.
            /// </summary>
            public System.String SignedFile
            {
                set
                {
                    base.SetArgument(SignedFileArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.ReflectionPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Reflection : Permission
        {
            /// <summary>
            /// An instance of <see cref="Reflection"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Reflection Empty = new Reflection();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.ReflectionPermissionAttribute);
                }
            }

            private static readonly string FlagsArgumentName = typeof(System.Security.Permissions.ReflectionPermissionAttribute).GetProperty("Flags").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.ReflectionPermissionAttribute.Flags" />.
            /// </summary>
            public System.Security.Permissions.ReflectionPermissionFlag Flags
            {
                set
                {
                    base.SetArgument(FlagsArgumentName, value);
                }
            }

            private static readonly string TypeInformationArgumentName = typeof(System.Security.Permissions.ReflectionPermissionAttribute).GetProperty("TypeInformation").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.ReflectionPermissionAttribute.TypeInformation" />.
            /// </summary>
            public System.Boolean TypeInformation
            {
                set
                {
                    base.SetArgument(TypeInformationArgumentName, value);
                }
            }

            private static readonly string MemberAccessArgumentName = typeof(System.Security.Permissions.ReflectionPermissionAttribute).GetProperty("MemberAccess").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.ReflectionPermissionAttribute.MemberAccess" />.
            /// </summary>
            public System.Boolean MemberAccess
            {
                set
                {
                    base.SetArgument(MemberAccessArgumentName, value);
                }
            }

            private static readonly string ReflectionEmitArgumentName = typeof(System.Security.Permissions.ReflectionPermissionAttribute).GetProperty("ReflectionEmit").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.ReflectionPermissionAttribute.ReflectionEmit" />.
            /// </summary>
            public System.Boolean ReflectionEmit
            {
                set
                {
                    base.SetArgument(ReflectionEmitArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.RegistryPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Registry : Permission
        {
            /// <summary>
            /// An instance of <see cref="Registry"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Registry Empty = new Registry();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.RegistryPermissionAttribute);
                }
            }

            private static readonly string ReadArgumentName = typeof(System.Security.Permissions.RegistryPermissionAttribute).GetProperty("Read").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.RegistryPermissionAttribute.Read" />.
            /// </summary>
            public System.String Read
            {
                set
                {
                    base.SetArgument(ReadArgumentName, value);
                }
            }

            private static readonly string WriteArgumentName = typeof(System.Security.Permissions.RegistryPermissionAttribute).GetProperty("Write").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.RegistryPermissionAttribute.Write" />.
            /// </summary>
            public System.String Write
            {
                set
                {
                    base.SetArgument(WriteArgumentName, value);
                }
            }

            private static readonly string CreateArgumentName = typeof(System.Security.Permissions.RegistryPermissionAttribute).GetProperty("Create").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.RegistryPermissionAttribute.Create" />.
            /// </summary>
            public System.String Create
            {
                set
                {
                    base.SetArgument(CreateArgumentName, value);
                }
            }

            private static readonly string ViewAccessControlArgumentName = typeof(System.Security.Permissions.RegistryPermissionAttribute).GetProperty("ViewAccessControl").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.RegistryPermissionAttribute.ViewAccessControl" />.
            /// </summary>
            public System.String ViewAccessControl
            {
                set
                {
                    base.SetArgument(ViewAccessControlArgumentName, value);
                }
            }

            private static readonly string ChangeAccessControlArgumentName = typeof(System.Security.Permissions.RegistryPermissionAttribute).GetProperty("ChangeAccessControl").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.RegistryPermissionAttribute.ChangeAccessControl" />.
            /// </summary>
            public System.String ChangeAccessControl
            {
                set
                {
                    base.SetArgument(ChangeAccessControlArgumentName, value);
                }
            }

            private static readonly string ViewAndModifyArgumentName = typeof(System.Security.Permissions.RegistryPermissionAttribute).GetProperty("ViewAndModify").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.RegistryPermissionAttribute.ViewAndModify" />.
            /// </summary>
            public System.String ViewAndModify
            {
                set
                {
                    base.SetArgument(ViewAndModifyArgumentName, value);
                }
            }

            private static readonly string AllArgumentName = typeof(System.Security.Permissions.RegistryPermissionAttribute).GetProperty("All").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.RegistryPermissionAttribute.All" />.
            /// </summary>
            public System.String All
            {
                set
                {
                    base.SetArgument(AllArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.SecurityPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Security : Permission
        {
            /// <summary>
            /// An instance of <see cref="Security"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Security Empty = new Security();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.SecurityPermissionAttribute);
                }
            }

            private static readonly string FlagsArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("Flags").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.Flags" />.
            /// </summary>
            public System.Security.Permissions.SecurityPermissionFlag Flags
            {
                set
                {
                    base.SetArgument(FlagsArgumentName, value);
                }
            }

            private static readonly string AssertionArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("Assertion").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.Assertion" />.
            /// </summary>
            public System.Boolean Assertion
            {
                set
                {
                    base.SetArgument(AssertionArgumentName, value);
                }
            }

            private static readonly string UnmanagedCodeArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("UnmanagedCode").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.UnmanagedCode" />.
            /// </summary>
            public System.Boolean UnmanagedCode
            {
                set
                {
                    base.SetArgument(UnmanagedCodeArgumentName, value);
                }
            }

            private static readonly string SkipVerificationArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("SkipVerification").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.SkipVerification" />.
            /// </summary>
            public System.Boolean SkipVerification
            {
                set
                {
                    base.SetArgument(SkipVerificationArgumentName, value);
                }
            }

            private static readonly string ExecutionArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("Execution").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.Execution" />.
            /// </summary>
            public System.Boolean Execution
            {
                set
                {
                    base.SetArgument(ExecutionArgumentName, value);
                }
            }

            private static readonly string ControlThreadArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("ControlThread").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.ControlThread" />.
            /// </summary>
            public System.Boolean ControlThread
            {
                set
                {
                    base.SetArgument(ControlThreadArgumentName, value);
                }
            }

            private static readonly string ControlEvidenceArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("ControlEvidence").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.ControlEvidence" />.
            /// </summary>
            public System.Boolean ControlEvidence
            {
                set
                {
                    base.SetArgument(ControlEvidenceArgumentName, value);
                }
            }

            private static readonly string ControlPolicyArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("ControlPolicy").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.ControlPolicy" />.
            /// </summary>
            public System.Boolean ControlPolicy
            {
                set
                {
                    base.SetArgument(ControlPolicyArgumentName, value);
                }
            }

            private static readonly string SerializationFormatterArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("SerializationFormatter").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.SerializationFormatter" />.
            /// </summary>
            public System.Boolean SerializationFormatter
            {
                set
                {
                    base.SetArgument(SerializationFormatterArgumentName, value);
                }
            }

            private static readonly string ControlDomainPolicyArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("ControlDomainPolicy").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.ControlDomainPolicy" />.
            /// </summary>
            public System.Boolean ControlDomainPolicy
            {
                set
                {
                    base.SetArgument(ControlDomainPolicyArgumentName, value);
                }
            }

            private static readonly string ControlPrincipalArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("ControlPrincipal").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.ControlPrincipal" />.
            /// </summary>
            public System.Boolean ControlPrincipal
            {
                set
                {
                    base.SetArgument(ControlPrincipalArgumentName, value);
                }
            }

            private static readonly string ControlAppDomainArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("ControlAppDomain").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.ControlAppDomain" />.
            /// </summary>
            public System.Boolean ControlAppDomain
            {
                set
                {
                    base.SetArgument(ControlAppDomainArgumentName, value);
                }
            }

            private static readonly string RemotingConfigurationArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("RemotingConfiguration").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.RemotingConfiguration" />.
            /// </summary>
            public System.Boolean RemotingConfiguration
            {
                set
                {
                    base.SetArgument(RemotingConfigurationArgumentName, value);
                }
            }

            private static readonly string InfrastructureArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("Infrastructure").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.Infrastructure" />.
            /// </summary>
            public System.Boolean Infrastructure
            {
                set
                {
                    base.SetArgument(InfrastructureArgumentName, value);
                }
            }

            private static readonly string BindingRedirectsArgumentName = typeof(System.Security.Permissions.SecurityPermissionAttribute).GetProperty("BindingRedirects").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SecurityPermissionAttribute.BindingRedirects" />.
            /// </summary>
            public System.Boolean BindingRedirects
            {
                set
                {
                    base.SetArgument(BindingRedirectsArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.SiteIdentityPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class SiteIdentity : Permission
        {
            /// <summary>
            /// An instance of <see cref="SiteIdentity"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly SiteIdentity Empty = new SiteIdentity();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.SiteIdentityPermissionAttribute);
                }
            }

            private static readonly string SiteArgumentName = typeof(System.Security.Permissions.SiteIdentityPermissionAttribute).GetProperty("Site").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.SiteIdentityPermissionAttribute.Site" />.
            /// </summary>
            public System.String Site
            {
                set
                {
                    base.SetArgument(SiteArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.StorePermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class Store : Permission
        {
            /// <summary>
            /// An instance of <see cref="Store"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly Store Empty = new Store();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.StorePermissionAttribute);
                }
            }

            private static readonly string FlagsArgumentName = typeof(System.Security.Permissions.StorePermissionAttribute).GetProperty("Flags").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StorePermissionAttribute.Flags" />.
            /// </summary>
            public System.Security.Permissions.StorePermissionFlags Flags
            {
                set
                {
                    base.SetArgument(FlagsArgumentName, value);
                }
            }

            private static readonly string CreateStoreArgumentName = typeof(System.Security.Permissions.StorePermissionAttribute).GetProperty("CreateStore").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StorePermissionAttribute.CreateStore" />.
            /// </summary>
            public System.Boolean CreateStore
            {
                set
                {
                    base.SetArgument(CreateStoreArgumentName, value);
                }
            }

            private static readonly string DeleteStoreArgumentName = typeof(System.Security.Permissions.StorePermissionAttribute).GetProperty("DeleteStore").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StorePermissionAttribute.DeleteStore" />.
            /// </summary>
            public System.Boolean DeleteStore
            {
                set
                {
                    base.SetArgument(DeleteStoreArgumentName, value);
                }
            }

            private static readonly string EnumerateStoresArgumentName = typeof(System.Security.Permissions.StorePermissionAttribute).GetProperty("EnumerateStores").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StorePermissionAttribute.EnumerateStores" />.
            /// </summary>
            public System.Boolean EnumerateStores
            {
                set
                {
                    base.SetArgument(EnumerateStoresArgumentName, value);
                }
            }

            private static readonly string OpenStoreArgumentName = typeof(System.Security.Permissions.StorePermissionAttribute).GetProperty("OpenStore").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StorePermissionAttribute.OpenStore" />.
            /// </summary>
            public System.Boolean OpenStore
            {
                set
                {
                    base.SetArgument(OpenStoreArgumentName, value);
                }
            }

            private static readonly string AddToStoreArgumentName = typeof(System.Security.Permissions.StorePermissionAttribute).GetProperty("AddToStore").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StorePermissionAttribute.AddToStore" />.
            /// </summary>
            public System.Boolean AddToStore
            {
                set
                {
                    base.SetArgument(AddToStoreArgumentName, value);
                }
            }

            private static readonly string RemoveFromStoreArgumentName = typeof(System.Security.Permissions.StorePermissionAttribute).GetProperty("RemoveFromStore").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StorePermissionAttribute.RemoveFromStore" />.
            /// </summary>
            public System.Boolean RemoveFromStore
            {
                set
                {
                    base.SetArgument(RemoveFromStoreArgumentName, value);
                }
            }

            private static readonly string EnumerateCertificatesArgumentName = typeof(System.Security.Permissions.StorePermissionAttribute).GetProperty("EnumerateCertificates").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StorePermissionAttribute.EnumerateCertificates" />.
            /// </summary>
            public System.Boolean EnumerateCertificates
            {
                set
                {
                    base.SetArgument(EnumerateCertificatesArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.StrongNameIdentityPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class StrongNameIdentity : Permission
        {
            /// <summary>
            /// An instance of <see cref="StrongNameIdentity"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly StrongNameIdentity Empty = new StrongNameIdentity();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.StrongNameIdentityPermissionAttribute);
                }
            }

            private static readonly string NameArgumentName = typeof(System.Security.Permissions.StrongNameIdentityPermissionAttribute).GetProperty("Name").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StrongNameIdentityPermissionAttribute.Name" />.
            /// </summary>
            public System.String Name
            {
                set
                {
                    base.SetArgument(NameArgumentName, value);
                }
            }

            private static readonly string VersionArgumentName = typeof(System.Security.Permissions.StrongNameIdentityPermissionAttribute).GetProperty("Version").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StrongNameIdentityPermissionAttribute.Version" />.
            /// </summary>
            public System.String Version
            {
                set
                {
                    base.SetArgument(VersionArgumentName, value);
                }
            }

            private static readonly string PublicKeyArgumentName = typeof(System.Security.Permissions.StrongNameIdentityPermissionAttribute).GetProperty("PublicKey").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.StrongNameIdentityPermissionAttribute.PublicKey" />.
            /// </summary>
            public System.String PublicKey
            {
                set
                {
                    base.SetArgument(PublicKeyArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.UIPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class UI : Permission
        {
            /// <summary>
            /// An instance of <see cref="UI"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly UI Empty = new UI();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.UIPermissionAttribute);
                }
            }

            private static readonly string WindowArgumentName = typeof(System.Security.Permissions.UIPermissionAttribute).GetProperty("Window").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.UIPermissionAttribute.Window" />.
            /// </summary>
            public System.Security.Permissions.UIPermissionWindow Window
            {
                set
                {
                    base.SetArgument(WindowArgumentName, value);
                }
            }

            private static readonly string ClipboardArgumentName = typeof(System.Security.Permissions.UIPermissionAttribute).GetProperty("Clipboard").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.UIPermissionAttribute.Clipboard" />.
            /// </summary>
            public System.Security.Permissions.UIPermissionClipboard Clipboard
            {
                set
                {
                    base.SetArgument(ClipboardArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.UrlIdentityPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class UrlIdentity : Permission
        {
            /// <summary>
            /// An instance of <see cref="UrlIdentity"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly UrlIdentity Empty = new UrlIdentity();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.UrlIdentityPermissionAttribute);
                }
            }

            private static readonly string UrlArgumentName = typeof(System.Security.Permissions.UrlIdentityPermissionAttribute).GetProperty("Url").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.UrlIdentityPermissionAttribute.Url" />.
            /// </summary>
            public System.String Url
            {
                set
                {
                    base.SetArgument(UrlArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Security.Permissions.ZoneIdentityPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class ZoneIdentity : Permission
        {
            /// <summary>
            /// An instance of <see cref="ZoneIdentity"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly ZoneIdentity Empty = new ZoneIdentity();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Security.Permissions.ZoneIdentityPermissionAttribute);
                }
            }

            private static readonly string ZoneArgumentName = typeof(System.Security.Permissions.ZoneIdentityPermissionAttribute).GetProperty("Zone").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Security.Permissions.ZoneIdentityPermissionAttribute.Zone" />.
            /// </summary>
            public System.Security.SecurityZone Zone
            {
                set
                {
                    base.SetArgument(ZoneArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.ServiceProcess.ServiceControllerPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class ServiceController : Permission
        {
            /// <summary>
            /// An instance of <see cref="ServiceController"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly ServiceController Empty = new ServiceController();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.ServiceProcess.ServiceControllerPermissionAttribute);
                }
            }

            private static readonly string MachineNameArgumentName = typeof(System.ServiceProcess.ServiceControllerPermissionAttribute).GetProperty("MachineName").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.ServiceProcess.ServiceControllerPermissionAttribute.MachineName" />.
            /// </summary>
            public System.String MachineName
            {
                set
                {
                    base.SetArgument(MachineNameArgumentName, value);
                }
            }

            private static readonly string PermissionAccessArgumentName = typeof(System.ServiceProcess.ServiceControllerPermissionAttribute).GetProperty("PermissionAccess").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.ServiceProcess.ServiceControllerPermissionAttribute.PermissionAccess" />.
            /// </summary>
            public System.ServiceProcess.ServiceControllerPermissionAccess PermissionAccess
            {
                set
                {
                    base.SetArgument(PermissionAccessArgumentName, value);
                }
            }

            private static readonly string ServiceNameArgumentName = typeof(System.ServiceProcess.ServiceControllerPermissionAttribute).GetProperty("ServiceName").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.ServiceProcess.ServiceControllerPermissionAttribute.ServiceName" />.
            /// </summary>
            public System.String ServiceName
            {
                set
                {
                    base.SetArgument(ServiceNameArgumentName, value);
                }
            }
        }

        /// <summary>
        /// Builds a <see cref="System.Web.AspNetHostingPermissionAttribute"/> attribute.
        /// </summary>
        [Serializable, CLSCompliant(true)]
        public sealed class AspNetHosting : Permission
        {
            /// <summary>
            /// An instance of <see cref="AspNetHosting"/> representing the default values.
            /// See <see cref="Permission"/> for an explanation about the defaults.
            /// </summary>
            public static readonly AspNetHosting Empty = new AspNetHosting();

            /// <summary>
            /// Returns the attribute type associated with this permission.
            /// </summary>
            protected internal override Type Type
            {
                get
                {
                    return typeof(System.Web.AspNetHostingPermissionAttribute);
                }
            }

            private static readonly string LevelArgumentName = typeof(System.Web.AspNetHostingPermissionAttribute).GetProperty("Level").Name;

            /// <summary>
            /// Sets the value to be placed in <see cref="System.Web.AspNetHostingPermissionAttribute.Level" />.
            /// </summary>
            public System.Web.AspNetHostingPermissionLevel Level
            {
                set
                {
                    base.SetArgument(LevelArgumentName, value);
                }
            }
        }
        #endregion
    }
}