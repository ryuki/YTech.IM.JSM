<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MyMaster.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<MCustomer>>" %>

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
                url: '<%= Url.Action("Update", "Customer") %>'
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
                    $('#PersonDob').datepicker({ dateFormat: "dd-M-yy" });
                    $('#CustomerJoinDate').datepicker({ dateFormat: "dd-M-yy" });
                    $('#CustomerLastBuy').datepicker({ dateFormat: "dd-M-yy" });
                }
                , width: "400"
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
            };
            var insertDialog = {
                url: '<%= Url.Action("Insert", "Customer") %>'
                , closeAfterAdd: true
                , closeAfterEdit: true
                , modal: true
                , afterShowForm: function (eparams) {
                    $('#Id').attr('disabled', '');
                    $('#PersonDob').datepicker({ dateFormat: "dd-M-yy" });
                    $('#CustomerJoinDate').datepicker({ dateFormat: "dd-M-yy" });
                    $('#CustomerLastBuy').datepicker({ dateFormat: "dd-M-yy" });
                }
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
                , width: "400"
            };
            var deleteDialog = {
                url: '<%= Url.Action("Delete", "Customer") %>'
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
            $.jgrid.edit.addCaption = "Tambah Konsumen Baru";
            $.jgrid.edit.editCaption = "Edit Konsumen";
            $.jgrid.del.caption = "Hapus Konsumen";
            $.jgrid.del.msg = "Anda yakin menghapus Konsumen yang dipilih?";
            $("#list").jqGrid({
                url: '<%= Url.Action("List", "Customer") %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Konsumen',
                            'Nama',
                            '',
                            'Nama',
                            'Alamat',
                            '',
                            'Alamat',
                            'Telp',
                            'Jenis Kelamin',
                            'Tgl Lahir',
                            'Agama',
                            'Suku',
                            'Keluhan Kesehatan',
                            'Tgl Bergabung',
                            'Tgl Terakhir Berkunjung',
                            'Diskon Barang (%)',
                            'Diskon Jasa (%)',
                            'Status',
                            'Kekuatan Pijatan',
                            'Keterangan'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'PersonFirstName', index: 'PersonFirstName', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: true, edithidden: true }, formoptions: { elmsuffix: ' *'} },
                   { name: 'PersonLastName', index: 'PersonLastName', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'PersonName', index: 'PersonName', width: 200, align: 'left', editable: false, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'AddressLine1', index: 'AddressLine1', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'AddressLine2', index: 'AddressLine2', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'Address', index: 'Address', width: 200, align: 'left', editable: false, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'AddressPhone', index: 'AddressPhone', width: 200, hidden: false, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'PersonGender', index: 'PersonGender', width: 200, sortable: false, align: 'left', editable: true, edittype: 'select', editrules: { required: false} },
                   { name: 'PersonDob', index: 'PersonDob', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'PersonReligion', index: 'PersonReligion', width: 200, hidden: true, align: 'left', editable: true, edittype: 'select', editrules: { required: false, edithidden: true} },
                   { name: 'PersonRace', index: 'PersonRace', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'CustomerHealthProblem', index: 'CustomerHealthProblem', width: 200, hidden: true, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false, edithidden: true} },
                   { name: 'CustomerJoinDate', index: 'CustomerJoinDate', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'CustomerLastBuy', index: 'CustomerLastBuy', width: 200, hidden: false, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true} },
                   { name: 'CustomerProductDisc', index: 'CustomerProductDisc', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true },
                       editoptions: {
                           dataInit: function (elem) {
                               $(elem).autoNumeric();
                               $(elem).attr("style", "text-align:right;");
                           }
                       }
                   },
                   { name: 'CustomerServiceDisc', index: 'CustomerServiceDisc', width: 200, hidden: true, align: 'left', editable: true, edittype: 'text', editrules: { required: false, edithidden: true },
                       editoptions: {
                           dataInit: function (elem) {
                               $(elem).autoNumeric();
                               $(elem).attr("style", "text-align:right;");
                           }
                       }
                   },
                 { name: 'CustomerStatus', index: 'CustomerStatus', width: 200, hidden: true, sortable: false, align: 'left', editable: true, edittype: 'checkbox', editoptions: { value: "Aktif:Tidak Aktif" }, editrules: { required: false} },
                   { name: 'CustomerMassageStrength', index: 'CustomerMassageStrength', width: 200, hidden: true, align: 'left', editable: true, edittype: 'select', editrules: { required: false, edithidden: true} },
                     { name: 'CustomerDesc', index: 'CustomerDesc', width: 200, hidden: true, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false, edithidden: true} }
                   ],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 300,
                caption: 'Daftar Konsumen',
                autowidth: true,
                loadComplete: function () {
                    $('#list').setColProp('PersonGender', { editoptions: { value: genders} });
                    $('#list').setColProp('PersonReligion', { editoptions: { value: religions} });
                    $('#list').setColProp('CustomerMassageStrength', { editoptions: { value: massage} });
                },
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
        var genders = $.ajax({ url: '<%= Url.Action("GetGenderList","Customer") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the genders.'); } }).responseText;
        var religions = $.ajax({ url: '<%= Url.Action("GetReligionList","Customer") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the religions.'); } }).responseText;
        var massage = $.ajax({ url: '<%= Url.Action("GetMassageStrengthList","Customer") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the massage.'); } }).responseText;
    </script>
    <div id="dialog" title="Status">
        <p>
        </p>
    </div>
</asp:Content>
