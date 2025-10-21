<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Grade.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Forms.Grade" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/RubricCriteriaList.ascx" TagName="RubricCriteriaList" TagPrefix="uc" %>
<%@ Register Src="../Controls/RubricSummary.ascx" TagName="RubricSummary" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="ViewerStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Rubric" />

    <uc:RubricSummary runat="server" ID="Summary" />

    <section runat="server" id="RubricPanel">
        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <h3><%= Translate("Question") %></h3>

                <div runat="server" id="QuestionText" style="white-space:pre-wrap;">
                </div>
            </div>
        </div>

        <div runat="server" id="ExemplarCard" class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <h3><%= Translate("Exemplar") %></h3>

                <div runat="server" id="ExemplarText">
                </div>
            </div>
        </div>

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <h3><%= Translate("Response") %></h3>

                <div>
                    <asp:Literal runat="server" ID="AnswerText" />
                    <insite:OutputAudio runat="server" ID="AnswerAudio" />
                </div>
            </div>
        </div>

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">
                <h3>
                    <asp:Literal runat="server" ID="RubricTitle" />
                </h3>

                <div class="mb-3">
                    <b><%= Translate("Total Rubric Points:") %> <asp:Literal runat="server" ID="CriteriaRubricPoints" /></b>
                </div>

                <uc:RubricCriteriaList runat="server" ID="RubricCriteriaList" />

            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-6">
            <insite:Button runat="server" ID="PrevButton" Text="Prev" Icon="fas fa-arrow-alt-left" ButtonStyle="Default" />
            <insite:NextButton runat="server" ID="NextButton" ValidationGroup="Rubric" />
            <insite:SaveButton runat="server" ID="SubmitButton" Text="Submit" />
            <insite:CancelButton runat="server" ID="CancelButton" ConfirmText="Are you sure to cancel the grade process?" />

            <span runat="server" id="RubricPosition" class="ms-4">
            </span>
        </div>
        <div runat="server" id="SumPanel" class="col-lg-6 text-end">
        </div>
    </div>

    <asp:CustomValidator runat="server" ID="RubricValidator" ErrorMessage="Points are required for all criteria" Display="None" ValidationGroup="Rubric" />

    <insite:PageFooterContent runat="server">
        <script>
            function updateAnswerSumPoints(sum) {
                document.getElementById("<%= ClientID %>_AnswerSumPoints").innerHTML = sum.toFixed(2);
            }
        </script>
    </insite:PageFooterContent>

</asp:Content>
