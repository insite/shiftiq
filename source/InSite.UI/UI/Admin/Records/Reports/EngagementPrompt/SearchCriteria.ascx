<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Records.Reports.EngagementPrompt.SearchCriteria" %>

<%@ Register Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" TagName="FilterManager" TagPrefix="uc" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h1>Criteria</h1>

            <div class="row">
                <div class="col-4">
                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="LearnerNameLast" EmptyMessage="Learner Last Name" MaxLength="100" />
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="LearnerNameFirst" EmptyMessage="Learner First Name" MaxLength="100" />
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="LearnerEmail" EmptyMessage="Learner Email " MaxLength="100" />
                    </div>

	                <insite:FilterButton ID="SearchButton" runat="server" />
	                <insite:ClearButton ID="ClearButton" runat="server" />
                </div>
                <div class="col-4">
                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="ProgramName" EmptyMessage="Program Name " MaxLength="100" />
                    </div>

                    <div class="mb-3">
                        <insite:TextBox runat="server" ID="GradebookName" EmptyMessage="Gradebook Name " MaxLength="100" />
                    </div>

                    <div class="mb-3">
                        <insite:ComboBox runat="server" ID="CertificateStatus" EmptyMessage="Certificate Status" Enabled="false">
                            <Items>
                                <insite:ComboBoxOption Value="Not Granted" Text="Certificate Not Granted" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-3">
                        <insite:ComboBox runat="server" ID="EngagementPromptStatus" EmptyMessage="Engagement Prompt Status" Enabled="false">
                            <Items>
                                <insite:ComboBoxOption Value="PromptNeeded" Text="Engagement Prompt Needed" />
                                <insite:ComboBoxOption Value="NoPromptNeeded" Text="No Engagement Prompt Needed" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-3">       
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>