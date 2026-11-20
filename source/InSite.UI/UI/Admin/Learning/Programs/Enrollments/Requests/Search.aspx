<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.Admin.Records.Programs.EnrollmentRequests" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <div class="row mb-3">
        <div class="col-lg-3">
            <insite:ComboBox runat="server" ID="Status" AutoPostBack="true" EmptyMessage="All statuses" />
        </div>
    </div>

    <div runat="server" id="CompanySponsoredTerms" class="alert alert-info" visible="false">
        <i class="fas fa-file-contract me-2"></i>
        Program fees are non-refundable. Once you approve a request, the learner is enrolled, the fee is added to
        <asp:Literal runat="server" ID="CompanySponsoredTermsOrganization" />'s quarterly program billing, and the fee
        cannot be refunded, transferred, or credited, including if the learner withdraws from the program or does not
        complete it.
    </div>

    <div runat="server" id="SelfFundedTerms" class="alert alert-warning" visible="false">
        <i class="fas fa-money-check-alt me-2"></i>
        Note: Please ensure payment has been collected before approving this request.
    </div>

    <insite:Grid runat="server" ID="Grid" DataKeyNames="ProgramEnrollmentRequestIdentifier">
        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False">
                <ItemTemplate>
                    <insite:IconLink runat="server" Name="search" ToolTip="Outline"
                        NavigateUrl='<%# InSite.Admin.Records.Programs.EnrollmentRequestOutline.GetNavigateUrl((Guid)Eval("ProgramEnrollmentRequestIdentifier")) %>' />
                    <insite:Button runat="server" Text="Approve" Icon="fas fa-thumbs-up" ButtonStyle="Success" Size="Small"
                        Visible='<%# IsPending(Eval("RequestStatus")) %>'
                        CommandName="Approve"
                        ConfirmText='<%# ApproveConfirmText(Eval("RequestFundingPath"), Eval("OrganizationIdentifier")) %>' />
                    <insite:Button runat="server" Text="Reject" Icon="fas fa-thumbs-down" ButtonStyle="Danger" Size="Small"
                        Visible='<%# IsPending(Eval("RequestStatus")) %>'
                        CommandName="Reject"
                        ConfirmText="Reject this enrollment request?" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Submitted" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                <ItemTemplate><%# LocalizeDate(Eval("RequestSubmittedAt")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Learner">
                <ItemTemplate><%# LearnerCell(Eval("LearnerName"), Eval("LearnerUserEmail")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Program">
                <ItemTemplate><%# Eval("ProgramName") %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Fee" ItemStyle-Wrap="False">
                <ItemTemplate>
                    <%# Eval("RequestFee", "{0:C}") %>
                    <%# FundingPathBadge(Eval("RequestFundingPath")) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status" ItemStyle-Wrap="False">
                <ItemTemplate><%# StatusText(Eval("RequestStatus")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Decided" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                <ItemTemplate><%# LocalizeDate(Eval("RequestDecidedAt")) %></ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </insite:Grid>

</asp:Content>
