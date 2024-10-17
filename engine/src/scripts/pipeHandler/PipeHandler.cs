using System;
using Godot;
using System.IO.Pipes;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class PipeHandler : Singleton<PipeHandler>
{
	public string PipeName
	{
		get => this.pipeName;
		set
		{
			if (this.IsConnected) throw new Exception("Already connected! Disconnect first to change pipe name.");
			this.pipeName = value;
		}
	}
	
	private string pipeName = Config.Get().Data.Pipe.Name;
	private NamedPipeClientStream pipe;
	private readonly int ReadBufferSize = Config.Get().Data.Pipe.BufferSize;
	private bool IsConnected { get; set; } = false;

	public static void LogTimeToFile(string filename, double elapsedTime)
	{
		using (StreamWriter writer = new StreamWriter(filename, true))
		{
			writer.WriteLine($"{elapsedTime:F4}");
		}
	}
	
	public void Connect()
	{
		this.pipe = new NamedPipeClientStream(".", this.pipeName, PipeDirection.InOut);
		this.pipe.Connect();
		this.IsConnected = true;
		GD.Print($"Connected to '{this.pipeName}' pipe.");
	}
	
	public void Disconnect()
	{
		this.pipe.Close();
		this.IsConnected = false;
		GD.Print($"Disconnected from '{this.pipeName}' pipe.");
	}
	
	public void Send(byte[] data) {
		Stopwatch stopwatch = Stopwatch.StartNew();
		this.pipe.Write(data, 0, data.Length);
		stopwatch.Stop();
		double elapsedTime = stopwatch.Elapsed.TotalSeconds;
		LogTimeToFile("C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\godot_pipe_send.txt", elapsedTime);
	}

	public byte[] Receive()
	{
		byte[] buffer = new byte[ReadBufferSize];
		Stopwatch stopwatch = Stopwatch.StartNew();
		int readBytes = this.pipe.Read(buffer, 0, buffer.Length);
		stopwatch.Stop();
		double elapsedTime = stopwatch.Elapsed.TotalSeconds;
		LogTimeToFile("C:\\Users\\sokol\\OneDrive\\Pulpit\\INZYNIERKA\\inzynierka-mini-projekt\\godot_pipe_recv.txt", elapsedTime);
		return buffer;
	}

	private PipeHandler()
	{
		
	}
}
