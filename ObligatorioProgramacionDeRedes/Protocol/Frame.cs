using System;

namespace Protocol
{
    public class Frame
    {
        public int Header { get; set; }
        public int Command { get; set; }
        public int Status { get; set; }
        public int DataLength { get; set; }
        public byte[] Data { get; set; }

        public Frame()
        {
            DataLength = 0;
            Data = new byte[0];
        }

        public void CreateFrame(int header, int command)
        {
            this.Header = header;
            this.Command = command;
            this.Status = (int) FrameStatus.Ok;
        }
    }
}