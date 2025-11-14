<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyJobOpportunityGrid.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.MyProfile.Controls.MyJobOpportunityGrid" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="row mb-4">

            <div class="col-12">

                <div class="card shadow-lg">

                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Job Opportunities
                            <span class="badge bg-secondary rounded-pill">
                                <asp:Literal runat="server" ID="Count" />
                            </span>
                        </h4>

                        <div class="mb-3">
                            <insite:Button runat="server" ID="AddNewLink"
                                Text="Add New Opportunity"
                                ButtonStyle="Default"
                                Icon="fas fa-plus-circle"
                            />
                        </div>

                        <insite:Grid runat="server" ID="Grid" DataKeyNames="OpportunityIdentifier" Translation="Header" CssClass="table table-striped table-hover">
                            <Columns>

                                <asp:TemplateField headertext="Job Position" ItemStyle-Width="30%">
                                    <itemtemplate>
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
                                        <div class="mt-3">
                                            <insite:CheckBox runat="server" ID="Published" Text="Post on Job Board" Checked='<%# Eval("WhenPublished") != null ? true : false %>' />
                                        </div>
                                        <div class="form-text">
                                            <asp:Literal ID="PublishedOn" runat="server" Text='<%# GetDateString("Posted on",(DateTimeOffset?)Eval("WhenPublished")) %>'></asp:Literal>
                                        </div>
                                        <div class="form-text">
                                            <%# GetDateString("Created on",(DateTimeOffset?)Eval("WhenCreated")) %>
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
                                    </itemtemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end" HeaderStyle-CssClass="text-end">
                                    <ItemTemplate>
                                        <div>
                                            <insite:Button Icon="far fa-search" runat="server" 
                                                Text="View"
                                                NavigateUrl='<%# GetRedirectUrl((Guid)Eval("OpportunityIdentifier")) %>' 
                                                ToolTip="View Posting" 
                                                CssClass="btn btn-default mb-2"
                                                Style="width:120px"
                                            />
                                        </div>
                                        <div>
                                            <insite:Button Icon="far fa-pencil" runat="server" 
                                                Text="Edit"
                                                NavigateUrl='<%# Eval("OpportunityIdentifier", "/ui/portal/job/employers/opportunities/edit?id={0}") %>' 
                                                ToolTip="Edit Posting" 
                                                CssClass="btn btn-default mb-2"
                                                Style="width:120px"
                                            />
                                        </div>
                                        <div>
                                            <insite:Button Icon="far fa-trash-alt" runat="server" 
                                                ID="DeleteColumnButton" 
                                                Text="Delete"
                                                ToolTip="Delete Posting" 
                                                CommandName="DeleteJob" 
                                                CommandArgument='<%# Eval("OpportunityIdentifier") %>' 
                                                CssClass="btn btn-default mb-2"
                                                ConfirmText="Are you sure to delete this posting?"
                                                Style="width:120px"
                                            />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </insite:Grid>
                    </div>
            
                </div>

            </div>

        </div>
    </ContentTemplate>
</insite:UpdatePanel>