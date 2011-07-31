using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Web.Controllers.ViewModel
{
    public class TransDetWithRef
    {
        public TTransDet TransDet { get; internal set; }
        public decimal? StockPrice { get; internal set; }
        public decimal? StockRefQty { get; internal set; }
    }
}
