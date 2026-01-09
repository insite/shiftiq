<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ValidPlaceHolderNames.ascx.cs" Inherits="InSite.Admin.Messages.Outlines.Controls.ValidPlaceHolderNames" %>

<div>
    <hr />

    <div runat="server" id="MessageVariablesField" class="form-text">
        <strong><asp:Literal runat="server" ID="MessageVariablesLabel" Text="Message Variables"/></strong>: <asp:Literal runat="server" ID="MessageVariables" />
    </div>

    <div runat="server" id="RecipientVariablesField" class="form-text">
        <strong>Recipient Variables</strong>: <asp:Literal runat="server" ID="RecipientVariables" />
    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .message-variable {
            user-select: all;
            cursor: pointer;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        document.querySelectorAll('.message-variable').forEach(element => {
            element.addEventListener('click', function () {
                const value = this.dataset.value;
                navigator.clipboard.writeText(value);
            });
        });
    </script>
</insite:PageFooterContent>