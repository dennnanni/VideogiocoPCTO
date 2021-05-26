<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProgettoPCTO.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Avventura testuale</title>
    <link rel="stylesheet" href="~/Stylesheets/Stylesheet.css"/>
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
                    <asp:Button ID="btnAvanti" runat="server" Width="100" Height="50" Text="Avanti" OnClick="btnCardinal_Click"/>
                    <asp:Button ID="btnDestra" runat="server" Width="100" Height="50" Text="Destra" OnClick="btnCardinal_Click"/>
                    <asp:Button ID="btnIndietro" runat="server" Width="100" Height="50" Text="Indietro" OnClick="btnCardinal_Click"/>
                    <asp:Button ID="btnSinistra" runat="server" Width="100" Height="50" Text="Sinistra" OnClick="btnCardinal_Click"/>
                </asp:Panel>
                <asp:TextBox ID="txtStory" runat="server" TextMode="MultiLine" Width="420" Height="430" ReadOnly="True"></asp:TextBox>
                <asp:DropDownList ID="drpActions" runat="server" Width="330" Height="50"/>
                <asp:Button ID="btnDo" runat="server" Text="Agisci" Height="52" Width="92" OnClick="btnDo_Click"/> 
                <asp:ListBox ID="lstInventory" runat="server" Width="210" Height="130" />
                <asp:Button ID="btnDrop" runat="server" Text="Lascia" Height="30" Width="100" OnClick="btnDrop_Click"/>
                <asp:Button ID="btnUse" runat="server" Text="Usa" Height="30" Width="100" OnClick="btnUse_Click"/>
                <asp:Panel ID="pnlStats" runat="server" Width="210" Height="130">
                    <asp:Label ID="lblExperience" runat="server" Text="Esperienza: 0" Width="210" Height="10"></asp:Label>
                    <asp:Label ID="lblHealth" runat="server" Text="Salute: 100" Width="210" Height="10"></asp:Label>
                    <asp:Label ID="lblStrength" runat="server" Text="Forza: 50" Width="210" Height="10"></asp:Label>
                </asp:Panel>
                <asp:Button ID="btnLogOut" runat="server" Text="Disconnettiti" Height="50" Width="210" OnClick="btnLogOut_Click"/>
                <asp:Button ID="btnLoad" runat="server" Text="Carica profilo" Height="50" Width="210"/>
                <asp:Button ID="btnRestart" runat="server" Text="Ricomincia" Height="50" Width="210"/>
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
