using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shutmypc
{
    public partial class Service : ServiceBase
    {
        private IHost _host;
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            _host.StartAsync();
        }

        protected override void OnStop()
        {
            _host?.StopAsync().Wait();
            _host?.Dispose();
        }
        public void RunAsConsole()
        {
            OnStart(null);
            Console.WriteLine("Service running... Press Enter to stop.");
            Console.ReadLine();
            OnStop();
        }
    }
}
