namespace YTech.IM.JSM.Web.Controllers.Transaction
{
    public class TransactionAreaRegistration : System.Web.Mvc.AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Transaction";
            }
        }

        public override void RegisterArea(System.Web.Mvc.AreaRegistrationContext context)
        {
            context.MapRoute(
                    "Transaction_default",
                    "Transaction/{controller}/{action}/{id}",
                    new { action = "Index", id = "" }
                );
         }
    }
}
