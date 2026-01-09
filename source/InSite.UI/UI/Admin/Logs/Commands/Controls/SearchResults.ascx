<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Logs.Commands.Controls.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="CommandIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-CssClass="text-end" HeaderStyle-Width="30px" ItemStyle-Wrap="false"  HeaderStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconButton runat="server" CssClass="btn-execute" Name="bolt" ToolTip="Send"
                    CommandName="SendCommand" ConfirmText="Are you sure you want to send this command?"
                    Visible='<%# Eval("SendStarted") == null && Eval("SendCompleted") == null && Eval("SendCancelled") == null %>'
                />

                <insite:IconButton runat="server" CssClass="btn-execute" Name="redo" ToolTip="Repeat"
                    CommandName="RepeatCommand" ConfirmText="Are you sure you want to repeat this command?"
                    Visible='<%# Eval("SendStarted") != null || Eval("SendCompleted") != null || Eval("SendCancelled") != null %>'
                />

                <insite:IconButton runat="server" CssClass="btn-data" Name="info-circle" ToolTip="View Command Data" 
                    Visible='<%# !string.IsNullOrEmpty((string)Eval("CommandData")) %>'
                    />

                <a title="Edit" href="<%# Eval("CommandIdentifier", "/ui/admin/logs/commands/edit?command={0}") %>"><i class="icon far fa-pencil"></i></a>

                <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete"
                    CommandName="DeleteCommand" ConfirmText="Are you sure you want to delete this command?"
                />

                <asp:HiddenField runat="server" Value='<%# Eval("CommandData") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Identity">
            <ItemTemplate>
                <%# GetUserName(Eval("OriginUser")) %>
                <br />
                <span class="form-text"><%# GetOrganizationCode(Eval("OriginOrganization")) %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Scheduled">
            <ItemTemplate>
                <%# LocalizeTime((DateTimeOffset?)Eval("SendScheduled")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Command">
            <ItemTemplate>
                <%# Eval("CommandType") %>
                <div class="form-text">
                    <%# Eval("CommandIdentifier") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# GetCommandStatus(Container.DataItem) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<insite:Modal runat="server" ID="CommandDataWindow" Title="Command Data">
    <ContentTemplate>
        <pre></pre>
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            $('.btn-execute')
                .each(function () {
                    if (this.onclick) {
                        $(this).data('initial-onclick', this.onclick);
                        this.onclick = null;
                    }
                })
                .on('click', function (e) {
                    var onclick = $(this).data('initial-onclick');
                    if (onclick) {
                        var result = onclick.call(this, e || window.event);
                        if (result != false)
                            showLoadingPanel();
                        else
                            e.preventDefault();
                    } else {
                        showLoadingPanel();
                    }
                });

            function showLoadingPanel() {
                $('#desktop > .loading-panel').show();
            }
        })();

        (function () {
            var $modal = $('#<%= CommandDataWindow.ClientID %>')
                .on('hidden.bs.modal', onModalHidden);
            var $codeOutput = $modal.find('pre');

            $('.btn-data')
                .each(function () {
                    this.onclick = null;
                })
                .on('click', function (e) {
                    e.preventDefault();

                    var json = $(this).siblings('input[type="hidden"]').val();
                    if (!json)
                        return;

                    try {
                        var obj = JSON.parse(json);
                        if (obj) {
                            $codeOutput.text(JSON.stringify(obj, null, 4));
                        }
                    } catch (e) {
                        $codeOutput.text(json);
                    }

                    modalManager.show($modal);
                });

            function onModalHidden() {
                $codeOutput.empty();
            }
        })();

    </script>

</insite:PageFooterContent>