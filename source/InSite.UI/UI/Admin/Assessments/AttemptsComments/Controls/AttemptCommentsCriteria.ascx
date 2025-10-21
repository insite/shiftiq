<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttemptCommentsCriteria.ascx.cs" Inherits="InSite.Admin.Assessments.Reports.Controls.SubmissionCommentaryCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="FormTitle" EmptyMessage="Form Title" MaxLength="100" />
                    </div>
    
                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="AssetNumber" EmptyMessage="Asset #" NumericMode="Integer" DigitGrouping="false" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Comment" EmptyMessage="Comment" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
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
