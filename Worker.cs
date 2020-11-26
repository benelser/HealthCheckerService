using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HealthChecker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient Client;
        private bool Alive;
        private EnvironmentData EnvData;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
             this.Client = new HttpClient();
            this.Alive = true;
            this.EnvData  = EnvironmentData.Read();
        }

        // Entry point
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.MonitorDestination(stoppingToken);
        }

        public void MonitorDestination(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                if (stoppingToken.IsCancellationRequested == true)
                {
                    break;
                }
                while ((this.GetConnectionResult() == true) || stoppingToken.IsCancellationRequested == true)
                {
                    if (stoppingToken.IsCancellationRequested == true)
                    {
                        break;
                    }
                }
                this.SendNotification(this.Alive);
                
                do
                {
                    if (stoppingToken.IsCancellationRequested == true)
                    {
                        break;
                    }
                    _logger.LogInformation("Waiting for destination to become alive: {time}", DateTimeOffset.Now);

                } while ((GetConnectionResult() == false) || stoppingToken.IsCancellationRequested == true);
                
                this.SendNotification(this.Alive);
                
            }

        }
        private bool GetConnectionResult()
        {
            _logger.LogInformation($"Heartbeat set for every {EnvData.Heartbeat / 1000} seconds");
            Thread.Sleep(EnvData.Heartbeat);
            return this.TestConnection().GetAwaiter().GetResult();
        }

        private void SendNotification(bool status)
        {
            
            // Do STUFF HERE
            // GET LOGS 
            // BUILD EMAIL TEMPLATE
            if (status)
            {
                _logger.LogInformation($"Sending notifiation: SYSTEM UP to Recipients: {string.Join(",", EnvData.To)}");  
            }
            else
            {
                _logger.LogInformation($"Sending notifiation: SYSTEM DOWN to Recipients: {string.Join(",", EnvData.To)}");  
            }
            
        }

        public async Task<bool> TestConnection()
        {
            try
            {

                HttpResponseMessage response = await this.Client.GetAsync(EnvData.Destination);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Health check succeeded to: {EnvData.Destination} at: {DateTimeOffset.Now}");
                    this.Alive = true;
                    return this.Alive;
                }
                else
                {
                    _logger.LogWarning($"Health check failed to: {EnvData.Destination} at: {DateTimeOffset.Now}");
                    this.Alive = false;
                    return this.Alive;
                }
            }
            catch (System.Exception)
            {
                _logger.LogWarning($"Health check failed to: {EnvData.Destination} at: {DateTimeOffset.Now}");
                this.Alive = false;
                return this.Alive;
            }

            return this.Alive;
        }
    }
}