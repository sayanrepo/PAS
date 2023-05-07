<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportTemplate.aspx.cs" Inherits="BaseSite.Reports.ReportTemplate" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"

    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
 
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<!DOCTYPE html>
 <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE11">
 
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form2" runat="server">
    <div>
        <asp:ScriptManager ID="scriptManagerReport" runat="server">
         </asp:ScriptManager>
 
        <rsweb:ReportViewer runat="server" Width="99.9%" Height="100%" ID="rvSiteMapping" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt"
             ShowPrintButton="true" ShowZoomControl="true" AsyncRendering="false">
        </rsweb:ReportViewer>                  
    </div>
    </form>
</body>
</html>
