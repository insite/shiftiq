<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MFAAuthenticatorApp.ascx.cs" Inherits="InSite.UI.Portal.Accounts.Users.Controls.MFAAuthenticatorApp" %>

<asp:CustomValidator runat="server"
    ID="CodeValidator"
    ErrorMessage="You have entered a wrong PIN, please try again."
    Display="None"
    ValidationGroup="MFA"
/>

<div class="card mb-3">
    <div class="card-body">
        <asp:HiddenField ID="QRCodeText" runat="server" Value='<%# Eval("OTPLink") %>' />

        <asp:MultiView runat="server" ID="Steps">

            <asp:View runat="server" ID="Step1">
                <div id="QRCode" runat="server" class="qrcode mb-2"></div>
                <span>
                    Please scan the above qrcode using Microsoft Authenticator app on your phone,
                    you will then be required to enter the pin code in the next window.
                </span>
            </asp:View>

            <asp:View runat="server" ID="Step2">
                <div id="QRCode2" runat="server" class="qrcode mb-3"></div>

                <div class="form-group w-50">
                    <label class="form-label">
                        PIN Code
                        <insite:RequiredValidator runat="server"
                            ControlToValidate="ConfirmationCode"
                            FieldName="PIN code"
                            ValidationGroup="MFA"
                        />
                    </label>
                    <div>
                        <insite:TextBox ID="ConfirmationCode" runat="server" MaxLength="32" />
                        <div class="form-text">Please enter your PIN code.</div>
                    </div>
                </div>
            </asp:View>

            <asp:View runat="server" ID="Step3">
                <div class="form-group mb-3">
                    <label class="form-label">Your Recovery Phrases</label>
                    <div>
                        <insite:TextBox TextMode="MultiLine" Rows="12" ID="RecoveryPhrasesTextBox" runat="server" MaxLength="32" ReadOnly="true" />
                        <div class="form-text">
                            Please take note of the recovery phrases above, you can use them to login in case you lose access to your primary MFA method.
                        </div>
                    </div>
                </div>
            </asp:View>

        </asp:MultiView>
    </div>
</div>

<insite:Button runat="server" ID="BackButton" IconPosition="BeforeText" Icon="fas fa-arrow-alt-left" Text="Back" ButtonStyle="Default" />

<insite:Button runat="server"
    ID="ContinueButton"
    IconPosition="AfterText"
    Icon="fas fa-arrow-alt-right"
    Text="Continue"
    ValidationGroup="MFA"
/>

<insite:SaveButton runat="server" ID="SaveButton" />

<insite:CancelButton runat="server" ID="CancelButton" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            const qrcodeDiv = document.getElementById('<%= QRCode.ClientID %>')
                || document.getElementById('<%= QRCode2.ClientID %>')

            if (!qrcodeDiv) {
                return;
            }

            const qrcodeText = document.getElementById('<%= QRCodeText.ClientID %>').value;

            const _ = new QRCode(qrcodeDiv, {
                text: qrcodeText,
                width: 128,
                height: 128,
                colorDark: "#000000",
                colorLight: "#ffffff",
                correctLevel: QRCode.CorrectLevel.H
            });
        })();

        (() => {
            const confirmationCodeInput = document.getElementById('<%= ConfirmationCode.ClientID %>');
            if (confirmationCodeInput) {
                confirmationCodeInput.addEventListener("keydown", (e) => {
                    if (e.key == "Enter") {
                        __doPostBack('<%= ContinueButton.UniqueID %>', "");
                    }
                });

                setTimeout(() => confirmationCodeInput.focus(), 0);
            }
        })();
    </script>
</insite:PageFooterContent>