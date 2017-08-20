using Serilog;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using Newtonsoft.Json;

namespace SplunkTest
{
    class Program
    {
        static void SetGlobalValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                var httpWebRequest = sender as HttpWebRequest;
                if (httpWebRequest==null)
                {
                    return false;
                }

                if (httpWebRequest.RequestUri.AbsoluteUri.ToString().ToLower().Contains("cloud.splunk.com"))
                {
                    return true;
                }

                return false;
            };
        }

        static void Main(string[] args)
        {
            var splunkUrl = @"https://input-xxxxxx.cloud.splunk.com:8088/services/collector";
            var splunkToken = @"";

            //SetGlobalValidation();

            SerilogLogging(splunkUrl, splunkToken);

        }

        static void SerilogLogging(string splunkUrl, string splunkToken)
        {
            // Only required if Splunk instance is using untrusted SSL Certificate
            var client = new HttpClientHandler();
            client.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            // Only required if using Proxy
            client.UseProxy = true;
            client.UseDefaultCredentials = true;

            // Write Serilog error in console
            Serilog.Debugging.SelfLog.Enable(Console.Error);
            
            // Create the Serilog configuration
            var config = new LoggerConfiguration();

            // Configure the Splunk Sink
            config.WriteTo.EventCollector(
                splunkUrl, 
                splunkToken,
                messageHandler:client);

            // Create the logger
            var logger = config.CreateLogger();

            // Log
            logger.Information("Hello World from C#");

            // Flush
            logger.Dispose();
        }
        
    }
}
