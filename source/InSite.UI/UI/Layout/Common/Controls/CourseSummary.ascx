<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CourseSummary.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.CourseSummary" %>

<div class="container">
    <div class="row">
        <div class="col-lg-12">

            <div class="card card-hover card-tile mb-4">
                <div class="card-body">
                    <h5 class="card-title" runat="server" id="BlockTitle"><%= Translate("Title") %></h5>
                    <div class="card-text" runat="server" ID="BlockDescription"><%= Translate("Description") %></div>
                    <div class="card-text text-body-secondary mt-3 mb-3"><%= Translate("Time Required") %>: <asp:Literal runat="server" ID="BlockTimeRequired" /></div>
                    <insite:Button NavigateUrl="#" ButtonStyle="Success" CssClass="btn-sm" runat="server" ID="BlockStartUrl" Text="Start" Icon="far fa-rocket-launch" />
                </div>
            </div>

        </div>
    </div>
</div>