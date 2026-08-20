namespace MiniMes.AI.Models
{
    public class AssistantRequest
    {
        private string message = string.Empty;
        private string? conversationId;
        private string? productionOrderCode;

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

        public string? ProductionOrderCode
        {
            get
            {
                return productionOrderCode;
            }
            set
            {
                productionOrderCode = value;
            }
        }
    }
}
