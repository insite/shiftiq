<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Issues.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">

            <div class="row">
                <div class="col-4">

                    <h4 class="mt-3">Details</h4>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="IssueNumber" EmptyMessage="Case Number" MaxLength="6" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IssueStatusCategory" EmptyMessage="Case Status Category">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Open" Value="Open" />
                                <insite:ComboBoxOption Text="Closed" Value="Closed" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameComboBox runat="server" ID="IssueType" EmptyMessage="Case Type" />
                    </div>

                    <div class="mb-2" runat="server" id="IssueStatusField" visible="false">
                        <insite:IssueStatusComboBox runat="server" ID="IssueStatus" EmptyMessage="Case Status" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="IssueTitle" EmptyMessage="Case Summary" MaxLength="200" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="IssueDescription" EmptyMessage="Case Description" MaxLength="200" />
                    </div>

                    <div>
                        <h4 class="mt-3">Comments</h4>

                        <div class="mb-2">
                            <insite:FindPerson runat="server" ID="IssueCommentAssignedIdentifier" EmptyMessage="Assigned to" MaxLength="200" />
                        </div>

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CategoryUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="CategoryUpdatePanel">
                            <ContentTemplate>
                                <div class="mb-2">
                                    <insite:CaseCommentCategoryComboBox runat="server" ID="CommentCategory" EmptyMessage="Category" />
                                </div>

                                <div runat="server" id="CommentSubCategoryPanel" class="mb-2" visible="false">
                                    <insite:ItemNameComboBox runat="server" ID="CommentSubCategory" EmptyMessage="Subcategory" />
                                </div>
                            </ContentTemplate>
                        </insite:UpdatePanel>

                        <div class="mb-2">
                            <insite:FlagComboBox runat="server" ID="CommentFlag" AllowBlank="true" EmptyMessage="Flag" MaxLength="200" />
                        </div>

                        <div class="mb-2">
                            <insite:ItemNameComboBox runat="server" ID="CommentTag" EmptyMessage="Tag" MaxLength="200" />
                        </div>

                        <h4 class="mt-3">Contacts</h4>

                        <div class="mb-2">
                            <insite:FindPerson runat="server" ID="ManagerIdentifier" EmptyMessage="Manager" />
                        </div>

                        <div class="mb-2">
                            <insite:FindPerson runat="server" ID="OwnerIdentifier" EmptyMessage="Owner" />
                        </div>

                        <div class="mb-2">
                            <insite:CaseLawyerComboBox runat="server" ID="LawyerID" EmptyMessage="Lawyer" />
                        </div>
                    </div>

                </div>
                <div class="col-4">

                    <h4 class="mt-3">Topic (Member/Account)</h4>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="AssigneeName" EmptyMessage="Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:FindEmployer runat="server" ID="AssigneeEmployerIdentifiers" EmptyMessage="Employer" MaxLength="100" MaxSelectionCount="0" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="PersonCode" EmptyMessage="Person Code" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="AssigneeOrganization" EmptyMessage="Organization(s)" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:ItemNameMultiComboBox runat="server" ID="MembershipStatus" EmptyMessage="Account/Association Status" />
                    </div>

                    <h4 class="mt-3">Dates</h4>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateReportedSince" runat="server" EmptyMessage="Reported Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateReportedBefore" runat="server" EmptyMessage="Reported Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateOpenedSince" runat="server" EmptyMessage="Opened Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateOpenedBefore" runat="server" EmptyMessage="Opened Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateClosedSince" runat="server" EmptyMessage="Closed Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateClosedBefore" runat="server" EmptyMessage="Closed Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DateCaseStatusEffectiveSince" EmptyMessage="Case Status Effective Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DateCaseStatusEffectiveBefore" EmptyMessage="Case Status Effective Before" />
                    </div>

                </div>
                <div class="col-4">

                    <h4 class="mt-3">Attachments</h4>

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AttachmentsPanel" />

                    <insite:UpdatePanel runat="server" ID="AttachmentsPanel">
                        <ContentTemplate>
                            <div class="mb-2">
                                <insite:ItemNameComboBox runat="server" ID="AttachmentFileStatus" EmptyMessage="Document Status">
                                    <Settings
                                        CollectionName="Assets/Files/Document/Status"
                                        UseCurrentOrganization="true"
                                        UseGlobalOrganizationIfEmpty="true"
                                    />
                                </insite:ItemNameComboBox>
                            </div>

                            <div runat="server" id="AttachmentFileCategoryField" class="mb-2">
                                <insite:ItemNameComboBox runat="server" ID="AttachmentFileCategory" EmptyMessage="Document Type">
                                    <Settings
                                        CollectionName="Assets/Files/Document/Type"
                                        UseCurrentOrganization="true"
                                        UseGlobalOrganizationIfEmpty="true"
                                    />
                                </insite:ItemNameComboBox>
                            </div>

                            <div runat="server" id="AttachmentDocumentNameField" class="mb-2">
                                <insite:TextBox runat="server" ID="AttachmentDocumentName" EmptyMessage="Document Name" MaxLength="100" />
                            </div>

                            <div runat="server" id="AttachmentHasClaimsField" class="mb-2">
                                <insite:ComboBox runat="server" ID="AttachmentHasClaims" EmptyMessage="Permissions">
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Value="true" Text="Private" />
                                        <insite:ComboBoxOption Value="false" Text="Public" />
                                    </Items>
                                </insite:ComboBox>
                            </div>

                            <div runat="server" id="AttachmentFileExpirySinceField" class="mb-2">
                                <insite:DateSelector ID="AttachmentFileExpirySince" runat="server" EmptyMessage="Expired Since" />
                            </div>

                            <div runat="server" id="AttachmentFileExpiryBeforeField" class="mb-2">
                                <insite:DateSelector ID="AttachmentFileExpiryBefore" runat="server" EmptyMessage="Expired Before" />
                            </div>

                            <div runat="server" id="AttachmentFileReceivedSinceField" class="mb-2">
                                <insite:DateSelector ID="AttachmentFileReceivedSince" runat="server" EmptyMessage="Received Since" />
                            </div>

                            <div runat="server" id="AttachmentFileReceivedBeforeField" class="mb-2">
                                <insite:DateSelector ID="AttachmentFileReceivedBefore" runat="server" EmptyMessage="Received Before" />
                            </div>

                            <div runat="server" id="AttachmentFileAlternatedSinceField" class="mb-2">
                                <insite:DateSelector ID="AttachmentFileAlternatedSince" runat="server" EmptyMessage="Alternated Since" />
                            </div>

                            <div runat="server" id="AttachmentFileAlternatedBeforeField" class="mb-2">
                                <insite:DateSelector ID="AttachmentFileAlternatedBefore" runat="server" EmptyMessage="Alternated Before" />
                            </div>

                            <div runat="server" id="AttachmentApprovedSinceField" class="mb-2">
                                <insite:DateSelector ID="AttachmentApprovedSince" runat="server" EmptyMessage="Approved Since" />
                            </div>

                            <div runat="server" id="AttachmentApprovedBeforeField" class="mb-2">
                                <insite:DateSelector ID="AttachmentApprovedBefore" runat="server" EmptyMessage="Approved Before" />
                            </div>

                            <div runat="server" id="AttachmentUploadedSinceField" class="mb-2">
                                <insite:DateSelector ID="AttachmentUploadedSince" runat="server" EmptyMessage="Uploaded Since" />
                            </div>

                            <div runat="server" id="AttachmentUploadedBeforeField" class="mb-2">
                                <insite:DateSelector ID="AttachmentUploadedBefore" runat="server" EmptyMessage="Uploaded Before" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>

                <div class="mb-2">
                    <insite:FilterButton runat="server" ID="SearchButton" />
                    <insite:ClearButton runat="server" ID="ClearButton" />
                </div>
            </div>
        </div>
    </div>
    <div class="col-3">
        <div class="mt-3">
            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>
        </div>

        <div>
            <h4 class="mt-4">Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        $(".checkbox input[type=radio]").addClass('form-check-input');
    </script>
</insite:PageFooterContent>
