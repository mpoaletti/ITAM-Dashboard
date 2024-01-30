<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="WebApplication1.Reports" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %></h2>
        <h3>Report on all items:&nbsp;
			<asp:Button ID="AllItemsReport" runat="server" OnClick="AllItemsReport_Click" Text="Generate Report" />
	</h3>
        <p>&nbsp;</p>
    </main>
</asp:Content>
