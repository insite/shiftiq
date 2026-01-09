<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Portal.Standards.Documents.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">

    <div class="col-lg-8">
        <div id="toolbox" class="toolbox-section">
            <h4><insite:Literal runat="server" Text="Criteria" /></h4>
            <div class="row">

                <div class="col-lg-6">

                    <div class="mb-2">
                        <insite:TextBox ID="Keyword" runat="server" MaxLength="30" EmptyMessage="Keyword" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="IsTemplate" EmptyMessage="Template">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="True" Text="Templates Only" />
                                <insite:ComboBoxOption Value="False" Text="My Documents Only" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                
                    <div class="mb-2">
                        <insite:TextBox ID="Title" runat="server" MaxLength="256" EmptyMessage="Title" />
                    </div>

                    <div class="mb-2">
                        <insite:ComboBox runat="server" ID="Level" EmptyMessage="Level">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Frontline" Text="Frontline" />
                                <insite:ComboBoxOption Value="Supervisory" Text="Supervisory" />
                                <insite:ComboBoxOption Value="Management/Executive" Text="Management/Executive" />
                            </Items>
                        </insite:ComboBox>
                    </div>

                    <div class="mb-2">
                        <insite:SearchButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>

                </div>

                <div class="col-lg-6">

                    <div class="mb-2">
                        <insite:DateSelector ID="UtcPostedSince" runat="server" EmptyMessage="Posted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateSelector ID="UtcPostedBefore" runat="server" EmptyMessage="Posted Before" />
                    </div>

                </div>

            </div>
        </div>
    </div>

    <div class="col-lg-4">
        <div class="mb-2">
            <h4><insite:Literal runat="server" Text="Saved Filters" /></h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>

</div>