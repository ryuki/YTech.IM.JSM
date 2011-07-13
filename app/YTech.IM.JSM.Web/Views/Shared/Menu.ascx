<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %> 

<%@ Import Namespace="YTech.IM.JSM.Web.Controllers.Master" %>


<div id="accordion">
    <h3><a href="#">Home</a></h3>
    <div>
        <div>
            <%=Html.ActionLinkForAreas<HomeController>(c => c.Index(), "Home") %>
        </div>
    </div>

    <% if (Request.IsAuthenticated)
       {
    %>      

        <h3><a href="#">Data Pokok</a></h3>
        <div>
            <div>
                <%= Html.ActionLinkForAreas<BrandController>(c => c.Index(),"Master Merek") %>
            </div>
            <div>
                <%= Html.ActionLinkForAreas<WarehouseController>(c => c.Index(),"Master Gudang") %>
            </div>
      
        <div>
            <hr />
        </div>

        <div>
            <%= Html.ActionLinkForAreas<DepartmentController>(c => c.Index(),"Master Departemen") %>
        </div>
       

  
    <%
        }
    %>
</div>
