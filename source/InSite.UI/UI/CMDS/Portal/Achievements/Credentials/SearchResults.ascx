<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.CMDS.Portal.Achievements.Credentials.SearchResults" %>
<%@ Import Namespace="InSite.Cmds.Infrastructure" %>

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

                                        <insite:Grid runat="server" ID="Grid" AllowPaging="false" AllowCustomPaging="false">
                                            <Columns>

                                                <asp:TemplateField HeaderText="Achievement">
                                                    <ItemTemplate>
                                                        <asp:Literal runat="server" ID="AchievementTitle" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Status" ItemStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Eval("ValidationStatus") %>
                                                        <span class="form-text"><%# Eval("Score") %></span>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Completion" ItemStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Eval("DateCompletedHtml") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Expiration" ItemStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%# Eval("DateExpiredHtml") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Files" ItemStyle-Width="60">
                                                    <ItemTemplate>
                                                        <asp:Repeater ID="Attachments" runat="server">
                                                            <ItemTemplate>
                                                                <insite:IconLink runat="server" Type="Regular" Name="download" ToolTip='<%# Eval("Title") %>' 
                                                                    Target="_blank" NavigateUrl='<%# CmdsUploadProvider.GetFileRelativeUrl((Guid)Eval("Identifier"), (string)Eval("Name")) %>' />
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Flags" ItemStyle-Width="60">
                                                    <ItemTemplate>
                                                        <insite:Icon runat="server" ID="FlagIcon" Type="Regular" Name="flag-checkered" />
                                                        <insite:Icon runat="server" ID="TimeSensitiveIcon" Type="Regular" Name="history" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </insite:Grid>

                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </div>

                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
