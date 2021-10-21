using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class ClientProgram
    {
        static async Task Main(string[] args)
        {
            ClientUserInterface userInterface = new ClientUserInterface();
            try
            {
                await userInterface.StartClient();
            }
            catch (SocketException)
            {
                Console.WriteLine("ERROR: Server is offline.");
            }
            
        }
        
    }
}