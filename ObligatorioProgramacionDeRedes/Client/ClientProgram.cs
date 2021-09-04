using System;
using System.Threading;

namespace Client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            ClientUserInterface UI = new ClientUserInterface();
            Thread startClientThread = new Thread(() => UI.StartClient());
            startClientThread.Start();
        }
    }
}