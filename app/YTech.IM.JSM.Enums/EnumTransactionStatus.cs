using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YTech.IM.JSM.Enums
{
    public enum EnumTransactionStatus
    {
        [StringValue("")]
        None,
        [StringValue("Penjualan")]
        Sales,
        [StringValue("Pembelian")]
        Purchase,
        [StringValue("Order Pembelian")]
        PurchaseOrder,
        [StringValue("Retur Penjualan")]
        ReturSales,
        [StringValue("Retur Pembelian")]
        ReturPurchase,
        [StringValue("Pemakaian Material")]
        Using,
        [StringValue("Mutasi Stok")]
        Mutation,
        [StringValue("Penyesuaian Stok")]
        Adjusment,
        [StringValue("Penerimaan Stok")]
        Received,
        [StringValue("Rencana Anggaran Belanja")]
        Budgeting,
        Service
    }
}
