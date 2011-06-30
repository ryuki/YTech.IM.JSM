using System.Collections.Generic;
using System.Web.Security;

namespace YTech.IM.JSM.Web.Controllers.ViewModel.UserAdministration
{
	public class RoleViewModel
	{
		public string Role { get; set; }
		public IEnumerable<MembershipUser> Users { get; set; }
	}
}