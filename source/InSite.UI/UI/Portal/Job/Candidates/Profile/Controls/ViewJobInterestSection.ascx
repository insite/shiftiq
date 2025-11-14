<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewJobInterestSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.ViewJobInterestSection" %>

<div class="card mb-3">
    <div class="card-body">

        <h4 class="card-title mb-3">Job Interest Information</h4>

        <div class="row">
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Are you actively seeking employment?
                    </label>
                    <div class="px-4">
                        <asp:Literal ID="ActivelySeeking" runat="server"></asp:Literal>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Current city
                    </label>
                    <div class="px-4">
                        <asp:Literal ID="HomeAddressCity" runat="server"></asp:Literal>
                    </div>
                </div>

                <div class="form-group mb-3" runat="server" id="RelocateInformation">
                    <label class="form-label">
                        Are you willing to move to another city?
                    </label>
                    <div class="px-4">
                        <asp:Literal ID="IsWillingToRelocate" runat="server"></asp:Literal>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        LinkedIn Profile
                    </label>
                    <div class="px-4">
                        <asp:HyperLink ID="LinkedInUrl" runat="server"></asp:HyperLink>
                    </div>
                </div>

            </div>

            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Seeking employment in:
                    </label>
                    <div class="px-4">
                        <asp:Literal ID="Occupation" runat="server"></asp:Literal>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label runat="server" id="AccountStatusLabel" class="form-label">
                        Current Status
                    </label>
                    <div class="px-4">
                        <asp:Literal ID="AccountStatus" runat="server"></asp:Literal>
                    </div>
                </div>

            </div>

        </div>

    </div>
</div>
