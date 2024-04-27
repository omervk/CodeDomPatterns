using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// This library is hosted on CodePlex at http://www.codeplex.com/Wiki/View.aspx?ProjectName=CodeDomPatterns.

// This work is licensed under a Creative Commons Attribution 2.5 License.
// For more information: http://creativecommons.org/licenses/by/2.5/

// The following issues prevent this library from being complete. Please vote on them:
// 1. http://lab.msdn.microsoft.com/ProductFeedback/viewFeedback.aspx?feedbackId=FDBK48431
// 2. http://lab.msdn.microsoft.com/ProductFeedback/viewFeedback.aspx?feedbackId=FDBK39456

[assembly: AssemblyTitle("DotNetZen.CodeDom.Patterns")]
[assembly: AssemblyDescription("CodeDom Patterns")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Omer van Kloeten")]
[assembly: AssemblyProduct("DotNetZen.CodeDom.Patterns")]
[assembly: AssemblyCopyright("2006, Omer van Kloeten. Some rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.8.0.0")]
[assembly: AssemblyFileVersion("1.8.0.0")]

[assembly: CLSCompliant(true)]

//	Changelog:
//	----------
//
//	# 2006-10-31 - Version 1.8.
//
//      1. The Code Access Security Decorator Patterns have been added.
//      2. The Assembly Information Pattern has been added.
//		3. Security demand added to GetObjectData in the Serializable Type Pattern.
//      4. Xml Comment Patterns moved to .Xml namespace and had their CodeXmlComment prefix removed (too long).
//      5. Binaries now target .NET 2.0 instead of .NET 1.1, but they are mostly still backwards compatible.
//
//	# 2006-09-23 - Version 1.7.
//
//		1. The Nullable Value Type Property Pattern has been added.
//		2. The Enum.IsDefined and String.IsNullOrEmpty assertions have been added.
//		3. The Serializable Type Pattern has been added.
//		4. The Disposable Type Pattern is now a part of CodePatternTypeDeclaration.
//
//	# 2006-04-29 - Version 1.6.
//
//		1. The Asynchronous Operation Pattern has been added.
//		2. The Disposable Type Pattern has been added.
//		3. The Xml Comment Patterns have been added.
//		4. Automatic documentation of the Begin/End Process, Custom Attribute, Custom Exception, Delegate, Event, Observer, Singleton and Typed Collection patterns.
//		5. The Unary Operators IsNull and NotNull have been added.
//		6. You can now access each flag's CodeMemberField in the Flags Pattern.
//		7. The Singleton Pattern (Lazy Load) no longer publicly exposes the internally used class InstanceContainer.
//		8. The Typed Collection Pattern's ToArray method has been fixed.
//		9. The work is now licensed under the Creative Commons Attribution 2.5 License.
//		   You can copy, freely distribute, derive and even use the code in a commercial product, but you must attribute it to the author.
//
//	# 2006-03-31 - Version 1.5.
//
//		1. The Typed Collection Pattern has been added.
//		2. The Argument Assertion Patterns have been added.
//		3. Assembly and all types are now CLSCompliant.
//		4. All types are now marked as Serializable.
//		5. The Custom Attribute Pattern now produces sealed attributes, to increase effeciency of generated code.
//		6. Several overload additions and bug fixes.
//
//	# 2006-02-10 - Version 1.4.
//
//		1. Compatible with generation for Visual Basic.
//		2. Custom Exception Pattern altered to take CodeParameterDeclarationExpressions instead of CodePatternGetFields.
//		3. The Event Pattern now has an overload that takes a delegate type (and deduces the parameters and return type).
//		4. The For Each Pattern now works according to the C# specification.
//		5. Several bug fixes.
//
//	# 2006-01-28 - Version 1.3.
//
//		1. Assembly renamed to DotNetZen.CodeDom.Patterns.
//		2. The Custom Attribute Pattern has been added.
//		3. The Custom Exception Pattern has been added.
//
//	# 2006-01-13 - Version 1.2.
//
//		1. The Binary Operator Pattern has been added.
//		2. The Unary Operator Pattern has been added.
//		3. The Compound Assignment Pattern has been added.
//
//	# 2005-11-11 - Version 1.1.
//
//		1. The Cursor Lock Pattern now changes the cursor back to its original icon, rather than Cursors.Default.
//		2. The For Each Pattern has been added.
//		3. The Is Instance Of Pattern has been added.
//		4. Boolean flags for scope are now implemented using the Scope enumeration.
//		5. Boolean flags for resource type on The Using Pattern are now implemented using the ResourceType enumeration.
//		6. Boolean flags for load type on The Singleton Pattern are now implemented using the LoadType enumeration.
//
//	# 2005-10-30 - Initial release.
