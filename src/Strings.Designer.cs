﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lenoard.Core {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
#if NetCore
                    var assembly = typeof(Strings).GetTypeInfo().Assembly;
#else
                    var assembly = typeof(Strings).Assembly;
#endif
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Lenoard.Core.Strings", assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 Comparand is not of the correct type. 的本地化字符串。
        /// </summary>
        internal static string BadComparandType {
            get {
                return ResourceManager.GetString("BadComparandType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Cannot find the Parse method in the type &quot;{0}&quot; to convert string value to {0}. Please use the Parse(string input, Func&lt;string, T&gt; boundaryParser) method instead. 的本地化字符串。
        /// </summary>
        internal static string CannotFindParseMethod {
            get {
                return ResourceManager.GetString("CannotFindParseMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Cannot find the TryParse method in the type &quot;{0}&quot; to try to convert string value to {0}. Please use the TryParse(string input, BoundaryParser&lt;T&gt; boundaryParser, out Range&lt;T&gt; result) method instead. 的本地化字符串。
        /// </summary>
        internal static string CannotFindTryParse {
            get {
                return ResourceManager.GetString("CannotFindTryParse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The value cannot be null or empty. 的本地化字符串。
        /// </summary>
        internal static string CannotNullOrEmpty {
            get {
                return ResourceManager.GetString("CannotNullOrEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The provided version string is invalid. 的本地化字符串。
        /// </summary>
        internal static string InvalidVersion {
            get {
                return ResourceManager.GetString("InvalidVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 The provided version comparator string is invalid. 的本地化字符串。
        /// </summary>
        internal static string InvalidVersionComparator {
            get {
                return ResourceManager.GetString("InvalidVersionComparator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Type &quot;{0}&quot; does not implement IComparable&lt;{0}&gt; or IComparable. 的本地化字符串。
        /// </summary>
        internal static string UncomparableType {
            get {
                return ResourceManager.GetString("UncomparableType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Unrecognized range format. 的本地化字符串。
        /// </summary>
        internal static string UnrecognizedRange {
            get {
                return ResourceManager.GetString("UnrecognizedRange", resourceCulture);
            }
        }
    }
}
