<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Jobs.Employers.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="40px">
                <itemtemplate>
                    <insite:IconLink runat="server" Name="pencil" NavigateUrl='<%# GetEditLink(Eval("GroupIdentifier")) %>' ToolTip="Edit" />
                </itemtemplate>
            </asp:TemplateField>
           
            <asp:TemplateField HeaderText="Date Registered">
                <itemtemplate>
                    <asp:Literal ID="DateRegistered" runat="server" Text='<%# GetString((DateTimeOffset?)(Eval("EmployerContactCreated"))) %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Name">
                <itemtemplate>
                    <asp:HyperLink runat="server" NavigateUrl='<%# GetEditLink(Eval("GroupIdentifier")) %>' Text='<%# Eval("GroupName") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Contact Name" ItemStyle-Wrap="false">
                <itemtemplate>
                    <a href="/ui/admin/contacts/people/edit?contact=<%# Eval("UserIdentifier") %>">
                        <%# Eval("ContactFullName") %>
                    </a>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Position">
                <itemtemplate>
                    <asp:Literal ID="Position" runat="server" Text='<%# Eval("ContactJobTitle") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Address" >
                <itemtemplate>
                    <asp:Literal ID="Address" runat="server" Text='<%# Eval("AddressLine") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="City">
                <itemtemplate>
                    <asp:Literal ID="City" runat="server" Text='<%# Eval("AddressCity") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Province">
                <itemtemplate>
                    <asp:Literal ID="Province" runat="server" Text='<%# Eval("AddressProvince") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Country" >
                <itemtemplate>
                    <asp:Literal ID="Country" runat="server" Text='<%# Eval("AddressCountry") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Email" ItemStyle-Wrap="false">
                <itemtemplate>
                    <insite:IconLink runat="server" ID="EmailStatus" Name="paper-plane" ToolTip="Send Email" CssClass="me-1" NavigateUrl='<%# CreateMailToLink(Eval("Email")) %>' /><asp:Literal ID="Email" runat="server" Text='<%# Eval("Email") %>'></asp:Literal>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Contact Phone">
                <itemtemplate>
                    <asp:Literal ID="ContactPhone" runat="server" Text='<%# Eval("GroupPhone") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Approved">
                <itemtemplate>
                    <asp:Literal ID="Approved" runat="server" Text='<%# DateToBitToHTML(Eval("Approved")) %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Employer Size">
                <itemtemplate>
                    <asp:Literal ID="EmployerSize" runat="server" Text='<%# Eval("GroupSize") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Industry">
                <itemtemplate>
                    <asp:Literal ID="EmployerIndustry" runat="server" Text='<%# Eval("GroupIndustry") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Sector">
                <itemtemplate>
                    <asp:Literal ID="CompanySector" runat="server" Text='<%# Eval("CompanySector") %>' />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="WebSite">
                <itemtemplate>
                    <asp:Literal ID="WebSite" runat="server" Text='<%# Eval("Url") %>' />
                </itemtemplate>
            </asp:TemplateField>

    </Columns>
</insite:Grid>

