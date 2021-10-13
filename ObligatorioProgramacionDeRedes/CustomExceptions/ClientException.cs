using System;

namespace CustomExceptions
{
    public class ClientExcpetion : Exception
    {
        public ClientExcpetion(String message) : base(message){ }
    }
}