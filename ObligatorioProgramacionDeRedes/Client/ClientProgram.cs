using System;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            ClientUserInterface userInterface = new ClientUserInterface();
            try
            {
                userInterface.StartClient();
            }
            catch (SocketException)
            {
                Console.WriteLine("ERROR: Server is offline.");
            }
            
        }
        
    }
}