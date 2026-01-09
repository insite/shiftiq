<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserScoreGrid.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.UserScoreGrid" %>

<div runat="server" id="NoScores" class="alert alert-warning" role="alert">
    There are no scores
</div>

<asp:Panel runat="server" ID="ScorePanel">
    <div class="row mb-3">
        <div class="col-lg-6">
            <insite:TextBox runat="server" ID="FilterText" Width="300" EmptyMessage="Filter" CssClass="d-inline-block" />
            <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />

            <insite:PageFooterContent runat="server">
                <script type="text/javascript"> 
                    (function () {
                        Sys.Application.add_load(function () {
                            $('#<%= FilterText.ClientID %>')
                                .off('keydown', onKeyDown)
                                .on('keydown', onKeyDown);
                        });

                        function onKeyDown(e) {
                            if (e.which === 13) {
                                e.preventDefault();
                                $('#<%= FilterButton.ClientID %>')[0].click();
                            }
                        }
                    })();
                </script>
            </insite:PageFooterContent>
        </div>
        <div class="col-lg-6" style="text-align:right;">
            Not Included to Report:
            <asp:RadioButtonList runat="server" ID="ShowNotIncludedToReport" RepeatLayout="Flow" RepeatDirection="Horizontal">
                <asp:ListItem Value="Hide" Text="Hide" Selected="true" />
                <asp:ListItem Value="Show" Text="Show" />
            </asp:RadioButtonList>
        </div>
    </div>
        
    <asp:Repeater runat="server" ID="ItemRepeater">
        <HeaderTemplate>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Gradebook</th>
                        <th>Achievement</th>
                        <th>Score</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td style="width:65%;">
                    <div style='<%# "padding-left:" + (10 * (int)Eval("Level")) + "px" %>'>
                        <a runat="server" visible='<%# Eval("Url") != null %>' href='<%# Eval("Url") %>'>
                            <%# Eval("Name") %>
                        </a>
                        <asp:Literal runat="server" Visible='<%# Eval("Url") == null %>' Text='<%# Eval("Name") %>' />
                        <div class="form-text"><%# Eval("Comment") %></div>
                    </div>
                </td>
                <td style="width:35%;">
                    <a runat="server" visible='<%# Eval("AchievementIdentifier") != null %>' href='<%# Eval("AchievementIdentifier", "/ui/admin/records/achievements/outline?id={0}") %>'>
                        <%# Eval("AchievementTitle") %>
                    </a>
                </td>
                <td style="white-space:nowrap;">
                    <%# Eval("ScoreValue") %>
                </td>
                <td style="width:40px;text-align:right;">
                    <insite:IconLink runat="server"
                        Visible='<%# (int)Eval("Level") != 0 %>'
                        ToolTip="Edit Score"
                        NavigateUrl='<%# "/ui/admin/records/scores/change?gradebook=" + Eval("GradebookIdentifier") + "&item=" + Eval("GradebookItemKey") + "&student=" + UserIdentifier + "&backToContact=1" %>'
                        Name="pencil"
                    />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>
