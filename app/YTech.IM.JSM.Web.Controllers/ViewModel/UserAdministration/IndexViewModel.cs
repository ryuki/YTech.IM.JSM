using System.Collections.Generic;
using System.Web.Security;
using PagedList;

namespace YTech.IM.JSM.Web.Controllers.ViewModel.UserAdministration
{
	public class IndexViewModel
	{
		public IPagedList<MembershipUser> Users { get; set; }
		public IEnumerable<string> Roles { get; set; }
	}
}