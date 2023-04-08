using System.Reflection;
using System.Resources;

// General Information
[assembly: AssemblyTitle("SecureSpareTire")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tommo J. Productions")]
[assembly: AssemblyProduct("SecureSpareTire")]
[assembly: AssemblyCopyright("Tommo J. Productions Copyright © 2023")]
[assembly: AssemblyTrademark("Azine")]
[assembly: NeutralResourcesLanguage("en-AU")]

// Version information
[assembly: AssemblyVersion("1.1.457.3")]
[assembly: AssemblyFileVersion("1.1.457.3")]

namespace TommoJProductions.SecureSpareTire
{

    public class VersionInfo
    {
	    public const string lastestRelease = "03.04.2023 07:17 PM";
	    public const string version = "1.1.457.3";

        /// <summary>
        /// Represents if the mod has been complied for x64
        /// </summary>
        #if x64
            internal const bool IS_64_BIT = true;
        #else
            internal const bool IS_64_BIT = false;
        #endif
        /// <summary>
        /// Represents if the mod has been complied in Debug mode
        /// </summary>
        #if DEBUG
            internal const bool IS_DEBUG_CONFIG = true;
        #else
            internal const bool IS_DEBUG_CONFIG = false;
        #endif
    }
}
