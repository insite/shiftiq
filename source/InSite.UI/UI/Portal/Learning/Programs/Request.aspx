<%@ Page Language="C#" CodeBehind="Request.aspx.cs" Inherits="InSite.UI.Portal.Learning.Programs.ProgramRequest" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <section class="mb-3">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Alert runat="server" ID="StatusAlert" />

                <h1 class="h3 mb-3"><asp:Literal runat="server" ID="ProgramName" /></h1>

                <div runat="server" id="FeePanel" class="mb-3">
                    <span class="badge bg-info fs-6"><asp:Literal runat="server" ID="ProgramFee" /></span>
                </div>

                <div runat="server" id="SummaryPanel" class="mb-4">
                    <asp:Literal runat="server" ID="Summary" />
                </div>

                <div runat="server" id="AchievementsPanel" class="mb-4">
                    <h2 class="h5 mb-2">What you will earn</h2>
                    <p class="form-text mb-2">Enrollment assigns these achievements to your training plan</p>
                    <asp:Repeater runat="server" ID="AchievementRepeater">
                        <HeaderTemplate>
                            <ul class="list-group list-group-flush">
                        </HeaderTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                        <ItemTemplate>
                            <li class="list-group-item">
                                <%# Eval("AchievementTitle") %>
                                <asp:Label runat="server" CssClass="badge bg-secondary ms-2" Text="Required" Visible='<%# (bool)Eval("IsRequired") %>' />
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div runat="server" id="OpenRequestPanel" class="alert alert-info mb-0">
                    <i class="fas fa-hourglass-half me-2"></i>
                    You requested enrollment in this program on <strong><asp:Literal runat="server" ID="OpenRequestSubmittedAt" /></strong> and the request is waiting for a decision.
                    Contact <a runat="server" id="OpenRequestContactLink"><asp:Literal runat="server" ID="OpenRequestContact" /></a> if you have a question about it.
                </div>

                <div runat="server" id="RequestPanel">

                    <h2 class="h5 mb-2">Terms and conditions</h2>
                    <p>
                        Program fees are non-refundable. Once your request is approved and you are enrolled, the fee is due in full
                        and cannot be refunded, transferred, or credited, including if you withdraw from the program or do not
                        complete it. A company-sponsored request is billed to your organization; a self-funded request is billed to you.
                    </p>

                    <h3 class="h6 mb-2">Course content &amp; intellectual property</h3>
                    <p>
                        All course materials are the proprietary property of the course provider. By enrolling, you agree that content
                        is provided solely for your personal training and may not be copied, recorded, shared, or distributed.
                    </p>

                    <p class="form-text">Choose how the program fee will be paid. You will be asked to agree to the terms before the request is sent.</p>

                    <div>
                        <insite:Button runat="server" ID="SelfFundedButton" Text="Self-funded request" Icon="fas fa-user" ButtonStyle="Primary"
                            ConfirmText="I agree to the terms and conditions and will pay the program fee myself. Submit this enrollment request?" />
                        <insite:Button runat="server" ID="CompanySponsoredButton" Text="Company-sponsored request" Icon="fas fa-building" ButtonStyle="OutlinePrimary"
                            ConfirmText="I agree to the terms and conditions and ask my organization to sponsor the program fee. Submit this enrollment request?" />
                    </div>

                </div>

            </div>
        </div>

    </section>
</asp:Content>
