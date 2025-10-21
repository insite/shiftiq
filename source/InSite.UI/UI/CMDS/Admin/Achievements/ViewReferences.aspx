<%@ Page Language="C#" CodeBehind="ViewReferences.aspx.cs" Inherits="InSite.Cmds.Admin.Achievements.Forms.ViewDependencies"
    MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <cmds:CmdsAlert runat="server" ID="ScreenStatus" />

    <div class="alert alert-info">
        <asp:Literal runat="server" ID="InstructionText" />
    </div>
    
    <div>
        <asp:Repeater runat="server" ID="ReferenceRepeater">
            <HeaderTemplate><ul></HeaderTemplate>
            <ItemTemplate>
                <li>
                    <span class="badge bg-custom-default"><%# Eval("Type") %></span>
                    <%# Eval("Name") %>
                </li>
            </ItemTemplate>
            <FooterTemplate></ul></FooterTemplate>
        </asp:Repeater>
    </div>

    <insite:CloseButton runat="server" ID="CloseButton" />

</asp:Content>
