namespace CustomExceptions
{
    public static class MessageException
    {
        public static string InvalidGameException = "ERROR: Remember to complete all fields.";
        public static string InvalidReviewException = "ERROR: Remember to complete all fields. Also score must be numeric.";
        public static string ClientDisconnectedException = "Client has disconnected.";
    }
}