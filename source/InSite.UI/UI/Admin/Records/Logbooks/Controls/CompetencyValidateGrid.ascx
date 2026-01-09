<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencyValidateGrid.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.CompetencyValidateGrid" %>

<asp:Repeater runat="server" ID="AreaRepeater">
    <HeaderTemplate>
        <table id="ValidateGridTable" class="table table-striped">
            <thead>
                <tr>
                    <th></th>
                    <th>
                        Competency
                    </th>
                    <th style="text-align:right; padding-right:20px;">
                        <insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/>
                    </th>
                    <th>
                        Satisfaction Level
                    </th>
                    <th runat="server" id="SkillRatingHeader">
                        Skill Rating
                    </th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td colspan="5">
                <b><%# Eval("Name") %></b>
            </td>
        </tr>

        <asp:Repeater runat="server" ID="CompetencyRepeater">
            <ItemTemplate>
                <tr class="competency-row" data-identifier='<%# Eval("Identifier") %>'>
                    <td style="width:10px;">

                    </td>
                    <td>
                        <%# Eval("Name") %>
                    </td>
                    <td style="text-align:right; padding-right:20px;">
                        <%# Eval("Hours", "{0:n2}") %>
                        <insite:IconLink runat="server" Name="pencil" ToolTip="Change"
                            NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/validators/change-experience-competency?experience={0}&competency={1}", ExperienceIdentifier, Eval("Identifier")) %>'
                            Visible="<%# IsValidator %>"
                        />
                        <insite:IconLink runat="server" Name="pencil" ToolTip="Change"
                            NavigateUrl='<%# string.Format("/ui/admin/records/logbooks/change-experience-competency?experience={0}&competency={1}", ExperienceIdentifier, Eval("Identifier")) %>'
                            Visible="<%# !IsValidator %>"
                        />
                    </td>
                    <td class="satisfaction-level w-25">
                        <asp:RadioButtonList runat="server" ID="SatisfactionLevel" />
                    </td>
                    <td runat="server" id="SkillRatingData" class="skill-rating">
                        <insite:ComboBox runat="server" ID="SkillRating" Width="100px" />
                        <asp:Literal ID="SkillRatingUnavailable" Visible="false" runat="server">N/A</asp:Literal>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>

<insite:LoadingPanel runat="server" ID="GridLoadingPanel" />

<insite:PageFooterContent runat="server">
<script type="text/javascript">
    (function () {
        $(".satisfaction-level label").addClass('form-label');
        $(".satisfaction-level input[type=radio]").addClass('form-check-input');
        $(".satisfaction-level input[type=radio]").on("click", function () {
            var $this = $(this);
            var competency = $this.closest("tr.competency-row").data("identifier");
            var value = $this.val();

            var $loadingPanel = $('#<%= GridLoadingPanel.ClientID %>').show();

            $.ajax({
                type: 'POST',
                data:
                {
                    experience: '<%= ExperienceIdentifier %>',
                    competency: competency,
                    action: 'change-satisfaction',
                    value: value
                },
                error: function () {
                    alert('An error occurred during operation.');
                },
                complete: function () {
                    $loadingPanel.hide();
                }
            });
        });

        $(".skill-rating select").on("change", function () {
            var $this = $(this);
            var competency = $this.closest("tr.competency-row").data("identifier");
            var value = $this.val();

            var $loadingPanel = $('#<%= GridLoadingPanel.ClientID %>').show();

            $.ajax({
                type: 'POST',
                data:
                {
                    experience: '<%= ExperienceIdentifier %>',
                    competency: competency,
                    action: 'change-skill-rating',
                    value: value
                },
                error: function () {
                    alert('An error occurred during operation.');
                },
                complete: function () {
                    $loadingPanel.hide();
                }
            });
        });
    })();
</script>
</insite:PageFooterContent>
