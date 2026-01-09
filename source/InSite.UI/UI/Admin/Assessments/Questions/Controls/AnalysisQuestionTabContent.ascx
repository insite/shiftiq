<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisQuestionTabContent.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.AnalysisQuestionTabContent" %>

<%@ Register TagPrefix="uc" TagName="AnalysisTable" Src="~/UI/Admin/Assessments/Attempts/Controls/QuestionAnalysisTable.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionRepeater" Src="~/UI/Admin/Assessments/Questions/Controls/QuestionRepeater.ascx" %>

<div class="row">
    <div class="col-lg-6">
        <div class="card h-100">
            <div class="card-body">
                <h3>Question</h3>
                <uc:QuestionRepeater runat="server" id="QuestionRepeater" />
            </div>
        </div>
    </div>
    <div class="col-lg-6">
        <div class="card h-100">
            <div class="card-body">
                <asp:Literal runat="server" ID="MessageLiteral" />

                <asp:Repeater runat="server" ID="AnalysisRepeater">
                    <ItemTemplate>
                        <h3><%# Eval("FormName") %></h3>

                        <uc:AnalysisTable runat="server" id="AnalysisTable" />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>
