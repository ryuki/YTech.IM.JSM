<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MyMaster.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
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

                }
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
                , width: "400"
            };
            var deleteDialog = {
                url: '<%= Url.Action("Delete", "MItemCat") %>'
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
            $.jgrid.edit.addCaption = "Tambah Kategori Produk Baru";
            $.jgrid.edit.editCaption = "Edit Kategori Produk";
            $.jgrid.del.caption = "Hapus Kategori Produk";
            $.jgrid.del.msg = "Anda yakin menghapus Kategori Produk yang dipilih?";
            $("#list").jqGrid({
                url: '<%= Url.Action("List", "MItemCat") %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Kategori Produk', 'Nama', 'Keterangan'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'ItemCatName', index: 'ItemCatName', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true }, formoptions: { elmsuffix: ' *'} },
                   { name: 'ItemCatDesc', index: 'ItemCatDesc', width: 200, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false}}],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 300,
                caption: 'Daftar Kategori Produk',
                autowidth: true, 
                editurl: '<%= Url.Action("InsertOrUpdate", "MItemCat") %>',
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
    </script>
</asp:Content>
