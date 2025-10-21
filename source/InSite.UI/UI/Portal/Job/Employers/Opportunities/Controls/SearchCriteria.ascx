<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Opportunities.Controls.SearchCriteria" %>

<%@ Register Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" TagPrefix="uc" TagName="FilterManager" %>

<div class="row">

    <div class="col-12">
        <div id="toolbox" class="toolbox-section">
            <h4><insite:Literal runat="server" Text="Criteria" /></h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="JobTitle" EmptyMessage="Job Title" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="EmployerName" EmptyMessage="Employer Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:PositionTypeComboBox runat="server" ID="PositionType" EmptyMessage="Position Type"/>
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="JobLocation" EmptyMessage="Job Location" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:EmploymentTypeComboBox runat="server" ID="EmploymentType" EmptyMessage="Employment Type" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:GroupComboBox runat="server" ID="StreamIdentifier" EmptyMessage="Choose opportunity stream" />
                    </div>
                </div>
            </div>
        </div>
    </div>



</div>
