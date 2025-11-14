<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.Opportunities.Controls.SearchResults" %>

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
                <div class="mb-2">
                    <insite:Button runat="server" ButtonStyle="OutlineInfo" Icon="fas fa-cloud-upload" Text="Apply Now" ToolTip="Apply"  
                        CssClass="btn-bg-white" style="width:130px;" Visible="<%# IsUserApproved %>" 
                        NavigateUrl='<%# Eval("OpportunityIdentifier", "/ui/portal/job/candidates/opportunities/apply?id={0}") %>' />
                </div>
                <div runat="server" visible='<%# !IsUserApproved %>' class="mb-2" style="width:120px; white-space: normal;">
                    You can apply when your profile is approved.
                </div>
                <div class="mb-2">
                    <insite:Button runat="server" ButtonStyle="OutlineSecondary" Icon="fas fa-search" Text="View" ToolTip="View"
                        CssClass="btn-bg-white" style="width:130px;"
                        NavigateUrl='<%# Eval("OpportunityIdentifier", "/ui/portal/job/candidates/opportunities/view?id={0}") %>' />
                </div>
            </itemtemplate>
        </asp:TemplateField>
        </Columns>
    </insite:Grid>
</div>
