<%@ Page Language="C#" CodeBehind="ChangeSurveyCondition.aspx.cs" Inherits="InSite.Admin.Surveys.Forms.Forms.ChangeSurveyCondition" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/SurveyConditionDetail.ascx" TagName="SurveyConditionDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Survey" />

    <section runat="server" id="SurveyInformationSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-check-square me-1"></i>
            Change Condition
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <uc:SurveyConditionDetail runat="server" ID="Detail" />
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" Text="Change Condition" ValidationGroup="Survey" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>
</asp:Content>
