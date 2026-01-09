<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonRegistrations.ascx.cs" Inherits="InSite.UI.Portal.Contacts.People.Controls.PersonRegistrations" %>

<div class="card">
    <div class="card-body">

        <asp:Literal runat="server" ID="NoRegistrations" Text="No registrations to display." />

        <insite:Grid runat="server" id="Registrations">
            <Columns>
                <asp:TemplateField HeaderText="Class Title">
                    <ItemTemplate>
                        <%# Eval("EventTitle") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Registered">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("RegistrationRequestedOn")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Approval" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <%# Eval("ApprovalStatus") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comments">
                    <ItemTemplate>
                        <%# Eval("RegistrationComment") != null ? ((string)Eval("RegistrationComment")).Replace("\n", "<br>") : "" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Fee" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <ItemTemplate>
                        <%# Eval("RegistrationFee", "{0:n2}") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Attendance" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <%# Eval("AttendanceStatus") %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </insite:Grid>

    </div>
</div>