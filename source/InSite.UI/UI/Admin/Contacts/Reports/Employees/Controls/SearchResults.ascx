<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Contacts.Reports.Employees.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="EmployeeUserIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("EmployeeUserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'>
                    <%# Eval("EmployeeFirstName") %>&nbsp;<%# Eval("EmployeeLastName") %>
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <%# Eval("EmployeeEmail", "<a href='mailto:{0}'>{0}</a>") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Job Title">
            <ItemTemplate>
                <%# Eval("EmployeeJobTitle") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Membership Status">
            <ItemTemplate>
                <%# Eval("EmployeeProcessStep") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Gender">
            <ItemTemplate>
                <%# Eval("EmployeeGender") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Join Date">
            <ItemTemplate>
                <%# GetDateString((DateTime?)Eval("EmployeeMemberStartDate")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Honorific">
            <ItemTemplate>
                <%# Eval("EmployeeHonorific") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Phone">
            <ItemTemplate>
                <%# Eval("EmployeePhone") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Home Phone">
            <ItemTemplate>
                <%# Eval("EmployeePhoneHome") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Mobile Phone">
            <ItemTemplate>
                <%# Eval("EmployeePhoneMobile") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Member End Date">
            <ItemTemplate>
                <%# GetDateString((DateTime?)Eval("EmployeeMemberEndDate")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Shipping Address">
            <ItemTemplate>
                <%# GetAddressString((string)Eval("EmployeeShippingAddressStreet1"),
                    (string)Eval("EmployeeShippingAddressCity"),
                    (string)Eval("EmployeeShippingAddressProvince"),
                    (string)Eval("EmployeeShippingAddressPostalCode"),
                    (string)Eval("EmployeeShippingAddressCountry")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Shipping Preference">
            <ItemTemplate>
                <%# Eval("EmployeeShippingPreference") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Person Code">
            <ItemTemplate>
                <%# Eval("EmployeeAccountNumber") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Home Address">
            <ItemTemplate>
                <%# GetAddressString((string)Eval("EmployeeHomeAddressStreet1"),
                    (string)Eval("EmployeeHomeAddressCity"),
                    (string)Eval("EmployeeHomeAddressProvince"),
                    (string)Eval("EmployeeHomeAddressPostalCode"),
                    (string)Eval("EmployeeHomeAddressCountry")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("EmployerGroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'
                    Text='<%# Eval("EmployerGroupName") %>' ToolTip="Open Employer Edit page" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer Number">
            <ItemTemplate>
                <%# Eval("EmployerGroupNumber") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer Category">
            <ItemTemplate>
                <%# Eval("EmployerContactLabel") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer Address">
            <ItemTemplate>
                <%# Eval("EmployerShippingAddressStreet1") %>
                <div>
                    <%# Eval("EmployerShippingAddressStreet2") %>
                </div>
                <div>
                    <%# Eval("EmployerShippingAddressCity") %>
                    <%# Eval("EmployerShippingAddressProvince") %>
                    <%# Eval("EmployerShippingAddressPostalCode") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer Phone">
            <ItemTemplate>
                <%# Eval("EmployerPhone") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer Fax">
            <ItemTemplate>
                <%# Eval("EmployerPhoneFax") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Empoyer Parent (Functional)">
            <ItemTemplate>
                <asp:Repeater runat="server" ID="EmployerParentFunctional">
                    <SeparatorTemplate>, </SeparatorTemplate>
                    <ItemTemplate>
                        <asp:HyperLink runat="server"
                            NavigateUrl='<%# Eval("Identifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'
                            Text='<%# Eval("Name") %>' ToolTip="Open Edit page" />
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="District">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("EmployerDistrictIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'
                    Text='<%# Eval("EmployerDistrictName") %>' ToolTip="Open District Edit page" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="District Region">
            <ItemTemplate>
                <%# Eval("EmployerDistrictRegion") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Roles - Participation">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("RolesParticipationGroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'
                    Text='<%# Eval("RolesParticipationGroupName") %>' ToolTip="Open Role Edit page" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>