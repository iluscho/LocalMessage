using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Messenger App!");

        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Create a server");
        Console.WriteLine("2. Connect to a server");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            StartServer();
        }
        else if (choice == "2")
        {
            Console.Write("Enter server IP address: ");
            string ipAddress = Console.ReadLine();
            ConnectToServer(ipAddress);
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
    }

    static void StartServer()
    {
        Console.WriteLine("Starting server...");
        TcpListener server = null;
        try
        {
            int port = 8888;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);
            server.Start();

            Console.WriteLine("Server started. Waiting for connections...");

            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client connected.");

            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                SendMessage(client, message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e}");
            server?.Stop();
        }
    }

    static void ConnectToServer(string ipAddress)
    {
        try
        {
            int port = 8888;
            TcpClient client = new TcpClient(ipAddress, port);
            Console.WriteLine("Connected to server.");

            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                SendMessage(client, message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e}");
        }
    }

    static void ReceiveMessages(TcpClient client)
    {
        try
        {
            while (true)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {dataReceived}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e}");
            client.Close();
        }
    }

    static void SendMessage(TcpClient client, string message)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
            Console.WriteLine("Message sent.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e}");
        }
    }
}
