<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidateBadge.aspx.cs" Inherits="InSite.UI.Lobby.ValidateBadge" MasterPageFile="~/UI/Layout/Lobby/LobbyEmpty.master" %>

<%@ Register Src="~/UI/Layout/Common/Controls/BundleCss.ascx" TagPrefix="uc" TagName="BundleCss" %>
<%@ Register Src="~/UI/Layout/Common/Controls/BundleJsHeader.ascx" TagPrefix="uc" TagName="BundleJsHeader" %>
<%@ Register Src="~/UI/Layout/Common/Controls/BundleJsFooter.ascx" TagPrefix="uc" TagName="BundleJsFooter" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <uc:BundleCss runat="server" />
    <insite:StyleLink runat="server" />
    <uc:BundleJsHeader runat="server" ID="HeaderCurrentScripts" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:PageHeadContentRenderer runat="server" />

    <div class="mt-5">

        <div class="card shadow mb-3 mx-auto" style="width:600px;">
            <h5 class="card-header">
                Validate Open Badge
            </h5>
            <div class="card-body">

                <div runat="server" id="ViewInput" style="display:none;">
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Assertion
                        </label>
                        <insite:ComboBox runat="server" ID="AssertionType">
                            <Items>
                                <insite:ComboBoxOption Value="url" Text="URL" Selected="true" />
                                <insite:ComboBoxOption Value="json" Text="JSON" />
                            </Items>
                        </insite:ComboBox>
                        <insite:TextBox runat="server" ID="AssertionUrl" EmptyMessage="https://..." CssClass="mt-2" />
                        <insite:TextBox runat="server" ID="AssertionJson" EmptyMessage="{ ... }" TextMode="MultiLine" Rows="10" CssClass="mt-2" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Identity <small class="form-text">(optional)</small>
                        </label>
                        <insite:TextBox runat="server" ID="Identity" />
                    </div>

                    <div class="mt-3 text-end">
                        <insite:Button runat="server" ID="ValidateButton" Text="Validate" ButtonStyle="Success" PostBackEnabled="false" />
                    </div>
                </div>

                <div runat="server" id="ViewOutput" style="display:none;">
                    <div class="alert alert-success mb-4" role="alert">
                        <i class="far fa-check me-2"></i> This badge has passed all verification checks.
                    </div>

                    <div class="alert alert-danger" role="alert">
                        <i class="fas fa-stop-circle me-2"></i> <span class="output-error"></span>
                    </div>

                    <div class="badge-info">
                        <div class="output-badge text-center mb-5">
                            <h4></h4>
                            <img src="" alt="" style="max-width:250px;" class="mb-3" >
                            <p class="fs-sm" style="white-space: pre-line"></p>
                        </div>

                        <div class="form-group mb-3">
                            <div class="float-end opacity-70">
                                <a href="#" title="Email" target="_blank" class="me-1 output-issuer-email"><i class="far fa-envelope"></i></a>
                                <a href="#" title="Website" target="_blank" class="me-1 output-issuer-url"><i class="far fa-globe-pointer"></i></a>
                            </div>
                            <label class="form-label">
                                Issuer
                            </label>
                            <div class="output-issuer-name"></div>
                            <div class="output-issuer-description fs-xs mt-1 text-body-secondary"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Issued On
                            </label>
                            <div class="output-issuedon"></div>
                        </div>
                    
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Expires
                            </label>
                            <div class="output-expires"></div>
                        </div>
                    
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Recipient
                            </label>
                            <div class="output-recipient"></div>
                        </div>
                    </div>

                    <div class="mt-3 text-end">
                        <insite:Button runat="server" ID="BackButton" Text="Back" ButtonStyle="Default" PostBackEnabled="false" />
                    </div>
                </div>

            </div>
        </div>

    </div>

    <uc:BundleJsFooter runat="server" ID="FooterCurrentScripts" />

    <insite:PageFooterContentRenderer runat="server" />

    <script type="text/javascript">
        $(function () {
            const $typeSelector = $('#<%= AssertionType.ClientID %>').on('changed.bs.select', onTypeChanged);
            const $urlInput = $('#<%= AssertionUrl.ClientID %>');
            const $jsonInput = $('#<%= AssertionJson.ClientID %>');
            const $identityInput = $('#<%= Identity.ClientID %>');
            const $validateButton = $('#<%= ValidateButton.ClientID %>').on('click', onValidateClick);

            $('#<%= BackButton.ClientID %>').on('click', onBackClick);

            const $viewInput = $('#<%= ViewInput.ClientID %>');
            const $viewOutput = $('#<%= ViewOutput.ClientID %>');

            setViewVisibility('input');
            onTypeChanged();

            function onTypeChanged() {
                const value = $typeSelector.selectpicker('val');

                $urlInput.hide();
                $urlInput.val('');

                $jsonInput.hide();
                $jsonInput.val('');

                if (value == 'json')
                    $jsonInput.show();
                else
                    $urlInput.show();
            }

            function onValidateClick() {
                const type = $typeSelector.selectpicker('val');
                const data = type == 'json' ? $jsonInput.val().trim() : $urlInput.val().trim();

                if (!data)
                    return;

                $validateButton.addClass('disabled');

                $.ajax({
                    type: 'POST',
                    url: '/api/openbadges/validate',
                    data:
                    {
                        data: data,
                        identity: $identityInput.val().trim()
                    },
                    error: function () {
                        alert('Invalid request.');
                    },
                    success: function (data) {
                        setViewVisibility('output');
                        bindOutput(data);
                    },
                    complete: function () {
                        $validateButton.removeClass('disabled');
                    }
                });
            }

            function bindOutput(data) {
                const isValid = data.valid === true;
                const hasAssestion = !!data.assertion;

                $viewOutput.find('.badge-info').toggle(hasAssestion);
                $viewOutput.find('.alert-success').toggle(isValid);
                $viewOutput.find('.alert-danger').toggle(!isValid).find('.output-error').text(String(data.error));

                if (hasAssestion) {
                    $viewOutput.find('.output-badge h4').text(data.assertion.badge.name);
                    $viewOutput.find('.output-badge img').prop('src', '').prop('src', data.assertion.badge.image);
                    $viewOutput.find('.output-badge p').text(data.assertion.badge.description);

                    $viewOutput.find('.output-issuer-name').text(data.assertion.badge.issuer.name);
                    $viewOutput.find('.output-issuer-email').attr('href', 'mailto:' + data.assertion.badge.issuer.email).toggle(data.assertion.badge.issuer.email && data.assertion.badge.issuer.email.indexOf('@') > 0);
                    $viewOutput.find('.output-issuer-url').attr('href', data.assertion.badge.issuer.url);
                    $viewOutput.find('.output-issuer-description').text(data.assertion.badge.issuer.description).toggle(!!data.assertion.badge.issuer.description);
                    $viewOutput.find('.output-issuedon').text(moment(data.assertion.issuedOn).format('MMMM d, YYYY'));

                    if (data.assertion.expires)
                        $viewOutput.find('.output-expires').text(moment(data.assertion.expires).format('MMMM d, YYYY'));
                    else
                        $viewOutput.find('.output-expires').text('No Expiration Date');

                    if (data.identity == true)
                        $viewOutput.find('.output-recipient').html('<span class="text-success">Valid</span>');
                    else if (data.identity == false)
                        $viewOutput.find('.output-recipient').html('<span class="text-danger">Invalid</span>');
                    else
                        $viewOutput.find('.output-recipient').text('Not Verified');
                }
            }

            function onBackClick() {
                $urlInput.val('');
                $jsonInput.val('');
                $identityInput.val('');

                setViewVisibility('input');
            }

            function setViewVisibility(view) {
                $viewInput.hide();
                $viewOutput.hide();

                if (view === 'input')
                    $viewInput.show();
                else if (view === 'output')
                    $viewOutput.show();
            }
        });
    </script>

</asp:Content>