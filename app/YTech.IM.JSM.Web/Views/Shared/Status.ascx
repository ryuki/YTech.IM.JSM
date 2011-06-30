<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %> 
<%
    if (TempData != null)
    {
        if (TempData[EnumCommonViewData.SaveState.ToString()] != null)
        {
            if (TempData[EnumCommonViewData.SaveState.ToString()].Equals(EnumSaveState.Success))
            {%>
    <div class="ui-state-highlight ui-corner-all" style="padding: 5pt; margin-bottom: 5pt;">
        <p>
            <span class="ui-icon ui-icon-info" style="float: left; margin-right: 0.3em;"></span>
            Data berhasil disimpan.</p>
    </div>
    <%
            }
            else if (TempData[EnumCommonViewData.SaveState.ToString()].Equals(EnumSaveState.Failed))
            {%>
    <div class="ui-state-error ui-corner-all" style="padding: 5pt; margin-bottom: 5pt;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin-right: 0.3em;"></span>
            Data gagal disimpan.
            <br />
            Detail Error : 
            <br />
              <%# TempData[EnumCommonViewData.ErrorMessage.ToString()] %>
        </p>
    </div>
    <%
            }
        }
    }
%>