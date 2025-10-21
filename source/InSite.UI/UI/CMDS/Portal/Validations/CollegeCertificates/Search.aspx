<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Cmds.Portal.Validations.CollegeCertificates.Search" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="TabCriteria" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">

            <div runat="server" ID="NoCertifications" class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="alert alert-info mb-0" role="alert">
                        There are no college certificates currently available for your occupation profile(s). 
                        If you have any questions, then please contact us: 
                        <a href="mailto:admin_cmds@keyera.com">admin_cmds@keyera.com</a>
                    </div>

                </div>
            </div>

            <asp:Repeater runat="server" ID="Certifications">
                <ItemTemplate>

                    <h2 class="h4 mb-3"><%# Eval("ProfileTitle") %></h2>

                    <div class="card border-0 shadow-lg mb-4">
                        <div class="card-body">

                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Requirements and Eligibility Status</th>
                                        <th>Institution</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td class="text-nowrap w-50">
                                            <table class="requirements">
                                                <tr>
                                                    <td>
                                                        <span runat="server" Visible='<%# (Boolean)Eval("CoreIsSatisfied") %>' class="text-success"><i class="far fa-flag-checkered"></i></span>
                                                        <span runat="server" Visible='<%# !(Boolean)Eval("CoreIsSatisfied") %>' class="text-danger"><i class="fas fa-engine-warning"></i></span>
                                                    </td>
                                                    <td>Core Hours:</td>
                                                    <td class="ps-2">Total</td>
                                                    <td class="text-end"><%# Eval("CoreHoursTotal") %></td>
                                                    <td class="ps-2">Required</td>
                                                    <td class="text-end"><%# Eval("CoreHoursRequired") %></td>
                                                    <td class="ps-2">Completed</td>
                                                    <td class="text-end"><%# Eval("CoreHours") %></td>
                                                    <td class="text-end">( <%# Eval("CorePercentage", "{0:p0}" ) %> )</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span runat="server" Visible='<%# (Boolean)Eval("NonCoreIsSatisfied") %>' class="text-success"><i class="far fa-flag-checkered"></i></span>
                                                        <span runat="server" Visible='<%# !(Boolean)Eval("NonCoreIsSatisfied") %>' class="text-danger"><i class="fas fa-engine-warning"></i></span>
                                                    </td>
                                                    <td>Non-Core Hours:</td>
                                                    <td class="ps-2">Total</td>
                                                    <td class="text-end"><%# Eval("NonCoreHoursTotal") %></td>
                                                    <td class="ps-2">Required</td>
                                                    <td class="text-end"><%# Eval("NonCoreHoursRequired") %></td>
                                                    <td class="ps-2">Completed</td>
                                                    <td class="text-end"><%# Eval("NonCoreHours") %></td>
                                                    <td class="text-end">( <%# Eval("NonCorePercentage", "{0:p0}") %> )</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <span runat="server" Visible='<%# (Boolean)Eval("CoreIsSatisfied") && (Boolean)Eval("NonCoreIsSatisfied") %>' class="text-success"><i class="far fa-flag-checkered"></i></span>
                                                        <span runat="server" Visible='<%# !(Boolean)Eval("CoreIsSatisfied") || !(Boolean)Eval("NonCoreIsSatisfied") %>' class="text-danger"><i class="fas fa-engine-warning"></i></span>
                                                    </td>
                                                    <td colspan="8">
                                                        You have 
                                                        <asp:Literal runat="server" Text="not" Visible='<%# !(Boolean)Eval("CoreIsSatisfied") || !(Boolean)Eval("NonCoreIsSatisfied") %>' />
                                                        met the requirements for this certification.
                                                    </td>
                                                </tr>
                                                <tr id="ViewMissingCompetenciesRow" runat="server">
                                                    <td colspan="9">
                                                        <asp:LinkButton ID="ViewMissingCompetenciesButton" runat="server" Text="View missing competencies" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="w-50">
                                            <asp:PlaceHolder ID="RequestNotAllowedPanel" runat="server">
                                                You must complete the requirements for this certificate before you are eligible to request it from an institution.
                                            </asp:PlaceHolder>

                                            <asp:PlaceHolder ID="RequestPanel" runat="server">
                                                <asp:RadioButtonList ID="Institution" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" />
                                                <p>
                                                    Click the "Send Request" button to send an email application for this certificate.
                                                </p>
                                                <cmds:CmdsButton ID="SendRequestButton" runat="server" Text="Send Request" CommandName="SendRequest" CommandArgument='<%# Eval("ProfileStandardIdentifier") %>' OnClientClick="return sendRequest(this);" />
                                            </asp:PlaceHolder>

                                            <asp:PlaceHolder ID="StatusPanel" runat="server">
                                                <asp:Literal ID="StatusText" runat="server" />
                                            </asp:PlaceHolder>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>

                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
        </insite:NavItem>
        
        <insite:NavItem runat="server" ID="TabResults" Title="Results" Icon="far fa-database" IconPosition="BeforeText" Visible="false">

            <div class="text-end mb-3">
                <insite:Button runat="server" ID="ExportButton" Text="Export to CSV" ButtonStyle="Default" Icon="fas fa-download" />
            </div>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:Grid runat="server" ID="Grid">
                        <Columns>
                    
                            <asp:BoundField HeaderText="Number" DataField="CompetencyNumber" />    
                            <asp:BoundField HeaderText="Summary" DataField="CompetencySummary" />
                            <asp:BoundField HeaderText="Core Hours" DataField="CertificationHoursCore" HeaderStyle-Wrap="false" ItemStyle-CssClass="text-end" />
                            <asp:BoundField HeaderText="Non-Core Hours" DataField="CertificationHoursNonCore" HeaderStyle-Wrap="false" ItemStyle-CssClass="text-end" />

                            <asp:TemplateField HeaderText="Total Hours" HeaderStyle-Wrap="false" ItemStyle-CssClass="text-end">
                                <ItemTemplate>
                                    <asp:Literal ID="TotalHours" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Status" ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <%# Eval("ValidationStatus") == DBNull.Value ? "Not Assigned" : Eval("ValidationStatus") %>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </insite:Grid>

                    <asp:PlaceHolder ID="NoDataPanel" runat="server">
                        <div class="alert alert-info mb-0" role="alert">
                            No missing competencies.
                        </div>
                    </asp:PlaceHolder>

                </div>
            </div>
        </insite:NavItem>
    </insite:Nav>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            table.requirements td { padding: 0.25rem; }
        </style>
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            function showMissingCompetencies(profileId) {
                var url = "/ui/cmds/portal/validations/college-certificates/search?profile=" + String(profileId);

                window.location.replace(url);
            }

            function sendRequest(button) {
                var inputs = button.parentNode.getElementsByTagName("input");
                var name = null;

                for (var i = 0; i < inputs.length; i++) {
                    var input = inputs[i];

                    if (input.type.toLowerCase() == "radio" && input.checked) {
                        name = getLabelText(button.parentNode, input.id);
                        break;
                    }
                }

                if (name == null) {
                    alert("Institution must be selected.");
                    return false;
                }

                return confirm("Are you sure you want to send request for applying for " + name + " Certification?");
            }

            function getLabelText(parentNode, forValue) {
                var labels = parentNode.getElementsByTagName("label");

                for (var i = 0; i < labels.length; i++) {
                    var label = labels[i];

                    if (label.getAttribute("for") == forValue)
                        return label.innerHTML;
                }
            }

        </script>
    </insite:PageFooterContent>
</asp:Content>
