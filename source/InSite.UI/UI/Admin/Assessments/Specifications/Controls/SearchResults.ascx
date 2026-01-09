<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Assessments.Specifications.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Specification Name">
            <ItemTemplate>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&spec=<%# Eval("SpecIdentifier") %>"><%# Eval("SpecName") %></a>
                <div class="form-text">
                    Bank: <%# Eval("BankName") %> <%# Eval("BankEdition") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Type">
            <ItemTemplate>
                <%# Eval("SpecType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Consequence">
            <ItemTemplate>
                <%# Eval("SpecConsequence") %> 
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Asset #" HeaderStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("SpecAsset") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Scoring">
            <ItemTemplate>
                Pass &ge; <%# Eval("CalcPassingScore","{0:p0}") %> 
                <div class="form-text">
                    <%# Eval("CalcDisclosureHtml") %> 
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Limits">
            <ItemTemplate>
                <%# Eval("SpecFormLimitHtml") %>
                <div class="form-text">
                    <%# Eval("SpecQuestionLimit") %> questions per form
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Criteria">
            <ItemTemplate>
                <%# Eval("CriterionCountHtml") %>
                <div class="form-text">
                    <%# Eval("SetCountHtml") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Forms" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# Eval("SpecFormCount") %> 
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
