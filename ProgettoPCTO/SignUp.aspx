<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignUp.aspx.cs" Inherits="ProgettoPCTO.SignUp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Registrazione</title>
    <link rel="stylesheet" href="~/Stylesheets/StylesheetSignup.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="pnlContainer" runat="server" Height="270px" Width="350px" Visible="true">
                <asp:Label ID="lblTitle" runat="server" Text="Label"><b>Registrazione</b></asp:Label>
                <asp:Label ID="lblUsername" runat="server" Text="Label"><b>Username</b></asp:Label>
                <asp:TextBox runat="server" ID="txtUsername" placeholder="Enter Username"></asp:TextBox>
                <asp:Label ID="lblEmail" runat="server" Text="Label"><b>Email</b></asp:Label>
                <asp:TextBox runat="server" ID="txtEmail" placeholder="Enter Email"></asp:TextBox>
                <asp:Label ID="lblPassword" runat="server" Text="Label"><b>Password</b></asp:Label>
                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" placeholder="Enter Password"></asp:TextBox>
                <asp:Button runat="server" ID="btnRegister"  Text="Register" OnClick="btnRegister_Click"  />
                <asp:HyperLink ID="hypLogin" runat="server" NavigateUrl="~/Login.aspx">Accedi</asp:HyperLink>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
