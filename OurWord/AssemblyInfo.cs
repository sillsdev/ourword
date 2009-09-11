/**********************************************************************************************
 * Project: OurWord!
 * File:    AssemblyInfo.cs
 * Author:  John Wimbish
 * Created: 01 Dec 2003
 * Purpose: Information about the assembly.
 * Legal:   Copyright (c) 2004-09, John S. Wimbish. All Rights Reserved.  
 *********************************************************************************************/

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("Our Word!")]
[assembly: AssemblyDescription("Translation Environment for Collaborative MTT projects.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("John S. Wimbish")]
[assembly: AssemblyProduct("OurWord!")]
[assembly: AssemblyCopyright("� 2004-09, John S. Wimbish")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information consists of the following four values:
//    Major Version - E.g., major functionality has been added
//    Minor Version - A new feature here or there
//    Build Number  - generally just bug fixes
//    Revision      - not used.
//

// Huichol throwaway versions
// 1.1.0 - initial
// 1.1.1 - fixed Load problems with Absolute Paths
// 1.3.0 - What I'm leaving at Tomohon 4 June 2009

[assembly: AssemblyVersion("1.3.5")]

//
// In order to sign your assembly you must specify a key to use. Refer to the 
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing. 
//
// Notes: 
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the 
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key 
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile 
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]

