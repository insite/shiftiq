<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Quizzes.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-6">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-6">
                    <div class="mb-2">
                        <insite:QuizTypeComboBox runat="server" ID="QuizType" EmptyMessage="Quiz Type" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuizName" EmptyMessage="Quiz Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="QuizText" EmptyMessage="Quiz Text" MaxLength="100" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="TimeLimitFrom" MinValue="0" NumericMode="Integer" EmptyMessage="Time Limit &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="TimeLimitThru" MinValue="0" NumericMode="Integer" EmptyMessage="Time Limit &le;" />
                    </div> 

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="AttemptLimitFrom" MinValue="0" NumericMode="Integer" EmptyMessage="Attempt Limit &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:NumericBox runat="server" ID="AttemptLimitThru" MinValue="0" NumericMode="Integer" EmptyMessage="Attempt Limit &le;" />
                    </div> 
                </div>
            </div>
            <div class="mb-2">
                <insite:FilterButton runat="server" ID="SearchButton" />
                <insite:ClearButton runat="server" ID="ClearButton" />
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
