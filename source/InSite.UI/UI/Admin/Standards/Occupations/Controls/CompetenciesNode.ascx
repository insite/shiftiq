<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesNode.ascx.cs" Inherits="InSite.Admin.Standards.Occupations.Controls.CompetenciesNode" %>

<div class="competency-node">
    <asp:Literal runat="server" ID="NodeTitle" />

    <asp:Repeater runat="server" ID="LeafRepeater">
        <HeaderTemplate>
            <div class="competency-leaves">
                <table class="table">
                    <thead>
                        <tr>
                            <th style="width:40px;">
                                <asp:CheckBox runat="server" ID="AllSelected" />
                            </th>
                            <th>Title</th>
                            <th style="width:100px;">Number</th>
                            <th style="width:100px;">Code</th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
        <FooterTemplate>
                    </tbody>
                </table>
            </div>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td style="text-align:center;">
                    <asp:CheckBox runat="server" ID="IsSelected" />
                </td>
                <td>
                    <%# Eval("Title") %>
                </td>
                <td><%# Eval("AssetNumber") %></td>
                <td><%# Eval("Code") %></td>
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
    </style>
</insite:PageHeadContent>