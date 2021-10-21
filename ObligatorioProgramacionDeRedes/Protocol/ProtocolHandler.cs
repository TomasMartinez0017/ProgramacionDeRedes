using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using CustomExceptions;

namespace Protocol
{
    public class ProtocolHandler
    {
        private TcpClient _tcpClient;

        public ProtocolHandler(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public async Task SendAsync(Frame frame)
        {
            await SendAndDivideDataAsync(BitConverter.GetBytes(frame.Header), 1);
            await SendAndDivideDataAsync(BitConverter.GetBytes(frame.Command), 1);
            await SendAndDivideDataAsync(BitConverter.GetBytes(frame.DataLength), 1);

            if ((Header) frame.Header == Header.Response)
            {
                await SendAndDivideDataAsync(BitConverter.GetBytes(frame.Status), 1);
            }
            
            await SendAndDivideDataAsync(frame.Data, AmountOfSlotsNeeded(frame));
        }
        
        private async Task SendAndDivideDataAsync(byte[] data, int slots)
        {
            NetworkStream stream = _tcpClient.GetStream();
            int offset = 0;
            int currentSlot = 1;
            
            while(offset < data.Length)
            {
                if(currentSlot < slots)
                {
                    var dataToSend = new byte[FrameConstants.MaxPacketSize];
                    Array.Copy(data, offset, dataToSend, 0, FrameConstants.MaxPacketSize);
                    await stream.WriteAsync(dataToSend);
                    offset += FrameConstants.MaxPacketSize;
                }
                else
                {
                    int dataLeftSize = data.Length - offset;
                    byte[] dataToSend = new byte[dataLeftSize];
                    Array.Copy(data, offset, dataToSend, 0, dataLeftSize);
                    await stream.WriteAsync(dataToSend);
                    offset += dataLeftSize;
                }
                currentSlot++;
            }
        }

        public async Task<Frame> ReceiveAsync()
        {
            Frame frame = new Frame();

            frame.Header = BitConverter.ToInt32(await ReceiveDividedDataAsync(FrameConstants.HeaderLength, 1));
            frame.Command = BitConverter.ToInt32(await ReceiveDividedDataAsync(FrameConstants.CommandLength, 1));
            frame.DataLength = BitConverter.ToInt32(await ReceiveDividedDataAsync(FrameConstants.DataLength, 1));
            
            if ((Header) frame.Header == Header.Response)
            {
                frame.Status = BitConverter.ToInt32(await ReceiveDividedDataAsync(FrameConstants.StatusLength, 1));
            }
            
            frame.Data = await ReceiveDividedDataAsync(frame.DataLength, AmountOfSlotsNeeded(frame));

            return frame;
        }

        private async Task <byte[]> ReceiveDividedDataAsync(int fileLength, int slots)
        {
            NetworkStream stream = _tcpClient.GetStream();
            byte[] buffer = new byte[fileLength];
            var offset = 0;
            var currentSlot = 1;

            while(offset < fileLength)
            {
                if(currentSlot < slots)
                {
                    byte[] receivedBytes = await ReadAsync(FrameConstants.MaxPacketSize, stream);
                    Array.Copy(receivedBytes,0,buffer,offset,FrameConstants.MaxPacketSize);
                    offset += FrameConstants.MaxPacketSize;
                }
                else
                {
                    int dataLeft = fileLength - offset;
                    byte[] receivedBytes = await ReadAsync(dataLeft, stream);
                    Array.Copy(receivedBytes,0,buffer,offset,dataLeft);
                    offset += dataLeft; 
                }
                currentSlot++;
            }
            return buffer;
        }

        private async Task<byte[]> ReadAsync(int length, NetworkStream stream)
        {
            int dataReceived = 0;
            var data = new byte[length];
            while (dataReceived < length)
            {
                var received = await stream.ReadAsync(data, dataReceived, length - dataReceived);
                if (received == 0)
                {
                    throw new ClientExcpetion(MessageException.ClientDisconnectedException); 
                }
                dataReceived += received;
            }
            return data;
        }

        private int AmountOfSlotsNeeded(Frame frame)
        {
            int slots = 0;
            
            if (frame.DataLength % FrameConstants.MaxPacketSize == 0)
            {
                slots = frame.DataLength / FrameConstants.MaxPacketSize;
            }
            else
            {
                slots = (frame.DataLength / FrameConstants.MaxPacketSize) + 1;
            }

            return slots;
        }
    }
}