using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Laboratory_12_Data;

namespace Laboratory_12_Client
{
    class Client
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);

        private static TcpClient client;
        private static bool connected = false;
        private static ManualResetEvent receiveEvent = new ManualResetEvent(false);
        private static bool isSendManyData = false;
        private static int temporary = 0;

        public static void Main()
        {
            SetConsoleTitle("Client-2");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Console.WriteLine("Client> Type 'help' to see available commands.");

            while (true)
            {
                Console.Write(connected ? "Client-connected> " : "Client> ");
                string command = Console.ReadLine();

                if (command == "server-connect")
                {
                    if (connected)
                    {
                        Console.WriteLine("Client-connected> Already connected to the server.");
                        continue;
                    }

                    try
                    {
                        client = new TcpClient();
                        client.Connect("127.0.0.1", 9999);
                        connected = true;
                        Console.WriteLine("Client-connected> Connected to the server.");

                        Thread receiveThread = new Thread(ReceiveMessages);
                        receiveThread.Start();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Client> Connection error. The server is probably offline.");
                    }
                }
                else if (command == "server-disconnect")
                {
                    if (connected)
                    {
                        SendMessage("disconnect");
                        client.Close();
                        connected = false;
                        Console.WriteLine("Client> Disconnected from the server.");
                    }
                    else
                    {
                        Console.WriteLine("Client> Not connected to the server.");
                    }
                }
                else if (command == "server-status")
                {
                    CheckServerStatus();
                }
                else if (command == "send-data")
                {
                    if (connected)
                    {
                        Console.Write("Client-connected> Enter InputA and InputB separated by space: ");
                        string input = Console.ReadLine();
                        string[] parts = input.Split(' ');

                        if (parts.Length == 2 && int.TryParse(parts[0], out int inputA) && int.TryParse(parts[1], out int inputB))
                        {
                            Data data = new Data { InputA = inputA, InputB = inputB, Content = "Sent" };
                            string serializedData = DataSerializer.Serialize(data);
                            isSendManyData = false;
                            SendMessage(serializedData);
                            Console.WriteLine($"Client-connected> Data sent for calculation: {serializedData}.");

                            ReceiveResponse();
                        }
                        else
                        {
                            Console.WriteLine("Client-connected> Invalid input. Please enter two integers.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Client> Not connected to the server.");
                    }
                }
                else if (command == "send-many-data")
                {
                    if (connected)
                    {
                        Console.Write("Client-connected> Enter the number of data points to send: ");
                        string input = Console.ReadLine();

                        if (int.TryParse(input, out int numberOfData))
                        {
                            isSendManyData = true;
                            SendManyData(numberOfData);
                        }
                        else
                        {
                            Console.WriteLine("Client-connected> Invalid number. Please enter an integer.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Client> Not connected to the server.");
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
        }

        private static void ReceiveMessages()
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                while (connected && (bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (message == "server-shutdown")
                    {
                        Console.Write("Lost connection to the server...\nClient> The server has been shut down. Disconnected from the server.\nClient> ");
                        connected = false;
                        client.Close();
                        break;
                    }
                    else if (message.StartsWith("{") && message.EndsWith("}"))
                    {
                        Data data = DataSerializer.Deserialize(message);
                        Thread.Sleep(500);

                        if (isSendManyData)
                        {
                            Console.WriteLine($"Client-connected> Received data from server: {message}.");
                        }
                        else
                        {
                            if (temporary == 0)
                            {
                                Console.WriteLine($"Client-connected> Received data from server: {message}.");
                                temporary++;
                            }
                            else
                            {
                                Console.Write($"Received data from server: {message}\nClient-connected> ");
                            }
                        }

                        receiveEvent.Set();
                    }
                }
            }
            catch (Exception e)
            {
                if (connected)
                {
                    Console.Write("Existing connection was abruptly closed by the remote host...\nClient> Disconnected from the server.\nClient>");
                    connected = false;
                }
            }
            finally
            {
                if (connected)
                {
                    connected = false;
                    client.Close();
                    Console.Write("Disconnecting client by server...\nClient> Client disconnected by the server.\nClient> ");
                }
            }
        }

        private static void SendMessage(string message)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
                else
                {
                    //Console.WriteLine("Client> Not connected to the server.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Client> Error while sending message.");
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            if (connected)
            {
                SendMessage("disconnect");
                client.Close();
                Console.WriteLine("Client> Closing client, disconnected from the server.");
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("> server-connect    - Connect to the server.");
            Console.WriteLine("> server-disconnect - Disconnect from the server.");
            Console.WriteLine("> server-status     - Check server status.");
            Console.WriteLine("> send-data         - Send a task for computation (InputA and InputB).");
            Console.WriteLine("> send-many-data    - Send multiple tasks for computation (Number of data points).");
            Console.WriteLine("> help              - Display available commands.");
        }

        private static void CheckServerStatus()
        {
            try
            {
                if (connected)
                {
                    Console.WriteLine("Client-connected> Server is running.");
                }
                else
                {
                    using (TcpClient pingClient = new TcpClient())
                    {
                        pingClient.Connect("127.0.0.1", 9999);
                        SendMessage("ping");
                        Console.WriteLine("Client> Server is running.");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Client> Server is stopped.");
            }
        }

        private static void SendManyData(int numberOfData)
        {
            Random random = new Random();

            for (int i = 0; i < numberOfData; i++)
            {
                int inputA = random.Next(0, 16);
                int inputB = random.Next(0, 16);

                Data data = new Data { InputA = inputA, InputB = inputB, Content = "Sent" };
                string serializedData = DataSerializer.Serialize(data);
                Thread.Sleep(500);
                SendMessage(serializedData);

                Console.WriteLine($"Client-connected> {{{i}}} Data sent for calculation: {serializedData}.");

                receiveEvent.Reset();
                ReceiveResponse();
            }
        }

        private static void ReceiveResponse()
        {
            receiveEvent.WaitOne();
        }
    }
}
