using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Laboratory_12_Data;

namespace Laboratory_12_Server;

class Server
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleTitle(string lpConsoleTitle);

    private static List<(TcpClient client, DateTime startTime)> clients = new List<(TcpClient, DateTime)>();
    private static object clientsLock = new object();
    private static bool isRunning = true;

    private static void HandleClient(TcpClient client)
    {
        DateTime startTime = DateTime.Now; 

        lock (clientsLock)
        {
            clients.Add((client, startTime));
        }

        IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        Console.Write($"Connected to client: {clientEndPoint}\nServer> ");
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (client.Connected && (bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (message == "disconnect")
                {
                    break;
                }
                else if (message == "ping")
                {
                    SendMessage(client, "pong");
                }
                else if (message.StartsWith("{") && message.EndsWith("}"))
                {
                    Data data = DataSerializer.Deserialize(message);
                    data.Content = "InProgress";
                    string updatedDataMessage = DataSerializer.Serialize(data);
                    Console.WriteLine($"Calculating data for {clientEndPoint}: {updatedDataMessage}.");

                    data.Result = (long)Math.Pow(data.InputA, data.InputB);
                    data.Content = "Done";

                    string serializedData = DataSerializer.Serialize(data);
                    Thread.Sleep(500);
                    SendMessage(client, serializedData);
                    Console.Write($"Server> Calculated data for {clientEndPoint}: {serializedData}\nServer> ");
                }
            }
        }
        catch (Exception e)
        {
            //Console.WriteLine("Server> Error: " + e.Message);
        }
        finally
        {
            DateTime endTime = DateTime.Now;
            TimeSpan sessionDuration = endTime - startTime;
            Console.Write($"Client {clientEndPoint} disconnected. Session duration: {sessionDuration}\nServer> ");
            lock (clientsLock)
            {
                clients.Remove((client, startTime));
            }
            client.Close();
        }
    }

    private static void ListClients()
    {
        lock (clientsLock)
        {
            Console.WriteLine($"Server> Current number of connections: {clients.Count}");
            foreach (var (client, startTime) in clients)
            {
                IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                DateTime currentTime = DateTime.Now;
                TimeSpan sessionDuration = currentTime - startTime;
                Console.WriteLine($"> Client: {clientEndPoint}, Session duration: {sessionDuration}.");
            }
        }
    }

    private static void NotifyClientsOfShutdown()
    {
        lock (clientsLock)
        {
            foreach (var (client, _) in clients)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes("server-shutdown");
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Server> Error sending message to client: " + e.Message);
                }
            }
        }
    }

    private static void DisconnectClient(string port)
    {
        lock (clientsLock)
        {
            foreach (var (client, startTime) in clients)
            {
                IPEndPoint clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                if (clientEndPoint.Port.ToString() == port)
                {
                    Console.WriteLine($"Server> Disconnecting client: {clientEndPoint}.");
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        byte[] data = Encoding.UTF8.GetBytes("server-disconnect");
                        stream.Write(data, 0, data.Length);
                        client.Close();
                        clients.Remove((client, startTime));
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Server> Error disconnecting client: " + e.Message);
                    }
                }
            }
        }
    }

    private static void ShutdownServer()
    {
        isRunning = false;
        NotifyClientsOfShutdown();
        lock (clientsLock)
        {
            foreach (var (client, _) in clients)
            {
                client.Close();
            }
            clients.Clear();
        }
        Console.Write("Server> Server has been shut down. All clients have been disconnected.\nServer> ");
        Environment.Exit(0);
    }

    public static void Main()
    {
        SetConsoleTitle("Server");
        TcpListener server = new TcpListener(IPAddress.Any, 9999);
        server.Start();
        Console.WriteLine($"Server> Server is listening on: {((IPEndPoint)server.LocalEndpoint).Address}:{((IPEndPoint)server.LocalEndpoint).Port}.");
        Console.WriteLine("Server> Type 'help' to see available commands.");
        Thread consoleThread = new Thread(() =>
        {
            while (true)
            {
                Console.Write("Server> ");
                string command = Console.ReadLine();
                if (command == "list")
                {
                    ListClients();
                }
                else if (command == "shutdown")
                {
                    server.Stop();
                    ShutdownServer();
                }
                else if (command.StartsWith("client-disconnect "))
                {
                    string[] parts = command.Split(' ');
                    if (parts.Length == 2)
                    {
                        DisconnectClient(parts[1]);
                    }
                    else
                    {
                        Console.WriteLine("Server> Incorrect usage of 'client-disconnect' command. Usage: client-disconnect [port].");
                    }
                }
                else if (command == "help")
                {
                    PrintHelp();
                }
                else
                {
                    Console.WriteLine("> Unknown command. Type 'help' to see available commands.");
                }
            }
        });

        consoleThread.Start();

        while (isRunning)
        {
            try
            {
                if (server.Pending())
                {
                    TcpClient client = server.AcceptTcpClient();
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
                else
                {
                    Thread.Sleep(100); 
                }
            }
            catch (SocketException)
            {
                if (!isRunning)
                {
                    // Server has been stopped
                }
            }
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("> list                - Display list of connected clients.");
        Console.WriteLine("> shutdown            - Shut down the server and disconnect all clients.");
        Console.WriteLine("> client-disconnect [port] - Disconnect a selected client.");
        Console.WriteLine("> help                - Display available commands.");
    }

    private static void SendMessage(TcpClient client, string message)
    {
        if (client.Connected)
        {
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}
