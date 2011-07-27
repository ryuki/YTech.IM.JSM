using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Web.Controllers.ViewModel.Reports
{
    public class TransViewModel : TTrans
    {
        public string CustomerName { get; set; }
    }
}
