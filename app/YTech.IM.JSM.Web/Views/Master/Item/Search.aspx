<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPopup.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="YTech.IM.JSM.Enums" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        if (false)
        {%>
    <script src="../../../Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script>
    <%
        }%>
    <div>
        <label for="ddlSearchBy">
            Cari berdasar :</label>
        <select id="ddlSearchBy">
            <option value="0">Kode Produk</option>
            <option value="1">Nama</option>
        </select>
        <input id="txtSearch" type="text" />
        <input id="btnSearch" type="button" value="Cari" />
    </div>
    <table id="list" class="scroll" cellpadding="0" cellspacing="0">
    </table>
    <div id="listPager" class="scroll" style="text-align: center;">
    </div>
    <div id="listPsetcols" class="scroll" style="text-align: center;">
    </div>
    <script type="text/javascript">

        $(document).ready(function () {
            $.jgrid.nav.addtext = "Tambah";
            $.jgrid.nav.edittext = "Edit";
            $.jgrid.nav.deltext = "Hapus";
            $.jgrid.edit.addCaption = "Tambah Produk Baru";
            $.jgrid.edit.editCaption = "Edit Produk";
            $.jgrid.del.caption = "Hapus Produk";
            $.jgrid.del.msg = "Anda yakin menghapus Produk yang dipilih?";
            $("#list").jqGrid({
                url: '<%=Url.Action("ListSearch", "Item")%>',
                  postData: {
                    itemCatId: function () { return '<%= Request.QueryString["itemCatId"] %>'; },
                     customerId: function () { return '<%= Request.QueryString["customerId"] %>'; }
                },
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Produk', 'Nama', 'Kategori Produk', 'Merek', 'Satuan', 'Harga Beli', 'Harga Jual', 'Keterangan'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'ItemName', index: 'ItemName', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true }, formoptions: { elmsuffix: ' *'} },
                 { name: 'ItemCatName', index: 'ItemCatName', width: 200, align: 'left', editable: false, edittype: 'select', editrules: { edithidden: true} },
                    { name: 'BrandName', index: 'BrandName', width: 200, align: 'left', editable: false, edittype: 'select', editrules: { edithidden: true} },
                    { name: 'ItemUomName', index: 'ItemUomName', width: 200, editable: true, editrules: { edithidden: true} },
                    
                
                 { name: 'ItemUomPurchasePrice', index: 'ItemUomPurchasePrice', width: 200, editable: true, editrules: { edithidden: true  }
                 <% if(Request.QueryString["Price"].Equals(EnumPrice.None.ToString()) || Request.QueryString["Price"].Equals(EnumPrice.Sale.ToString())) { %>
                 ,hidden:true
                  <%}%>
                   },
                
                  { name: 'ItemUomSalePrice', index: 'ItemUomSalePrice', width: 200, editable: true, editrules: { edithidden: true}
                   <% if(Request.QueryString["Price"].Equals(EnumPrice.None.ToString()) || Request.QueryString["Price"].Equals(EnumPrice.Purchase.ToString())) { %>
                 ,hidden:true
                  <%}%>
                   }, 
                   { name: 'ItemDesc', index: 'ItemDesc', width: 200, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false}}],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 250,
                caption: 'Daftar Produk',
                autowidth: true,
                loadComplete: function () {
                },
                ondblClickRow: function (rowid, iRow, iCol, e) {
                    var list = $("#list");
                    var rowData = list.getRowData(rowid);
//                    alert(rowid);
//                    alert(rowData["Id"]);
//                    alert(rowData["ItemName"]);
//                    alert(rowData["ItemUomPurchasePrice"]);
                    //                    alert(window.parent.location);
<% if(Request.QueryString["Price"].Equals(EnumPrice.Sale.ToString())) { %>
var price = rowData["ItemUomSalePrice"];
<%
    } else if(Request.QueryString["Price"].Equals(EnumPrice.Purchase.ToString()))
   {
%>
var price = rowData["ItemUomPurchasePrice"];
<%
   }
   else
    {
%>
var price = 0;
   <%
    }%>
    <% if (!string.IsNullOrEmpty(Request.QueryString["src"])) {	%>
                      window.parent.SetItemDetail('<%= Request.QueryString["src"] %>',rowData["Id"], rowData["ItemName"], price);
  <%} else {%>
   window.parent.SetItemDetail(rowData["Id"], rowData["ItemName"], price);
  <%}%>
                    return false;
                }
            }).navGrid('#listPager',
                {
                    edit: false, add: false, del: false, search: false, refresh: true
                }
            );

            $('#btnSearch').click(function () {
                var newurl = '<%= Url.Action("ListSearch", "Item") %>';                
//                var itemCatId = '<%= Request.QueryString["itemCatId"] %>';
//                newurl += '?itemCatId='+itemCatId;
                var searchby = $("#ddlSearchBy option:selected").val();
                if (searchby == "0") {
                    newurl += '?itemId=';
                }
                else if (searchby == "1") {
                     newurl += '?itemName=';
                }
                newurl += $("#txtSearch").val();
                //                alert(newurl);
                $("#list").jqGrid().setGridParam({ url: newurl }).trigger("reloadGrid");

            });

        });
    </script>
</asp:Content>
