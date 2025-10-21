<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TranslationEditor.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.TranslationEditor" %>
<%@ Register Src="TranslationWindow.ascx" TagName="TranslationWindow" TagPrefix="uc" %>

<uc:TranslationWindow runat="server" ID="TranslationWindow" />

<asp:Button runat="server" ID="UpdateButton" style="display:none;" />

<div class="row">
    <div class="col-md-2 col-sm-3 col-xs-4">
        <insite:CustomValidator runat="server" ID="RequiredValidator" Enabled="false" Display="Dynamic" />
        <insite:Button runat="server" ID="EditButton" ButtonStyle="Default" style="margin-bottom:30px;"
            Text="Edit" Icon="fas fa-pencil" Width="100%" />

        <asp:PlaceHolder runat="server" ID="NavContainer">

        </asp:PlaceHolder>
    </div>
    <div class="col-md-10 col-sm-9 col-xs-8">
        <insite:NavContent runat="server" ID="TranslationContent" />
    </div>
</div>

<insite:PageFooterContent runat="server" ID="CommonScript">

    <script type="text/javascript">
        
        (function () {
            var instance = window.translationEditor = window.translationEditor || {};

            instance.init = function (id) {
                var $nav = $('#' + String(id));

                $nav.find('a[role="tab"]').on('shown.bs.tab', function () {
                    $($(this).attr('href')).find('iframe').each(function () {
                        updateIFrameHeight(this);
                    });
                });
            };

            instance.setHtml = function (id, html) {
                var $iframe = $('#' + String(id));
                if ($iframe.length !== 1)
                    return;

                if (html == null || typeof html != 'string' || html.length == 0) {
                    $iframe.css('display', 'none');
                    return;
                }

                $iframe.css('display', '');

                var iframe = $iframe.get(0);
                iframe.contentWindow.document.open();
                iframe.contentWindow.document.write('<html>');
                iframe.contentWindow.document.write('<head>');
                iframe.contentWindow.document.write('  <link href="/library/fonts/source-sans-pro/400italic-700italic-400-700-300_subset--latin__latin-ext.css" rel="stylesheet" type="text/css">');
                iframe.contentWindow.document.write('  <style type="text/css">');
                iframe.contentWindow.document.write('    body { color: #545454; font-family: Calibri,"Source Sans Pro",Helvetica,Arial; font-size: 16px; overflow:hidden; }');
                iframe.contentWindow.document.write('    body > div { margin-bottom: 16px; }');
                iframe.contentWindow.document.write('    img { max-width: 100%; }');
                iframe.contentWindow.document.write('  </style>');
                iframe.contentWindow.document.write('</head>');
                iframe.contentWindow.document.write('<body>');
                iframe.contentWindow.document.write('<div>');
                iframe.contentWindow.document.write(html);
                iframe.contentWindow.document.write('</div>');
                iframe.contentWindow.document.write('</body>');
                iframe.contentWindow.document.write('</html>');
                iframe.contentWindow.document.close();

                updateIFrameHeight(iframe);
            };

            function updateIFrameHeight(iframe) {
                var $frame = $(iframe);
                if (!$frame.is(':visible'))
                    return;

                var frameHeight = $(iframe.contentWindow.document).find('body > div').outerHeight(true);
                if (frameHeight == null)
                    iframe.contentWindow.document.body.scrollHeight;

                $frame.css('height', frameHeight == 0 ? '' : String(frameHeight + 5) + 'px');
            }
        })();

    </script>

</insite:PageFooterContent>