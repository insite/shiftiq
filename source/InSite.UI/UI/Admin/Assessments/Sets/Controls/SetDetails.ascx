<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SetDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Sets.Controls.SetDetails" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<div class="row">
    <div class="col-lg-6">

        <h3>Identification</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Set Number
                <insite:IconLink Name="trash-alt" runat="server" ID="DeleteSetLink" ToolTip="Delete Set" />
            </label>
            <div>
                <asp:Literal runat="server" ID="SetNumber" />
            </div>
            <div class="form-text">
                This set contains <asp:Literal runat="server" ID="SetQuestionCount" /> of the <asp:Literal runat="server" ID="BankQuestionCount" /> questions in the bank.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Set Name
                <insite:IconLink Name="pencil" runat="server" ID="RenameSetLink" ToolTip="Rename Set" />
            </label>
            <div>
                <asp:Literal runat="server" ID="Name" />
            </div>
            <div class="form-text">
                The name that uniquely identifies this question set within the bank.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Standard
                <insite:IconLink Name="pencil" runat="server" ID="EditStandardLink" ToolTip="Change Set Standard" />
            </label>
            <div>
                <assessments:AssetTitleDisplay runat="server" ID="Standard" />
            </div>
            <div class="form-text">
                The standard evaluated by question items in this set.
            </div>
        </div>

    </div>
    <div class="col-lg-6">

        <h3>Configuration</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Randomization
                <insite:IconLink Name="pencil" runat="server" ID="EditRandomization" ToolTip="Change Randomization" />
            </label>
            <div>
                <asp:Literal runat="server" ID="Randomization" />
                <asp:Literal runat="server" ID="RandomizationDescription" />
            </div>
            <div class="form-text">
                The randomization settings for the question items in this set.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Cut Score
            </label>
            <div>
                <asp:Literal runat="server" ID="CutScore" />
            </div>
            <div class="form-text">
                In the special-case of a Scenario question set, this is the cut-score for this question set. This property
                does not apply to sets where Type = Pool.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Points
            </label>
            <div>
                <asp:Literal runat="server" ID="Points" />
            </div>
            <div class="form-text">
                In the special-case of a Scenario question set, this is the maximum number of points that can be awarded 
                for answers to the question items in the set. This property does not apply to sets where Type = Pool.
            </div>
        </div>

    </div>
</div>