using Godot;
using System.IO.Pipes;

public partial class PipeHandler : Node
{
    [Export]
    public string PipeName { get; set; } = "godot-python-pipe";
    
    private NamedPipeClientStream pipe;
    private const int ReadBufferSize = 64; 

    public override void _Ready()
    {
        //this.pipe = new NamedPipeClientStream(".", this.PipeName, PipeDirection.InOut);
        //this.pipe.Connect();
        GD.Print($"Connected to {this.PipeName} pipe.");
    }
    
    public byte[] Receive()
    {
        byte[] buffer = new byte[ReadBufferSize];
        int readBytes = this.pipe.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    public void Send(byte[] data)
    {
        this.pipe.Write(data, 0, data.Length);
    }

    public PipeHandler()
    {
        
    }
}