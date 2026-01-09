<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailCommentBox.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Controls.DetailCommentBox" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <h3>Comment Box</h3>

        <insite:Alert runat="server" ID="TextCharacterWarning" Indicator="Warning" CssClass="d-none">
            Allowing more than 4000 characters is permitted but not recommended for system and form performance reasons.
            If you decide to allow a respondent to input more than 4000 characters in their answers to this question,
            then it is important to test your form very carefully using all major web browsers.
        </insite:Alert>

        <div class="form-group mb-3">
            <label class="form-label">Lines of Text</label>
            <div>
                <insite:NumericBox runat="server" ID="TextLineCount" MinValue="1" MaxValue="50" NumericMode="Integer" />
            </div>
            <div class="form-text">
                The height of the comment box, measured in lines.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Character Limit
                <insite:RequiredValidator runat="server" ControlToValidate="TextCharacterLimit" FieldName="Character Limit" ValidationGroup="SurveyQuestion" />
            </label>
            <div>
                <insite:NumericBox runat="server" ID="TextCharacterLimit" MinValue="1" NumericMode="Integer" ValueAsText="200" />
            </div>
            <div class="form-text">
                The maximum number of characters allowed in the comment box.
            </div>
        </div>

    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            const alert = document.querySelector('#<%= TextCharacterWarning.ClientID %> .alert');
            const input = document.getElementById('<%= TextCharacterLimit.ClientID %>');
            input.addEventListener('input', onCharacterLimitChanged);

            function onCharacterLimitChanged(e) {
                const value = parseInt(e.target.value);
                if (!isNaN(value) && value > 4000)
                    alert.classList.remove('d-none');
                else
                    alert.classList.add('d-none');
            }
        })();
    </script>
</insite:PageFooterContent>