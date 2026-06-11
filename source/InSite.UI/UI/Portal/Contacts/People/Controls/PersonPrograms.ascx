<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonPrograms.ascx.cs" Inherits="InSite.UI.Portal.Contacts.People.Controls.PersonPrograms" %>

<div class="card">
    <div class="card-body">

        <asp:Literal runat="server" ID="NoPrograms" Text="No programs to display." />

        <insite:Grid runat="server" ID="Grid">
            <Columns>
                <asp:BoundField DataField="ProgramName" HeaderText="Program Name" />

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

    </div>
</div>
