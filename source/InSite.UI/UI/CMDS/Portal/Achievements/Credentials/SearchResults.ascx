<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.CMDS.Portal.Achievements.Credentials.SearchResults" %>
<%@ Import Namespace="InSite.Cmds.Infrastructure" %>

<style>

    .credential-grid tbody tr:nth-child(4n+1),
    .credential-grid tbody tr:nth-child(4n+2) {
        background-color: #F5F5F5 !important;
    }
    .credential-grid tbody tr:nth-child(2n+1) {
        border-top: 1px solid #C3D0DC;
    }
    .credential-grid tbody tr:nth-child(2n+1) td { padding: 10px 10px 0 10px; }
    .credential-grid tbody tr:nth-child(2n+2) td { padding: 0px 10px 10px 10px; }
    .credential-grid thead tr th { padding: 10px; }

</style>

<div class="row">
    <div class="col-lg-3">
        <ul class="nav nav-pills flex-column" role="tablist">
            <asp:Repeater runat="server" ID="AchievementTypeLinkRepeater">
                <ItemTemplate>

                    <li class="nav-item" role="presentation">
                        <a
                            class='nav-link <%# Container.ItemIndex == 0 ? "active": "" %>'
                            href='<%# "#vt" + Container.ItemIndex %>'
                            data-bs-toggle="tab"
                            role="tab"
                            aria-controls='<%# "#vt" + Container.ItemIndex %>'
                            aria-selected='<%# Container.ItemIndex == 0 ? "true": "" %>'
                        >
                            <asp:Literal runat="server" ID="Title" />
                        </a>
                    </li>

                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
    <div class="col-lg-9 tab-content">
        <asp:Repeater runat="server" ID="AchievementTypeBodyRepeater">
            <ItemTemplate>
                <div id='<%# "vt" + Container.ItemIndex %>' class='tab-pane fade <%# Container.ItemIndex == 0 ? "show active": "" %>' role="tabpanel">

                    <div class="card">
                        <div class="card-body">

                            <asp:Repeater runat="server" ID="AchievementCategoryRepeater">
                                <ItemTemplate>

                                    <asp:Literal runat="server" ID="AchievementCategoryTitle" />

                                    <table class="credential-grid">
                                        
                                        <thead>
                                            <tr>
                                                <th></th>
                                                <th>Achievement</th>
                                                <th>Status</th>
                                                <th>Completion</th>
                                                <th>Expiration</th>
                                                <th>Files</th>
                                            </tr>
                                        </thead>

                                        <tbody>

                                        <asp:Repeater runat="server" ID="Repeater">
                                            <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <insite:Icon runat="server" ID="FlagIcon" Type="Solid" Name="flag-checkered" />
                                                </td>
                                                <td>
                                                    <div class="mb-1">
                                                        <asp:Literal runat="server" ID="AchievementTitle" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <%# Eval("ValidationStatus") %>
                                                    <span class="form-text"><%# Eval("Score") %></span>
                                                </td>
                                                <td><%# Eval("DateCompletedHtml") %></td>
                                                <td><%# Eval("DateExpiredHtml") %></td>
                                                <td>
                                                    <asp:Repeater ID="Attachments" runat="server">
                                                        <ItemTemplate>
                                                            <insite:IconLink runat="server" Type="Regular" Name="download" ToolTip='<%# Eval("Title") %>'
                                                                Target="_blank" NavigateUrl='<%# CmdsUploadProvider.GetFileRelativeUrl((Guid)Eval("Identifier"), (string)Eval("Name")) %>' />
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td colspan="5">
                                                    <asp:Literal runat="server" ID="AchievementLabels" />
                                                </td>
                                            </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        </tbody>

                                    </table>

                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </div>

                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
