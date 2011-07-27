<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<YTech.IM.JSM.Web.Controllers.ViewModel.TransactionFormViewModel>" %>
<%@ Import Namespace="YTech.IM.JSM.Enums" %>
<%@ Import Namespace="YTech.IM.JSM.Web.Controllers.Helper" %>
<% if (false)
   { %>
<script src="../../../Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script>
<% } %>
<% using (Ajax.BeginForm(new AjaxOptions
                                       {
                                           InsertionMode = InsertionMode.Replace,
                                           OnBegin = "ajaxValidate",
                                           OnSuccess = "onSavedSuccess"
                                       }

          ))
   {%>
<div id="status">
</div>
<div class="ui-state-highlight ui-corner-all" style="padding: 5pt; margin-bottom: 5pt;
    display: none;" id="error">
    <p>
        <span class="ui-icon ui-icon-error" style="float: left; margin-right: 0.3em;"></span>
        <span id="error_msg"></span>.<br clear="all" />
    </p>
</div>
<%--<% using (Html.BeginForm())
   { %>--%>
<%= Html.AntiForgeryToken() %>
<%= Html.Hidden("Trans.Id", (ViewData.Model.Trans != null) ? ViewData.Model.Trans.Id : "")%>
<%= Html.Hidden("Trans.TransStatus", (ViewData.Model.Trans != null) ? ViewData.Model.Trans.TransStatus : "")%>
<%= Html.Hidden("IsAddStock", ViewData.Model.IsAddStock.ToString())%>
<div>
    <span id="toolbar" class="ui-widget-header ui-corner-all"><a id="newTrans" href="<%= Url.Action(ViewData.Model.Trans.TransStatus.Equals(EnumTransactionStatus.PurchaseOrder.ToString()) ? "Index" : Model.Trans.TransStatus.ToString(), "Inventory") %>">
        Baru</a>
        <button id="btnSave" name="btnSave" type="submit">
            Simpan</button>
        <button id="btnDelete" name="btnDelete" type="submit">
            Hapus
            <%= ViewData.Model.Title %></button>
        <button id="btnList" name="btnList" type="button">
            Daftar
            <%= ViewData.Model.Title %></button>
    </span>
</div>
<table>
    <tr>
        <td colspan="2">
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <% if (ViewData.Model.ViewDate)
                   {	%>
                <tr>
                    <td>
                        <label for="Trans_TransDate">
                            Tanggal :</label>
                    </td>
                    <td>
                        <%= Html.TextBox("Trans.TransDate", (Model.Trans.TransDate.HasValue) ? Model.Trans.TransDate.Value.ToString(CommonHelper.DateFormat) : "")%>
                        <%= Html.ValidationMessage("Trans.TransDate")%>
                    </td>
                </tr>
                <% } %>
                <% if (ViewData.Model.ViewFactur)
                   {	%>
                <tr>
                    <td>
                        <label for="Trans_TransFactur">
                            No Faktur :</label>
                    </td>
                    <td>
                        <%= Html.TextBox("Trans.TransFactur", Model.Trans.TransFactur)%>
                        <%= Html.ValidationMessage("Trans.TransFactur")%>
                    </td>
                </tr>
                <% } %>
                <% if (ViewData.Model.ViewPaymentMethod)
                   {	%>
                <tr>
                    <td>
                        <label for="Trans_TransPaymentMethod">
                            Cara Pembayaran :</label>
                    </td>
                    <td>
                        <%= Html.DropDownList("Trans.TransPaymentMethod", Model.PaymentMethodList)%>
                        <%= Html.ValidationMessage("Trans.TransPaymentMethod")%>
                    </td>
                </tr>
                <% } %>
            </table>
        </td>
        <td>
            <table>
                <% if (ViewData.Model.ViewTransBy)
                   {	%>
                <tr>
                    <td>
                        <label for="Trans_TransBy">
                            <%= ViewData.Model.TransByText %></label>
                    </td>
                    <td>
                        <%= Html.DropDownList("Trans.TransBy", Model.TransByList)%>
                        <%= Html.ValidationMessage("Trans.TransBy")%>
                    </td>
                </tr>
                <% } %>
                <% if (ViewData.Model.ViewWarehouse)
                   {	%>
                <tr>
                    <td>
                        <label for="Trans_WarehouseId">
                            Gudang :</label>
                    </td>
                    <td>
                        <%= Html.DropDownList("Trans.WarehouseId", Model.WarehouseList)%>
                        <%= Html.ValidationMessage("Trans.WarehouseId")%>
                    </td>
                </tr>
                <% } %>
                <% if (ViewData.Model.ViewWarehouseTo)
                   {	%>
                <tr>
                    <td>
                        <label for="Trans_WarehouseIdTo">
                            Ke Gudang :</label>
                    </td>
                    <td>
                        <%= Html.DropDownList("Trans.WarehouseIdTo", Model.WarehouseToList)%>
                        <%= Html.ValidationMessage("Trans.WarehouseIdTo")%>
                    </td>
                </tr>
                <% } %>
                <% if (ViewData.Model.ViewTransDueDate)
                   {	%>
                <tr>
                    <td>
                        <label for="Trans_TransDueDate">
                            Tgl Jatuh Tempo :</label>
                    </td>
                    <td>
                        <%= Html.TextBox("Trans.TransDueDate", (Model.Trans.TransDueDate.HasValue) ? Model.Trans.TransDueDate.Value.ToString(CommonHelper.DateFormat) : "")%>
                        <%= Html.ValidationMessage("Trans.TransDueDate")%>
                    </td>
                </tr>
                <% } %>
            </table>
        </td>
    </tr>
</table>
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
    <iframe width='100%' height='380px' id="popup_frame" frameborder="0"></iframe>
</div>
<% } %>
<script language="javascript" type="text/javascript">

function onSavedSuccess(e) {
 //$("#Save").attr('disabled', 'disabled');
  var json = e.get_response().get_object();
//alert(json);
    var success = json.Success;
       //alert(json.Success);
        var msg = json.Message;
    if (success == false) {
        //alert(json.Message);
        if (msg) {

            if (msg == "redirect") {
                var urlreport = '<%= ResolveUrl("~/ReportViewer.aspx?rpt=RptPrintFactur") %>';
                   // alert(urlreport);
                window.open(urlreport);
            }
            else {
                $('#dialog p:first').text(msg);
                $("#dialog").dialog("open"); 
            }
            return false ;  
        }
    }
    else{
        $("#btnSave").attr('disabled', 'disabled');
        $("#btnDelete").attr('disabled', 'disabled');
        $("#btnPrint").removeAttr('disabled');
        $('#dialog p:first').text(msg);
        $("#dialog").dialog("open"); 
    }
}


function ajaxValidate() {
var imgerror = '<%= Url.Content("~/Content/Images/cross.gif") %>';
    return $('form').validate({
    rules: {
     "Trans.Id": { required: true }
     <% if (ViewData.Model.ViewDate) {	%>
        ,"Trans.TransDate": { required: true } 
        <% } %>
       <% if (ViewData.Model.ViewFactur) {	%> 
       ,"Trans.TransFactur": { required: true  } 
       <% } %>
         <% if (ViewData.Model.ViewTransBy) {	%> 
         ,"Trans.TransBy": { required: true  }
         <% } %>
         <% if (ViewData.Model.ViewWarehouse) {	%>
         ,"Trans.WarehouseId": { required: true  }
         <% } %>
        <% if (ViewData.Model.ViewWarehouseTo) {	%> 
        ,"Trans.WarehouseIdTo": { required: true  }
        <% } %>
    },
    messages: {
     "Trans.Id":  "<img id='TransIderror' src='"+imgerror+"' hovertext='Id tidak boleh kosong' />"
       <% if (ViewData.Model.ViewDate) {	%> 
       ,"Trans.TransDate": "<img id='TransDateerror' src='"+imgerror+"' hovertext='Tanggal tidak boleh kosong' />"  
       <% } %>
        <% if (ViewData.Model.ViewFactur) {	%>  
        ,"Trans.TransFactur": "<img id='TransFacturerror' src='"+imgerror+"' hovertext='No Faktur harus diisi' />"
        <% } %>
        <% if (ViewData.Model.ViewTransBy) {	%>  
        ,"Trans.TransBy": "<img id='TransByerror' src='"+imgerror+"' hovertext='Pilih <%= ViewData.Model.TransByText %>' />"
        <% } %>
        <% if (ViewData.Model.ViewWarehouse) {	%>  
        ,"Trans.WarehouseId": "<img id='WarehouseIderror' src='"+imgerror+"' hovertext='Pilih Gudang' />"
        <% } %>
        <% if (ViewData.Model.ViewWarehouseTo) {	%>  
        ,"Trans.WarehouseIdTo": "<img id='WarehouseIdToerror' src='"+imgerror+"' hovertext='Pilih Gudang Tujuan' />"
        <% } %>
        },
        invalidHandler: function(form, validator) {
          var errors = validator.numberOfInvalids();
						  if (errors) {
                          var message = "Validasi data kurang";
				$("div#error span#error_msg").html(message);
                  $("div#error").dialog("open");
			} else {
                  $("div#error").dialog("close");
			}
            		},
		errorPlacement: function(error, element) { 
			error.insertAfter(element);
		}
    }).form();
}


    function CalculateTotal() {
        var price = $('#TransDetPrice').val().replace(/,/gi,"");
        var qty = $('#TransDetQty').val().replace(/,/gi,"");
        var disc = $('#TransDetDisc').val().replace(/,/gi,"");
        var subtotal = (price * qty)
        var total = subtotal - (disc * subtotal / 100);
//        alert(price);
//        alert(qty);
//        alert(disc);
//        alert(subtotal);
//        alert(total);
//        $('#TransDetTotal').val(total);
        $('#TransDetTotal').val($.fn.autoNumeric.Format('', total));
    }

    $(function () {
        $("#newTrans").button();
//        $("#Save").button();
        $("#Trans_TransDate").datepicker({ dateFormat: "dd-M-yy" });
        $("#Trans_TransDueDate").datepicker({ dateFormat: "dd-M-yy" });

          <% if (TempData[EnumCommonViewData.SaveState.ToString()] != null)
{
    if (TempData[EnumCommonViewData.SaveState.ToString()].Equals(EnumSaveState.Failed))
    {%>
   $("#btnPrint").attr('disabled', 'disabled');
    <%
    }
} else { %>
  $("#btnPrint").attr('disabled', 'disabled');
<% } %>
    });

    $(document).ready(function () {
     $("form").mouseover(function () {
                generateTooltips();
            });
        $("#dialog").dialog({
            autoOpen: false
        });
        $("div#error").dialog({
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

              $("#btnList").click(function () {
        var urlList = '<%= ResolveUrl("~/Transaction/Inventory/ListTransaction") %>';
          $("#popup_frame").attr("src", urlList+"?src=cc&transStatus="+$("#Trans_TransStatus").val());
            $("#popup").dialog("open");
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
                , beforeSubmit: function (postdata, formid) {
                    postdata.IsAddStock = $('#IsAddStock').val();
                    postdata.WarehouseId = $('#Trans_WarehouseId').val();
                    return [true, ''];
                }
                , afterShowForm: function (eparams) {
                   $('#Id').removeAttr('disabled');
                    $('#TransDetQty').attr('value', '1');
                     <% if (ViewData.Model.ViewPrice)
               {%> 
                    $('#TransDetPrice').attr('value', '0');
                    $('#TransDetDisc').attr('value', '0');
                    $('#TransDetTotal').attr('value', '0');

//                    $('#ItemId').change(function () {
//                        var price = $.ajax({ url: '<%= ResolveUrl("~/Master/Item/Get") %>/' + $('#ItemId :selected').val(), async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the items.'); } }).responseText;
//                        $('#TransDetPrice').attr('value', price);
//                        CalculateTotal();
//                    });
                    $('#TransDetPrice').change(function () {
                        CalculateTotal();
                    });
                    $('#TransDetQty').change(function () {
                        CalculateTotal();
                    });
                    $('#TransDetDisc').change(function () {
                        CalculateTotal();
                    });
                   <%
               }%>  
                     $('#imgItemId').click(function(){
                        OpenPopupItemSearch();
                        });
                    
                }
                , afterComplete: function (response, postdata, formid) {
                    $('#dialog p:first').text(response.responseText);
                    $("#dialog").dialog("open");
                }
                , width: "400"
        };
        var deleteDialog = {
            url: '<%= Url.Action("Delete", "Inventory") %>'
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
        $.jgrid.edit.addCaption = "Tambah Detail Baru";
        $.jgrid.edit.editCaption = "Edit Detail";
        $.jgrid.del.caption = "Hapus Detail";
        $.jgrid.del.msg = "Anda yakin menghapus Detail yang dipilih?";
        var imgLov = '<%= Url.Content("~/Content/Images/window16.gif") %>';
        $("#list").jqGrid({
            url: '<%= Url.Action("List", "Inventory", new { usePrice = ViewData.Model.ViewPrice} ) %>',
//                postData: {
//                    UsePrice: function () { return <% if (ViewData.Model.ViewPrice) {%>' true' <%} else { %>                                                      'false'
//                                                      <% } %>; }
//                },
            datatype: 'json',
            mtype: 'GET',
            colNames: ['Id', 'Produk', 'Produk', 'Kuantitas',
            <% if (ViewData.Model.ViewPrice)
               {%> 'Harga', 'Diskon', 'Total',
                   <%
               }%>                   
                    'Keterangan'],
            colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: false }, hidedlg: true, hidden: true, editable: false },
                    { name: 'ItemId', index: 'ItemId', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { edithidden: true }, hidden: true,
                       formoptions: {
                        "elmsuffix": "&nbsp;<img src='" + imgLov + "' style='cursor:hand;' id='imgItemId' />"
                    } }, 
                    { name: 'ItemName', index: 'ItemName', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { edithidden: true} },
                     { name: 'TransDetQty', index: 'TransDetQty', width: 200, sortable: false, align: 'right', editable: true, editrules: { required: false  },
                       editoptions: {
                           dataInit: function (elem) {
                               $(elem).autoNumeric();
                               $(elem).attr("style","text-align:right;");
                           }
                       }
                        },
                   <% if (ViewData.Model.ViewPrice) {%> 
                   { name: 'TransDetPrice', index: 'TransDetPrice', width: 200, sortable: false, align: 'right', editable: true, editrules: { required: false },
                       editoptions: {
                           dataInit: function (elem) {
                               $(elem).autoNumeric();
                               $(elem).attr("style","text-align:right;");
                           }
                       }
                        },
                   { name: 'TransDetDisc', index: 'TransDetDisc', width: 200, sortable: false, align: 'right', editable: true, editrules: { required: false },
                       editoptions: {
                           dataInit: function (elem) {
                               $(elem).autoNumeric();
                               $(elem).attr("style","text-align:right;");
                           }
                       }
                        },
                   { name: 'TransDetTotal', index: 'TransDetTotal', width: 200, sortable: false, align: 'right', editable: true, editrules: { required: false } ,
                       editoptions: {
                           dataInit: function (elem) {
                               $(elem).autoNumeric();
                               $(elem).attr("style","text-align:right;");
                           }
                       }
                        },
                   <%}%> 
                { name: 'TransDetDesc', index: 'TransDetDesc', width: 200, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false}}],

            pager: $('#listPager'),
            rowNum: -1,
            //              rowList: [20, 30, 50, 100],
            rownumbers: true,
            //              sortname: 'Id',
            //              sortorder: "asc",
            //              viewrecords: true,
            height: 150,
            caption: 'Daftar Detail',
            autowidth: true,
            editurl : '<%= Url.Action("Insert", "Inventory") %>',
            loadComplete: function () {
//                $('#list').setColProp('ItemId', { editoptions: { value: items} });
                $('#listPager_center').hide();
            },
            ondblClickRow: function (rowid, iRow, iCol, e) {
                //$("#list").editGridRow(rowid, editDialog);
            }, footerrow: true, userDataOnFooter: true, altRows: true 
            });
            jQuery("#list").jqGrid('navGrid', '#listPager',
                 { edit: false, add: true, del: true, search: false, refresh: true }, //options 
                  editDialog,
                insertDialog,
                deleteDialog,
                {}
            );
    });

//        var items = $.ajax({ url:  '<%= ResolveUrl("~/Master/Item/GetList") %>', async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the items.'); } }).responseText;
               

    
//function to generate tooltips
		function generateTooltips() {
		  //make sure tool tip is enabled for any new error label
//          alert('s');
			$("img[id*='error']").tooltip({
				showURL: false,
				opacity: 0.99,
				fade: 150,
				positionRight: true,
					bodyHandler: function() {
						return $("#"+this.id).attr("hovertext");
					}
			});

			//make sure tool tip is enabled for any new valid label
			$("img[src*='tick.gif']").tooltip({
				showURL: false,
					bodyHandler: function() {
						return "OK";
					}
			});
		}

        function OpenPopupItemSearch()
        {
          $("#popup_frame").attr("src", "<%= ResolveUrl("~/Master/Item/Search?Price=") %><%= Model.UsePrice.ToString() %>");
            $("#popup").dialog("open");
            return false;   
        }

         function SetItemDetail(itemId, itemName, price)
        {
//        alert(itemId);
//        alert(itemName);
//        alert(price);
  $("#popup").dialog("close");
          $('#ItemId').val(itemId);
          $('#ItemName').val(itemName);
           <% if (ViewData.Model.ViewPrice)
               {%>   
               $('#TransDetPrice').val(price.toString());
//               alert('SetItemDetail' + price);
            CalculateTotal();
                   <%
               }%> 
       
        }

         function SetTransDetail(src,transId)
        {
            $("#popup").dialog("close");
            var trans = $.parseJSON($.ajax({ url: '<%= Url.Action("GetJsonTrans","Inventory") %>?transId=' + transId, async: false, cache: false, success: function (data, result) { if (!result) alert('Failure to retrieve the trans.'); } }).responseText);
            if (trans) {
                if (trans.TransDate)
                {
                    var transDate = new Date(parseInt(trans.TransDate.substr(6)));
                    //alert('debug 3');
                    $("#Trans_TransDate").val(transDate.format('dd-mmm-yyyy'));
                }
                
                if (trans.TransDueDate)
                {
                    var transDate = new Date(parseInt(trans.TransDueDate.substr(6)));
                    //alert('debug 3');
                    $("#Trans_TransDueDate").val(transDate.format('dd-mmm-yyyy'));
                }

            $("#Trans_Id").val(trans.TransId); 
            $("#Trans_TransFactur").val(trans.TransFactur); 
            $("#Trans_WarehouseId").val(trans.WarehouseId); 
            $("#Trans_TransPaymentMethod").val(trans.TransPaymentMethod); 
            $("#Trans_TransBy").val(trans.TransBy); 
            $("#Trans_WarehouseIdTo").val(trans.WarehouseIdTo);

            setTimeout("$('#list').trigger('reloadGrid')",1000); 
             $("#btnPrint").removeAttr('disabled');
             $("#btnDelete").removeAttr('disabled');
        }

        }
</script>
