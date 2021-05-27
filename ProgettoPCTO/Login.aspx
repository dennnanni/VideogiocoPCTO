<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ProgettoPCTO.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Login</title>
    <link rel="stylesheet" href="~/Stylesheets/StylesheetLogin.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="pnlContainer" runat="server" Height="270px" Width="350px" Visible="true">
                <asp:Label ID="lblTitle" runat="server" Text="Label"><b>Avventura testuale</b></asp:Label>
                <asp:Label ID="lblUsername" runat="server" Text="Label"><b>Username</b></asp:Label>
                <asp:TextBox runat="server" ID="txtUsername" placeholder="Enter Username"></asp:TextBox>
                <asp:Label ID="lblPassword" runat="server" Text="Label"><b>Password</b></asp:Label>
                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" placeholder="Enter Password"></asp:TextBox>
                <asp:Button runat="server" ID="btnLogin"  Text="Login" OnClick="btnLogin_Click" />
                <asp:HyperLink ID="hypResetPassword" runat="server" NavigateUrl="~/ResetPassword.aspx">Reimposta password</asp:HyperLink>
                <asp:HyperLink ID="hypSignUp" runat="server" NavigateUrl="~/SignUp.aspx">Registrati</asp:HyperLink>
            </asp:Panel>
           
        </div>
    </form>
</body>
</html>
