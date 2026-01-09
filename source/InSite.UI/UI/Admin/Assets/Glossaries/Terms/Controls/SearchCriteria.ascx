<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Assets.Glossaries.Terms.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="TermIdentifier" EmptyMessage="Term Identifier" MaxLength="36" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="TermName" EmptyMessage="Term Name" MaxLength="50" />
                    </div> 
                    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="TermTitle" EmptyMessage="Term Title" MaxLength="50" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="TermDefinition" EmptyMessage="Term Definition" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="TermStatus" EmptyMessage="Term Status" Width="100%">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Approved" Value="Approved" />
                                <insite:ComboBoxOption Text="Proposed" Value="Proposed" />
                                <insite:ComboBoxOption Text="Revised" Value="Revised" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    
                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsTranslated" EmptyMessage="Translation Status" Width="100%">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Translated" Value=True />
                                <insite:ComboBoxOption Text="Not Translated" Value=False />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    
                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="RevisionCountFrom" MinValue="0" NumericMode="Integer" EmptyMessage="Revision Count &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="RevisionCountThru" MinValue="0" NumericMode="Integer" EmptyMessage="Revision Count &le;" />
                    </div> 
                    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ProposedBy" EmptyMessage="Proposed By" MaxLength="50" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DateProposedSince" EmptyMessage="Proposed &ge;"  />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DateProposedBefore" EmptyMessage="Proposed &lt;" /> 
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LastRevisedBy" EmptyMessage="Last Revised By" MaxLength="50" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DateLastRevisedSince" EmptyMessage="Last Revised &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DateLastRevisedBefore" EmptyMessage="Last Revised &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ApprovedBy" EmptyMessage="Approved By" MaxLength="50" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DateApprovedSince" EmptyMessage="Approved &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="DateApprovedBefore" EmptyMessage="Approved &lt;" />
                    </div>
                </div>
            </div> 
        </div>
    </div>

    <div class="col-3">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
