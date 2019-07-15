using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace WebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var currentIP = getLocalIP();
            return WebHost.CreateDefaultBuilder(args)
                        .UseUrls($"http://{currentIP.ToString()}:80", "http://localhost:80")
                        .UseStartup<Startup>();
        }
        public static IPAddress getLocalIP()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .Where(ip =>
                {
                    return ip.ToString().StartsWith("192");
                }).FirstOrDefault();
        }
        
    }


}
