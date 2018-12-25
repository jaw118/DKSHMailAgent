using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MailKit;
using Serilog;
using Serilog.Debugging;
using Topshelf;
using Autofac;
using Topshelf.Autofac;
using Dapper;
using Fclp;
using Quartz;
using Quartz.Impl;
 using System.Collections.Specialized;
using System.Globalization;
using Topshelf.Quartz;
using Topshelf.ServiceConfigurators;
using DKSH.MailAgent.Settings;
using DKSH.MailAgent.Container;
using DKSH.MailAgent.WindowServices;
using DKSH.MailAgent.Extensions;
using DKSH.MailAgent.Jobs;
using DKSH.MailAgent.Jobs.Interfaces;
using DKSH.MailAgent.Constants;
using DKSH.MailAgent.Services;
using DKSH.MailAgent.Models;
using DKSH.MailAgent.Services.Interfaces;




namespace DKSH.MailAgent
{
    class Program
    {
  
        static IProtocolLogger pLogger = new ProtocolLogger(DKSHMailFolder.POP3Log, true);


        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            var p = new FluentCommandLineParser<Options>();
            p.Setup(arg => arg.JobState).As('c', "console").WithDescription("Execute operation in console mode for specific job");
            p.Setup(arg => arg.Migration).As('m', "migration").WithDescription("Mark this operation as migration process");
            p.Setup(arg => arg.Date).As('d', "date").WithDescription("Specify the date for this operation to run on");
            p.Setup(arg => arg.ExportTextType).As('e', "exporttext").WithDescription("Specify the export text type to process");
            p.Parse(args);

            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.AppSettings()
                .CreateLogger();

            SqlMapper.AddTypeMap(typeof(DateTime), System.Data.DbType.DateTime2);
            SqlMapper.AddTypeMap(typeof(DateTime?), System.Data.DbType.DateTime2);

            var builder = new ContainerBuilder();
            builder.RegisterModule<ImportModule>();
           
            builder.Register(x => p.Object).SingleInstance();
            var container = builder.Build();

            if (ConsoleMode(p.Object, container))
            {
                Log.CloseAndFlush();
                return;
            }

            var jobSettings = JobSettingConfig.GetConfig();
            var MailSetting = ConfigurationManager.GetSection(DKSHConfig.MailConfig) as DKSH.MailAgent.Settings.MailConfig;
            var LogConfing = ConfigurationManager.GetSection(DKSHConfig.LogConfig) as DKSH.MailAgent.Settings.LogConfig;

            var host = HostFactory.New(c =>
            {
                
                c.UseSerilog();
                c.Service<IMailAgentService>(s =>
                {
                    s.ConstructUsing(name => new MailAgentService(MailSetting, LogConfing, Log.Logger));
                    s.WhenStarted(tc => tc.Start()); 
                    s.WhenStopped(tc => tc.Stop()); 
                });

                c.RunAsLocalSystem();
                c.StartAutomaticallyDelayed();
                c.SetDisplayName("DKSHMailAgent2 Mail Upload");
                c.SetDescription("DKSHMailAgent2 Scheduler");
                c.SetServiceName("DKSHMailAgent2");
                c.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(5);
                });
                c.OnException(e =>
                {
                    Log.Logger.Error(e, "Unhandled exception on service level");
                });
            });

            host.Run();
        }

        static void ScheduleJob<TJob>(ServiceConfigurator<ImportService> s, JobSettingConfig jobSettings)
            where TJob : IJob
        {
            var jobSetting = jobSettings.Instances[typeof(TJob).Name];
            if (jobSetting != null)
            {
                s.ScheduleQuartzJob(q =>
                {

                    var job = JobBuilder.Create<TJob>().WithIdentity(typeof(TJob).Name).Build();
                    q.WithJob(() => job).AddTrigger(() => CreateTrigger(jobSettings.Instances[typeof(TJob).Name]));
                });
            }
        }

        static bool ConsoleMode(Options o, IContainer container)
        {
            if (o.JobState != null)
            {
                try
                {
                    using (var scope = container.BeginLifetimeScope())
                    {
                        var job = scope.ResolveKeyed<IConsoleJob>(o.JobState.Value);
                        if (job != null)
                        {
                            job.RunConsole();
                        }
                        else
                        {
                            Log.Logger.Warning("Failed to get the job for type : {Type}", o.JobState);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Fatal(ex, "Failed running console");
                }
                return true;
            }
            return false;
        }

        static ITrigger CreateTrigger(JobSetting setting)
        {
            //default setup to per hour
            string cron = "0 0 * ? * * *";
            switch (setting.Type)
            {
                case IntervalType.Second:
                    cron = $"0/{setting.Interval} * * ? * * *";
                    break;
                case IntervalType.Minute:
                    cron = $"0 0/{setting.Interval} * ? * * *";
                    break;
                case IntervalType.Hour:
                    cron = $"0 0 0/{setting.Interval} ? * * *";
                    break;
                case IntervalType.Quarter:
                    cron = "0 0,15,30,45 * ? * * *";
                    break;
                case IntervalType.Half:
                    cron = "0 0,30 * ? * * *";
                    break;
                case IntervalType.Day:
                    cron = $"0 0 {setting.DailyHour} ? * * *";
                    break;
                default:
                    return TriggerBuilder.Create().StartNow().Build();
            }
            var trigger = TriggerBuilder.Create()
                                .WithCronSchedule(cron, x => x.WithMisfireHandlingInstructionIgnoreMisfires()).Build();
            return trigger;
        }



    }
}
