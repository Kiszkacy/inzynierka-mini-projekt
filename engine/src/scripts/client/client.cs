using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    public static void Main()
    {
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 12345);

        sender.Connect(remoteEP);

        Console.WriteLine("Połączono z serwerem {0}", sender.RemoteEndPoint);

        string message = "Hello, server!";
        byte[] msg = Encoding.ASCII.GetBytes(message);
        int bytesSent = sender.Send(msg);

        byte[] bytes = new byte[1024];
        int bytesRec = sender.Receive(bytes);
        Console.WriteLine("Odebrano: {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }
}
