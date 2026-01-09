<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesNode.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.CompetenciesNode" %>

<div class="competency-node">
    <asp:Literal runat="server" ID="NodeTitle" />

    <asp:Repeater runat="server" ID="LeafRepeater">
        <HeaderTemplate>
            <div class="competency-leaves">
                <table class="table">
                    <thead>
                        <tr>
                            <th><%# Translate("Title") %></th>
                            <th style="width:100px;"><%# Translate("Number") %></th>
                            <th style="width:100px;"><%# Translate("Code") %></th>

                            <th style="width:40px;">

                                <p style="display:<%= DocumentIsTemplate ? "none;" : "block;" %> margin: 0 auto; position: relative; top: -48px;">
                                    <insite:IconButton runat="server" Name="trash-alt" CommandName="DeleteAll" CommandArgument='<%# Eval("ParentStandardIdentifier") %>'
                                        ToolTip="Remove Competency"
                                        ConfirmText="Are you sure you want to remove this competency from the document?" 
                                    /></p>

                            </th>
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
                <td>
                    <%# Eval("Title") %>
                </td>
                <td><%# Eval("AssetNumber") %></td>
                <td><%# Eval("Code") %></td>
                <td style="white-space:nowrap;">
                    <div style='display:<%= DocumentIsTemplate ? "none" : "block" %>'>
                        <insite:IconButton runat="server" Name="trash-alt" CommandName="Delete"
                            ToolTip="Remove Competency"
                            ConfirmText="Are you sure you want to remove this competency from the document?"
                        />
                    </div>
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
    </style>
</insite:PageHeadContent>