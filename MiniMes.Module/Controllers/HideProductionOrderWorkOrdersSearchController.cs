using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace MiniMes.Module.Controllers
{
    public class HideProductionOrderWorkOrdersSearchController : ViewController<ListView>
    {
        public HideProductionOrderWorkOrdersSearchController()
        {
            TargetViewId = "ProductionOrder_WorkOrders_ListView";
            TargetViewNesting = Nesting.Nested;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            FilterController filterController = Frame.GetController<FilterController>();

            if (filterController != null && filterController.FullTextFilterAction != null)
            {
                filterController.FullTextFilterAction.Active["HideProductionOrderWorkOrdersSearch"] = false;
            }
        }

        protected override void OnDeactivated()
        {
            FilterController filterController = Frame.GetController<FilterController>();

            if (filterController != null && filterController.FullTextFilterAction != null)
            {
                filterController.FullTextFilterAction.Active.RemoveItem("HideProductionOrderWorkOrdersSearch");
            }

            base.OnDeactivated();
        }
    }
}
