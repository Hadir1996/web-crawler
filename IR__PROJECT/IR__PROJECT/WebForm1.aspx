<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="IR__PROJECT.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Search Engine</title>
     <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 419px;
        }
        .auto-style3 {
            width: 419px;
            height: 157px;
        }
        .auto-style4 {
            height: 157px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Search" />
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Search by Sequence" />
        <br />
        <table class="auto-style1">
            <tr>
                <td class="auto-style3">
                    <asp:CheckBox ID="SpeliingCorrection" runat="server" Text="Spelling Correction" />
                    <br />
                    <asp:CheckBox ID="Soundex1" runat="server" Text="Soundex" />
                    <asp:ListBox ID="ListBox2" runat="server" style="margin-left: 275px" Width="261px">
                        <asp:ListItem> Did You Mean :</asp:ListItem>
                    </asp:ListBox>
                </td>
                <td class="auto-style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style2">
        <asp:ListBox ID="ListBox1" runat="server"></asp:ListBox>
    
                </td>
                <td>&nbsp;</td>
            </tr>
        </table>
        <br />
        <br />
    
        </div>
    </form>
</body>
</html>
