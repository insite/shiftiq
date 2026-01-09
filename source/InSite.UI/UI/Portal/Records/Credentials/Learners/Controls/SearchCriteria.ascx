<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Portal.Records.Credentials.Learners.Controls.SearchCriteria" %>

<%@ Register Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" TagPrefix="uc" TagName="FilterManager" %>

<div class="row">

    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4><insite:Literal runat="server" Text="Filter" /></h4>
            <div class="row">
                <div class="col-12">
                    <div class="mb-2">
                        <insite:FindAchievement runat="server" ID="AchievementIdentifier" EmptyMessage="Achievements" EnableTranslation="true" />
                    </div>
                    <div class="mb-2">
                        <insite:SearchButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-6">

        <div class="mb-2">
            <h4><insite:Literal runat="server" Text="Saved Filters" /></h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>

    </div>

</div>