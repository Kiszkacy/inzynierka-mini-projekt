using Godot;
using System.Net;
using System.Net.Sockets;
using System.Text;

public partial class SocketHandler : Node
{
    private Socket sender;

    public override void _Ready()
    {
        this.Run();
    }
    
    private void Run()
    {
        sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 12345);
        
        sender.Connect(remoteEP);
        
        GD.Print($"Connected to server {sender.RemoteEndPoint}.");
        
        string message = "Hello, server!";
        byte[] msg = Encoding.ASCII.GetBytes(message);
        Send(msg, msg.Length);

        byte[] bytes = new byte[1024];
        int bytesRec = sender.Receive(bytes);
        string receivedMessage = Encoding.UTF8.GetString(bytes, 0, bytesRec);
        GD.Print($"Received: {receivedMessage}");

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }
    
    public void Send(byte[] data, int size)
    {
        sender.Send(data, size, SocketFlags.None);
    }

    public override void _Process(double delta)
    {
        
    }
    
    public SocketHandler()
    {
    
    }
}
