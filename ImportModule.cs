//-----------------------------------------------------------------------

// <copyright file="ImportModule.cs" company="PT. Solusi Manufaktur Teknologi">

// Copyright (c) PT. Solusi Manufaktur Teknologi 2016 All rights reserved.

// </copyright>

// <author>PT. Solusi Manufaktur Teknologi</author>


//-----------------------------------------------------------------------

using Autofac;
using AutofacSerilogIntegration;
using DKSH.MailAgent.Constants;
using DKSH.MailAgent.Data.Models;
using DKSH.MailAgent.Factories;
using DKSH.MailAgent.Jobs;
using DKSH.MailAgent.Jobs.Interfaces;
using DKSH.MailAgent.Models;
using DKSH.MailAgent.Repositories;
using DKSH.MailAgent.Repositories.Interfaces;
using DKSH.MailAgent.Services;
using DKSH.MailAgent.Services.Interfaces;
using Quartz;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using DKSH.MailAgent.Factories;
namespace CustomsInventory.WinConsole
{
    internal class ImportModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            LoadRepositories(builder);
            LoadServices(builder);
            LoadLogger(builder);
            LoadScheduler(builder);
            builder.RegisterType<PolicyFactory>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<FileWatcherJob>().Keyed<IConsoleJob>(JobState.FileWatcher);
            builder.RegisterType<ImportJob>().Keyed<IDatabaseJob>(JobState.Import).Keyed<IConsoleJob>(JobState.Import);
            builder.RegisterType<JobWatcher>().As<IJobWatcher>();
        }

        private void LoadScheduler(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(p => typeof(IJob).IsAssignableFrom(p));
        }

        private void LoadLogger(ContainerBuilder builder)
        {
            builder.RegisterLogger();
        }

        private void LoadServices(ContainerBuilder builder)
        {
            builder.RegisterType<FileService>().As<IFileService>();
            builder.RegisterType<FileReadService>().As<IFileReadService>();
            builder.RegisterType<TextService>().As<ITextService>();
            builder.RegisterType<JobService>().As<IJobService>();
           
        }

        private void LoadRepositories(ContainerBuilder builder)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SmartConnection"].ConnectionString;
            var fileConnectionString = ConfigurationManager.ConnectionStrings["SmartConnectionFile"].ConnectionString;
            builder.Register(a => new SqlConnection(connectionString)).As<IDbConnection>();
            builder.RegisterAssemblyTypes(typeof(ImportRepository).Assembly)
                .Where(t => t.Name.EndsWith("Repository") && t.Name != "FileRepository")
                .AsImplementedInterfaces();

            builder.Register(a => new FileRepository(new SqlConnection(fileConnectionString))).As<IFileRepository>();

            //Register Data Context related to Export to Microsoft Access
            string destinationDirectory = Path.Combine(Environment.CurrentDirectory, "tempFile");
            string destinationUrl = Path.Combine(destinationDirectory, "export.mdb");

            builder
                .RegisterType<OleDataContext>()
                .WithParameter("filePath", destinationUrl)
                .WithParameter("fileDirectory", destinationDirectory)
                .As<IOleDataContext>();
           
        }
    }
}
