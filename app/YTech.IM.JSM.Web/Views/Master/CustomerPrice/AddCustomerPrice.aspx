<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPopup.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<IEnumerable<MCustomerPrice>>" %>

<asp:Content ID="popupContent" ContentPlaceHolderID="MainContent" runat="server">
    <table id="list" class="scroll" cellpadding="0" cellspacing="0">
    </table>
    <div id="listPager" class="scroll" style="text-align: center;">
    </div>
    <div id="listPsetcols" class="scroll" style="text-align: center;">
    </div>
    <script type="text/javascript">

        //SETTING UP OUR POPUP
        //0 means disabled; 1 means enabled;
        var popupStatus = 0;

        //loading popup with jQuery magic!
        function loadPopup() {
            //loads popup only if it is disabled
            if (popupStatus == 0) {
                $("#backgroundPopup").css({
                    "opacity": "0.7"
                });
                $("#backgroundPopup").fadeIn("slow");
                $("#popupContact").fadeIn("slow");
                popupStatus = 1;
            }
        }

        //disabling popup with jQuery magic!
        function disablePopup() {
            //disables popup only if it is enabled
            if (popupStatus == 1) {
                $("#backgroundPopup").fadeOut("slow");
                $("#popupContact").fadeOut("slow");
                popupStatus = 0;
            }
        }

        //centering popup
        function centerPopup() {
            //request data for centering
            var windowWidth = document.documentElement.clientWidth;
            var windowHeight = document.documentElement.clientHeight;
            var popupHeight = $("#popupContact").height();
            var popupWidth = $("#popupContact").width();
            //centering
            $("#popupContact").css({
                "position": "absolute",
                "top": windowHeight / 2 - popupHeight / 2,
                "left": windowWidth / 2 - popupWidth / 2
            });
            //only need force for IE6

            $("#backgroundPopup").css({
                "height": windowHeight
            });

        }

        $(document).ready(function () {
            //following code will be here
            //LOADING POPUP
            //Click the button event!
            $("#button").click(function () {
                //centering with css
                centerPopup();
                //load popup
                loadPopup();
            });
            //CLOSING POPUP
            //Click the x event!
            $("#popupContactClose").click(function () {
                disablePopup();
            });
            //Click out event!
            $("#backgroundPopup").click(function () {
                disablePopup();
            });
            //Press Escape event!
            $(document).keypress(function (e) {
                if (e.keyCode == 27 && popupStatus == 1) {
                    disablePopup();
                }
            });

        });


        $(function () {
            $("button, input:submit, a", ".demo").button();

            $("a", ".demo").click(function () { return false; });
        });


        var path = location.pathname;
        var packetId = path.substr(path.lastIndexOf('/') + 1);

        $(document).ready(function () {
            $("#dialog").dialog({
                autoOpen: false
            });


            var editDialog = {
                url: '<%= Url.Action("Update", "CustomerPrice") %>?packetId=' + packetId
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
                url: '<%= Url.Action("PopupInsert", "CustomerPrice") %>?packetId=' + packetId
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
                url: '<%= Url.Action("Delete", "CustomerPrice") %>?packetId=' + packetId
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
            $.jgrid.edit.addCaption = "Tambah Customer Price";
            $.jgrid.edit.editCaption = "Edit Customer Price";
            $.jgrid.del.caption = "Hapus Customer Price";
            $.jgrid.del.msg = "Anda yakin menghapus Customer Price yang dipilih?";

            //            alert(packetId);
            $("#list").jqGrid({
                url: '<%= Url.Action("List", "CustomerPrice") %>?id=' + packetId,
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Paket Perawatan', 'Kategori Perawatan', 'Kategori Perawatan', 'Kuantitas', 'Status', 'Deskripsi'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: true, editable: false },
                    { name: 'ItemCatId', index: 'ItemCatId', width: 200, align: 'left', editable: true, edittype: 'select', editrules: { required: true, edithidden: true }, formoptions: { elmsuffix: ' *' }, hidden: true },
                    { name: 'ItemCatName', index: 'ItemCatName', width: 200, align: 'left', editable: false, edittype: 'select', editrules: { required: true, edithidden: true }, formoptions: { elmsuffix: ' *'} },
                    { name: 'ItemCatQty', index: 'ItemCatQty', width: 200, align: 'right', editable: true, edittype: 'text', editrules: { edithidden: true }, hidden: false,
                        editoptions: {
                            dataInit: function (elem) {
                                $(elem).autoNumeric();
                                $(elem).attr("style", "text-align:right;");
                            }
                        }
                    },
                
                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 250,
                caption: 'Daftar Customer Price',
                autowidth: true,
                loadComplete: function () {
                    $('#list').setColProp('ItemCatId', { editoptions: { value: itemCats} });
                    $('#list').setColProp('PacketId', { editoptions: { value: packets} });
                },
                ondblClickRow: function (rowid, iRow, iCol, e) {
                    $('#list').editGridRow(rowid, editDialog);
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

        var packets = $.ajax({ url: '<%= Url.Action("GetList","Packet") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the Packets.'); } }).responseText;
        var itemCats = $.ajax({ url: '<%= Url.Action("GetList","MItemCat") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the ItemCats.'); } }).responseText;

        
    </script>
    <div id="dialog" title="Status">
        <p>
        </p>
    </div>
</asp:Content>
