<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailConfiguration.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailConfiguration" %>

<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailConfigurationPlatform.ascx" TagName="DetailConfigurationPlatform" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailConfigurationLocationDescription.ascx" TagName="DetailConfigurationLocationDescription" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailConfigurationLocationAddress.ascx" TagName="DetailConfigurationLocationAddress" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailConfigurationCustomizationUrl.ascx" TagName="DetailConfigurationCustomizationUrl" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailConfigurationBambora.ascx" TagName="DetailConfigurationBambora" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailConfigurationUpload.ascx" TagName="DetailConfigurationUpload" TagPrefix="uc" %>

<insite:Nav runat="server">
            
    <insite:NavItem runat="server" Title="Platform">
        <uc:DetailConfigurationPlatform runat="server" ID="DetailPlatform" />
    </insite:NavItem>

    <insite:NavItem runat="server" Title="Location">
        <div class="row">
            <div class="col-md-6">
                            
                <h3>Type and Description</h3>

                <uc:DetailConfigurationLocationDescription runat="server" ID="DetailLocationDescription" />

            </div>
            <div class="col-md-6">

                <h3>Address</h3>

                <uc:DetailConfigurationLocationAddress runat="server" ID="DetailLocationAddress" />
                    
            </div>
        </div>
    </insite:NavItem>
        
    <insite:NavItem runat="server" ID="UrlTab" Title="URL">
        <uc:DetailConfigurationCustomizationUrl runat="server" ID="DetailCustomizationUrl" />
    </insite:NavItem>

    <insite:NavItem runat="server" Title="Bambora">
        <uc:DetailConfigurationBambora runat="server" ID="DetailBambora" />
    </insite:NavItem>

    <insite:NavItem runat="server" Title="Upload">
        <uc:DetailConfigurationUpload runat="server" ID="DetailUpload" />
    </insite:NavItem>

    <insite:NavItem runat="server" Title="Advanced">
        <div class="row">
            <div class="col-lg-12">
                            
                <h3>JSON</h3>

                <div class="form-group mb-3">
                    <div>
                        <insite:TextBox runat="server" ID="Snapshot" TextMode="MultiLine" Width="100%" CssClass="json-editor" />
                        <asp:CustomValidator runat="server" ID="SnapshotValidator" ControlToValidate="Snapshot" Display="None" ErrorMessage="Configuration JSON is invalid" ValidationGroup="Organization" />
                    </div>
                    <div class="form-text">
                        This is a JSON representation of the organization.
                        We use
                        <a target="_blank" href="https://jsoneditoronline.org/"><i class="far fa-external-link"></i> JSON Editor Online</a>.
                    </div>
                </div>

            </div>
        </div>
    </insite:NavItem>

</insite:Nav>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        div.json-editor {
            height: 500px;
        }

        div.json-editor > div {
            padding: 6px 12px;
            border: 1px solid #ccc;
            border-radius: 4px;
            width: 100%;
            height: 100%;
        }

    </style>
</insite:PageHeadContent>


<insite:PageFooterContent runat="server">

    <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/plugins/ace.cloud9/ace.js" />

    <script type="text/javascript">

        (function () {
            $('textarea.json-editor').each(function () {
                const $input = $(this).hide();
                const $wrapper = $('<div class="json-editor">');
                const $editor = $('<div>');

                $wrapper.insertAfter($input);
                $wrapper.append($editor);

                const editor = ace.edit($editor[0], {
                    minLines: 15
                });

                editor.$blockScrolling = Infinity;
                editor.setFontSize(15);
                editor.setShowPrintMargin(false);
                editor.session.setMode('ace/mode/json');

                editor.session.setValue($input.val());
                editor.session.on('change', function () {
                    $input.val(editor.session.getValue());
                });
            });
        })();
    </script>

</insite:PageFooterContent>