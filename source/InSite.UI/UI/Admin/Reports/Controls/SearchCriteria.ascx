<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>
    
<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ReportTitle" EmptyMessage="Title" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="ReportDescription" EmptyMessage="Description" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="ReportType" EnableTranslation="true" EmptyMessage="Report Type">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Custom" Value="Custom" />
                                <insite:ComboBoxOption Text="Search" Value="Search" />
                                <insite:ComboBoxOption Text="Shared" Value="Shared" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Shared Reports" />
                        </label>
                        <insite:RadioButton runat="server" ID="SharedReportMine" Value="Mine" GroupName="SharedReports" Checked="true" />
                        <insite:RadioButton runat="server" ID="SharedReportYes" Value="Shared with me" GroupName="SharedReports" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="CreatedBy" EmptyMessage="Created By" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector runat="server" ID="ModifiedSince" EmptyMessage="Modified &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="ModifiedBefore" runat="server" EmptyMessage="Modified &lt;" />
                    </div>
                </div>
            </div>
            
            <insite:FilterButton runat="server" ID="SearchButton" />
            <insite:ClearButton runat="server" ID="ClearButton" />
        </div>
    </div>
    <div class="col-3">       
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
