<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MyMaster.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<MCostCenter>>" %>

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


            var editDialog = {
                url: '<%= Url.Action("Update", "CostCenter") %>'
                , closeAfterAdd: true
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
                    $('#CostCenterStartDate').datepicker({ dateFormat: "dd-M-yy" });
                    $('#CostCenterEndDate').datepicker({ dateFormat: "dd-M-yy" });
                }
                , width: "400"
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
            };
            var insertDialog = {
                url: '<%= Url.Action("Insert", "CostCenter") %>'
                , closeAfterAdd: true
                , closeAfterEdit: true
                , modal: true
                , afterShowForm: function (eparams) {
                    $('#Id').attr('disabled', '');
                    $('#CostCenterStartDate').datepicker({ dateFormat: "dd-M-yy" });
                    $('#CostCenterEndDate').datepicker({ dateFormat: "dd-M-yy" });

                }
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
                , width: "400"
            };
            var deleteDialog = {
                url: '<%= Url.Action("Delete", "CostCenter") %>'
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
            $.jgrid.edit.addCaption = "Tambah Cost Center Baru";
            $.jgrid.edit.editCaption = "Edit Cost Center";
            $.jgrid.del.caption = "Hapus Cost Center";
            $.jgrid.del.msg = "Anda yakin menghapus Cost Center yang dipilih?";
            $("#list").jqGrid({
                url: '<%= Url.Action("List", "CostCenter") %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Cost Center', 'Nama', 'Total Budget', 'Status', 'Tgl Mulai', 'Tgl Selesai', 'Keterangan'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'CostCenterName', index: 'CostCenterName', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true }, formoptions: { elmsuffix: ' *'} },
                       { name: 'CostCenterTotalBudget', index: 'CostCenterTotalBudget', width: 200, sortable: false, align: 'right', editable: true, editrules: { required: false },
                           editoptions: {
                               dataInit: function (elem) {
                                   $(elem).autoNumeric();
                                   $(elem).attr("style", "text-align:right;");
                               }
                           }
                       },
                   { name: 'CostCenterStatus', index: 'CostCenterStatus', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: false} },
                       { name: 'CostCenterStartDate', index: 'CostCenterStartDate', width: 200, sortable: false, align: 'left', editable: true, editrules: { required: false} },
                       { name: 'CostCenterEndDate', index: 'CostCenterEndDate', width: 200, sortable: false, align: 'left', editable: true, editrules: { required: false} },
                   { name: 'CostCenterDesc', index: 'CostCenterDesc', width: 200, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false}}],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 300,
                caption: 'Daftar Cost Center',
                autowidth: true,
                ondblClickRow: function (rowid, iRow, iCol, e) {
                    $("#list").editGridRow(rowid, editDialog);
                }
            }).navGrid('#listPager',
                {
                    edit: true, add: true, del: true, search: false, refresh: true
                },
                editDialog,
                insertDialog,
                deleteDialog
            );
        });       
    </script>
    <div id="dialog" title="Status">
        <p></p>
    </div>
</asp:Content>
