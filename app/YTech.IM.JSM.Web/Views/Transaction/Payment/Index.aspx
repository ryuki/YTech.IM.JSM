<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MyMaster.master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage<YTech.IM.JSM.Web.Controllers.ViewModel.PaymentViewModel>" %>
<%@ Import Namespace="YTech.IM.JSM.Web.Controllers.Helper" %>
<%@ Import Namespace="YTech.IM.JSM.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="title" runat="server">
    <%= ViewData.Model.Title %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <%
        if (false)
        {%>
    <script src="../../../Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script>
    <%
        }%>
    <%
        using (Ajax.BeginForm(new AjaxOptions
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
    <%=Html.AntiForgeryToken()%>
    <%=Html.Hidden("Id", (ViewData.Model.Payment != null) ? ViewData.Model.Payment.Id : "")%>
     <%=Html.Hidden("SelectedTransId", ViewData.Model.SelectedTransId)%>

    <div>
        <span id="toolbar" class="ui-widget-header ui-corner-all"><a id="newPayment" href="<%=Url.Action("?paymentType=" + ViewData.Model.Payment.PaymentType)%>">
            Baru</a>
            <button id="btnSave" name="btnSave" type="submit">
                Simpan</button>
            <%--<button id="btnDelete" name="btnDelete" type="submit">
            Hapus
            <%= ViewData.Model.Title %></button>
        <button id="btnList" name="btnList" type="button">
            Daftar
            <%= ViewData.Model.Title %></button>--%>
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
                    <tr>
                        <td>
                            <label for="PaymentDate">
                                Tanggal :</label>
                        </td>
                        <td>
                            <%=Html.TextBox("PaymentDate",
                                           (Model.Payment.PaymentDate.HasValue)
                                               ? Model.Payment.PaymentDate.Value.ToString(CommonHelper.DateFormat)
                                               : "")%>
                            <%=Html.ValidationMessage("PaymentDate")%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="CashAccountId">
                                <%=Model.CashAccountLabel%></label>
                        </td>
                        <td>
                            <%=Html.TextBox("CashAccountId", Model.CashAccountId)%>&nbsp;<img src='<%= Url.Content("~/Content/Images/window16.gif") %>'
                                style='cursor: hand;' id='imgCashAccountId' />
                            <%=Html.ValidationMessage("CashAccountId")%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <%=Html.TextBox("CashAccountName", Model.CashAccountName)%>
                            <%=Html.ValidationMessage("CashAccountName")%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <label for="PaymentDesc">
                                Keterangan :</label>
                        </td>
                        <td>
                            <%=Html.TextBox("PaymentDesc", Model.Payment.PaymentDesc)%>
                            <%=Html.ValidationMessage("PaymentDesc")%>
                        </td>
                    </tr>
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
        <iframe width='100%' height='420px' id="popup_frame" frameborder="0"></iframe>
    </div>
    <%
        }%>
    <script language="javascript" type="text/javascript">
 $(function () {
        $("#newPayment").button();
//        $("#Save").button();
        $("#PaymentDate").datepicker({ dateFormat: "dd-M-yy" });
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
            height: 380,
            width: '80%',
            modal: true,
            close: function (event, ui) {
                //$("#list").trigger("reloadGrid");
            }
        });

        $.jgrid.nav.addtext = "Tambah";
        $.jgrid.nav.edittext = "Edit";
        $.jgrid.nav.deltext = "Hapus";
        $.jgrid.edit.addCaption = "Tambah Detail Baru";
        $.jgrid.edit.editCaption = "Edit Detail";
        $.jgrid.del.caption = "Hapus Detail";
        $.jgrid.del.msg = "Anda yakin menghapus Detail yang dipilih?";
        var imgLov = '<%=Url.Content("~/Content/Images/window16.gif")%>';
        
        <% if (Model.Payment.PaymentType == EnumPaymentType.Piutang.ToString()) { %>
var transStatus = '<%=EnumTransactionStatus.Sales.ToString()%>';
<%  }  else { %>
           var transStatus = '<%=EnumTransactionStatus.Purchase.ToString()%>';
        <% } %>
        $("#list").jqGrid({
            url: '<%= Url.Action("ListSearchTrans", "Inventory") %>',
            postData: {
                transStatus: function () { return transStatus; }
            },
            datatype: 'json',
            mtype: 'GET',
            colNames: ['Kode Trans', 'No Faktur', 'Tanggal', 'Keterangan', 'Grand Total'],
                colModel: [
                    { name: 'Id', index: 'Id', width: 100, align: 'left', key: true, editrules: { required: true, edithidden: true }, hidedlg: true, hidden: true, editable: false },
                    { name: 'TransFactur', index: 'TransFactur', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true } },
                    { name: 'TransDate', index: 'TransDate', width: 200, align: 'left', editable: true, edittype: 'text', editrules: { required: true } },
                    { name: 'TransDesc', index: 'TransDesc', width: 200, sortable: false, align: 'left', editable: true, edittype: 'textarea', editoptions: { rows: "3", cols: "20" }, editrules: { required: false}},
                    { name: 'TransGrandTotal', index: 'TransGrandTotal', width: 200, align: 'right', editable: true, edittype: 'text', editrules: { required: true } }],

            pager: $('#listPager'),
            rowNum: -1,
            //              rowList: [20, 30, 50, 100],
            rownumbers: true,
            //              sortname: 'Id',
            //              sortorder: "asc",
            //              viewrecords: true,
            multiselect: true,
            height: 150,
            caption: 'Daftar Transaksi',
            autowidth: true,
            loadComplete: function () {
                $('#listPager_center').hide();
            },
            ondblClickRow: function (rowid, iRow, iCol, e) {
                //$("#list").editGridRow(rowid, editDialog);
            }, footerrow: true, userDataOnFooter: true, altRows: true
        });
        jQuery("#list").jqGrid('navGrid', '#listPager',
                 { edit: false, add: false, del: false, search: false, refresh: true }, //options 
                  {},
                {},
                {},
                {}
            );

             $('#imgCashAccountId').click(function () {
                                   OpenPopupCashAccountSearch();
                               });
    });

     function OpenPopupCashAccountSearch()
  {
          $("#popup_frame").attr("src", "<%= ResolveUrl("~/Master/Account/Search") %>?src=CashAccountId");
            $("#popup").dialog("open");
            return false;   
        }
        
         function SetAccountDetail(src,accountId, accountName)
        {
  $("#popup").dialog("close");
  if (src == 'AccountId') {
     $('#AccountId').attr('value', accountId);
          $('#AccountName').attr('value', accountName); 
}         
 else if (src == 'CashAccountId') {
     $('#CashAccountId').attr('value', accountId);
          $('#CashAccountName').attr('value', accountName);

  }
       
        }  

        
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

//set value for selected transid
$("#SelectedTransId").val(jQuery("#list").jqGrid('getGridParam','selarrrow'));
alert($("#SelectedTransId").val());
    var validateResult = $('form').validate({
    rules: {
     "PaymentDate": { required: true }
     ,"CashAccountId": { required: true }
    },
    messages: {
     "PaymentDate":  "<img id='PaymentDateerror' src='"+imgerror+"' hovertext='Tanggal tidak boleh kosong' />"
       ,"CashAccountId": "<img id='CashAccountIderror' src='"+imgerror+"' hovertext='Akun kas tidak boleh kosong' />" 
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

    return validateResult;
}


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
    </script>
</asp:Content>
