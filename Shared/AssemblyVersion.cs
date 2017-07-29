using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#endif

[assembly: AssemblyCompany("P3Net")]
[assembly: AssemblyProduct("SSIS Custom Tasks")]
[assembly: AssemblyCopyright("(c) Michael Taylor 2016, All Rights Reserved")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion(AssemblyMetadata.ProductVersion)]
[assembly: AssemblyFileVersion(AssemblyMetadata.FileVersion)]
[assembly: AssemblyInformationalVersion(AssemblyMetadata.FileVersion)]

internal static class AssemblyMetadata
{
    public const string ProductVersion = "2.0.0.0";

    public const string FileVersion = "2.0.0.0";
}