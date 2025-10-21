<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Jobs.Applications.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-3">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <insite:FindOpportunity runat="server" ID="JobID" EmptyMessage="Job" />
                    </div>

                    <div class="mb-2">
                         <insite:FindPerson runat="server" ID="CandidateContactID" EmptyMessage="Candidate" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployerName" EmptyMessage="Employer Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="JobPosition" EmptyMessage="Job Position" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateUpdatedSince" runat="server" EmptyMessage="Date Updated Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="DateUpdatedBefore" runat="server" EmptyMessage="Date Updated Before" />
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
    </div>
    <div class="col-3">              
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
