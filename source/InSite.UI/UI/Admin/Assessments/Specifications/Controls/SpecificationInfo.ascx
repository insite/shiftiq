<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SpecificationInfo.ascx.cs" Inherits="InSite.Admin.Assessments.Specifications.Controls.SpecificationInfo" %>

<div id="NameDiv" runat="server" class="form-group mb-3">
    <label class="form-label">Specification Name</label>
    <div>
        <asp:Literal runat="server" ID="SpecificationName" />
    </div>
</div>

<div id="TypeDiv" runat="server" class="form-group mb-3">
    <label class="form-label">Specification Type</label>
    <div>
        <asp:Literal runat="server" ID="SpecificationType" />
    </div>
</div>

<div id="ConfDiv" runat="server">
    <div class="form-group mb-3">
        <label class="form-label">Form Limit</label>
        <div>
            <asp:Literal runat="server" ID="SpecificationFormLimit" />
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">Question Limit per Form</label>
        <div>
            <asp:Literal runat="server" ID="SpecificationQuestionLimit" />
        </div>
    </div>
</div>

<div id="CalcDiv" runat="server">

    <div class="form-group mb-3">
        <label class="form-label">Disclosure Type</label>
        <div>
            <asp:Literal runat="server" ID="SpecificationCalculationDisclosure" />
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">Passing Score</label>
        <div>
            <asp:Literal runat="server" ID="SpecificationCalculationPassingScore" />
        </div>
    </div>

</div>