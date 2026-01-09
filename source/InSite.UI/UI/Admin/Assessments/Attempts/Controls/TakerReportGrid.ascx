<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TakerReportGrid.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.TakerReportGrid" %>

<asp:Repeater runat="server" ID="AttemptRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>
                        <asp:CheckBox runat="server" ID="AllCheckBox" CssClass="select-all-attempts" Checked="true" />
                    </th>
                    <th></th>
                    <th>Exam Form</th>
                    <th>Date and Time</th>
                    <th class="text-end">Score</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td style="width:40px;" class="text-center">
                <asp:Literal runat="server" ID="AttemptIdentifier" Visible="false" Text='<%# Eval("AttemptIdentifier") %>' />
                <asp:CheckBox runat="server" ID="AttemptCheckBox" CssClass="select-attempt" Checked="true" />
            </td>
            <td style="width:40px;" class="text-center">
                <insite:IconLink runat="server" ID="ViewAttemptLink" Name="search" Target="_blank"
                    NavigateUrl='<%# Eval("AttemptIdentifier", "/ui/admin/assessments/attempts/view?attempt={0}") %>' 
                />
            </td>
            <td>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&form=<%# Eval("FormIdentifier") %>"><%# Eval("FormTitle") %></a>
                <div class="form-text">
                    <%# Eval("FormName") %>
                    &bull;
                    Exam Form Asset #<%# GetFormAsset() %>
                </div>
            </td>
            <td>
                <%# FormatTime() %>
            </td>
            <td class="text-end">
                <%# FormatScore() %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<insite:PageFooterContent runat="server">
    <script>
        (function () {
            document.querySelector(".select-all-attempts input").addEventListener("click", e => {                
                document.querySelectorAll(".select-attempt input").forEach(chk => {
                    chk.checked = e.target.checked;
                });
                enableButtons();
            });

            document.querySelectorAll(".select-attempt input").forEach(chk => {
                chk.addEventListener("click", () => {
                    enableButtons();
                });
            });

            function enableButtons() {
                var enabled = document.querySelectorAll(".select-attempt input:checked").length > 0;

                <%-- Selects report buttons from TakeReport --%>
                document.querySelectorAll(".report-buttons a").forEach(btn => {
                    if (enabled) {
                        btn.removeAttribute("disabled");
                        btn.classList.remove("disabled");
                    } else {
                        btn.setAttribute("disabled", "disabled");
                        btn.classList.add("disabled");
                    }
                });
            }
        })();
    </script>
</insite:PageFooterContent>