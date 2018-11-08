using CmdLine;
using GHC.Operations;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PHDataImportJob
{
    /// <summary>
    /// This class is used for calling the process specific objects, and should not
    /// contain any actual business logic.
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {

            #region "Common Configuration"

            eLogLevel logLevel = eLogLevel.Informational;   // set default log level to informational. this can be overridden through a CLI argument or in code below.
            CentralArguments arguments = null;

            // parse command-line. properties set in the parameter string
            // can override the properties in the CentralEngine.
            try
            {
                arguments = GetCommandLineArguments();
            }
            catch
            {
                // return error code to caller
                return 4;
            }

            // instantiate central engine
            CentralEngine engine = new CentralEngine();

            // check for arguments that override CentralEngine defaults
            if (arguments != null)
            {
                if (arguments.Input != null) { engine.InputPath = arguments.Input; }
                if (arguments.Output != null) { engine.OutputPath = arguments.Output; }
                if (arguments.LogLevel != 0)
                {
                    logLevel = (eLogLevel)arguments.LogLevel;
                }
            }

            // instantiate central logger.  an additional parameter can be used to override
            // the default log level of Informational. Informational will include all messages
            // flagged as informational, warning, or error.  Use eLogLevel.Debug if you want 
            // to see all debug messages.            
            CentralLogger log = new CentralLogger(engine.OutputPath, engine.AppName, logLevel);

            // log all command-line arguments that are not Help, null, or empty.
            if (arguments != null)
            {
                log.WriteLine("Command-Line Arguments:");
                foreach (PropertyInfo property in arguments.GetType().GetProperties())
                {
                    if (property.Name != "Help" &&
                        property.GetValue(arguments, null) != null &&
                        property.GetValue(arguments, null).ToString().Trim().Length > 0) { log.WriteLine("".PadRight(4) + property.Name + " = " + property.GetValue(arguments, null)); }
                }
            }
            else
            {
                log.WriteLine("No parameters for current execution.");
            }

            #endregion

            /******************************************************************
             * 
             *  This section is for executing the bulk of the business logic.
             *  This should mostly be handled by the application processor, which
             *  CentralProcessor is a template for.  This should be renamed with
             *  the application name: <AppName>Processor.cs, allowing the IDE to
             *  change any existing references.
             * 
             *  Main() must return an int.  0 is used to indicate success.  4 is
             *  reserved for errors loading command-line arguments.  All other codes
             *  are currently dependent on the application, although common error
             *  codes may be defined in the future.
             * 
             *  The processor must have a constructor that accepts the
             *  CentralEngine and CentralLogger objects, instantiated above.
             *  These objects should be made available to any other classes that
             *  may be defined, to provide consist logging, application paths and 
             *  other additional functionality.
             *  
             *  There is an override to the constuctor that also accepts the
             *  arguments object.  If there are no arguments that pertain to
             *  the actual execution of the job (versus configuration) this
             *  can be excluded.
             * 
             *  Example use:
             *  CentralProcessor proc = new CentralProcessor(engine, log);
             *  CentralProcessor proc = new CentralProcessor(engine, log, arguments);
             ******************************************************************/

            // excecute processes here...
            CentralProcessor proc = new CentralProcessor(engine, log);

            try
            {
                proc.Run();
                return 0;
            }
            catch (Exception ex)
            {
                // TODO: log additional information related to error...

                log.WriteLine(ex.Message, eLogLevel.Error, true);
                log.WriteLine("Exit(1) - Failure", eLogLevel.Error, true);
                return 1;
            }
            finally
            {
                log.WriteLine("Job Complete!");
            }
        }

        #region "Helper Methods"

        private static CentralArguments GetCommandLineArguments()
        {
            CentralArguments arguments = null;

            try
            {
                if (Environment.GetCommandLineArgs().Count() > 1)
                {
                    arguments = CommandLine.Parse<CentralArguments>();
                }
            }
            catch (Exception ex)
            {
                // failed to parse parameters.  log message to central console explicitly, since at this point
                // we don't have a a logger or engine.
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("*** ERR *** :: " + ex.Message);

                throw ex;
            }

            return arguments;
        }
        #endregion
    }
}
