<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesNode.ascx.cs" Inherits="InSite.Admin.Courses.Activities.Controls.CompetenciesNode" %>

<div class="competency-node">

    <asp:Literal runat="server" ID="NodeTitle" />

    <asp:Repeater runat="server" ID="LeafRepeater">
        <HeaderTemplate>
            <div class="competency-leaves">
                <table class="table table-striped">
                    <tbody>
        </HeaderTemplate>
        <FooterTemplate>
                    </tbody>
                </table>
            </div>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td style="width:100%">
                    <%# Eval("Title") %>
                    <span class="form-text"><%# Eval("AssetNumber") %></span>
                </td>
                <td style="width:100px"><%# Eval("Code") %></td>
                <td style="width:40px; white-space:nowrap;">
                    <insite:IconButton runat="server" Name="trash-alt" CommandName="Delete"
                        ToolTip="Remove Competency"
                        ConfirmText="Are you sure you want to remove this competency from the document?"
                    />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Repeater runat="server" ID="NodeRepeater">
        <ItemTemplate>
            <insite:DynamicControl runat="server" ID="Container" />
        </ItemTemplate>
    </asp:Repeater>
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .competency-node,
        .competency-node > .competency-leaves {
            padding-left: 32px;
        }
        div.settings .competency-node h4 { font-size: 18px; color: #555; border-bottom: 0px !important; }
    </style>
</insite:PageHeadContent>