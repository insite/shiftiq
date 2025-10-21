<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PaymentGrid.ascx.cs" Inherits="InSite.Admin.Invoices.Controls.PaymentGrid" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Status" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("PaymentStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Amount">
            <ItemTemplate>
                <%# Eval("PaymentAmount", "{0:n2}") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Started">
            <ItemTemplate>
                <%# GetLocalTime(Eval("PaymentStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Aborted">
            <ItemTemplate>
                <%# GetLocalTime(Eval("PaymentAborted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Declined">
            <ItemTemplate>
                <%# GetLocalTime(Eval("PaymentDeclined")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Payment Approved">
            <ItemTemplate>
                <%# GetLocalTime(Eval("PaymentApproved")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Customer IP">
            <ItemTemplate>
                <%# Eval("CustomerIP") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Result Code">
            <ItemTemplate>
                <%# Eval("ResultCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Result Message">
            <ItemTemplate>
                <%# Eval("ResultMessage") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="30px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="Payment Outline"
                    NavigateUrl='<%# Eval("PaymentIdentifier", "/ui/admin/sales/payments/outline?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>

</insite:Grid>
