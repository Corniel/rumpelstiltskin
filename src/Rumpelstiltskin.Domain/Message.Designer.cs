﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rumpelstiltskin.Domain {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Message {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Message() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Rumpelstiltskin.Domain.Message", typeof(Message).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to The name &apos;{0}&apos; is not in the current selection..
        /// </summary>
        internal static string CandidateNotFound {
            get {
                return ResourceManager.GetString("CandidateNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The names should be different..
        /// </summary>
        internal static string DifferentNames {
            get {
                return ResourceManager.GetString("DifferentNames", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Some names occurred multiple times: {0}..
        /// </summary>
        internal static string DoubleNames {
            get {
                return ResourceManager.GetString("DoubleNames", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There should be at least {0} names to choose from..
        /// </summary>
        internal static string MinimumSelectionSize {
            get {
                return ResourceManager.GetString("MinimumSelectionSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No (new) names have been added..
        /// </summary>
        internal static string NoNewNames {
            get {
                return ResourceManager.GetString("NoNewNames", resourceCulture);
            }
        }
    }
}