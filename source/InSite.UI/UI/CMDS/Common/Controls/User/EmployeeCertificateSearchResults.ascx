<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeCertificateSearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.EmployeeCertificates.EmployeeCertificateSearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Learner"  ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("LearnerIdentifier", "/ui/cmds/admin/users/edit?userID={0}") %>'
                    Text='<%# Eval("LearnerName") %>'
                />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:HyperLinkField HeaderText="Certificate" DataTextField="ProfileTitle" DataNavigateUrlFields="LearnerIdentifier,ProfileIdentifier" DataNavigateUrlFormatString="/ui/cmds/admin/validations/college-certificates/edit?id={0}&certificateID={1}" />
        <asp:BoundField HeaderText="Institution" DataField="AuthorityName" ItemStyle-Wrap="false" />
        <asp:BoundField HeaderText="Requested" DataField="DateRequested" DataFormatString="{0:MMM d, yyyy}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-Wrap="false" />
        <asp:BoundField HeaderText="Submitted" DataField="DateSubmitted" DataFormatString="{0:MMM d, yyyy}" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-Wrap="false" />

        <asp:TemplateField HeaderText="Granted" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("DateGranted", "{0:MMM d, yyyy}") %>
                <cmds:CmdsButton runat="server" Visible="false" Text="Grant" CommandName="Grant" CommandArgument='<%# Eval("LearnerIdentifier") + ";" + Eval("ProfileIdentifier") %>' OnClientClick="return confirm('Are you sure you want to grant this certificate?')" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
