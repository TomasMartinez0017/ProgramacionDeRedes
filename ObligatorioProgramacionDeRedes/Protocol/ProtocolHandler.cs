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
            SendAndDivideData(frame.Data, AmountOfSlotsNeeded(frame));
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

        public Frame Receive()
        {
            Frame frame = new Frame();

            frame.Header = BitConverter.ToInt32(ReceiveDividedData(3, 1));
            frame.Command = BitConverter.ToInt32(ReceiveDividedData(2, 1));
            frame.DataLength = BitConverter.ToInt32(ReceiveDividedData(4, 1));
            frame.Data = ReceiveDividedData(frame.DataLength, AmountOfSlotsNeeded(frame));

            return frame;
        }

        private byte[] ReceiveDividedData(int fileLength, int slots)
        {
            NetworkStream stream = _tcpClient.GetStream();
            byte[] buffer = new byte[fileLength];
            var offset = 0;
            var currentSlot = 1;

            while(offset < fileLength)
            {
                if(currentSlot < slots)
                {
                    byte[] receivedBytes = Read(32768, stream);
                    Array.Copy(receivedBytes,0,buffer,offset,32768);
                    offset += 32768;
                }
                else
                {
                    int dataLeft = fileLength - offset;
                    byte[] receivedBytes = Read(dataLeft, stream);
                    Array.Copy(receivedBytes,0,buffer,offset,dataLeft);
                    offset += dataLeft; 
                }
                currentSlot++;
            }
            return buffer;
        }

        private byte[] Read(int length, NetworkStream stream)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = stream.Read(data, dataReceived, length - dataReceived);
                if (received == 0)
                {
                    throw new SocketException(); 
                }
                dataReceived += received;
            }
            return data;
        }

        private int AmountOfSlotsNeeded(Frame frame)
        {
            int slots = 0;
            
            if (frame.DataLength % 32768 == 0)
            {
                slots = frame.DataLength / 32768;
            }
            else
            {
                slots = (frame.DataLength / 32768) + 1;
            }

            return slots;
        }
    }
}