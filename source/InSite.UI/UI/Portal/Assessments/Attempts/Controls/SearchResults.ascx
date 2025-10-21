<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Assessments.Attempts.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="AttemptIdentifier" Translation="Header">
        <Columns>

        <asp:TemplateField HeaderText="Exam Form">
            <ItemTemplate>
                <%# Eval("Form.FormTitle") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Date and Time">
            <ItemTemplate>
                <%# FormatTime() %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Score" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end" ItemStyle-Wrap="False" >
            <ItemTemplate>
                <%# FormatScore() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end"  HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:Button runat="server" ButtonStyle="Primary" Icon="fas fa-search" Text="View" ToolTip="View Attempt" Width="100px"
                    NavigateUrl='<%# string.Format("/ui/portal/assessments/attempts/result?form={0}&attempt={1}", Eval("Form.FormIdentifier"), GetEncodedGuid()) %>'
                    Visible='<%# IsDisclosureVisible() %>' />
                <insite:Button runat="server" ButtonStyle="OutlineSecondary" Icon="fas fa-print" Text="Print" ToolTip="Print Attempt" Width="100px" CssClass="mt-2 d-block btn-bg-white"
                    CommandName="Print"
                    Visible='<%# IsDisclosureVisible() %>' />
            </ItemTemplate>
        </asp:TemplateField>

        </Columns>
    </insite:Grid>
</div>