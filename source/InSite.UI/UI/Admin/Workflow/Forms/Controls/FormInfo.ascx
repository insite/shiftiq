<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormInfo.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.FormInfo" %>

<div runat="server" id="NameField" class="form-group mb-3">
    <label class="form-label">
        Internal Name
    </label>
    <div>
        <a runat="server" id="SurveyLink">
            <asp:Literal runat="server" ID="InternalName" />
        </a>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        External Title
    </label>
    <div>
        <asp:Literal runat="server" ID="ExternalTitle" />
    </div>
</div>

<insite:Container runat="server" ID="StatusContainer">
    <div class="form-group mb-3">
        <label class="form-label">
            Current Status
        </label>
        <div>
            <asp:Literal runat="server" ID="CurrentStatus" />
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">
            Opened Date
        </label>
        <div>
            <asp:Literal runat="server" ID="Opened" />
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">
            Close Date
        </label>
        <div>
            <asp:Literal runat="server" ID="Closed" />
        </div>
    </div>
</insite:Container>
