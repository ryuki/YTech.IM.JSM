<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPopup.Master" Inherits="System.Web.Mvc.ViewPage<YTech.IM.JSM.Web.Controllers.ViewModel.UserAdministration.RoleViewModel>" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
	Role: <%= Model.Role %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

	<link href='<% =Url.Content("~/Content/MvcMembership.css") %>' rel="stylesheet" type="text/css" />

	<h2 class="mvcMembership">Role: <%= Model.Role %></h2>
	<div class="mvcMembership-roleUsers">
		<% if(Model.Users.Count() > 0){ %>
			<ul class="mvcMembership">
				<% foreach(var user in Model.Users){ %>
				<li>
					<% =Html.ActionLink(user.UserName, "Details", "UserAdministration", new { id = user.ProviderUserKey },null)%>
					<% using(Html.BeginForm("RemoveFromRole", "UserAdministration", new{id = user.ProviderUserKey, role = Model.Role})){ %>
						<input type="submit" value="Remove From" />
					<% } %>
				</li>
				<% } %>
			</ul>
		<% }else{ %>
		<p>No users are in this role.</p>
		<% } %>
	</div>

</asp:Content>