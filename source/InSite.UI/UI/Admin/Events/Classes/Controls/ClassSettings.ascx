<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassSettings.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.ClassSettings" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <asp:Repeater runat="server" ID="RowRepeater">
            <HeaderTemplate>
                <table id='<%= RowRepeater.ClientID %>' class="table table-striped table-bordered table-hover" style="margin-top:15px;">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th class="text-center" style="width:100px;">Visible</th>
                            <th class="text-center" style="width:100px;">Required</th>
                            <th class="text-center" style="width:100px;">Editable</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody>
                </table>
            </FooterTemplate>
            <ItemTemplate>
                <insite:Container runat="server" Visible='<%# IsGroupVisible() %>'>
                    <tr>
                        <th colspan="4"><%# Eval("GroupName") %></th>
                    </tr>
                </insite:Container>
                <tr>
                    <td><%# Eval("Title") %></td>
                    <td class="text-center"><insite:CheckBox runat="server" ID="IsVisible" RenderMode="Input" /></td>
                    <td class="text-center"><insite:CheckBox runat="server" ID="IsRequired" RenderMode="Input" /></td>
                    <td class="text-center"><insite:CheckBox runat="server" ID="IsEditable" RenderMode="Input" /></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var $inputs = null;
            var postbackHandler = null;

            Sys.Application.add_load(function () {
                if ($inputs != null && $inputs.length > 0 && document.body.contains($inputs[0]))
                    return;

                $inputs = $('#<%= RowRepeater.ClientID %> input[type="checkbox"]').on('change', onInputChange);

                if (postbackHandler != null) {
                    clearTimeout(postbackHandler);
                    postbackHandler = null;
                }
            });

            function onInputChange() {
                if (postbackHandler != null)
                    clearTimeout(postbackHandler);

                postbackHandler = setTimeout(onPostback, 1500);
            }

            function onPostback() {
                postbackHandler = null;
                document.getElementById('<%= UpdatePanel.ClientID %>').ajaxRequest('save');
            }
        })();
    </script>
</insite:PageFooterContent>
