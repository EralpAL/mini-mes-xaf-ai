using DevExpress.ExpressApp;
using MiniMes.AI.Models;
using MiniMes.Module.BusinessObjects;

namespace MiniMes.Module.Services
{
    public class XafAiAnalysisLogService
    {
        private readonly IObjectSpaceFactory _objectSpaceFactory;

        public XafAiAnalysisLogService(IObjectSpaceFactory objectSpaceFactory)
        {
            _objectSpaceFactory = objectSpaceFactory;
        }

        public void Save(string userMessage, AssistantResponse response)
        {
            if (response == null || !response.IsSuccessful || string.IsNullOrWhiteSpace(userMessage))
            {
                return;
            }

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<AiAnalysisRecord>())
            {
                AiAnalysisRecord analysisRecord = objectSpace.CreateObject<AiAnalysisRecord>();
                analysisRecord.AnalysisDate = DateTime.Now;
                analysisRecord.ModelName = "qwen3:1.7b";
                analysisRecord.UserQuestion = userMessage;
                analysisRecord.AnalysisText = response.Message ?? string.Empty;
                objectSpace.CommitChanges();
            }
        }
    }
}
