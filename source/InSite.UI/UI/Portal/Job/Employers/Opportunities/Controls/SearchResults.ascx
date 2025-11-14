<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Opportunities.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="OpportunityIdentifier">
        <Columns>

        <asp:TemplateField headertext="Job Position" ItemStyle-Width="30%">
            <itemtemplate>
                <div class="form-text" style="text-align:right">
                    <%# GetDateString((DateTimeOffset?)Eval("WhenPublished")) %>
                </div>
                <asp:Literal ID="JobTitle" runat="server" Text='<%# Eval("JobTitle") %>' />
               
                <div class="form-text">
                    <asp:Literal ID="EmploymentType" runat="server" Text='<%# EmploymentTypeConverter((String)Eval("EmploymentType")) %>' />
                </div>
                <div class="form-text">
                    <asp:Literal ID="JobLevel" runat="server" Text='<%# Eval("JobLevel") %>' />
                </div>
                <div class="form-text">
                    <asp:Literal ID="LocationType" runat="server" Text='<%# PositionTypeConverter((String)Eval("LocationType")) %>' />
                </div>
                <div style="text-align:right">
                    <%# GetJobPositionAdditionalInfoHtml((String)Eval("SalaryOther"),"Salary") %>
                </div>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField headertext="Description" >
            <itemtemplate>
                <div class="row mb-3">
                    <div class="col-6">
                        <%# GetDescriptionAdditionalInfoHtml((String)Eval("EmployerGroupName"),"Organization") %>
                    </div>
                    <div class="col-6">
                        <div class="form-text">Location</div>
                        <%# GetLocation((String)Eval("LocationName"),(String)Eval("LocationType")) %>
                    </div>
                </div>
                <%# GetDescriptionAdditionalInfoHtml((String)Eval("JobDescription"),"Description") %>
                <%# GetDescriptionAdditionalInfoHtml((String)Eval("EmployerGroupDescription"),"Company Description") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField itemstyle-wrap="False" itemstyle-width="120px">
            <itemtemplate>

                <asp:Panel ID="EmployerApproved" runat="server" Visible='<%# CurrentUserRole.Equals("Employer") %>'>
                 <p runat="server" visible='<%# CanEdit((Guid?)Eval("EmployerGroupIdentifier"),CurrentUser.EmployerGroupIdentifier) %>'>
                    <insite:Button runat="server" 
                        NavigateUrl='<%# Eval("OpportunityIdentifier", "/ui/portal/job/employers/opportunities/edit?id={0}") %>' 
                        tooltip="Apply"  
                        ButtonStyle="Info" 
                        Text="Edit" 
                        CssClass="btn btn-default mb-2"
                        Style="width:120px"
                        Icon="fas fa-pencil" />
                </p>
                </asp:Panel>
                <p>
                    <insite:Button runat="server" 
                        NavigateUrl='<%# Eval("OpportunityIdentifier", "/ui/portal/job/employers/opportunities/view?id={0}") %>' 
                        tooltip="View"  
                        ButtonStyle="Default" 
                        Text="View" 
                        CssClass="btn btn-default mb-2" 
                        Style="width:120px" 
                        Icon="fas fa-search" />
                </p>

            </itemtemplate>
        </asp:TemplateField>
        </Columns>
    </insite:Grid>
</div>
