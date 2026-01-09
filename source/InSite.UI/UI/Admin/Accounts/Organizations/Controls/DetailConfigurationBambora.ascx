<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailConfigurationBambora.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailConfigurationBambora" %>

<div class="row">
    <div class="col-md-4">

        <h3>Development &amp; Local</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Merchant ID
            </label>
            <insite:TextBox runat="server" ID="DevMerchant" CssClass="bambora-merchant" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Passcode
            </label>
            <insite:TextBox runat="server" ID="DevPasscode" CssClass="bambora-passcode" /> 
        </div>

    </div>
    <div class="col-md-4">

        <h3>Sandbox</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Merchant ID
            </label>
            <insite:TextBox runat="server" ID="SandboxMerchant" CssClass="bambora-merchant" /> 
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Passcode
            </label>
            <insite:TextBox runat="server" ID="SandboxPasscode" CssClass="bambora-passcode" />
        </div>

    </div>
    <div class="col-md-4">

        <h3>Production</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Merchant ID
            </label>
            <insite:TextBox runat="server" ID="ProdMerchant" CssClass="bambora-merchant" /> 
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Passcode
            </label>
            <insite:TextBox runat="server" ID="ProdPasscode" CssClass="bambora-passcode" /> 
        </div>

    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            const defaultMerchantId = '<%= BamboraMerchantId %>';
            const defaultPasscode = '<%= BamboraPasscode %>';

            $(() => {
                $('.bambora-merchant')
                    .on('change', onMerchantChange)
                    .each(function () { onMerchantChange.apply(this) });

                $('.bambora-passcode')
                    .on('change', onPasscodeChange)
                    .each(function () { onPasscodeChange.apply(this) });
            });

            function onMerchantChange() {
                toggleWarning(this, defaultMerchantId);
            }

            function onPasscodeChange() {
                toggleWarning(this, defaultPasscode);
            }

            function toggleWarning(input, value) {
                const $input = $(input);
                const $warning = $input.data('waning');

                if ($input.val().trim() == value) {
                    if (!$warning) {
                        $input.data('waning', $('<div class="bambora-warning mt-1 text-danger fs-xs">')
                            .append($('<strong>').text(value), ' refers to the InSite test account')
                            .insertAfter($input));
                    }
                } else if ($warning) {
                    $warning.remove();
                    $input.data('waning', null);
                }
            }
        })();
    </script>

</insite:PageFooterContent>