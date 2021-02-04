using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SpeedTest;
using SpeedTest.Models;

namespace Network_Utility
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(120,35);
            Console.Title = "Network Utility By Param Kalaria";
            localip();
            Externalip();
            speedtest();
            Console.ReadLine();
        }

        public static void localip()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            // string myIP2 = Dns.GetHostByName(hostName).AddressList[1].ToString();

            Console.WriteLine("Computer           := " + hostName);
            Console.WriteLine("Local IP Address   := " + myIP);
            //Console.WriteLine("Local IP Address 2 := " + myIP2);            
        }


        public static void Externalip()
        {
            try
            {
                string externalip = new WebClient().DownloadString("http://icanhazip.com");
                Console.WriteLine("External IP Address:= " + externalip);
                Console.WriteLine("\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Not Connected to Internet");
                Console.WriteLine("\n");
            }

        }

        private static SpeedTestClient client;
        private static Settings settings;
        public static void speedtest()
        {
            try
            {
                client = new SpeedTestClient();
                settings = client.GetSettings();
                Console.WriteLine("Getting speedtest.net settings and server list...");
                var servers = SelectServers();
                var bestServer = SelectBestServer(servers);

                Console.WriteLine("Testing speed...");
                Console.WriteLine("\n");
                var downloadSpeed = client.TestDownloadSpeed(bestServer, settings.Download.ThreadsPerUrl);
                PrintSpeed("Download", downloadSpeed);
                var uploadSpeed = client.TestUploadSpeed(bestServer, settings.Upload.ThreadsPerUrl);
                PrintSpeed("Upload", uploadSpeed);
                Console.WriteLine("\n");
                Console.WriteLine("Press a key to exit.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n");
                Console.WriteLine("Not Connected to Internet");
                Console.WriteLine("\n");
                Console.WriteLine("Press a key to exit.");
            }

        }


        private static Server SelectBestServer(IEnumerable<Server> servers)
        {
            Console.WriteLine();
            Console.WriteLine("Best server by latency:");
            var bestServer = servers.OrderBy(x => x.Latency).First();
            PrintServerDetails(bestServer);
            Console.WriteLine();
            return bestServer;
        }

        private static IEnumerable<Server> SelectServers()
        {
            Console.WriteLine();
            Console.WriteLine("Selecting best server by distance...");
            var servers = settings.Servers.Take(10).ToList();

            foreach (var server in servers)
            {
                server.Latency = client.TestServerLatency(server);
                PrintServerDetails(server);
            }
            return servers;
        }

        private static void PrintServerDetails(Server server)
        {
            Console.WriteLine("Hosted by {0} ({1}/{2}), distance: {3}km, latency: {4}ms", server.Sponsor, server.Name,
                server.Country, (int)server.Distance / 1000, server.Latency);
        }

        private static void PrintSpeed(string type, double speed)
        {
            if (speed > 1024)
            {
                Console.WriteLine("{0} speed: {1} Mbps", type, Math.Round(speed / 1024, 2));
            }
            else
            {
                Console.WriteLine("{0} speed: {1} Kbps", type, Math.Round(speed, 2));
            }
        }
    }
}
