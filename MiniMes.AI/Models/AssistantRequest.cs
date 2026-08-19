namespace MiniMes.AI.Models
{
    public class AssistantRequest
    {
        private string message = string.Empty;
        private string? conversationId;

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }

        // boþ null olabilir
        public string? ConversationId
        {
            get
            {
                return conversationId;
            }
            set
            {
                conversationId = value;
            }
        }
    }
}
