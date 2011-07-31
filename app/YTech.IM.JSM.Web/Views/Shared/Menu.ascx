<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="YTech.IM.JSM.Web.Controllers.Master" %>
<%@ Import Namespace="YTech.IM.JSM.Web.Controllers.Transaction" %>
<%@ Import Namespace="YTech.IM.JSM.Enums" %>
<%@ Import Namespace="YTech.IM.JSM.Web.Controllers.Utility" %>

<div id="accordion">
    <h3>
        <a href="#">Home</a></h3>
     <div class="child-menu-container">
       
            <%=Html.ActionLinkForAreas<HomeController>(c => c.Index(), "Home") %>
    </div>
    <% if (Request.IsAuthenticated)
       {
%>
<% if(Membership.GetUser().UserName.ToLower().Equals("admin")) { %> 
    <h3>
        <a href="#">Data Pokok</a></h3>
    <div class="child-menu-container">
       
            <%= Html.ActionLinkForAreas<WarehouseController>(c => c.Index(),"Master Gudang") %>
       
            <%= Html.ActionLinkForAreas<MItemCatController>(c => c.Index(), "Master Kategori Produk")%>
       
            <%= Html.ActionLinkForAreas<BrandController>(c => c.Index(),"Master Merek") %>
       
            <%= Html.ActionLinkForAreas<ItemController>(c => c.Index(), "Master Produk")%>
            <%= Html.ActionLinkForAreas<SupplierController>(c => c.Index(),"Master Supplier") %>
       
            <%= Html.ActionLinkForAreas<CustomerController>(c => c.Index(),"Master Konsumen") %>
       
            <%= Html.ActionLinkForAreas<DepartmentController>(c => c.Index(),"Master Departemen") %>
       
            <%= Html.ActionLinkForAreas<EmployeeController>(c => c.Index(), "Master Karyawan")%>
       
            <%--<hr />
       
            <%= Html.ActionLinkForAreas<CostCenterController>(c => c.Index(),"Master Cost Center") %>
       
            <%= Html.ActionLinkForAreas<AccountController>(c => c.Index(),"Master Akun") %>--%>
    </div>
    <% } %>
    <h3>
        <a href="#">Inventori</a></h3>
    <div class="child-menu-container">
       
            <%= Html.ActionLinkForAreas<InventoryController>(c => c.Index(), "Order Pembelian")%>
       
            <%= Html.ActionLinkForAreas<InventoryController>(c => c.Received(), "Penerimaan Stok")%>
            <%= Html.ActionLinkForAreas<InventoryController>(c => c.Purchase(), "Pembelian")%>
       
            <%= Html.ActionLinkForAreas<InventoryController>(c => c.ReturPurchase(), "Retur Pembelian")%>
           <hr />
       
            <%= Html.ActionLinkForAreas<InventoryController>(c => c.Sales(), "Penjualan")%>
       
           <%= Html.ActionLinkForAreas<InventoryController>(c => c.ReturSales(), "Retur Penjualan")%>
           <hr />

       
            <%= Html.ActionLinkForAreas<InventoryController>(c => c.Mutation(), "Mutasi Stok")%>
       
            <%= Html.ActionLinkForAreas<InventoryController>(c => c.Adjusment(), "Penyesuaian Stok")%>
       
    </div>
   
<% if(Membership.GetUser().UserName.ToLower().Equals("admin")) { %> 
    <h3>
        <a href="#">Hutang Piutang</a></h3>
     <div class="child-menu-container">
       
        <%--    <%= Html.ActionLinkForAreas<AccountingController>(c => c.GeneralLedger(), "General Ledger")%>
       
            <%= Html.ActionLinkForAreas<AccountingController>(c => c.CashIn(), "Kas Masuk")%>
       
            <%= Html.ActionLinkForAreas<AccountingController>(c => c.CashOut(), "Kas Keluar")%>--%>
            <hr />
            <%= Html.ActionLinkForAreas<PaymentController>(c => c.Index(EnumPaymentType.Hutang), "Pembayaran Hutang")%>
            <%= Html.ActionLinkForAreas<PaymentController>(c => c.Index(EnumPaymentType.Piutang), "Pembayaran Piutang")%>
    </div>
    <% } %>

    <h3>
        <a href="#">Laporan</a></h3>
     <div class="child-menu-container">
       
            <%= Html.ActionLinkForAreas<ReportController>(c => c.Report(EnumReports.RptBrand), "Daftar Merek")%>
       
            <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptTransDetail, EnumTransactionStatus.PurchaseOrder), "Lap. Detail Order Pembelian")%>
            <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptTransDetail, EnumTransactionStatus.Purchase), "Lap. Detail Pembelian")%> 
            <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptTransDetail, EnumTransactionStatus.ReturPurchase), "Lap. Detail Retur Pembelian")%>
            <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptTransDetail, EnumTransactionStatus.Sales), "Lap. Detail Penjualan")%>
            <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptTransDetail, EnumTransactionStatus.ReturSales), "Lap. Detail Retur Penjualan")%>        
           <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptTransDetail, EnumTransactionStatus.Mutation), "Lap. Detail Mutasi Stok")%>   
            <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptTransDetail, EnumTransactionStatus.Adjusment), "Lap. Detail Penyesuaian")%>     
             <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptTransDetailByTransBy, EnumTransactionStatus.Purchase), "Lap. Rekap Pembelian Per Barang")%> 
             <%= Html.ActionLinkForAreas<ReportController>(c => c.ReportTrans(EnumReports.RptLRDetailSales, EnumTransactionStatus.Sales), "Lap. Detail Laba Kotor")%> 

            <%= Html.ActionLinkForAreas<ReportController>(c => c.Report(EnumReports.RptStockCard), "Kartu Stok")%>
       
            <%= Html.ActionLinkForAreas<ReportController>(c => c.Report(EnumReports.RptStockItem), "Laporan Stok Per Gudang")%>
     
     
<% if(Membership.GetUser().UserName.ToLower().Equals("admin")) { %> 
       
          <%--  <hr />
       
            <%= Html.ActionLinkForAreas<ReportController>(c => c.Report(EnumReports.RptJournal), "Lap. Jurnal")%>
       
            <%= Html.ActionLinkForAreas<ReportController>(c => c.Report(EnumReports.RptNeraca), "Lap. Neraca")%>
       
            <%= Html.ActionLinkForAreas<ReportController>(c => c.Report(EnumReports.RptLR), "Lap. Laba / Rugi")%>--%>
        <% } %>
    </div>
  
  
    <h3>
        <a href="#">Utiliti</a></h3>
 <div class="child-menu-container">
   
     <%--  
            <%= Html.ActionLinkForAreas<ShiftController>(c => c.Closing(), "Tutup Shift")%>
        </div>--%>
        
<% if(Membership.GetUser().UserName.ToLower().Equals("admin")) { %> 
       <%--
         <%= Html.ActionLinkForAreas<InventoryController>(c => c.ListBilling(), "Daftar Billing")%>--%>

            <%= Html.ActionLinkForAreas<UserAdministrationController>(c => c.Index(null), "Daftar Pengguna")%>
       
            Ganti Password
       
            Backup Database
    <% } %>

    <%
        }
%>
</div>

</div>