<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MFAText.ascx.cs" Inherits="InSite.UI.Portal.Accounts.Users.Controls.MFAText" %>

<asp:CustomValidator runat="server"
    ID="CodeValidator"
    ErrorMessage="You have entered a wrong PIN, please try again."
    Display="None"
    ValidationGroup="MFA"
/>

<insite:Alert runat="server" ID="MFAStatus" />

<div runat="server" id="TryAgainPanel" class="alert alert-danger mb-3" role="alert" style="display: none">
    <i class='far fa-stop-circle fs-4 mt-1 me-3'></i>
    Please wait one minute before requesting another code.
</div>

<div class="card mb-3">
    <div class="card-body">
        <asp:MultiView runat="server" ID="Steps">

            <asp:View runat="server" ID="Step1">
                <div class="form-group mb-3 w-50">
                    <label class="form-label">
                        Cell Number

                        <insite:RequiredValidator runat="server" ControlToValidate="CellNumber" FieldName="Cell number" ValidationGroup="MFA" />
                        <insite:PatternValidator runat="server"
                            ControlToValidate="CellNumber"
                            Display="None"
                            ErrorMessage="Wrong cell number format."
                            ValidationExpression="^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$"
                            ValidationGroup="MFA"
                        />
                    </label>
                    <div>
                        <insite:TextBox ID="CellNumber" runat="server" MaxLength="32" />
                        <div class="form-text">
                            In order to verify your phone number you will recieve a text message from InSite containing a PIN code in the next step.
                        </div>
                        <div class="form-text">
                            Note: Text messaging is currently available only for Canadian mobile numbers registered with Canadian carriers.
                            If your number doesn't meet this requirement, please click <strong>Back</strong> and choose a different authentication method.
                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View runat="server" ID="Step2">
                <div class="form-group mb-3 w-50">
                    <label class="form-label">
                        Cell Number
                    </label>
                    <div>
                        <insite:TextBox ID="CellNumber2" runat="server" MaxLength="32" ReadOnly="true" />
                    </div>
                </div>

                <div class="form-group mb-3 w-50">
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
                        <div class="form-text">
                            Please enter your PIN code.
                        </div>
                        <div class="form-text">
                            If you're not receiving PIN codes, your mobile number may be incorrect or ineligible.
                            Click <strong>Back</strong> to update it or to choose a different authentication method.
                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View runat="server" ID="Step3">
                <div class="form-group">
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

<insite:Button runat="server"
    ID="ResendButton"
    Text="Resend"
    ButtonStyle="Default"
    Icon="fas fa-share"
    Visible="false"
    OnClientClick="return mfaText.clientClick()"
/>

<insite:SaveButton runat="server" ID="SaveButton" />

<insite:CancelButton runat="server" ID="CancelButton" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            const instance = window.mfaText = window.mfaText || {};

            const pageInitTime = Date.now();

            instance.clientClick = () => {
                const currentTime = Date.now();
                if ((currentTime - pageInitTime) / 1000 > 60) {
                    return true;
                }
                const ta = document.getElementById('<%= TryAgainPanel.ClientID %>');
                ta.style.display = 'block';
                return false;
            }
        })();

        (() => {
            const cellNumberInput = document.getElementById('<%= CellNumber.ClientID %>');
            if (cellNumberInput) {
                cellNumberInput.addEventListener("keydown", (e) => {
                    if (e.key == "Enter") {
                        __doPostBack('<%= ContinueButton.UniqueID %>', "");
                    }
                });

                setTimeout(() => cellNumberInput.focus(), 0);
            }

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