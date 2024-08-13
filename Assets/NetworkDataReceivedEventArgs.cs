using System;

public class NetworkDataReceivedEventArgs : EventArgs
{
    public DateTime TimeReceived { get; set; }
    public String Message { get; set; }
}