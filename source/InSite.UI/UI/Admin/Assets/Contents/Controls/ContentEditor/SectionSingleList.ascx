<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SectionSingleList.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.ContentEditor.SectionSingleList" %>

<%@ Register Src="Field.ascx" TagName="ContentEditorField" TagPrefix="uc" %>

<h3 runat="server" id="LabelOutput" style="margin-top:0;"></h3>
<p runat="server" id="DescriptionOutput" class="form-text"></p>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelI="UpdatePanel" />
<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <asp:Repeater runat="server" ID="Repeater">
            <ItemTemplate>
                <insite:Button runat="server" ID="DeleteButton" CommandName="Delete" ButtonStyle="Danger" Size="ExtraSmall" Icon="fas fa-trash-alt" />
                <uc:ContentEditorField runat="server" ID="EditorField" />
            </ItemTemplate>
        </asp:Repeater>

        <div style="margin-top:15px;">
            <insite:Button runat="server" ID="AddButton" />
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageFooterContent runat="server" ID="CommonScript">

    <script type="text/javascript">

        (function () {
            var instance = window.singleListSection = window.singleListSection || {};
            var items = {};

            instance.register = function (id, data) {
                items[id] = data;
                init(data);
            };

            function init(data) {
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    $('#' + String(item.fieldId) + ' > div.commands').each(function () {
                        var $this = $(this);
                        if ($this.data('editor-loaded') === true)
                            return;

                        $this.data('editor-loaded', true).append($('#' + item.deleteId).detach());
                    });
                }
            }

            function onLoad() {
                for (var id in items) {
                    if (!items.hasOwnProperty(id))
                        continue;

                    init(items[id]);
                }
            }

            Sys.Application.add_load(onLoad);
        })();

    </script>

</insite:PageFooterContent>