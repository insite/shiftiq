<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserInvoiceGrid.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.UserInvoiceGrid" %>

<h2 class="h4 mb-3">
    <asp:Literal runat="server" ID="InvoiceSectionTitle" /></h2>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <asp:Literal runat="server" ID="NoOtherInvoices" Text="No invoices to display." />

        <asp:Panel runat="server" ID="InvoicePanel" Visible="false">

            <insite:Grid runat="server" ID="InvoiceList">
                <Columns>
                    <asp:TemplateField HeaderText="Employer">
                        <ItemTemplate>
                            <%# Eval("CustomerEmployer") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Person Code">
                        <ItemTemplate>
                            <a href='/ui/admin/contacts/people/edit?<%# Eval("CustomerIdentifier", "contact={0}") %>'>
                                <%# Eval("CustomerPersonCode") %>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Customer Name">
                        <ItemTemplate>
                            <a href='/ui/admin/contacts/people/edit?<%# Eval("CustomerIdentifier", "contact={0}") %>'>
                                <%# Eval("CustomerFullName") %>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Customer Email">
                        <ItemTemplate>
                            <a href='mailto:<%# Eval("CustomerEmail") %>'>
                                <%# Eval("CustomerEmail") %>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Invoice Number">
                        <ItemTemplate>
                            <%# Eval("InvoiceNumber") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Invoice Status">
                        <ItemTemplate>
                            <%# GetInvoiceStatus(Eval("InvoiceStatus")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Invoice Paid Date">
                        <ItemTemplate>
                            <%# GetLocalTime(Eval("InvoicePaid")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Transaction Code">
                        <ItemTemplate>
                            <%# GetTransactionCode(Eval("Payments")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderStyle-Width="20px" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <insite:IconLink runat="server" Name="search" ToolTip="View Email" NavigateUrl='<%# GetViewEmailUrl() %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </insite:Grid>

        </asp:Panel>

    </div>
</div>

