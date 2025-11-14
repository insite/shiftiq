<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CandidatesGrid.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Opportunities.Controls.CandidatesGrid" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="row mb-3">

            <div class="col-12">

                <div class="card">

                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Matching Candidates
                            <span class="badge bg-secondary rounded-pill">
                                <asp:Literal runat="server" ID="Count" />
                            </span>
                        </h4>

                        <div runat="server" id="NoMatches" class="mb-3" visible="false">
                            <p>Based on the Occupational Area you have selected for this opportunity, there are currently no available matching candidates. Try editing the opportunity to select a similar or related Occupational Area, to see if a match can be found.</p>
                        </div>

                        <insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier" Translation="Header" CssClass="table table-striped table-hover">
                            
                            <Columns>

                                <asp:TemplateField HeaderText="Candidate">
                                    <ItemTemplate>
                                        <%# Eval("FullName") %>
                                        <div>
                                            <a target="_blank" href='/ui/portal/job/employers/candidates/view?id=<%# Eval("UserIdentifier") %>' class="fs-sm"><i class='far fa-id-card me-1'></i>View Candidate Profile</a>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Occupation Interest">
                                    <ItemTemplate>
                                        <%# GetStatusHtml(Container.DataItem) %>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Documents">
                                    <ItemTemplate>
                                        <asp:Repeater runat="server" ID="DocumentRepeater" >
                                            <ItemTemplate>
                                                <div>
                                                     <asp:HyperLink runat="server"
                                                        Target="_blank"
                                                        NavigateUrl='<%# GetFileRelativePath(Container.DataItem) %>' 
                                                        Text='<%# Eval("Name") %>' 
                                                        ToolTip='<%# GetFileSize((int)Eval("Length")) %>' 
                                                    />
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
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