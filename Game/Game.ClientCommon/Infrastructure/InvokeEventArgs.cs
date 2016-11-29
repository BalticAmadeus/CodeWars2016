using System;

namespace Game.ClientCommon.Infrastructure
{
    public class InvokeEventArgs : EventArgs
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public long OperationTime { get; set; }
    }
}