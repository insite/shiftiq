<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CertificateCompetencyList.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.Certificates.CertificateCompetencyList" %>

<insite:DownloadButton runat="server" ID="DownloadButton" Text="Export" CssClass="mb-3" />

<table id='<%= Competencies.ClientID %>' class="table table-condensed">
    <thead>
        <tr>
            <th colspan="2" class="text-center bg-secondary">Competencies</th>
            <th colspan="3" class="text-center bg-secondary">Hours</th>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
            <td class="text-center fw-bold" style="width:120px;">Core</td>
            <td class="text-center fw-bold" style="width:120px;">Non-Core</td>
            <td class="text-center fw-bold" style="width:120px;">Total</td>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="Competencies" runat="server">
            <ItemTemplate>
                <tr>
                    <td class="align-middle">
                        <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                        <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                    </td>
                    <td class="align-middle">
                        <asp:Label runat="server" Text='<%# Eval("Summary") %>' />
                    </td>
                    <td>
                        <insite:NumericBox ID="CertificationHoursCore" runat="server" DecimalPlaces="2" MaxValue="999.99">
                            <ClientEvents OnFocus="hourEditorFocus(this);" OnChange="hourEditorChange(this);" />
                        </insite:NumericBox>
                    </td>
                    <td>
                        <insite:NumericBox ID="CertificationHoursNonCore" runat="server" DecimalPlaces="2" MaxValue="999.99">
                            <ClientEvents OnFocus="hourEditorFocus(this);" OnChange="hourEditorChange(this);" />
                        </insite:NumericBox>
                    </td>
                    <td>
                        <insite:NumericBox ID="TotalHours" runat="server" ReadOnly="true" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
    <tfoot>
        <tr>
            <td colspan="2">&nbsp;</td>
            <td><insite:NumericBox ID="TotalHoursCore" runat="server" ReadOnly="true" /></td>
            <td><insite:NumericBox ID="TotalHoursNonCore" runat="server" ReadOnly="true" /></td>
            <td><insite:NumericBox ID="TotalHours" runat="server" ReadOnly="true" /></td>
        </tr>
    </tfoot>
</table>

<insite:Button runat="server" Icon="far fa-undo" Text="Clear Hours" ButtonStyle="Default" OnClientClick="clearHours(); return false;" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        function clearHours() {
            if (!confirm("Are you sure you want to clear hours?"))
                return;

            var table = $get("<%= Competencies.ClientID %>");
            var inputs = table.getElementsByTagName("input");

            for (var i = 0; i < inputs.length; i++) {
                var input = inputs[i];

                if (input.type.toLowerCase() == "text")
                    input.value = "";
            }
        }

        function hourEditorFocus(editor) {
            editor.setAttribute("oldValue", editor.value);
        }

        function hourEditorChange(editor) {
            var anotherEditorId;
            var summaryTotalEditorId;

            if (editor.id.indexOf("_CertificationHoursCore") > 0) {
                anotherEditorId = "CertificationHoursNonCore";
                summaryTotalEditorId = "<%= TotalHoursCore.ClientID %>"
            }
            else {
                anotherEditorId = "CertificationHoursCore";
                summaryTotalEditorId = "<%= TotalHoursNonCore.ClientID %>"
            }

            var anotherEditor = getEditor(editor.parentNode.parentNode, anotherEditorId);
            var totalHoursEditor = getEditor(editor.parentNode.parentNode, "TotalHours");

            var oldHours = editor.getAttribute("oldValue") == "" ? 0 : Number(editor.getAttribute("oldValue"));

            validateHours(editor);

            var hours1 = Number(editor.value);
            var hours2 = Number(anotherEditor.value);

            if (isNaN(hours1) && isNaN(hours2) || editor.value == "" && anotherEditor.value == "")
                totalHoursEditor.value = "";
            else {
                if (isNaN(hours1))
                    hours1 = 0;
                else if (isNaN(hours2))
                    hours2 = 0;

                totalHoursEditor.value = hours1 + hours2;
            }

            var difference = isNaN(hours1) ? -oldHours : hours1 - oldHours;
            var oldSummaryTotal = Number($get(summaryTotalEditorId).value);
            var newSummaryTotal = oldSummaryTotal + difference;

            $get(summaryTotalEditorId).value = newSummaryTotal;
            $get("<%= TotalHours.ClientID %>").value = Number($get("<%= TotalHoursCore.ClientID %>").value) + Number($get("<%= TotalHoursNonCore.ClientID %>").value);
        }

        function getEditor(parent, serverId) {
            var inputs = parent.getElementsByTagName("input");

            for (var i = 0; i < inputs.length; i++) {
                var input = inputs[i];

                if (input.id.indexOf("_" + serverId) > 0)
                    return input;
            }

            return null;
        }

        function validateHours(editor) {
            var text = editor.value;
            var decimalIndex = text.indexOf(".");

            if (decimalIndex >= 0) {
                if (decimalIndex == text.length - 1)
                    text = text.substring(0, text.length - 1);
                else if (text.length - decimalIndex - 1 > 2)
                    text = text.substring(0, decimalIndex + 3);
            }

            var number = Number(text);

            if (isNaN(number))
                editor.value = editor.getAttribute("oldValue");
            else if (number < 0)
                text = '0';
            else if (number > 999.99)
                text = '999.99';

            if (text != editor.value) {
                editor.value = text;
                editor.setAttribute("oldValue", editor.value);
            }
        }

    </script>
</insite:PageFooterContent>