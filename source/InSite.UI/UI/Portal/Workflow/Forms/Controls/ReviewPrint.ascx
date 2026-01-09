<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReviewPrint.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.ReviewPrint" %>

<%@ Register TagPrefix="uc" TagName="ReviewDetails" Src="~/UI/Portal/Workflow/Forms/Controls/ReviewDetails.ascx" %>

<insite:Container runat="server" ID="SummaryStyle">
    <style type="text/css">
        .sidebar { display: none; }
        .bg-primary { background-color: #457897 !important; }
    </style>
</insite:Container>

<div class="row">
    <div class="col-lg-12">
        <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
    </div>
</div>

<uc:ReviewDetails runat="server" ID="Details" />
