using Godot;
using System.Net;
using System.Net.Sockets;
using System.Text;

public partial class SocketHandler : Node
{
    public override void _Ready()
    {
        this.Run();
    }
	
    private void Run()
    {
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 12345);
        
        sender.Connect(remoteEP);
        
        GD.Print($"Connected to server {sender.RemoteEndPoint}.");
        
        string message = "Hello, server!";
        byte[] msg = Encoding.ASCII.GetBytes(message);
        int bytesSent = sender.Send(msg);

        byte[] bytes = new byte[1024];
        int bytesRec = sender.Receive(bytes);
        GD.Print($"Received: {sender.RemoteEndPoint}.");

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }
	
    public override void _Process(double delta)
    {
		
    }
	
    public SocketHandler()
    {
	
    }
}