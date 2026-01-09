<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultilingualStringInfo.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.MultilingualStringInfo" %>

<insite:PageHeadContent runat="server" ID="ControlStyle" RenderRequired="true">
    <style type="text/css">

        table.table-multiling {
            margin-bottom: 25px;
            width: 100%;
        }

            table.table-multiling .lang {
                width: 25px;
                vertical-align: top;
                padding-right: 15px;
            }

                table.table-multiling .lang .label {
                    background-color: #aaa;
                    text-transform: uppercase;
                }

            table.table-multiling .separator {
                height: 8px;
            }

            table.table-multiling img {
                max-width: 100%;
            }

    </style>
</insite:PageHeadContent>

<div>
    <asp:Literal runat="server" ID="NoDataText">None</asp:Literal>

    <asp:Repeater runat="server" ID="Repeater">
        <HeaderTemplate>
            <table id=<%# Repeater.ClientID %> class="table-multiling">
        </HeaderTemplate>
        <SeparatorTemplate>
            <tr><td colspan="2" class="separator"></td></tr>
        </SeparatorTemplate>
        <ItemTemplate>
            <tr>
                <td class="lang">
                    <span class="label badge bg-info"><%# Eval("Language") %></span>
                </td>
                <td>
                    <asp:HiddenField runat="server" Value='<%# Eval("TranslationHtml") %>' />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>

<insite:PageFooterContent runat="server" ID="CommonScript" RenderRequired="true">
    <script type="text/javascript">

        (function () {
            var instance = window.multilingualStringInfo = window.multilingualStringInfo || {};
            var iFrames = [];

            $(window).on('click', updateIFrameHeight);

            instance.init = function (id) {
                $('#' + String(id) + ' input[type="hidden"]').each(function () {
                    var $this = $(this);
                    if ($this.data('inited') === true)
                        return;

                    var html = $this.val();
                    var $iframe = $('<iframe style="display:none; width:100%; border:none;" scrolling="no" sandbox="allow-same-origin">');

                    $iframe.insertAfter($this);

                    var iframe = $iframe.get(0);
                    iframe.contentWindow.document.open();
                    iframe.contentWindow.document.write('<html>');
                    iframe.contentWindow.document.write('<head>');
                    iframe.contentWindow.document.write("<link rel='stylesheet' media='screen' href='https://fonts.googleapis.com/css?family=Inter:300,400,500,600,700&display=swap' />");
                    
                    iframe.contentWindow.document.write('  <style type="text/css">');
                    iframe.contentWindow.document.write('    body { color: #737491; font-family: Inter,Calibri,"Source Sans Pro",Helvetica,Arial; font-size: 0.9rem; line-height: 1.3rem; overflow:hidden; padding:0; margin-top:4px; }');
                    iframe.contentWindow.document.write('    img { max-width: 100%; }');
                    iframe.contentWindow.document.write('    body > div > *:first-child { margin-top:0; }');
                    iframe.contentWindow.document.write('    table.table-markdown { border-collapse: collapse; }');
                    iframe.contentWindow.document.write('    table.table-markdown th { background: white; padding: 5px; text-align:left; }');
                    iframe.contentWindow.document.write('    table.table-markdown td { border: 1px solid silver; padding: 5px; }');
                    iframe.contentWindow.document.write('    table.table-markdown tr:nth-child(odd) { background: #F9F9F9; }');
                    iframe.contentWindow.document.write('    table.table-markdown tr:nth-child(even) { background: #F0F0F0; }');
                    iframe.contentWindow.document.write('  </style>');

                    iframe.contentWindow.document.write('</head>');
                    iframe.contentWindow.document.write('<body><div style="min-height:1px;">');
                    iframe.contentWindow.document.write(html);
                    iframe.contentWindow.document.write('</div></body>');
                    iframe.contentWindow.document.write('</html>');
                    iframe.contentWindow.document.close();

                    iFrames.push(iframe);

                    $this.data('inited', true);
                });
            };

            updateIFrameHeight();

            $(window).on('load', updateIFrameHeight);

            function updateIFrameHeight() {
                for (var i = 0; i < iFrames.length; i++) {
                    var iframe = iFrames[i];

                    var $frame = $(iframe);
                    if (!$frame.parent().is(':visible'))
                        continue;

                    $frame.show();
                    
                    var frameHeight = $(iframe.contentWindow.document).find('body > div').outerHeight(true);
                    if (frameHeight == null)
                        iframe.contentWindow.document.body.scrollHeight;
                    
                    $frame.css('height', frameHeight == 0 ? '' : String(frameHeight + 20) + 'px');

                    iFrames.splice(i--, 1);
                }
            }
        })();

    </script>
</insite:PageFooterContent>