<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencyDetail.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Competencies.Controls.CompetencyDetail" %>

<div class="form-group mb-3">
    <label class="form-label">Framework</label>
    <div>
        <asp:Literal runat="server" ID="Framework" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">GAC</label>
    <div>
        <asp:Literal runat="server" ID="GAC" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Competency</label>
    <div>
        <asp:Literal runat="server" ID="Competency" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        <insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/>
    </label>
    <div>
        <asp:Literal runat="server" ID="Hours" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Satisfaction Level</label>
    <div>
        <asp:Literal runat="server" ID="SatisfactionLevel" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">Skill Rating</label>
    <div>
        <asp:Literal runat="server" ID="SkillRating" />
    </div>
</div>