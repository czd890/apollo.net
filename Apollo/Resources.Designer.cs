
namespace Com.Ctrip.Framework.Apollo
{
    using System;
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources()
        {
        }

        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }


        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }


        internal static string Error_InvalidFilePath
        {
            get
            {
                return ResourceManager.GetString("Error_InvalidFilePath", resourceCulture);
            }
        }

        internal static string Error_KeyIsDuplicated
        {
            get
            {
                return ResourceManager.GetString("Error_KeyIsDuplicated", resourceCulture);
            }
        }

        internal static string Error_NamespaceIsNotSupported
        {
            get
            {
                return ResourceManager.GetString("Error_NamespaceIsNotSupported", resourceCulture);
            }
        }

        internal static string Error_UnsupportedNodeType
        {
            get
            {
                return ResourceManager.GetString("Error_UnsupportedNodeType", resourceCulture);
            }
        }

        internal static string Msg_LineInfo
        {
            get
            {
                return ResourceManager.GetString("Msg_LineInfo", resourceCulture);
            }
        }
    }
}
