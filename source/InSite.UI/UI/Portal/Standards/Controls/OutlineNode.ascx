<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineNode.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.OutlineNode" %>

<ul class="asset-tree">

    <asp:Repeater runat="server" ID="ItemRepeater">
        <ItemTemplate>
            <li class='<%# (bool)Eval("IsExpanded") ? "asset-branch m-b-md" : "asset-branch m-b-md collapsed" %>' data-id='<%# Eval("StandardIdentifier") %>'>

                <div class="card">

                    <div class="card-body">

                        <div class="tree-commands">
                            <a href="#collapse" title="Collapse" class="btn-collapse"><i class="fas fa-minus"></i></a>
                            <a href="#expand" title="Expand" class="btn-expand"><i class="fas fa-plus"></i></a>
                        </div>

                        <div class="row">

                            <div class='col-md-12'>

                                <div class='<%# "title-" + Depth %>'>

                                    <span runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Icon")) %>'>
                                        <i class='<%# "fa " + Eval("Icon") %>'></i>
                                    </span>
                                    <span runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("CodePath")) %>' class="small text-body-secondary">
                                        <%# Eval("CodePath") %>
                                    </span>

                                    <%# Eval("Title") %>
                                    <%# GetSatisfactionHtml() %>

                                    <span runat="server" visible='<%# Eval("IsTheory") %>' class="badge bg-dark fs-6"><i class="fa fa-book"></i> <%# Translate("Theory") %></span>
                                    <span runat="server" visible='<%# Eval("IsPractical") %>' class="badge bg-info fs-6"><i class="far fa-hand-paper"></i> <%# Translate("Practical") %></span>

                                </div>

                                <asp:Repeater runat="server" ID="CompetencyContentRepeater">
                                    <ItemTemplate>
                                        <div class="competency-content">
                                            <h6><%# Eval("Title") %></h6>
                                            <%# Eval("Content") %>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <asp:Repeater runat="server" ID="ConnectionRepeater" Visible="false">
                                    <HeaderTemplate>
                                        <div class="row mt-3 competency-content">
                                            <div class="col-md-8">
                                                <h5><insite:Literal runat="server" Text="Connected Standards" /></h5>
                                    </HeaderTemplate>
                                    <FooterTemplate>
                                            </div>
                                        </div>
                                    </FooterTemplate>
                                    <ItemTemplate>
                                        <h6><%# Eval("Title") %></h6>

                                        <asp:Repeater runat="server" ID="ItemRepeater">
                                            <HeaderTemplate>
                                                <table class="table table-sm table-connections">
                                                    <tbody>
                                            </HeaderTemplate>
                                            <FooterTemplate>
                                                    </tbody>
                                                </table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td style="width:30px;">
                                                        <a href="#" data-action="connection-info" data-id="<%# Eval("Identifier") %>"><i class="far fa-search"></i></a>
                                                    </td>
                                                    <td>
                                                        <%# Eval("CodePath") %>
                                                        <%# HttpUtility.HtmlEncode(Eval("Title")) %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                        </div>
                    </div>
                </div>

                <!-- Outline Node -->

                <insite:DynamicControl runat="server" ID="ChildNodes" />

            </li>
        </ItemTemplate>
    </asp:Repeater>

</ul>
