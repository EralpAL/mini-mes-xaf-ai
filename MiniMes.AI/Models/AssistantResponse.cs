namespace MiniMes.AI.Models
{
    public class AssistantResponse
    {
        private string message = string.Empty;
        private bool isSuccessful;
        private string? usedToolName;
        private string? errorMessage;

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

        public bool IsSuccessful
        {
            get
            {
                return isSuccessful;
            }
            set
            {
                isSuccessful = value;
            }
        }

        public string? UsedToolName
        {
            get
            {
                return usedToolName;
            }
            set
            {
                usedToolName = value;
            }
        }

        public string? ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
            }
        }
    }
}
