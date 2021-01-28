<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProgettoPCTO.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Avventura testuale</title>
    <link rel="stylesheet" href="Stylesheet.css"/>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
        </div>

        <asp:UpdatePanel ID="pnlGameplay" UpdateMode="Conditional" runat="server"  Width="1300px" Height="700px">
            <ContentTemplate>
                <asp:Panel ID="pnlImages" runat="server" Width="850" Height="500"></asp:Panel>
                <asp:Panel ID="pnlCardinals" runat="server" Width="210" Height="170px">
                    <asp:Button ID="btnNorth" runat="server" Width="100" Height="50" Text="North" OnClick="btnCardinal_Click"/>
                    <asp:Button ID="btnEast" runat="server" Width="100" Height="50" Text="East" OnClick="btnCardinal_Click"/>
                    <asp:Button ID="btnSouth" runat="server" Width="100" Height="50" Text="South" OnClick="btnCardinal_Click"/>
                    <asp:Button ID="btnWest" runat="server" Width="100" Height="50" Text="West" OnClick="btnCardinal_Click"/>
                </asp:Panel>
                <asp:TextBox ID="txtStory" runat="server" TextMode="MultiLine" Width="420" Height="430" ReadOnly="True"></asp:TextBox>
                <asp:DropDownList ID="drpActions" runat="server" Width="330" Height="50"/>
                <asp:Button ID="btnDo" runat="server" Text="Agisci" Height="52" Width="92"/> 
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
