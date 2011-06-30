using System.Web.Mvc;


namespace YTech.IM.JSM.Web.Controllers.Utility
{
	public class UtilityAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
                return "Utility";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
                "Utility_default",
                "Utility/{controller}/{action}/{id}",
                new { area = "Utility", action = "Index", id = UrlParameter.Optional },
                new[] { typeof(UserAccountController).Namespace }
			);
		}
	}
}
