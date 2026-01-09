<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Custom.NCSHA.History.Controls.SearchResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="RecordID">
    <Columns>

        <asp:TemplateField HeaderText="User" ItemStyle-Wrap="false">
            <itemtemplate>
                <a href="/ui/admin/contacts/people/edit?contact=<%# Eval("UserID") %>"><%# Eval("UserName") %></a>
                <div>
                    <%# Eval("UserEmail") %>
                </div>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group">
            <itemtemplate>
                <%# Eval("UserGroup") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event Name" ItemStyle-Wrap="false">
            <itemtemplate>
                <%# Eval("EventName") %>
                <div class="form-text"><%# Eval("EventGroup") %></div>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event Time" ItemStyle-Wrap="false">
            <itemtemplate>
                <%# LocalizeTime((DateTimeOffset)Eval("RecordTime")) %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Name">
            <itemtemplate>
                <%# Eval("Name") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Region">
            <itemtemplate>
                <%# Eval("Region") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Year" ItemStyle-Wrap="false">
            <itemtemplate>
                <%# Eval("Year") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Options">
            <itemtemplate>
                <%# Eval("Options") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Axis">
            <itemtemplate>
                <%# Eval("Axis") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-CssClass="text-end" HeaderStyle-Width="30px" ItemStyle-Wrap="false">
            <itemtemplate>
                <insite:IconButton runat="server" CssClass="btn-data" Name="info-circle" ToolTip="View Event" />
                <asp:HiddenField runat="server" Value='<%# Eval("EventData") %>' />
            </itemtemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<insite:Modal runat="server" ID="EventInfoWindow" Title="Event Data" Width="950px">
    <ContentTemplate>
        <pre></pre>
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            var $modal = $('#<%= EventInfoWindow.ClientID %>')
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
