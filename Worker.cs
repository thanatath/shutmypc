using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace shutmypc
{
    public class Worker : BackgroundService
    {
        private HttpListener _listener;
        private const string url = "http://localhost:5000/hibernation/";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(url);
            _listener.Start();

            Console.WriteLine("Service started and listening for HTTP hook...");

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();
                if (context.Request.HttpMethod == "POST")
                {
                    HibernatePC();
                    Console.WriteLine("Received hook, hibernating PC.");
                }

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Close();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _listener.Stop();
            await base.StopAsync(stoppingToken);
        }

        private void HibernatePC()
        {
            // Use Windows API to hibernate the PC
            SetSuspendState(true, true, true);
        }

        [DllImport("powrprof.dll", SetLastError = true)]
        private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
