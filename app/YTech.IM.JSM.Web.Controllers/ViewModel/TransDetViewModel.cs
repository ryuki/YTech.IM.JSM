using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YTech.IM.JSM.Core.Transaction;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Web.Controllers.ViewModel
{
    public class TransDetViewModel
    {
        public TTransDet TransDet { get; internal set; }
        public bool IsNew { get; internal set; }
    }
}
