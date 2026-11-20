<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Records.Programs.AddUser" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Messages/Subscribers/Controls/AddControl.ascx" TagName="AddControl" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section runat="server" id="FundingPanel" class="mb-3" visible="false">
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h3>Paid program</h3>

                <p class="form-text">
                    This program has a fee of <strong><asp:Literal runat="server" ID="ProgramFeeText" /></strong>.
                    Each learner you add is enrolled immediately, and an approved enrollment request is recorded for billing.
                    Choose how the fee is funded before adding learners.
                </p>

                <div class="form-group mb-3">
                    <label class="form-label">Funding path</label>
                    <div>
                        <asp:RadioButtonList runat="server" ID="FundingPath" RepeatLayout="Flow" RepeatDirection="Vertical" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Learner's organization</label>
                    <div>
                        <insite:FindOrganization runat="server" ID="LearnerOrganizationIdentifier" />
                    </div>
                    <div class="form-text">
                        The organization the learners below are searched in. A company-sponsored request is also billed to it, so it is required for that funding path. A self-funded request is billed to the learner.
                    </div>
                </div>

            </div>
        </div>
    </section>

    <uc:AddControl runat="server" ID="ScreenControl" />

</asp:Content>
