<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LaunchCardRepeater.ascx.cs" Inherits="InSite.UI.Portal.Controls.LaunchCardRepeater" %>

<div runat="server" id="SummaryPanel" class="text-body-secondary mb-3"></div>

<div class="row">
    <asp:Repeater runat="server" ID="CardRepeater">
        <ItemTemplate>

            <div class="col-lg-<%= ColumnSize %> col-sm-6 mb-4">
                <a class="card card-hover card-tile border-0 shadow h-100" href='<%# (bool)Eval("IsOverview") ? "#overview" : Eval("Url") %>' target='<%# Eval("Target") %>' data-modal='<%# (bool)Eval("IsOverview") ? Container.FindControl("ModalOverview").ClientID : null %>'>
                    <asp:Literal runat="server" ID="Category" />
                    <asp:Literal runat="server" ID="Flag" />
                    <asp:Literal runat="server" ID="Image" />
                    <div class="card-body text-center">
                        <asp:Literal runat="server" ID="Icon" />
                        <h3 runat="server" id="Title" class="h5 nav-heading mb-2"></h3>
                        <div runat="server" id="Summary" class="fs-sm text-body-secondary mb-2"></div>
                        <div runat="server" id="Progress" />
                    </div>
                </a>
                <insite:Modal runat="server" ID="ModalOverview" Visible="false">
                    <ContentTemplate>
                        <div runat="server" id="ModalOverviewBody"></div>
                        <div class="mt-4">
                            <insite:CloseButton runat="server" data-action="cancel" />
                        </div>
                    </ContentTemplate>
                </insite:Modal>
            </div>

        </ItemTemplate>
    </asp:Repeater>
</div>

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        (function () {
            document.querySelectorAll('a[href="#overview"]').forEach(anchor => {
                if (!anchor.dataset.modal)
                    return;

                anchor.addEventListener('click', onOverviewClick);
            });

            function onOverviewClick(e) {
                if (!this.dataset.modal)
                    return;

                const modal = document.getElementById(this.dataset.modal);
                if (!modal)
                    return;

                e.preventDefault();
                e.stopPropagation();

                modalManager.show(modal);
            }
        })();
    </script>
</insite:PageFooterContent>