<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseSection.ascx.cs" Inherits="InSite.Admin.Issues.Outlines.Controls.CaseSection" %>

    <div class="row mt-4 mb-4">
        <div class="col-lg-11">

            <insite:Button runat="server" ID="NewIssueLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/workflow/cases/open" />
            <insite:Button runat="server" ID="DuplicateLink" ButtonStyle="Default" Text="Duplicate" Icon="fas fa-copy" />
                    
            <insite:ButtonSpacer runat="server" />
            <insite:Button runat="server" id="ViewHistoryLink" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
            <insite:Button runat="server" ID="DownloadLink" Text="Download" icon="fas fa-download" ButtonStyle="Default" />
            <insite:Button runat="server" ID="EmailSenderButton" Text="Send Email" Icon="fas fa-paper-plane" ButtonStyle="Default" />
                    
            <insite:ButtonSpacer runat="server" />
            <insite:Button runat="server" ID="ReopenIssueButton" Text="Reopen" Icon="fas fa-folder-open" ButtonStyle="Default" />
            <insite:Button runat="server" id="CloseIssueLink" Text="Close" Icon="fas fa-folder" ButtonStyle="Default" />
            <insite:DeleteButton runat="server" ID="DeleteLink" />
            
        </div>

        <div class="col-lg-1">
            <div class="float-end">
                <insite:DropDownButton runat="server" ID="MoreInfoButton" ButtonStyle="Default" DefaultAction="None" IconType="Regular" IconName="circle-info" Text="More Info" CssClass="d-inline-block" />
            </div>
        </div>
    </div>

    <div class="row" >
        <div class="col-md-6">

            <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <h3 runat="server" id="DetailsHeading"></h3>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" ID="ChangeIssueType" ToolTip="Change Case Type" />
                    </div>
                    <asp:Label ID="IssueTypeLabel" runat="server" Text="Case Type" CssClass="form-label" AssociatedControlID="IssueType" />
                    <div>
                        <asp:Literal runat="server" ID="IssueType" />
                            
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" ID="ChangeIssueTitle" ToolTip="Change Case Title" />
                    </div>
                    <asp:Label ID="IssueTitleLabel" runat="server" Text="Case Summary" CssClass="form-label" AssociatedControlID="IssueTitle" />
                    <div>
                        <asp:Literal runat="server" ID="IssueTitle" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" ID="ChangeIssueDescription" ToolTip="Change Case Description" />
                    </div>
                    <asp:Label ID="IssueDescriptionLabel" runat="server" Text="Case Description" CssClass="form-label" AssociatedControlID="IssueDescription" />
                    <div>
                        <asp:Literal runat="server" ID="IssueDescription" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" ID="ChangeIssueStatus" ToolTip="Change Case Status" />
                    </div>
                    <asp:Label ID="IssueStatusLabel" runat="server" Text="Current Status" CssClass="form-label" AssociatedControlID="IssueStatus" />
                    <div>
                        <div>
                            <asp:Literal runat="server" ID="IssueStatus" />
                        </div>
                    </div>
                    <div class="form-text">
                        <asp:Literal runat="server" ID="IssueStatusTimestamp" />
                    </div>
                </div>

            </div></div>

        </div>

        <div class="col-md-6">

            <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <h3>Contacts</h3>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" ID="ChangeAdministrator" ToolTip="Change Administrator" />
                    </div>
                    <asp:Label ID="AdministratorNameLabel" runat="server" Text="Case Manager" CssClass="form-label" AssociatedControlID="AdministratorName" />
                    <div>
                        <asp:Literal runat="server" ID="AdministratorName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" ID="ChangeAssignee" ToolTip="Change Topic" />
                    </div>
                    <asp:Label ID="MemberLabel" runat="server" Text="Member" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="AssigneeName" />
                        <asp:Literal runat="server" ID="AssigneeMembershipStatus" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <asp:Label ID="EmployerNameLabel" runat="server" Text="Employer/Referrer" CssClass="form-label" AssociatedControlID="EmployerName" />
                    <div>
                        <asp:Literal runat="server" ID="EmployerName" />
                    </div>
                </div>
                
                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" ID="ChangeOwner" ToolTip="Change Owner" />
                    </div>
                    <asp:Label ID="OwnerNameLabel" runat="server" Text="Current Owner" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="OwnerName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" ID="ChangeLawyer" ToolTip="Change Lawyer" />
                    </div>
                    <asp:Label ID="LawyerNameLabel" runat="server" Text="Lawyer" CssClass="form-label" AssociatedControlID="LawyerName" />
                    <div>
                        <asp:Literal runat="server" ID="LawyerName" />
                    </div>
                </div>

                <hr class="mt-4 mb-3" />

                <h3>Dates</h3>

                <div class="form-group mb-3">
                    <asp:Label ID="IssueReportedLabel" runat="server" Text="Reported" CssClass="form-label" AssociatedControlID="IssueReported" />
                    <div>
                        <asp:Literal runat="server" ID="IssueReported" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <asp:Label ID="IssueOpenedLabel" runat="server" Text="Opened" CssClass="form-label" AssociatedControlID="IssueOpened" />
                    <div>
                        <asp:Literal runat="server" ID="IssueOpened" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <asp:Label ID="IssueClosedLabel" runat="server" Text="Closed" CssClass="form-label" AssociatedControlID="IssueClosed" />
                    <div>
                        <asp:Literal runat="server" ID="IssueClosed" />
                    </div>
                </div>

            </div></div>

        </div>

    </div>