<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MyMaster.master" Inherits="System.Web.Mvc.ViewPage<YTech.IM.JSM.Web.Controllers.ViewModel.UserAdministration.IndexViewModel>" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
	Administrasi Pengguna
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

     <table id="list" class="scroll" cellpadding="0" cellspacing="0">
    </table>
    <div id="listPager" class="scroll" style="text-align: center;">
    </div>
    <div id="listPsetcols" class="scroll" style="text-align: center;">
    </div>
    <div id="dialog" title="Status">
        <p></p>
    </div>
    <div id='popup'>
        <iframe width='100%' height='440px' id="popup_frame" frameborder="0"></iframe>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <script type="text/javascript">

      $(document).ready(function () {

          $("#dialog").dialog({
              autoOpen: false
          });

          $("#popup").dialog({
              autoOpen: false,
              height: 520,
              width: '80%',
              modal: true,
              close: function (event, ui) {
                  $("#list").trigger("reloadGrid");
              }
          });

          var editDialog = {
              url: '<%= Url.Action("Update", "UserAdministration") %>'
                , closeAfterAdd: true
                , closeAfterEdit: true
                , modal: true

                , onclickSubmit: function (params) {
                    var ajaxData = {};

                    var list = $("#list");
                    var selectedRow = list.getGridParam("selrow");
                    rowData = list.getRowData(selectedRow);
                    ajaxData = { UserName: rowData.UserName };

                    return ajaxData;
                }
                , afterShowForm: function (eparams) {
                    $('#UserName').attr('disabled', 'disabled');
                    $('#tr_Password', eparams).hide();
                    $('#tr_PasswordConfirm', eparams).hide();
                }
                , width: "400"
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
          };
          var insertDialog = {
              url: '<%= Url.Action("Insert", "UserAdministration") %>'
                , closeAfterAdd: true
                , closeAfterEdit: true
                , modal: true
                , afterShowForm: function (eparams) {
                    $('#UserName').removeAttr('disabled');
                    $('#tr_Password', eparams).show();
                    $('#tr_PasswordConfirm', eparams).show();
                }
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
                , width: "400"
          };
          var deleteDialog = {
              url: '<%= Url.Action("Delete", "UserAdministration") %>'
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
          $.jgrid.edit.addCaption = "Tambah User Baru";
          $.jgrid.edit.editCaption = "Edit User";
          $.jgrid.del.caption = "Hapus User";
          $.jgrid.del.msg = "Anda yakin menghapus User yang dipilih?";
          $("#list").jqGrid({
              url: '<%= Url.Action("List", "UserAdministration") %>',
              datatype: 'json',
              mtype: 'GET',
              colNames: ['', 'Id', 'Nama User', 'Password', 'Konfirmasi Password', 'Keterangan', 'Aktifitas Terakhir'],
              colModel: [
                    {
                        name: 'act', index: 'act', width: 75, sortable: false
                    },
                   { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: true }, hidedlg: true, hidden: true, editable: false },
                    { name: 'UserName', index: 'UserName', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'Password', index: 'Password', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: false }, hidden: true },
                    { name: 'PasswordConfirm', index: 'PasswordConfirm', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: false }, hidden: true },
                    { name: 'Comment', index: 'Comment', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: false} },
                   { name: 'LastActivityDate', index: 'LastActivityDate', width: 200, sortable: false, align: 'left', editable: false, editoptions: { edithidden: true }, editrules: { required: false}}],

              pager: $('#listPager'),
              rowNum: 20,
              rowList: [20, 30, 50, 100],
              rownumbers: true,
              sortname: 'UserName',
              sortorder: "asc",
              viewrecords: true,
              height: 300,
              caption: 'Daftar User',
              autowidth: true,
              loadComplete: function () {
                  var ids = jQuery("#list").getDataIDs();
                  for (var i = 0; i < ids.length; i++) {
                      var cl = ids[i];
                      var be = "<input type='button' value='Edit' tooltips='Edit Pengguna'  onClick=\"OpenPopup('" + cl + "');\" />";

                      //                                                alert(be); 
                      $(this).setRowData(ids[i], { act: be });
                  }
              },
              ondblClickRow: function (rowid, iRow, iCol, e) {
                  $("#list").editGridRow(rowid, editDialog);
              }
          });
          jQuery("#list").jqGrid('navGrid', '#listPager',
                 { edit: false, add: true, del: true, search: false, refresh: true }, //options 
                  {},
                insertDialog,
                deleteDialog,
                {}
            );
      });
            function OpenPopup(id) {
                var url = '<%= Url.Content("~/Utility/UserAdministration/Details/") %>';
                if (id) {
                    url += id;
                }
                $("#popup_frame").attr("src", url);
                $("#popup").dialog("open");
                return false;
            }    
    </script>
</asp:Content>