using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Microsoft.Extensions.DependencyInjection;
using MiniMes.AI.Interfaces;
using MiniMes.AI.Models;
using MiniMes.Module.BusinessObjects;

namespace MiniMes.Module.Controllers
{
    public class AiAssistantController : ViewController
    {
        private ParametrizedAction askMesAiAction;

        public AiAssistantController()
        {
            TargetObjectType = typeof(ProductionOrder);
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Root;
            Active["HideAskMesAiToolbar"] = false;

            askMesAiAction = new ParametrizedAction(this, "AskMesAi", PredefinedCategory.View, typeof(string));
            askMesAiAction.Caption = "AI Asistana Sor";
            askMesAiAction.ShortCaption = "Sor";
            askMesAiAction.ImageName = "BO_Message";
            askMesAiAction.SelectionDependencyType = SelectionDependencyType.Independent;
            askMesAiAction.Active["HideOnProductionOrderToolbar"] = false;
            askMesAiAction.Execute += AskMesAiAction_Execute;
        }

        private async void AskMesAiAction_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            string question = e.ParameterCurrentValue as string;

            if (string.IsNullOrWhiteSpace(question))
            {
                MessageOptions warningOptions = new MessageOptions();
                warningOptions.Duration = 10000;
                warningOptions.Message = "Lütfen bir MES sorusu yazın.";
                warningOptions.Type = InformationType.Warning;
                Application.ShowViewStrategy.ShowMessage(warningOptions);
                return;
            }

            try
            {
                IMesAiAssistantService assistantService = Application.ServiceProvider.GetRequiredService<IMesAiAssistantService>();

                AssistantRequest request = new AssistantRequest();
                request.Message = question;

                AssistantResponse response = await assistantService.SendAsync(request);

                if (response.IsSuccessful)
                {
                    MessageOptions successOptions = new MessageOptions();
                    successOptions.Duration = 10000;
                    successOptions.Message = response.Message;
                    successOptions.Type = InformationType.Success;
                    Application.ShowViewStrategy.ShowMessage(successOptions);
                }
                else
                {
                    MessageOptions errorOptions = new MessageOptions();
                    errorOptions.Duration = 10000;
                    errorOptions.Message = response.ErrorMessage;
                    errorOptions.Type = InformationType.Error;
                    Application.ShowViewStrategy.ShowMessage(errorOptions);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                MessageOptions exceptionOptions = new MessageOptions();
                exceptionOptions.Duration = 10000;
                exceptionOptions.Message = exception.Message;
                exceptionOptions.Type = InformationType.Error;
                Application.ShowViewStrategy.ShowMessage(exceptionOptions);
            }
        }
    }
}
