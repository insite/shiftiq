<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.Detail" %>

<%@ Register Src="~/UI/Admin/Standards/Categories/Controls/CategoryCheckList.ascx" TagName="CategoryCheckList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Standards/Standards/Controls/RelationshipList.ascx" TagName="RelationshipList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Standards/Standards/Controls/StandardPopupSelector.ascx" TagName="StandardPopupSelector" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/OrganizationTags.ascx" TagName="OrganizationTags" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Standards/Occupations/Controls/CompetenciesPanel.ascx" TagName="CompetenciesPanel" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Glossaries/Terms/Controls/TermGrid.ascx" TagName="GlossaryTermGrid" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Standards/Standards/Controls/ScenarioQuestions.ascx" TagName="ScenarioQuestions" TagPrefix="uc" %>

<insite:Nav runat="server">

    <insite:NavItem runat="server" ID="AssetTab" Title="Standard" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
        <div class="card border-0 shadow-lg mt-3">
            <div class="card-body">
                <div style="position: absolute; z-index: 1; right: 15px;">
                    <insite:Button runat="server" ID="UpdateStatusLink" ToolTip="Update Status" ButtonStyle="Success" Text="Update Status" />
                    <insite:Button runat="server" ID="OutlineLink" ToolTip="Outline" ButtonStyle="Default" Text="Outline" Icon="fas fa-sitemap" />
                    <insite:Button runat="server" ID="ClassifyLink" ToolTip="Classify" ButtonStyle="Default" Text="Classify" Icon="fas fa-tag" />
                </div>

                <insite:Nav runat="server">

                    <insite:NavItem runat="server" Title="Details">

                        <div class="row">

                            <div class="col-md-4">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="StandardType" FieldName="Type" ValidationGroup="Asset" />
                                    </label>
                                    <div>
                                        <insite:StandardTypeComboBox ID="StandardType" runat="server" />
                                    </div>
                                    <div class="form-text">What type of standard is this?</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Tier</label>
                                    <div>
                                        <insite:TextBox runat="server" ID="AssetTier" MaxLength="30" />
                                    </div>
                                    <div class="form-text">
                                        The tier identifies the hierarchical position above (+) or below (-) the standard that is used for assessments (0).
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Tag</label>
                                    <div>
                                        <insite:TextBox runat="server" ID="AssetLabel" MaxLength="30" />
                                    </div>
                                    <div class="form-text">
                                        A label allows you to use whatever term is used in your organization to refer to this type of standard.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Code
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="Code" MaxLength="40" />
                                    </div>
                                    <div class="form-text">
                                        An alphanumeric code used to identify the standard in a list.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Integration Hook
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="Hook" MaxLength="9" />
                                    </div>
                                    <div class="form-text">
                                        A unique code used to identify the standard.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Alias
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="Alias" MaxLength="300" />
                                    </div>
                                    <div class="form-text">
                                        An informal alternative to the formal title for this standard.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Department 
                                    </label>
                                    <div>
                                        <insite:GroupComboBox runat="server" ID="DepartmentIdentifier" />
                                    </div>
                                    <div class="form-text">
                                        (help)
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Industry
                                    </label>
                                    <div>
                                        <insite:CollectionItemComboBox runat="server" ID="IndustryIdentifier" MaxLength="40" />
                                    </div>
                                    <div class="form-text">
                                        Select Department first.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Internal Name</label>
                                    <div>
                                        <insite:TextBox ID="ContentName" runat="server" MaxLength="512" />
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        External Title
                                        <insite:RequiredValidator runat="server" ControlToValidate="ContentTitleEn" FieldName="External Title" ValidationGroup="Asset" />
                                    </label>
                                    <div>
                                        <insite:TextBox ID="ContentTitleEn" runat="server" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="download" runat="server" ID="DownloadLink" ToolTip="Download" />
                                        <insite:IconLink runat="server" ID="DeleteLink" ToolTip="Delete standard" Name="trash-alt" />
                                    </div>
                                    <label class="form-label">
                                        Standard Identifier
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="StandardIdentifier" />
                                    </div>
                                    <div class="form-text">
                                        A globally unique identifier for this standard.
                                    </div>
                                </div>

                            </div>

                            <div class="col-md-4">


                                <div class="form-group mb-3" runat="server" id="ParentField">
                                    <label class="form-label">
                                        Parent
                                        <insite:IconButton runat="server" ID="ShowParentInfoButton" CssClass="edit ms-1" Visible="false" Name="bars" ToolTip="View Details" />
                                    </label>
                                    <uc:StandardPopupSelector runat="server" ID="ParentAssetID" />
                                    <div class="form-text">
                                        Select the container for this standard to place it in a hierarchy.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Source/Reference</label>
                                    <div>
                                        <insite:TextBox runat="server" ID="SourceDescriptor" TextMode="MultiLine" Rows="6" MaxLength="3400"/>
                                    </div>
                                    <div class="form-text">
                                        The background source or external reference for this standard. 
                                        For example, this might be an industry code for a business activity, or a regulation identification number.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Author Name</label>
                                    <div>
                                        <insite:TextBox runat="server" ID="AuthorName" MaxLength="100" />
                                    </div>
                                    <div class="form-text">
                                        Name of the individual who authored this standard.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Date Authored</label>
                                    <div>
                                        <insite:DateTimeOffsetSelector runat="server" ID="AuthorDate" />
                                    </div>
                                    <div class="form-text">
                                        Date the standard was initially written.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Date Posted
                                        <insite:RequiredValidator runat="server" ControlToValidate="DatePosted" FieldName="Date Posted" ValidationGroup="Asset" />
                                    </label>
                                    <div>
                                        <insite:DateTimeOffsetSelector runat="server" ID="DatePosted" ReadOnly="true" />
                                    </div>
                                    <div class="form-text">
                                        Date the standard was entered into this system.
                                    </div>
                                </div>

                            </div>

                            <div class="col-md-4">


                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Configuration Properties
                                    </label>
                                    <div>
                                        <div class="row">
                                            <div class="col-xs-12">
                                                <div id="xsettings-list">

                                                    <label class="form-label">
                                                        <asp:CheckBox ID="IsPractical" runat="server" />
                                                        <span>Practical</span>
                                                    </label>
                                                    <label class="form-label">
                                                        <asp:CheckBox ID="IsTheory" runat="server" />
                                                        <span>Theory</span>
                                                    </label>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-text">
                                        Toggle these settings to control functionality related to this standard.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Competency Score Summarization Method</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="CompetencyScoreSummarizationMethod">
                                            <Items>
                                                <insite:ComboBoxOption />
                                                <insite:ComboBoxOption Value="sum_tier1" Text="Summarize by GAC" />
                                                <insite:ComboBoxOption Value="sum_tier0" Text="Summarize by GAC and Competency" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                    <div class="form-text">This determines how assessment results are calculated.</div>
                                </div>

                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CalculationMethodUpdatePanel" />
                                <insite:UpdatePanel runat="server" ID="CalculationMethodUpdatePanel">
                                    <ContentTemplate>
                                        <div class="form-group mb-3">
                                            <label class="form-label">Competency Score Calculation Method</label>
                                            <div>
                                                <insite:ComboBox runat="server" ID="CompetencyScoreCalculationMethod">
                                                    <Items>
                                                        <insite:ComboBoxOption />
                                                        <insite:ComboBoxOption Value="DecayingAverage" Text="Decaying Average" />
                                                        <insite:ComboBoxOption Value="NumberOfTimes" Text="Number of Times" />
                                                        <insite:ComboBoxOption Value="MostRecentScore" Text="Most Recent Score" />
                                                        <insite:ComboBoxOption Value="HighestScore" Text="Highest Score" />
                                                    </Items>
                                                </insite:ComboBox>
                                            </div>
                                            <div class="form-text">Calculation method used for student mastery.</div>
                                        </div>

                                        <div runat="server" id="CalculationArgumentField" class="form-group mb-3">
                                            <label class="form-label">Calculation Argument</label>
                                            <div>
                                                <insite:NumericBox runat="server" ID="CalculationArgument" NumericMode="Integer" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </insite:UpdatePanel>

                                <div class="form-group mb-3">
                                    <label class="form-label">Max Points</label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="PointsPossible" MinValue="0" MaxValue="999.99" DecimalPlaces="2" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Mastery Points</label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="MasteryPoints" MinValue="0" MaxValue="999.99" DecimalPlaces="2" />
                                    </div>
                                </div>


                            </div>

                        </div>

                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="SettingsTab" Title="Settings">

                        <div class="row">

                            <div class="col-md-4">

                                <h3>Identification</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Sequence
                                        <insite:RequiredValidator runat="server" FieldName="Sequence" ControlToValidate="Sequence" ValidationGroup="Asset" />
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="Sequence" NumericMode="Integer" MinValue="0" />
                                    </div>
                                    <div class="form-text">
                                        The ordinal/index position of this asset in a list of related assets.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Level</label>
                                    <div class="hstack">
                                        <insite:TextBox runat="server" ID="LevelType" MaxLength="32" EmptyMessage="Type" CssClass="me-1" />
                                        <insite:TextBox runat="server" ID="LevelCode" MaxLength="1" EmptyMessage="Code" />
                                    </div>
                                    <div class="form-text">The training or program level the standard is associated with (e.g., Level 1, 2,...)</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Version</label>
                                    <div class="hstack">
                                        <insite:TextBox runat="server" ID="MajorVersion" MaxLength="8" EmptyMessage="Major" CssClass="me-1" />
                                        <insite:TextBox runat="server" ID="MinorVersion" MaxLength="8" EmptyMessage="Minor" />
                                    </div>
                                    <div class="form-text">The version of the standard (e.g., Date, Code, etc.)</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Identifier</label>
                                    <div>
                                        <asp:Literal runat="server" ID="Thumbprint" />
                                    </div>
                                    <div class="form-text">
                                        A globally unique identifier for this asset.
                                    </div>
                                </div>

                                <div runat="server" id="CanvasIdentifierField" class="form-group mb-3">
                                    <label class="form-label">Canvas Identifier</label>
                                    <div>
                                        <asp:Literal runat="server" ID="CanvasIdentifier" />
                                    </div>
                                    <div class="form-text">
                                        A unique identifier in Canvas.
                                    </div>
                                </div>

                                <h3>Description</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Notes</label>
                                    <div>
                                        <insite:TextBox runat="server" ID="DescriptionEn" TextMode="MultiLine" Rows="4" />
                                    </div>
                                </div>

                            </div>

                            <div class="col-md-4">

                                <h3>Accreditation</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Credit Identifier</label>
                                    <div>
                                        <insite:TextBox ID="CreditIdentifier" runat="server" MaxLength="32" />
                                    </div>
                                    <div class="form-text">The code or reference for the credit satisfied by the standard.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Credit Hours</label>
                                    <div>
                                        <insite:NumericBox ID="CreditHours" runat="server" DecimalPlaces="2" MinValue="0" MaxValue="999.99" />
                                    </div>
                                    <div class="form-text">The number of credit hours associated with completion of the standard.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Passing Score %</label>
                                    <div>
                                        <insite:NumericBox ID="PassingScore" runat="server" DecimalPlaces="2" MinValue="0" MaxValue="100" />
                                    </div>
                                </div>

                                <h3>Flags</h3>

                                <p runat="server" id="NoOrganizationTags">There are no organization flags to assign.</p>
                                <uc:OrganizationTags runat="server" ID="OrganizationTags" />

                            </div>

                            <div class="col-md-4 d-none">

                                <h3>Classification</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Complexity / Taxonomy</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="Complexity" />
                                    </div>
                                    <div class="form-text">
                                        The taxonomic level of the standard (e.g., Bloom's; Modified Bloom's).
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Criticality</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="Criticality" />
                                    </div>
                                    <div class="form-text">
                                        The degree of risk associated with failing to meet the standards of a competency element.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Frequency</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="Frequency">
                                            <Items>
                                                <insite:ComboBoxOption />
                                                <insite:ComboBoxOption Value="Routinely" Text="Routinely" />
                                                <insite:ComboBoxOption Value="Occasionally" Text="Occasionally" />
                                                <insite:ComboBoxOption Value="Unexpectedly" Text="Unexpectedly" />
                                                <insite:ComboBoxOption Value="Circumstantially" Text="Circumstantially" />
                                                <insite:ComboBoxOption Value="Timely" Text="Timely" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                    <div class="form-text">The frequency with which the standard is typically performed.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Recurrence</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="Recurrence">
                                            <Items>
                                                <insite:ComboBoxOption />
                                                <insite:ComboBoxOption Value="Hourly" Text="Hourly" />
                                                <insite:ComboBoxOption Value="Daily" Text="Daily" />
                                                <insite:ComboBoxOption Value="Weekly" Text="Weekly" />
                                                <insite:ComboBoxOption Value="Monthly" Text="Monthly" />
                                                <insite:ComboBoxOption Value="Annually" Text="Annually" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                    <div class="form-text">If scheduled, how often?</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Difficulty Level</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="Difficulty" />
                                    </div>
                                    <div class="form-text">
                                        The effort required to perform or learn a competency element.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Proficiency Time</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="Proficiency" />
                                    </div>
                                    <div class="form-text">The typical time required to gain proficiency.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Autonomy</label>
                                    <div>

                                        <insite:MultiComboBox runat="server" ID="AutonomyType">
                                            <Items>
                                                <insite:ComboBoxOption Value="Alone" Text="Alone" />
                                                <insite:ComboBoxOption Value="As Part of a Team" Text="As Part of a Team" />
                                            </Items>
                                        </insite:MultiComboBox>

                                        <div class="mt-1">
                                            <insite:MultiComboBox runat="server" ID="AutonomyOversight">
                                                <Items>
                                                    <insite:ComboBoxOption Value="Without Supervision" Text="Without Supervision" />
                                                    <insite:ComboBoxOption Value="With Supervision" Text="With Supervision" />
                                                </Items>
                                            </insite:MultiComboBox>
                                        </div>

                                    </div>
                                    <div class="form-text">
                                        How do practitioners typically perform this competency?
                                        Here you can indicate the degree of autonomy with which a standard is typically performed.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Automation Potential</label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="AutomationPotential" />
                                    </div>
                                    <div class="form-text">
                                        The probablity that technology will have an impact on this element.
                                    </div>
                                </div>

                            </div>

                        </div>

                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="CategoriesTab" Title="Categories">
                        <div class="row">
                            <div class="col-md-6">
                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AssetCategoriesUpdatePanel" />

                                <insite:UpdatePanel runat="server" ID="AssetCategoriesUpdatePanel">
                                    <ContentTemplate>
                                        <uc:CategoryCheckList runat="server" ID="AssetCategories" />
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                            </div>
                        </div>
                    </insite:NavItem>

                </insite:Nav>

            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="ContentTab" Title="Content" Icon="far fa-edit" IconPosition="BeforeText">
        <div class="card border-0 shadow-lg mt-3">
            <div class="card-body">
                <div class="content-details">
                    <insite:Nav runat="server" ID="ContentNavigation">
                    </insite:Nav>
                </div>
            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem ID="CompetenciesTab" runat="server" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText" Visible="false">
        <div class="card border-0 shadow-lg mt-3">
            <div class="card-body">
                <uc:CompetenciesPanel runat="server" ID="CompetenciesPanel" />
            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="RelationshipsTab" Title="Relationships" Icon="far fa-sitemap" IconPosition="BeforeText">
        <div class="card border-0 shadow-lg mt-3">
            <div class="card-body">
                <insite:Nav runat="server">

                    <insite:NavItem runat="server" ID="ContainmentsTab" Title="Containments">
                        <div class="row">
                            <div runat="server" id="ContainmentParentsField" class="col-md-6">
                                <uc:RelationshipList runat="server" ID="ContainmentParents" />
                            </div>
                            <div class="col-md-6">
                                <uc:RelationshipList runat="server" ID="ContainmentChildren" />
                            </div>
                        </div>
                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="ConnectionsTab" Title="Connections">
                        <div class="row">
                            <div runat="server" id="ConnectionFromField" class="col-md-6">
                                <uc:RelationshipList runat="server" ID="ConnectionFrom" />
                            </div>
                            <div class="col-md-6">
                                <uc:RelationshipList runat="server" ID="ConnectionTo" />
                            </div>
                        </div>
                    </insite:NavItem>

                </insite:Nav>
            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="GlossaryTermTab" Title="Glossary Terms" Icon="far fa-scroll" IconPosition="BeforeText">
        <div class="card border-0 shadow-lg mt-3">
            <div class="card-body">
                <uc:GlossaryTermGrid runat="server" ID="GlossaryTermGrid" />
            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="ScenarioQuesionsTab" Title="Questions">
        <div class="card border-0 shadow-lg mt-3">
            <div class="card-body">

                <uc:ScenarioQuestions runat="server" ID="ScenarioQuestions" />

            </div>
        </div>
    </insite:NavItem>

</insite:Nav>

<insite:Modal runat="server" ID="BulkRegistrationWindow" Title="Bulk Registration">

    <ContentTemplate>

        <div class="mt-3">
            CSV File (Email, First Name, Last Name)
        </div>

        <div>

            <div>
                <asp:FileUpload runat="server" ID="BulkRegistartionFile" Width="400px" />
                <insite:RequiredValidator runat="server" ControlToValidate="BulkRegistartionFile" ValidationGroup="BulkRegistartion" />
            </div>

            <div>
                <insite:SaveButton runat="server" ID="BulkRegistartionSaveButton" Text="Upload" ValidationGroup="BulkRegistartion" />
                <insite:CancelButton runat="server" ID="BulkRegistartionCancelButton" OnClientClick="assetDetail.bulkRegistartion.close(); return false;" />
            </div>

        </div>

    </ContentTemplate>
</insite:Modal>

<insite:Modal runat="server" ID="AssetInfoWindow" />
<insite:Modal runat="server" ID="RelationshipCreatorWindow" Title="Add New Relationships" Width="800px" MinHeight="600px" />


<insite:PageHeadContent runat="server">
    <style type="text/css">
        .content-details {

        }

            .content-details .content-cmds {
                position: absolute;
                right: 15px;
            }

            .content-details .content-string {
                padding-right: 80px;
                min-height: 50px;
            }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var assetDetail = window.assetDetail = window.assetDetail || {};

            assetDetail.bulkRegistartion = {
                show: function () {
                    modalManager.show('<%= BulkRegistrationWindow.ClientID %>');
                },
                close: function () {
                    modalManager.close('<%= BulkRegistrationWindow.ClientID %>');
                },
            };

            // Asset Info

            var allowCloseInfoWindow = true;

            $(window).on('message', function (e) {
                var eventData = String(e.originalEvent.data);
                if (!eventData.startsWith('insite.assetSummary:'))
                    return;

                var command = eventData.substring(19);

                if (command === 'disable-close')
                    allowCloseInfoWindow = false;
                else if (command === 'enable-close')
                    allowCloseInfoWindow = true;
            });

            assetDetail.showAssetInfo = function (number) {
                if (typeof number !== 'number' || isNaN(number))
                    return;

                loadInfoWindow(number);
            }

            function loadInfoWindow(number) {
                if ($.active > 0)
                    return;
                
                allowCloseInfoWindow = true;

                var wnd = modalManager.load('<%= AssetInfoWindow.ClientID %>', '/ui/admin/standards/info?asset=' + String(number));

                modalManager.setTitle(wnd, 'Loading...');

                $(wnd)
                    .on('hide.bs.modal', function (e, s, a) {
                        if (!allowCloseInfoWindow) {
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            return false;
                        }
                    })
                    .one('closing.modal.insite', function (e, s, a) {
                        if (a === null)
                            return;

                        if (a.action === 'redirect')
                            window.location = a.url;
                        else if (a.action === 'refresh')
                            reloadEditor();
                    })
                    .one('closed.modal.insite', function (e, s, a) {
                        if (a === null)
                            return;

                        if (a.action === 'relate') {
                            var wnd = modalManager.load('<%= RelationshipCreatorWindow.ClientID %>', '/ui/admin/standards/relationships/create?assetId=' + String(a.asset.id));
                            $(wnd)
                                .data('number', a.asset.number)
                                .one('closed.modal.insite', function (e, s, a) {
                                    if (a != null)
                                        reloadEditor();
                                    else
                                        loadInfoWindow($(s).data('number'));
                                });
                        }
                    });
            }

        })();

    </script>
</insite:PageFooterContent>