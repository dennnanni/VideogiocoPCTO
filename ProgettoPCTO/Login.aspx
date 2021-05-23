<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ProgettoPCTO.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Login</title>
    <link rel="stylesheet" href="StylesheetLogin.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="pnlContainer" runat="server" Height="270px" Width="350px" Visible="true">
                <asp:Label ID="lblTitle" runat="server" Text="Label"><b>Avventura testuale</b></asp:Label>
                <br />
                <asp:Label ID="lblUsername" runat="server" Text="Label"><b>Username</b></asp:Label>
                <asp:TextBox runat="server" ID="txtUsername" placeholder="Enter Username"></asp:TextBox>
                <asp:Label ID="lblPassword" runat="server" Text="Label"><b>Password</b></asp:Label>
                <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" placeholder="Enter Password"></asp:TextBox>
                <asp:Button runat="server" ID="btnLogin"  Text="Login" OnClick="btnLogin_Click" />
                <asp:HyperLink ID="hypResetPassword" runat="server" NavigateUrl="~/Login.aspx?action=&quot;reset&quot;">Reimposta password</asp:HyperLink>
                <asp:HyperLink ID="hypSignUp" runat="server" NavigateUrl="~/Login.aspx?action=&quot;signup&quot;">Registrati</asp:HyperLink>
            </asp:Panel>
            <asp:Panel ID="pnlContainerReset" runat="server" Height="270px" Width="350px" Visible="false">
                <asp:Label ID="lbl" runat="server" Text="Label"><b>Avventura testuale</b></asp:Label>
                <br />
                <asp:Label ID="Label2" runat="server" Text="Label"><b>Username</b></asp:Label>
                <asp:TextBox runat="server" ID="TextBox1" placeholder="Enter Username"></asp:TextBox>
                <asp:Label ID="Label3" runat="server" Text="Label"><b>Password</b></asp:Label>
                <asp:TextBox runat="server" ID="TextBox2" TextMode="Password" placeholder="Enter Password"></asp:TextBox>
                <asp:Button runat="server" ID="Button1"  Text="Login" OnClick="btnLogin_Click" />
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Login.aspx?action=&quot;reset&quot;">Reimposta password</asp:HyperLink>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Login.aspx?action=&quot;signup&quot;">Registrati</asp:HyperLink>
            </asp:Panel>

            <asp:Panel ID="pnlContainerSignUp" runat="server" Height="270px" Width="350px" Visible="false">
                <asp:Label ID="Label1" runat="server" Text="Label"><b>Registrazione</b></asp:Label>
                <br />
                <asp:Label ID="Label4" runat="server" Text="Label"><b>Username</b></asp:Label>
                <asp:TextBox runat="server" ID="txtNewUsername" placeholder="Enter Username"></asp:TextBox>
                <asp:Label ID="Label6" runat="server" Text="Label"><b>Email</b></asp:Label>
                <asp:TextBox runat="server" ID="txtEmail" placeholder="Enter email"></asp:TextBox>
                <asp:Label ID="Label5" runat="server" Text="Label"><b>Password</b></asp:Label>
                <asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" placeholder="Enter Password"></asp:TextBox>
                <asp:Button runat="server" ID="btnSignUp"  Text="Login" OnClick="btnLogin_Click" />
                <asp:HyperLink ID="hypLogIn" runat="server" NavigateUrl="~/Login.aspx">Accedi</asp:HyperLink>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
