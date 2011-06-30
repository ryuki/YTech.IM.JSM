<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MyMaster.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<MEmployee>>" %>

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
    <div id='popup'>
        <iframe width='100%' height='340px' id="popup_frame" frameborder="0"></iframe>
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
                url: '<%= Url.Action("Update", "Employee") %>'
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
                }
                , width: "400"
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
            };
            var insertDialog = {
                url: '<%= Url.Action("Insert", "Employee") %>'
                , closeAfterAdd: true
                , closeAfterEdit: true
                , modal: true
                , afterShowForm: function (eparams) {
                    $('#Id').attr('disabled', '');

                }
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
                , width: "400"
            };
            var deleteDialog = {
                url: '<%= Url.Action("Delete", "Employee") %>'
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
            $.jgrid.edit.addCaption = "Tambah Karyawan Baru";
            $.jgrid.edit.editCaption = "Edit Karyawan";
            $.jgrid.del.caption = "Hapus Karyawan";
            $.jgrid.del.msg = "Anda yakin menghapus Karyawan yang dipilih?";
            $("#list").jqGrid({
                url: '<%= Url.Action("List", "Employee") %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['', 'Kode Karyawan', 'Nama Depan', 'Nama Belakang', 'Status', 'Jenis Kelamin', 'Departemen', 'Departemen', 'Tipe Komisi Produk', 'Komisi Produk', 'Tipe Komisi Jasa', 'Komisi Jasa', 'Keterangan'],
                colModel: [
                    { name: 'act', index: 'act', width: 75, sortable: false },
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'PersonFirstName', index: 'PersonFirstName', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true }, formoptions: { elmsuffix: ' *'} },
                     { name: 'PersonLastName', index: 'PersonLastName', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'EmployeeStatus', index: 'EmployeeStatus', width: 200, sortable: false, align: 'left', editable: true, edittype: 'checkbox', editoptions: { value: "Aktif:Tidak Aktif" }, editrules: { required: false} },
                   { name: 'PersonGender', index: 'PersonGender', width: 200, sortable: false, align: 'left', editable: true, edittype: 'select', editoptions: { value: "Pria:Pria;Wanita:Wanita" }, editrules: { required: false} },
                     { name: 'DepartmentId', index: 'DepartmentId', width: 200, align: 'left', editable: true, edittype: 'select', editrules: { edithidden: true }, hidden: true },
                    { name: 'DepartmentName', index: 'DepartmentName', width: 200, align: 'left', editable: false, edittype: 'select', editrules: { edithidden: true} },
                     { name: 'EmployeeCommissionProductType', index: 'EmployeeCommissionProductType', width: 200, align: 'left', editable: true, edittype: 'select', editrules: { edithidden: true} },
                    { name: 'EmployeeCommissionProductVal', index: 'EmployeeCommissionProductVal', width: 200, align: 'right', editable: true, editrules: { edithidden: true },
                        editoptions: {
                            dataInit: function (elem) {
                                $(elem).autoNumeric();
                                $(elem).attr("style", "text-align:right;");
                            }
                        }
                    },
                     { name: 'EmployeeCommissionServiceType', index: 'EmployeeCommissionServiceType', width: 200, align: 'left', editable: true, edittype: 'select', editrules: { edithidden: true} },
                    { name: 'EmployeeCommissionServiceVal', index: 'EmployeeCommissionServiceVal', width: 200, align: 'right', editable: true, editrules: { edithidden: true },
                        editoptions: {
                            dataInit: function (elem) {
                                $(elem).autoNumeric();
                                $(elem).attr("style", "text-align:right;");
                            }
                        }
                    },
                   { name: 'EmployeeDesc', index: 'EmployeeDesc', width: 200, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false }, formoptions: { elmsuffix: ' *'}}],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 300,
                caption: 'Daftar Karyawan',
                autowidth: true,
                loadComplete: function () {
                    $('#list').setColProp('DepartmentId', { editoptions: { value: departments} });
                    $('#list').setColProp('EmployeeCommissionProductType', { editoptions: { value: commissiontype} });
                    $('#list').setColProp('EmployeeCommissionServiceType', { editoptions: { value: commissiontype} });

                     var ids = jQuery("#list").getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var cl = ids[i];
                        var be = "<input type='button' value='T' tooltips='Tambah Komisi Paket' onClick=\"OpenPopup('"+cl+"');\" />";
                        //                        alert(be);
                        $(this).setRowData(ids[i], { act: be });
                    }
                },
                multiselect: false,
                subGrid: true,
                subGridUrl: '<%= Url.Action("GetListForSubGrid", "PacketComm") %>',
                subGridModel: [{ name: [ 'Paket', 'Jenis Komisi', 'Komisi', 'Deskripsi'],
                    width: [  55, 80, 80, 80],
                       //subrig columns aligns
                       align: ['left', 'left', 'right', 'left'],
                    params: ['Id']
                }],
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

            var departments = $.ajax({ url: '<%= Url.Action("GetList","Department") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the Department.'); } }).responseText;
            var commissiontype = $.ajax({ url: '<%= Url.Action("GetCommissionTypeList","Employee") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the commissiontype.'); } }).responseText;
        function OpenPopup(id)
        {
            $("#popup_frame").attr("src", "<%= Url.Action("PopupAdd", "PacketComm") %>/"+id);
            $("#popup").dialog("open");
            return false;   
        }
    </script>
</asp:Content>
