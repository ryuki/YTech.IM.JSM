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

            $("#popup").dialog({
                autoOpen: false,
                height: 420,
                width: '80%',
                modal: true,
                close: function (event, ui) {
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
                url: '<%= Url.Action("Delete", "Item") %>'
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
            $.jgrid.edit.addCaption = "Tambah Produk Baru";
            $.jgrid.edit.editCaption = "Edit Produk";
            $.jgrid.del.caption = "Hapus Produk";
            $.jgrid.del.msg = "Anda yakin menghapus Produk yang dipilih?";
            $("#list").jqGrid({
                url: '<%= Url.Action("List", "Item") %>',
                datatype: 'json',
                mtype: 'GET',
                colNames: ['Kode Produk', 'Nama', 'Kategori Produk', 'Kategori Produk', 'Merek', 'Merek', 'Satuan', 'Satuan', 'Harga Beli', 'Harga Jual', 'Keterangan'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: false, editable: true },
                    { name: 'ItemName', index: 'ItemName', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true }, formoptions: { elmsuffix: ' *'} },
                    { name: 'ItemCatId', index: 'ItemCatId', width: 200, align: 'left', editable: true, edittype: 'select', editrules: { edithidden: true }, hidden: true,
                        editoptions: {
                            value: itemCats
                        }
                    },
                    { name: 'ItemCatName', index: 'ItemCatName', width: 200, align: 'left', editable: false, edittype: 'select', editrules: { edithidden: true} },
                    { name: 'BrandId', index: 'BrandId', width: 200, align: 'left', editable: true, edittype: 'select', editrules: { edithidden: true }, hidden: true,
                        editoptions: {
                            value: brands
                        }
                    },
                    { name: 'BrandName', index: 'BrandName', width: 200, align: 'left', editable: false, edittype: 'select', editrules: { edithidden: true} },
                    { name: 'ItemUomId', index: 'ItemUomId', width: 200, align: 'left', editable: false, editrules: { edithidden: true }, hidden: true },
                    { name: 'ItemUomName', index: 'ItemUomName', width: 200, editable: true, editrules: { edithidden: true} },
                    { name: 'ItemUomPurchasePrice', index: 'ItemUomPurchasePrice', width: 200, align: 'right', editable: true, editrules: { edithidden: true },
                        editoptions: {
                            dataInit: function (elem) {
                                $(elem).autoNumeric();
                                $(elem).attr("style", "text-align:right;");
                            }
                        }
                    },
                    { name: 'ItemUomSalePrice', index: 'ItemUomSalePrice', width: 200, align: 'right', editable: true, editrules: { edithidden: true },
                        editoptions: {
                            dataInit: function (elem) {
                                $(elem).autoNumeric();
                                $(elem).attr("style", "text-align:right;");
                            }
                        }
                    },
                   { name: 'ItemDesc', index: 'ItemDesc', width: 200, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false}}],

                pager: $('#listPager'),
                rowNum: 20,
                rowList: [20, 30, 50, 100],
                rownumbers: true,
                sortname: 'Id',
                sortorder: "asc",
                viewrecords: true,
                height: 300,
                caption: 'Daftar Produk',
                autowidth: true, editurl: '<%= Url.Action("InsertOrUpdate", "Item") %>',
                ondblClickRow: function (rowid, iRow, iCol, e) {
                    $('#list').editGridRow(rowid, editDialog);
                },
                loadComplete: function () {
                    //                    $('#list').setColProp('ItemCatId', { editoptions: { value: itemCats} });
                    //                    $('#list').setColProp('BrandId', { editoptions: { value: brands} });
                },
                subGrid: true,
                subGridRowExpanded: function (subGridId, rowId) { 
                    var subGridTableId = subGridId + "_t";
                    var pagerId = "p_" + subGridTableId;

                    var addSubDialog = {
                        url: '<%= Url.Action("InsertSubGrid", "CustomerPrice") %>?itemId=' + rowId,
                        closeAfterAdd: true,
                        closeAfterEdit: true,
                        modal: true,
                        afterShowForm: function (eparams) {
                          
                        },
                        afterComplete: function (response, postdata, formid) {
                            $('#dialog p:first').text(response.responseText);
                            $("#dialog").dialog("open");
                        },
                        width: "400"
                    };

                    var editSubDialog = {
                        url: '<%= Url.Action("UpdateSubGrid", "CustomerPrice") %>?itemId=' + rowId
                        , closeAfterAdd: true
                        , closeAfterEdit: true
                        , modal: true
                        , onclickSubmit: function (params) {
                            var ajaxData = {};

                            var list = $("#" + subGridTableId);
                            var selectedRow = list.getGridParam("selrow");
                            rowData = list.getRowData(selectedRow);
                            ajaxData = { Id: rowData.Id };

                            return ajaxData;
                        }
                        , afterShowForm: function (eparams) { 

                        }
                        , width: "400"
                        , afterComplete: function (response, postdata, formid) {
                            $('#dialog p:first').text(response.responseText);
                            $("#dialog").dialog("open");
                        }
                    };

                    var deleteSubDialog = {
                        url: '<%= Url.Action("DeleteSubGrid", "CustomerPrice") %>'
                        , modal: true
                        , width: "400"
                        , afterComplete: function (response, postdata, formid) {
                            $('#dialog p:first').text(response.responseText);
                            $("#dialog").dialog("open");
                        }
                    };

                    $("#" + subGridId).html("<table id='" + subGridTableId + "' class='scroll'></table><div id='" + pagerId + "' class='scroll'></div>");
                    $("#" + subGridTableId).jqGrid({
                        url: '<%= Url.Action("ListSubGrid", "CustomerPrice") %>',
                        postData: { itemId: function () { return rowId; } },
                        datatype: 'json',
                        mtype: 'GET',
                        colNames: ['Id',
                                   'Nama Konsumen',
                                   'Nama Konsumen',
                                   'Harga'
                                   ],
                        colModel: [
                            { name: 'Id', index: 'Id', width: 75, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: true, editable: false },
                            { name: 'CustomerId', index: 'CustomerId', align: 'right', editable: true, hidedlg: true, hidden: true, edittype: 'select', editrules: { edithidden: true },
                                editoptions: {
                                    value: customers
                                }
                            },
                            { name: 'CustomerName', index: 'CustomerName', width: 75, align: 'left', editrules: { edithidden: false }, hidedlg: true, hidden: false, editable: false },
                            { name: 'Price', index: 'Price', align: 'right', editable: true, edittype: 'text', align: 'right',
                                editoptions: {
                                    dataInit: function (elem) {
                                        $(elem).autoNumeric();
                                        $(elem).attr("style", "text-align:right;");
                                    }
                                }
                            }
                        ],
                        pager: pagerId,
                        rowNum: -1,
                        rowList: [20, 30, 50, 100],
                        rownumbers: false,
                        sortname: 'DetailType',
                        sortorder: "asc",
                        viewrecords: true,
                        height: 'auto',
                        caption: 'Detail Harga Konsumen'
                    });
                    $("#" + subGridTableId).jqGrid('navGrid', "#" + pagerId,
                        { edit: true, add: true, del: true, search: false, refresh: true },
                        editSubDialog, addSubDialog, deleteSubDialog
                    );
                }
            });
            jQuery("#list").jqGrid('navGrid', '#listPager',
                 { edit: true, add: true, del: true, search
                  : false, refresh: true }, //options 
                  editDialog,
                insertDialog,
                deleteDialog,
                {}
            );
        });

            var itemCats = $.ajax({ url: '<%= Url.Action("GetList","MItemCat") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the ItemCats.'); } }).responseText;
            var brands = $.ajax({ url: '<%= Url.Action("GetList","Brand") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the Brands.'); } }).responseText;
            var customers = $.ajax({ url: '<%= Url.Action("GetList","Customer") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the customers.'); } }).responseText;
            
    </script>
</asp:Content>
