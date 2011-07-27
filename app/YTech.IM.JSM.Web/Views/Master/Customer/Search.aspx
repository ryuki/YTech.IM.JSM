<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPopup.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
   <%
    if (false)
    {%>
<script src="../../../Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script>
<%
    }%>
    <div>  
    <label for="ddlSearchBy">Cari berdasar :</label>  
        <select id="ddlSearchBy">
            <option value="cust.Id">Kode Konsumen</option>
            <option value="per.PersonFirstName">Nama</option> 
            <option value="per.PersonMobile">HP</option>
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
    <div id="dialog" title="Status">
        <p>
        </p>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {

            $("#dialog").dialog({
                autoOpen: false
            });

            $.jgrid.nav.addtext = "Tambah";
            $.jgrid.nav.edittext = "Edit";
            $.jgrid.nav.deltext = "Hapus";
            $.jgrid.edit.addCaption = "Tambah Konsumen Baru";
            $.jgrid.edit.editCaption = "Edit Konsumen";
            $.jgrid.del.caption = "Hapus Konsumen";
            $.jgrid.del.msg = "Anda yakin menghapus Konsumen yang dipilih?";
            $("#list").jqGrid({
                url: '<%= Url.Action("ListSearch", "Customer") %>',
                postData: {
                    searchBy: function () { return $('#ddlSearchBy option:selected').val(); },
                    searchText: function () { return $('#txtSearch').val(); }
                },
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Konsumen',
                            'Nama',
                            'Jenis Kelamin',
                            'Keterangan'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                   { name: 'PersonName', index: 'PersonName', width: 200, align: 'left', editable: false, edittype: 'text', editrules: { required: false, edithidden: true} },
                    { name: 'PersonGender', index: 'PersonGender', width: 200, sortable: false, align: 'left', editable: true, edittype: 'select',   editrules: { required: false} },
                 { name: 'CustomerDesc', index: 'CustomerDesc', width: 200, hidden: false, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false, edithidden: true} }
                   ],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 250,
                caption: 'Daftar Konsumen',
                autowidth: true,
                loadComplete: function () {
                    
                },
                ondblClickRow: function (rowid, iRow, iCol, e) {
                    var list = $("#list");
                    var rowData = list.getRowData(rowid);
                    window.parent.SetCustomerDetail(rowData["Id"], rowData["PersonName"], rowData["CustomerServiceDisc"]);
                    return false;
                }
            }).navGrid('#listPager',
                {
                    edit: false, add: false, del: false, search: false, refresh: true
                } 
            ); 

              $('#btnSearch').click(function () {
                $("#list").jqGrid().setGridParam().trigger("reloadGrid");

            });
        });       
    </script>
</asp:Content>
