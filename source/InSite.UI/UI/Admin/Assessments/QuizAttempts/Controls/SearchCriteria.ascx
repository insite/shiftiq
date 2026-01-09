<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Assessments.QuizAttempts.Controls.SearchCriteria" %>

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
                        <insite:TextBox runat="server" ID="LearnerName" EmptyMessage="Learner Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="LearnerEmail" EmptyMessage="Learner Email" MaxLength="100" />
                    </div>
                </div>
                <div class="col-6">
                    <div class="mb-2">
                        <insite:BooleanComboBox runat="server" ID="IsCompleted" EmptyMessage="Attempt Status" TrueText="Completed" FalseText="Not Completed" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="AttemptStartedSince" EmptyMessage="Attempt Started Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector runat="server" ID="AttemptStartedBefore" EmptyMessage="Attempt Started Before" />
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
