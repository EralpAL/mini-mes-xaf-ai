using System;
using System.Collections.Generic;

namespace MiniMes.Blazor.Server.Services
{
    public class MesAiChatLine
    {
        public bool IsUser { get; set; }

        public string Text { get; set; } = string.Empty;
    }

    public class MesAiChatPanelState
    {
        public bool IsAvailable { get; private set; }

        public bool IsExpanded { get; private set; }

        public bool IsBusy { get; set; }

        public string Draft { get; set; }

        public List<MesAiChatLine> Messages { get; }

        public event Action Changed;

        public MesAiChatPanelState()
        {
            Messages = new List<MesAiChatLine>();
            Draft = string.Empty;
        }

        public void Show()
        {
            IsAvailable = true;
            IsExpanded = true;
            RaiseChanged();
        }

        public void Hide()
        {
            IsAvailable = false;
            IsExpanded = false;
            RaiseChanged();
        }

        public void Collapse()
        {
            IsExpanded = false;
            RaiseChanged();
        }

        public void Expand()
        {
            IsExpanded = true;
            RaiseChanged();
        }

        public void AddMessage(bool isUser, string text)
        {
            MesAiChatLine line = new MesAiChatLine();
            line.IsUser = isUser;
            line.Text = text ?? string.Empty;
            Messages.Add(line);
            RaiseChanged();
        }

        public void RaiseChanged()
        {
            if (Changed != null)
            {
                Changed();
            }
        }
    }
}
