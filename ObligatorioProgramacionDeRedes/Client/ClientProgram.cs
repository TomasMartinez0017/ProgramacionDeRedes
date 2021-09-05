using System;
using System.Threading;

namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            ClientUserInterface userInterface = new ClientUserInterface();
            userInterface.StartClient();
        }
        
    }
}