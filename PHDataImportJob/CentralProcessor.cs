using GHC.Operations;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PHDataImportJob
{
    /// <summary>
    /// The CentralProcessor is responsible for the main business logic of the application.  This
    /// class can consume any other necessary tasks.
    /// </summary>
    class CentralProcessor
    {
        private CentralLogger log;
        private CentralEngine engine;
        private CentralArguments arguments;

        /// <summary>
        /// The CentralProcess constructor takes an instantiated engine and log object.  The engine
        /// will contain all of the paths and system variables consistent with a Central job. This
        /// constructor can be extended, but CentralEngine and CentralLogger must ALWAYS be implemented.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="log"></param>
        public CentralProcessor(CentralEngine inEngine, CentralLogger inLog) : this(inEngine, inLog, null)
        {
            // chained constructor
        }

        /// <summary>
        /// Override for constructor that also accepts arguments object.
        /// </summary>
        /// <param name="inEngine"></param>
        /// <param name="inLog"></param>
        /// <param name="inArguments"></param>
        public CentralProcessor(CentralEngine inEngine, CentralLogger inLog, CentralArguments inArguments)
        {
            log = inLog;
            engine = inEngine;
            arguments = inArguments;
        }

        /// <summary>
        /// A base method for executing a batch process.
        /// </summary>
        public void Run()
        {

        }

        #region "examples"
        ///// <summary>
        ///// This method demonstrates how the log and engine can be used in your application.
        ///// </summary>
        //public void ExampleProc()
        //{
        //    log.WriteLine("This is the beginning of the ExampleProc");
        //    log.WriteLine("The application processing path is " + engine.OutputPath);
        //}
        #endregion
    }
}
