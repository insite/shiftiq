<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Courses.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-3">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-12">

                    <div class="mb-2">
                        <insite:CatalogComboBox runat="server" ID="CatalogIdentifier" EmptyMessage="Catalog" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CourseName" EmptyMessage="Course Name" MaxLength="200" />
                    </div>
    
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="CourseLabel" EmptyMessage="Course Tag" MaxLength="20" />
                    </div> 
                    
                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="CourseAsset" EmptyMessage="Asset Number" NumericMode="Integer" />
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
        <div>
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
