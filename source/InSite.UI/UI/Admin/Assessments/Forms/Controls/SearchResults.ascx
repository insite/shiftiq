<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Assessments.Forms.Controls.SearchResults" %>
<%@ Import Namespace="Humanizer" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Form Name and Title">
            <ItemTemplate>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&form=<%# Eval("FormIdentifier") %>"><%# Eval("FormName") %></a>
                <div class="form-text">
                    <%# Eval("FormName") == Eval("FormTitle") ? "" : Eval("FormTitle") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Asset #" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&form=<%# Eval("FormIdentifier") %>"><%# Eval("FormAsset") %>.<%# Eval("FormAssetVersion") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Questions" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# "item".ToQuantity((int)Eval("FieldCount")) %>
                <div class="form-text">
                    <%# "section".ToQuantity((int)Eval("SectionCount")) %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Publication Status" DataField="FormPublicationStatus" HeaderStyle-Wrap="False" />

        <asp:TemplateField HeaderText="Attempts" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# "complete".ToQuantity((int)Eval("AttemptSubmittedCount")) %>
                <div class="form-text">
                    <%# "start".ToQuantity((int)Eval("AttemptStartedCount")) %>    
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Success Rate" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# GetSuccessRate(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField ItemStyle-Wrap="false" ItemStyle-Width="30px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="chart-bar" ToolTip="Report"
                    NavigateUrl='<%# Eval("FormIdentifier", "/ui/admin/assessments/attempts/report?form={0}") %>'
                    Visible='<%# (int)Eval("AttemptSubmittedCount") > 0 %>' />
            </ItemTemplate>
        </asp:TemplateField>
            
    </Columns>
</insite:Grid>