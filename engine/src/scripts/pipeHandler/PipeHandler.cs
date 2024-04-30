﻿using System;
using Godot;
using System.IO.Pipes;

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
    
    private string pipeName = "godot-python-pipe";
    private NamedPipeClientStream pipe;
    private const int ReadBufferSize = 64;
    private bool IsConnected { get; set; } = false;
    
    public void Connect()
    {
        this.pipe = new NamedPipeClientStream(".", this.pipeName, PipeDirection.InOut);
        this.pipe.Connect();
        this.IsConnected = true;
        GD.Print($"Connected to {this.pipeName} pipe.");
    }
    
    public void Disconnect()
    {
        this.pipe.Close();
        this.IsConnected = false;
        GD.Print($"Disconnected from {this.pipeName} pipe.");
    }
    
    public void Send(byte[] data) => this.pipe.Write(data, 0, data.Length);

    public byte[] Receive()
    {
        byte[] buffer = new byte[ReadBufferSize];
        int readBytes = this.pipe.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    private PipeHandler()
    {
        
    }
}