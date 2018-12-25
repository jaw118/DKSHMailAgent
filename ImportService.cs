using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using DKSH.MailAgent.Jobs.Interfaces;
using DKSH.MailAgent.Settings;
using DKSH.MailAgent.Services.Interfaces;

using Polly;
using Quartz.Spi;
using Serilog;
using System.Threading;
using Topshelf;


 
namespace DKSH.MailAgent
{
  
    public class ImportService : ServiceControl
    {
        public IJobFactory Factory { get; set; }
        public IContainer Container { get; set; }
        readonly ILogger logger;
        private CancellationTokenSource token;
        private readonly string[] extensions = new string[] { ".xls", ".xlsx", ".zip", ".txt", ".csv", ".mdb", ".b23", ".b25", ".bue" };
        private readonly ILifetimeScope lifetime;

        private IJobWatcher JobWatcher { get; }

        public ImportService(ILogger logger, 
            IJobWatcher jobwatcher, ILifetimeScope lifetime)
        {
            this.logger = logger;
            this.JobWatcher = jobwatcher;
            this.lifetime = lifetime;
        }

        public bool Start(HostControl control)
        {
            logger.Verbose("Creating file system watchers");
            WatcherFactory.Instance.Initialize(JobWatcher);

            token = new CancellationTokenSource();

            logger.Verbose("Starting the redis polling");
            //
             // process
            //
            logger.Debug("Import Service started");
            return true;
        }

        public bool Stop(HostControl control)
        {
            logger.Verbose("Request for token cancellation");
            token?.Cancel();

            logger.Verbose("Disposing all file system watchers");
            WatcherFactory.Instance.Dispose();

            logger.Verbose("Disposing serilog");
            Log.CloseAndFlush();

            logger.Debug("Import service stopped");
            return true;
        }

        private void RunQueue(string type)
        {
            logger.Warning("Unable to run database job with type of : {QueueType}");


            logger.Verbose("Running job {QueueType}", "");


           // Runn Process
            

    
        }
    }
}
