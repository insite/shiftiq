<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpportunityInfo.ascx.cs" Inherits="InSite.Admin.Jobs.Opportunities.Controls.OpportunityInfo" %>

<div class="card shadow">
    <div class="card-body">

        <h5 class="card-title">Opportunity</h5>
        <dl class="row mb-0">

            <dt class="col-sm-3">Job Position</dt>
            <dd class="col-sm-9"><asp:Literal runat="server" ID="OpportunityTitle" /></dd>

            <dt class="col-sm-3">Employer</dt>
            <dd class="col-sm-9"><asp:Literal runat="server" ID="OpportunityEmployer" /></dd>

            <dt class="col-sm-3">Type of Employment</dt>
            <dd class="col-sm-9"><asp:Literal runat="server" ID="OpportunityEmploymentType" /></dd>

            <dt class="col-sm-3">Position Type</dt>
            <dd class="col-sm-9"><asp:Literal runat="server" ID="OpportunityPositionType" /></dd>

            <dt class="col-sm-3">Position Level</dt>
            <dd class="col-sm-9"><asp:Literal runat="server" ID="OpportunityPositionLevel" /></dd>

            <dt runat="server" id="OpportunityPublishedLabel" class="col-sm-3">Published</dt>
            <dd runat="server" id="OpportunityPublishedValue" class="col-sm-9">
                <asp:Literal runat="server" ID="OpportunityPublished" />
            </dd>

        </dl>

    </div>
</div>