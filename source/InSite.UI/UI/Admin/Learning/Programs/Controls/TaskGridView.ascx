<%@ Control Language="C#" CodeBehind="TaskGridView.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.TaskGridView" %>

<asp:Repeater runat="server" ID="FolderRepeater">
    <ItemTemplate>
        <div class="mb-3">
            <h3><%# Eval("AchievementLabel") %></h3>

            <asp:Repeater ID="ItemRepeater" runat="server">
                <HeaderTemplate>
                    <div class="row p-2 ms-0 me-0 border-bottom border-top fw-bold bg-secondary">
                        <div class="col-4">
                            Achievement
                        </div>
                        <div class="col-2 text-center">
                            Planned
                        </div>
                        <div class="col-2 text-center">
                            Required
                        </div>
                        <div class="col-2 text-center">
                            Time-Sensitive
                        </div>
                        <div class="col-2 text-end">
                            Months
                        </div>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class='<%# Container.ItemIndex % 2 != 0 ? "bg-secondary": "" %>'>
                        <div class="row p-2 ms-0 me-0 border-bottom">
                            <div class="col-4">
                                <%# Eval("AchievementTitle") %>
                            </div>
                            <div class="col-2 text-center">
                                <%# GetBoolString("IsPlanned") %>
                            </div>
                            <div class="col-2 text-center">
                                <%# GetBoolString("IsRequired") %>
                            </div>
                            <div class="col-2 text-center IsTimeSensitive">
                                <%# GetBoolString("IsTimeSensitive") %>
                            </div>
                            <div class="col-2 text-end">
                                <%# Eval("LifetimeMonths") %>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </ItemTemplate>
</asp:Repeater>
