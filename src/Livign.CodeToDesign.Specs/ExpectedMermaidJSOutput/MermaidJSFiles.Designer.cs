﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Livign.CodeToDesign.Specs.ExpectedMermaidJSOutput {
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
    internal class MermaidJSFiles {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MermaidJSFiles() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Livign.CodeToDesign.Specs.ExpectedMermaidJSOutput.MermaidJSFiles", typeof(MermaidJSFiles).Assembly);
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
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_CallToDotnetSdkClass {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.CallToDotnetSdkClass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///    Actor1-&gt;&gt;Actor2: CalActor1Twice
        ///    activate Actor2
        ///    Actor2-&gt;&gt;Actor1: EmptyCall
        ///    Actor2-&gt;&gt;Actor1: EmptyCall
        ///    deactivate Actor2
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_MethodThatActivatesActor2_1 {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.MethodThatActivatesActor2_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///    Actor1-&gt;&gt;Actor2: MethodThatRecursIn2Steps_2
        ///    activate Actor2
        ///    Actor2-&gt;&gt;Actor1: MethodThatRecursIn2Steps_1
        ///    Actor2-&gt;&gt;Actor1: EmptyCall
        ///    deactivate Actor2
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_MethodThatRecursIn2Steps_1 {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.MethodThatRecursIn2Steps_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///    Actor1-&gt;&gt;Actor2: MethodThatRecursIn3Steps_2
        ///    activate Actor2
        ///    Actor2-&gt;&gt;Actor1: MethodThatRecursIn3Steps_3
        ///    deactivate Actor2
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_MethodThatRecursIn3Steps_1 {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.MethodThatRecursIn3Steps_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_OneCallToAClassFromAssemblyWithoutSymbols_NotWhiteListed {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.OneCallToAClassFromAssemblyWithoutSymbols_Not" +
                        "WhiteListed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///    Actor1-&gt;&gt;JsonConvert: DeserializeObject
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_OneCallToAClassFromAssemblyWithoutSymbols_WhiteListed {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.OneCallToAClassFromAssemblyWithoutSymbols_Whi" +
                        "teListed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///    Actor1-&gt;&gt;Actor2: EmptyMethod1
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_OneCallToOtherActorViaPrivateMethod {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.OneCallToOtherActorViaPrivateMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///    Actor1-&gt;&gt;Actor2: MethodWithReturnValue
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_OneCallToOtherActorWithAssignment {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.OneCallToOtherActorWithAssignment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///    Actor1-&gt;&gt;Actor2: StaticMethod
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_OneStaticCallToOtherActor {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.OneStaticCallToOtherActor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_SelfCallingRecursiveMethod {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.SelfCallingRecursiveMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sequenceDiagram
        ///    activate Actor1
        ///    Actor1-&gt;&gt;Actor2: EmptyMethod1
        ///    Actor1-&gt;&gt;Actor2: EmptyMethod2
        ///    deactivate Actor1.
        /// </summary>
        internal static string SequenceDiagram_TestProject1_Actor1_TwoDifferentCallsToOtherActor {
            get {
                return ResourceManager.GetString("SequenceDiagram.TestProject1.Actor1.TwoDifferentCallsToOtherActor", resourceCulture);
            }
        }
    }
}
