using GHC.Operations;
using CmdLine;

namespace PHDataImportJob
{
    /// <summary>
    /// Provides base functionality for command-line parsing.  This includes default properties available
    /// to all Central applications, as well as any properties overridden specifically for the application.
    /// 
    /// A property is defined using an attribute with a series of descriptors. Example syntax for a property:
    /// [CommandLineParameter(Command = "o", Description = "Output Path", Name = "Output")]
    /// public string Output { get; set; }
    /// 
    /// Default properties include:
    ///     Help     (?)     - Displays the help file for the applications, listing all valid parameters.
    ///     Output   (o)     - Allows overriding the output path for the application.
    ///     Input    (i)     - Allows overriding the input path for the application.
    ///     LogLevel (l)     - Explicitly set the log level.  Valid numeric options are:
    ///                          1 = Error
    ///                          2 = Warning
    ///                          3 = Informational (Default)
    ///                          4 = Debug
    ///     
    /// The cmdline api takes parameters in the following formats:
    ///   /o:c:\test\something\ /f:9/1/2013 /t:9/30/2013
    ///   /o=c:\test\something\ /f=9/1/2013 /t=9/30/2013
    ///   -o:c:\test\something\ -f:9/1/2013 -t:9/30/2013
    ///   -o=c:\test\something\ -f=9/1/2013 -t=9/30/2013
    /// </summary>
    class CentralArguments : CentralCommandLine
    {
        #region "Add Command Line Parameter Example"
        //[CommandLineParameter(Command = "c", Description = "Claim File Name", Name = "ClaimFileName")]
        //public string ClaimFileName { get; set; }
        #endregion
    }
}
