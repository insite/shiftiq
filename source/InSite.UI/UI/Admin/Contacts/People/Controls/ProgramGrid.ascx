<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramGrid.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.ProgramGrid" %>

<div runat="server" id="NoPrograms" class="alert alert-warning" role="alert">
    No programs to display
</div>

<div runat="server" id="FilterPanel" class="mb-3">
    <insite:TextBox runat="server" ID="FilterTextBox" Width="300" EmptyMessage="Filter" CssClass="d-inline-block" />
    <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />
    <insite:PageFooterContent runat="server">
        <script type="text/javascript"> 
            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= FilterTextBox.ClientID %>')
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

<insite:Grid runat="server" ID="Grid">
    <Columns>
        <asp:HyperLinkField
            DataTextField="ProgramName"
            DataNavigateUrlFields="ProgramId"
            DataNavigateUrlFormatString="/ui/admin/learning/programs/outline?id={0}"
        />

        <asp:TemplateField HeaderText="Assigned">
            <ItemTemplate>
                <%# LocalizeDate(Eval("ProgressAssigned")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Completed">
            <ItemTemplate>
                <%# LocalizeDate(Eval("ProgressCompleted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Time Taken">
            <ItemTemplate>
                <%# Eval("DaysTaken") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Progress" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <%# Eval("CompletionCounter") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Completion" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <%# Eval("CompletionPercent") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
