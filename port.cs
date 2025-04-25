// This is C# code 
using System;
using System.Net.Sockets;
using System.Threading;

namespace PortScannerFramework
{
    class Program
    {
        // Object to prevent console writing collisions between threads
        static object lockObj = new object();

        static void Main(string[] args)
        {
            Console.Title = "C# Port Scanner (.NET Framework)";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==== C# Simple Port Scanner ====\n");

            // Input from user
            Console.Write("Enter target IP or domain: ");
            string host = Console.ReadLine();

            Console.Write("Enter starting port (e.g., 1): ");
            int startPort = int.Parse(Console.ReadLine());

            Console.Write("Enter ending port (e.g., 1024): ");
            int endPort = int.Parse(Console.ReadLine());

            Console.WriteLine($"\n[+] Scanning {host} from port {startPort} to {endPort}...\n");

            for (int port = startPort; port <= endPort; port++)
            {
                int currentPort = port;

                // Start a new thread for each port
                Thread t = new Thread(() => ScanPort(host, currentPort));
                t.Start();

                Thread.Sleep(10); // Optional delay to avoid resource spike
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[!] Scan started. Results will appear above.\n");
            Console.ResetColor();

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }

        static void ScanPort(string host, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    IAsyncResult result = client.BeginConnect(host, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(800));

                    if (success && client.Connected)
                    {
                        lock (lockObj)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[OPEN] Port {port}");
                            Console.ResetColor();
                        }
                    }
                }
            }
            catch
            {
                // Port is closed or filtered; ignore
            }
        }
    }
}

