<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Sales.Reports.EventRegistrationPayment.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="PaymentIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Event Date" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("EventDate")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Event Name" DataField="EventName" />

        <asp:BoundField HeaderText="Achievement" DataField="AchievementTitle" />

        <asp:BoundField HeaderText="Employer at Time of Registration" DataField="EmployerName" />
        <asp:BoundField HeaderText="Registered By" DataField="RegistrantCardholder" />

        <asp:TemplateField HeaderText="Registrant">
            <ItemTemplate>
                <%# Eval("LearnerAttendee") %>

                <span class="form-text">
                    <%# Eval("LearnerCode") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invoice #" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("InvoiceNumber") %>

                <div runat="server" visible='<%# Eval("CreditNumber") != null %>' class="form-text">
                    Credit #: <%# Eval("CreditNumber") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invoice Status" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("InvoiceStatus") %>

                <div runat="server" visible='<%# Eval("CreditAmount") != null %>' class="form-text">
                    Moved on <%# LocalizeDate(Eval("CreditSubmitted")) %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Amount" DataField="RegistrationFee" DataFormatString="{0:c2}" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" />
        <asp:BoundField HeaderText="Status" DataField="TransactionStatus" />

        <asp:TemplateField HeaderText="Invoiced" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("InvoiceSubmitted")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Paid" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("TransactionDate")) %>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="Transaction" HeaderStyle-Wrap="false" DataField="TransactionCode" />

    </Columns>
</insite:Grid>