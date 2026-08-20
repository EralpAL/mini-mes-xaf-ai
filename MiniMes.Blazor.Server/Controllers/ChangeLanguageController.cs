using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.Persistent.Base;
using MiniMes.Module.BusinessObjects;
using System.Globalization;

namespace MiniMes.Blazor.Server.Controllers;

// Switches the application language between Turkish and English with a single action.
// SetCultureAsync writes the selected culture into the XAF culture cookie and reloads the page,
// so the next request is rendered with the localized model of that language.
public class ChangeLanguageController : ViewController
{
    private const string TurkishCulture = "tr-TR";
    private const string EnglishCulture = "en-US";

    private SimpleAction changeLanguageAction;

    public ChangeLanguageController()
    {
        changeLanguageAction = new SimpleAction(this, "ChangeLanguage", PredefinedCategory.Edit);
        changeLanguageAction.Caption = "Türkçe / English";
        changeLanguageAction.ToolTip = "Switch the application language between Turkish and English.";
        changeLanguageAction.Execute += ChangeLanguageAction_Execute;
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        UpdateChangeLanguageVisibility();
    }

    protected override void OnViewControlsCreated()
    {
        base.OnViewControlsCreated();
        UpdateChangeLanguageVisibility();
    }

    protected override void OnDeactivated()
    {
        changeLanguageAction.Active.RemoveItem("HideOnSelectedViews");
        base.OnDeactivated();
    }

    private void UpdateChangeLanguageVisibility()
    {
        bool hideLanguageAction = View != null &&
            View.ObjectTypeInfo != null &&
            (typeof(ProductionOrder).IsAssignableFrom(View.ObjectTypeInfo.Type) ||
             typeof(AiAnalysisRecord).IsAssignableFrom(View.ObjectTypeInfo.Type));

        changeLanguageAction.Active["HideOnSelectedViews"] = !hideLanguageAction;
    }

    private async void ChangeLanguageAction_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        string nextCulture = GetNextCulture();
        IXafCultureInfoService cultureInfoService = GetCultureInfoService();

        if (cultureInfoService == null)
        {
            throw new UserFriendlyException("The language service is not available.");
        }

        await cultureInfoService.SetCultureAsync(nextCulture);
    }

    private string GetNextCulture()
    {
        string currentCulture = CultureInfo.CurrentUICulture.Name;

        if (currentCulture == TurkishCulture)
            return EnglishCulture;

        return TurkishCulture;
    }

    private IXafCultureInfoService GetCultureInfoService()
    {
        BlazorApplication blazorApplication = (BlazorApplication)Application;
        return (IXafCultureInfoService)blazorApplication.ServiceProvider.GetService(typeof(IXafCultureInfoService));
    }
}
