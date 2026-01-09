<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TakerReportOptions.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.TakerReportOptions" %>

<h3>Report Options</h3>

<div class="form-group mb-3">
    <label class="form-label">
        Select which language to generate the report in
    </label>
    <div>
        <insite:RadioButton runat="server"
            ID="EnglishLanguage"
            Text="English"
            Value="en"
            Checked="true"
            GroupName="Report"
        />
        <insite:RadioButton runat="server"
            ID="FrenchLanguage"
            Text="French"
            Value="fr"
            GroupName="Report"
        />
    </div>
</div>