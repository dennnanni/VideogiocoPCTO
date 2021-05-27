<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="ProgettoPCTO.ResetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <link rel="stylesheet" href="~/Stylesheets/StylesheetReset.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="pnlContainer" runat="server" Height="270px" Width="350px" Visible="true">
                <asp:Label ID="lblTitle" runat="server" Text="Label"><b>Reimposta password</b></asp:Label>
                <asp:Label ID="lblEmail" runat="server" Text="Label"><b>Email</b></asp:Label>
                <asp:TextBox runat="server" ID="txtEmail" placeholder="Enter Email"></asp:TextBox>
                <asp:Label ID="lblOtp" runat="server" Text="Label" Enabled="false"><b>OTP</b></asp:Label>
                <asp:TextBox runat="server" ID="txtOtp" TextMode="Password" placeholder="Enter OTP"></asp:TextBox>
                <asp:Button runat="server" ID="btnSend"  Text="Send" OnClick="btnSend_Click"  />
                <asp:HyperLink ID="hypLogin" runat="server" NavigateUrl="~/Login.aspx">Accedi</asp:HyperLink>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
