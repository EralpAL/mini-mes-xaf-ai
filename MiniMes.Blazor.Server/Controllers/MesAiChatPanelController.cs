using DevExpress.ExpressApp;
using Microsoft.Extensions.DependencyInjection;
using MiniMes.Blazor.Server.Services;

namespace MiniMes.Blazor.Server.Controllers
{
    public class MesAiChatPanelController : WindowController
    {
        public MesAiChatPanelController()
        {
            TargetWindowType = WindowType.Main;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            MesAiChatPanelState chatPanelState = Application.ServiceProvider.GetService<MesAiChatPanelState>();

            if (chatPanelState != null)
            {
                chatPanelState.Show();
            }
        }

        protected override void OnDeactivated()
        {
            MesAiChatPanelState chatPanelState = Application.ServiceProvider.GetService<MesAiChatPanelState>();

            if (chatPanelState != null)
            {
                chatPanelState.Hide();
            }

            base.OnDeactivated();
        }
    }
}
