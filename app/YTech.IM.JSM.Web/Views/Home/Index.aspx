<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MyMaster.Master" AutoEventWireup="true"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
 <% if (false)
       { %>
    <script src="../../Scripts/jquery-1.5-vsdoc.js" type="text/javascript"></script>
    <% } %>
    <h2>Sistem Informasi Manajemen Inventori JSM</h2>
    <%--<input type="button" id="btnTest" value='line 1&#13;&#10;line 2' />

    <script type="text/javascript">
        $(document).ready(function () {
            var i = 3;
            $("#btnTest").click(function () {
                $("#btnTest").val($("#btnTest").val() + "\nline " + i); 
                i = i + 1;
            });
        });
    </script>--%>
</asp:Content>
