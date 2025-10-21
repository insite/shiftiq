<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankTranslation.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.BankTranslation" %>

<h3>Translation languages</h3>

<div class="form-group mb-3">
    <label class="form-label">
        Language
    </label>
    <div>
        <asp:Literal runat="server" ID="SurveyLanguage" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Translate To
    </label>
    <div>
        <asp:Literal runat="server" ID="SurveyTranslationLanguages" />
    </div>
</div>
