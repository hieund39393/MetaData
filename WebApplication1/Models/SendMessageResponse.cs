namespace WebApplication1.Models
{
    public class SendMessageResponse
    {
        public bool Ok { get; set; }
        public SendMessageResult result { get; set; }
    }

    public class SendMessageResult
    {
        public int message_id { get; set; }
    }
}
