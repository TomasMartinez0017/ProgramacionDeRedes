using System;
using System.Net.Sockets;

namespace Protocol
{
    public class ProtocolHandler
    {
        private TcpClient _tcpClient;

        public ProtocolHandler(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public void Send(Frame frame)
        {
            SendAndDivideData(BitConverter.GetBytes(frame.Header), 1);
            SendAndDivideData(BitConverter.GetBytes(frame.Command), 1);
            SendAndDivideData(BitConverter.GetBytes(frame.DataLength), 1);

            int slots = 0;
            
            if (frame.DataLength % 32768 == 0)
            {
                slots = frame.DataLength / 32768;
            }
            else
            {
                slots = (frame.DataLength / 32768) + 1;
            }
            
            SendAndDivideData(frame.Data, slots);
        }
        
        private void SendAndDivideData(byte[] data, int slots)
        {
            NetworkStream stream = _tcpClient.GetStream();
            int offset = 0;
            int currentSlot = 1;
            
            while(offset < data.Length)
            {
                if(currentSlot < slots)
                {
                    var dataToSend = new byte[32768];
                    Array.Copy(data, offset, dataToSend, 0, 32768);
                    stream.Write(dataToSend);
                    offset += 32768;
                }
                else
                {
                    int bytesSent = 0;
                    int dataLeftSize = data.Length - offset;
                    byte[] dataToSend = new byte[dataLeftSize];
                    Array.Copy(data, offset, dataToSend, 0, dataLeftSize);
                    stream.Write(dataToSend);
                    offset += dataLeftSize;
                }
                currentSlot++;
            }
        }
    }
}