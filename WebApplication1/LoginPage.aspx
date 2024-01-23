<%@ Page Title="Login Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LoginPage.aspx.cs" Inherits="WebApplication1.LoginPage" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<h2>ITAM Login</h2>
		<ContentTemplate>
			<table style="width: 100%;">
				<tr>
					<td class="text-left" style="width: 268px; " rowspan="3">
						<img alt="University of Wisconsin Logo" src="swooshLOGOrevSmall.jpg" style="width: 175px; height: 110px" /></td>
					<td class="text-right" style="width: 268px; height: 22px"></td>
					<td style="height: 22px; width: 265px"></td>
					<td style="height: 22px"></td>
				</tr>
				<tr>
					<td class="text-right" style="width: 268px"></td>
					<td style="width: 265px"></td>
					<td></td>
				</tr>
				<tr>
					<td style="width: 268px; height: 35px;" class="text-right"></td>
					<td style="width: 265px; height: 35px;" class="text-justify">
						<asp:Button ID="bttnLogin" runat="server" Font-Names="Montserrat" Text="Login" OnClick="bttnLogin_Click" /></td>
					<td style="height: 35px"></td>
				</tr>
			</table>
		</ContentTemplate>
</asp:Content>
