<%@ Page Language="C#" CodeBehind="Subscribe.aspx.cs" Inherits="InSite.UI.Lobby.Subscribe" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <div class="row">
        <div class="col-lg-4 col-md-6 offset-lg-1">
            <div class="view show" id="signin-view">

                <h2 class="h2"><insite:Literal runat="server" Text="Subscribe / Unsubscribe" /></h2>

                <p class="fs-ms text-body-secondary mb-4">
                    Please select your subscription preferences below.
                </p>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="InputPanel" />

                <insite:UpdatePanel runat="server" ID="InputPanel">
                    <ContentTemplate>

                        <insite:Alert runat="server" ID="Status" />

                        <insite:ValidationSummary runat="server" />

                        <div class="form-group mb-3">
                            <label class="form-label">
                                <insite:Literal runat="server" Text="Email" />
                            </label>
                            <insite:TextBox runat="server" ID="UserEmail" ReadOnly="true" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                <insite:Literal runat="server" Text="Name" />
                            </label>
                            <insite:TextBox runat="server" ID="UserName" ReadOnly="true" />
                        </div>

                        <div class="m-4">
                            <insite:CheckSwitch runat="server" ID="MarketingEmailDisabled" />
                            <div class="group-list">
                                <asp:Repeater runat="server" ID="GroupList">
                                    <ItemTemplate>
                                        <insite:CheckSwitch runat="server"
                                            ID="GroupIdentifier"
                                            Value='<%# Eval("Identifier") %>'
                                            Text='<%# Eval("Name") %>'
                                            Checked='<%# Eval("Selected") %>'
                                        />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                        <insite:SaveButton runat="server" ID="SubscribeButton" ValidationGroup="Subscribe" />

                    </ContentTemplate>
                </insite:UpdatePanel>

            </div>
        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script>
            (function () {
                Sys.Application.add_load(function () {
                    const chk = document.getElementById("<%= MarketingEmailDisabled.ClientID %>");

                    chk.addEventListener("click", enableGroups);

                    enableGroups();

                    function enableGroups() {
                        const unsubscribed = chk.checked;

                        document.querySelectorAll(".group-list input").forEach(input => {
                            if (unsubscribed) {
                                input.setAttribute("disabled", "disabled");
                            } else {
                                input.removeAttribute("disabled");
                            }
                        })
                    }
                });

            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>