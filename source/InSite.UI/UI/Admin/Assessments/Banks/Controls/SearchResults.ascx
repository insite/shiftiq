<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Assessments.Banks.Controls.SearchResults" %>
<%@ Import Namespace="Humanizer" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Bank Name">
            <ItemTemplate>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>"><%# Eval("BankTitle") %></a>
                <div class="form-text">
                    <%# Eval("BankName") %>
                    <%# Eval("BankEdition") %>
                    <div>
                        <%# GetAttachmentsLabel(Container.DataItem) %>
                        <%# GetCommentsLabel(Container.DataItem) %>
                        <%# GetStandardLabel(Container.DataItem) %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Asset #" HeaderStyle-Wrap="False">
            <ItemTemplate>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>"><%# Eval("AssetNumber") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Level" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("BankLevel") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Questions" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# "item".ToQuantity((int)Eval("QuestionCount")) %>
                <div class="form-text">
                    <%# "set".ToQuantity((int)Eval("SetCount")) %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Specifications" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# "form".ToQuantity((int)Eval("FormCount")) %>
                <div class="form-text">
                    <%# "specification".ToQuantity((int)Eval("SpecCount")) %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
