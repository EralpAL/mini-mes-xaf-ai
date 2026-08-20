using DevExpress.ExpressApp;
using Microsoft.Extensions.DependencyInjection;
using MiniMes.Blazor.Server.Services;

namespace MiniMes.Blazor.Server.Controllers
{
    // The chat panel is a general MiniMes assistant, so it stays available on every page.
    // The panel is never hidden here: the user closes it with the collapse button in the panel.
    public class MesAiChatPanelController : WindowController
    {
        public MesAiChatPanelController()
        {
            TargetWindowType = WindowType.Main;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            if (Application == null || Application.ServiceProvider == null)
            {
                return;
            }

            MesAiChatPanelState chatPanelState = Application.ServiceProvider.GetService<MesAiChatPanelState>();

            if (chatPanelState == null)
            {
                return;
            }

            // Show is called only when the panel is unavailable, so a panel the user collapsed
            // stays collapsed while navigating between pages.
            if (!chatPanelState.IsAvailable)
            {
                chatPanelState.Show();
            }
        }
    }
}
