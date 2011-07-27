<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPopup.master"
    AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<DetailItemFormViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <% if (false)
       { %>
    <script src="../../../Scripts/jquery-1.6.2-vsdoc.js" type="text/javascript"></script>
    <% } %>
    <% using (Ajax.BeginForm(new AjaxOptions
                                       {
                                           //UpdateTargetId = "status",
                                           InsertionMode = InsertionMode.Replace,
                                           OnSuccess = "onSavedSuccess",
                                           OnBegin = "validateForm"

                                       }

          ))
       {%>
    <%= Html.AntiForgeryToken() %>
    <table id='listPacketItemCat' class="scroll" cellpadding="0" cellspacing="0">
        <tr>
            <th align="center">
                Kategori Perawatan
            </th>
            <th align="center">
                Produk
            </th>
        </tr>
        <tbody>
            <%
                MPacketItemCat packetItemCat;
                for (int i = 0; i < Model.PacketItemCatList.Count; i++)
                {
                    packetItemCat = Model.PacketItemCatList[i];%>
            <tr>
                <td>
                    <%= packetItemCat.ItemCatId.ItemCatName %>
                </td>
                <td>
                    <input id="txtItemId_<%= packetItemCat.ItemCatId.Id %>" name="txtItemId_<%= packetItemCat.ItemCatId.Id %>"
                        type="text" />&nbsp;<img src='<%= Url.Content("~/Content/Images/window16.gif") %>'
                            style='cursor: hand;' id='imgItemId' data="<%= packetItemCat.ItemCatId.Id %>"
                            class="imgTrigger" />
                    <input id="txtItemName_<%= packetItemCat.ItemCatId.Id %>" name="txtItemName_<%= packetItemCat.ItemCatId.Id %>"
                        type="text" />
                </td>
            </tr>
            <%} %>
        </tbody>
        <tfoot>
            <tr>
                <td colspan="2" align="center">
                    <input id="Submit1" type="submit" value="Simpan" />
                </td>
            </tr>
        </tfoot>
    </table>
    <% }%>
    <div id="dialog" title="Status">
        <p>
        </p>
    </div>
    <div id="error" title="Error">
        <span id="error_msg"></span>
    </div>
    <div id='popup'>
        <iframe width='100%' height='100%' id="popup_frame" frameborder="0"></iframe>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#Submit1").button();
            tableToGrid('listPacketItemCat');
            $("#dialog").dialog({
                autoOpen: false
            });
            $("#popup").dialog({
                autoOpen: false,
                height: 400,
                width: '80%',
                modal: true,
                close: function (event, ui) {

                }
            });
            
        $("div#error").dialog({
            autoOpen: false
        });

            $(".imgTrigger").click(function () {
                var catid = $(this).attr('data');
                OpenPopupItemSearch(catid);
            });
             });
           function validateForm()
           {
var imgerror = '<%= Url.Content("~/Content/Images/cross.gif") %>';
                return $('form').validate({
                rules: {
                <%
                MPacketItemCat packetItemCat1;
                for (int i = 0; i < Model.PacketItemCatList.Count; i++)
                {
                    packetItemCat1 = Model.PacketItemCatList[i];%>
                    "txtItemId_<%= packetItemCat1.ItemCatId.Id %>" : { required: true  } 
                    <% if (i != Model.PacketItemCatList.Count-1){	%>,<% } %>
                      <%} %>
                      },  messages: {
                      <%
                MPacketItemCat packetItemCat2;
                for (int i = 0; i < Model.PacketItemCatList.Count; i++)
                {
                    packetItemCat2 = Model.PacketItemCatList[i];%>
                    "txtItemId_<%= packetItemCat2.ItemCatId.Id %>" : "<img id='TransByerror' src='"+imgerror+"' hovertext='Pilih Produk' />" 
                    <% if (i != Model.PacketItemCatList.Count-1){	%>,<% } %>
                      <%} %>
 },        invalidHandler: function(form, validator) {          var errors = validator.numberOfInvalids();
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

        function OpenPopupItemSearch(catid) {
            var src = "<%= ResolveUrl("~/Master/Item/Search?Price=") %><%= EnumPrice.Sale.ToString() %>";
            src += "&src=" + catid;
            src += "&itemCatId=" + catid;
            $("#popup_frame").attr("src", src);
            $("#popup").dialog("open");
            return false;
        }

        function SetItemDetail(src, itemId, itemName, price) {
            //        alert(itemId);
            //        alert(itemName);
            //        alert(price);
            $("#popup").dialog("close");
            $('#txtItemId_' + src).attr('value', itemId);
            $('#txtItemName_' + src).attr('value', itemName);
        }

        function onSavedSuccess(e) {
            var returndata = e.get_response().get_object();
            var success = returndata.Success;
//            alert(success);
            if (success == true) {
                $('#Submit1').attr('disabled', 'disabled');
                    window.parent.ClosePopUp();
            }
            else {
                var msg = returndata.Message;
                $('#dialog p:first').text(msg);
                $("#dialog").dialog("open");
            }
    
        }
    </script>
</asp:Content>
