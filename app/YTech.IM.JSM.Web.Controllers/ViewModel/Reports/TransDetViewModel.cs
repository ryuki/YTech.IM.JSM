using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YTech.IM.JSM.Core.Transaction;

using YTech.IM.JSM.Core.Transaction.Inventory;

namespace YTech.IM.JSM.Web.Controllers.ViewModel.Reports
{
    public class TransDetViewModel : TTransDet
    {
        public string ItemName { get; set; }
        public decimal TotalUsed { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string PacketName { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? TransDate { get; set; }
        public string TransFactur { get; set; }
    }
}
