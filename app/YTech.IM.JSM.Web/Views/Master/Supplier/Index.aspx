<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MyMaster.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <table id="list" class="scroll" cellpadding="0" cellspacing="0">
    </table>
    <div id="listPager" class="scroll" style="text-align: center;">
    </div>
    <div id="listPsetcols" class="scroll" style="text-align: center;">
    </div>
    <script type="text/javascript">
        $(document).ready(function () {

            $("#dialog").dialog({
                autoOpen: false
            });
            
            $("#popup").dialog({
                autoOpen: false,
                height: 420,
                width: '80%',
                modal: true,
                close: function(event, ui) {                 
                    $("#list").trigger("reloadGrid");
                 }
            });

            var editDialog = {
                closeAfterAdd: true
                , closeAfterEdit: true
                , modal: true

                , onclickSubmit: function (params) {
                    var ajaxData = {};

                    var list = $("#list");
                    var selectedRow = list.getGridParam("selrow");
                    rowData = list.getRowData(selectedRow);
                    ajaxData = { Id: rowData.Id };
                    return ajaxData;
                }
                , afterShowForm: function (eparams) {
                    $('#Id').attr('disabled', 'disabled');
                     $('#imgAccountId').click(function () {
                                   OpenPopupAccountSearch();
                               });
                }
                , width: "400"
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
            };
            var insertDialog = {
               closeAfterAdd: true
                , closeAfterEdit: true
                , modal: true
                , afterShowForm: function (eparams) {
                   $('#Id').removeAttr('disabled');
                     $('#imgAccountId').click(function () {
                                   OpenPopupAccountSearch();
                               });
                }
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
                , width: "400"
            };
            var deleteDialog = {
                url: '<%= Url.Action("Delete", "Supplier") %>'
                , modal: true
                , width: "400"
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
            };

            $.jgrid.nav.addtext = "Tambah";
            $.jgrid.nav.edittext = "Edit";
            $.jgrid.nav.deltext = "Hapus";
            $.jgrid.edit.addCaption = "Tambah Supplier Baru";
            $.jgrid.edit.editCaption = "Edit Supplier";
            $.jgrid.del.caption = "Hapus Supplier";
            $.jgrid.del.msg = "Anda yakin menghapus Supplier yang dipilih?";
            var imgLov = '<%= Url.Content("~/Content/Images/window16.gif") %>';
            $("#list").jqGrid({
                url: '<%= Url.Action("List", "Supplier") %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Supplier', 'Nama', 'Maksimal Hutang', 'Status', 'Alamat', '', '', 'Telp', 'Kota', 'Keterangan'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'SupplierName', index: 'SupplierName', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true }, formoptions: { elmsuffix: ' *'} },

                   { name: 'SupplierMaxDebt', index: 'SupplierMaxDebt', width: 200, sortable: false, align: 'right', editable: true, editrules: { required: false },
                       editoptions: {
                           dataInit: function (elem) {
                               $(elem).autoNumeric();
                               $(elem).attr("style", "text-align:right;");
                           }
                       }
                   },
                   { name: 'SupplierStatus', index: 'SupplierStatus', width: 200, sortable: false, align: 'left', editable: true, edittype: 'checkbox', editoptions: { value: "Aktif:Tidak Aktif" }, editrules: { required: false} },
                   { name: 'AddressLine1', index: 'AddressLine1', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: false} },
                   { name: 'AddressLine2', index: 'AddressLine2', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'AddressLine3', index: 'AddressLine3', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'AddressPhone', index: 'AddressPhone', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'AddressCity', index: 'AddressCity', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                     { name: 'SupplierDesc', index: 'SupplierDesc', width: 200, hidden: true, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false, edithidden: true} }
                   ],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 300,
                caption: 'Daftar Supplier',
                autowidth: true, 
                editurl: '<%= Url.Action("InsertOrUpdate", "Supplier") %>',
                ondblClickRow: function (rowid, iRow, iCol, e) {
                    $("#list").editGridRow(rowid, editDialog);
                }
            });
            jQuery("#list").jqGrid('navGrid', '#listPager',
                 { edit: true, add: true, del: true, search: false, refresh: true }, //options 
                  editDialog,
                insertDialog,
                deleteDialog,
                {}
            );
            });   
        
          function OpenPopupAccountSearch()
        {
          $("#popup_frame").attr("src", "<%= ResolveUrl("~/Master/Account/Search") %>");
            $("#popup").dialog("open");
            return false;   
        }

         function SetAccountDetail(accountId, accountName)
        {
//        alert(itemId);
//        alert(itemName);
//        alert(price);
  $("#popup").dialog("close");
          $('#AccountId').attr('value', accountId);
          $('#AccountName').attr('value', accountName); 
       
        }       
    </script>
    <div id="dialog" title="Status">
        <p>
        </p>
    </div>
<div id='popup'>
    <iframe width='100%' height='380px' id="popup_frame" frameborder="0"></iframe>
</div>
</asp:Content>
