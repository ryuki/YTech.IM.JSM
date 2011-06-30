<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPopup.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<MEmployee>>" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
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
            $.jgrid.edit.addCaption = "Tambah Karyawan Baru";
            $.jgrid.edit.editCaption = "Edit Karyawan";
            $.jgrid.del.caption = "Hapus Karyawan";
            $.jgrid.del.msg = "Anda yakin menghapus Karyawan yang dipilih?";
            $("#list").jqGrid({
                url: '<%= Url.Action("ListSearch", "Employee") %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Karyawan', 'Nama', 'Jenis Kelamin', 'Departemen', 'Keterangan'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'PersonName', index: 'PersonName', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true }, formoptions: { elmsuffix: ' *'} },
                   { name: 'PersonGender', index: 'PersonGender', width: 200, sortable: false, align: 'left', editable: true, edittype: 'select', editoptions: { value: "Pria:Pria;Wanita:Wanita" }, editrules: { required: false} },
                    { name: 'DepartmentName', index: 'DepartmentName', width: 200, align: 'left', editable: false, edittype: 'select', editrules: { edithidden: true} },
                   { name: 'EmployeeDesc', index: 'EmployeeDesc', width: 200, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false }, formoptions: { elmsuffix: ' *'}}],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 250,
                caption: 'Daftar Karyawan',
                autowidth: true,
                loadComplete: function () {
                },
                ondblClickRow: function (rowid, iRow, iCol, e) {
                    var list = $("#list");
                    var rowData = list.getRowData(rowid);
                     <% if (!string.IsNullOrEmpty(Request.QueryString["src"])) {	%>
                      window.parent.SetEmployeeDetail('<%= Request.QueryString["src"] %>',rowData["Id"], rowData["PersonName"]);
  <%} else {%>
 window.parent.SetEmployeeDetail(rowData["Id"], rowData["PersonName"]);
  <%}%>
                    
                    return false;
                }
            }).navGrid('#listPager',
                {
                    edit: false, add: false, del: false, search: false, refresh: true
                }
            );
        });       
    </script>
</asp:Content>
