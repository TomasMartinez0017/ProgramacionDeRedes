namespace Protocol
{
    public class ResponseHandler
    {
        public string ProcessResponse(Frame frame)
        {
            return null;
        }

        public Frame GetResponse(Frame frame)
        {
            Frame response = null;

            switch ((Command) frame.Command)
            {
                case Command.PublishGame:
                    response = CreatePublishGameResponse(frame);
                    break;
            }

            return response;
        }

        private Frame CreatePublishGameResponse(Frame frame)
        {
            return null;
        }
    }
}