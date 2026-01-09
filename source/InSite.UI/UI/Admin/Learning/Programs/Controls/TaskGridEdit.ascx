<%@ Control Language="C#" CodeBehind="TaskGridEdit.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.TaskGridEdit" %>

<asp:Repeater ID="FolderRepeater" runat="server">
    <ItemTemplate>
        <div class="mb-3 folder-container">
            <h3><%# Eval("AchievementLabel") %></h3>

            <asp:Repeater ID="ItemRepeater" runat="server">
                <HeaderTemplate>
                    <div class="row p-2 ms-0 me-0 border-bottom border-top fw-bold bg-secondary">
                        <div class="col-4">
                            Achievement
                        </div>
                        <div class="col-2 text-center">
                            Planned
                            <div class="mt-1">
                                <insite:CheckBox runat="server" CssClass="select-all-planned" RenderMode="Input" />
                            </div>
                        </div>
                        <div class="col-2 text-center">
                            Required
                            <div class="mt-1">
                                <insite:CheckBox runat="server" CssClass="select-all-required" RenderMode="Input" />
                            </div>
                        </div>
                        <div class="col-2 text-center">
                            Time-Sensitive
                            <div class="mt-1">
                                <insite:CheckBox runat="server" CssClass="select-all-timesensitive" RenderMode="Input" Enabled="false" />
                            </div>
                        </div>
                        <div class="col-2 text-end">
                            Months
                        </div>
                    </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class='<%# Container.ItemIndex % 2 != 0 ? "bg-secondary": "" %>'>
                        <div class="row p-2 ms-0 me-0 border-bottom">
                            <div class="col-4">
                                <asp:Literal ID="AchievementIdentifier" runat="server" Text='<%# Eval("AchievementIdentifier") %>' Visible="false" />
                                <%# Eval("AchievementTitle") %>
                            </div>
                            <div class="col-2 text-center">
                                <insite:CheckBox runat="server" ID="IsPlanned" CssClass="IsPlanned" Checked='<%# Eval("IsPlanned") %>' RenderMode="Input" />
                            </div>
                            <div class="col-2 text-center">
                                <insite:CheckBox runat="server" ID="IsRequired" CssClass="IsRequired" Checked='<%# Eval("IsRequired") %>' RenderMode="Input" />
                            </div>
                            <div class="col-2 text-center IsTimeSensitive">
                                <insite:CheckBox runat="server" Enabled="false" CssClass="IsTimeSensitive" Checked='<%# Eval("IsTimeSensitive") %>' RenderMode="Input" />
                            </div>
                            <div class="col-2 text-end">
                                <insite:TextBox runat="server" ID="LifetimeMonths"
                                    MaxLength="10"
                                    CssClass="text-end"
                                    Text='<%# Eval("LifetimeMonths") %>'
                                    data-value='<%# Eval("LifetimeMonths") %>'
                                    ClientEvents-OnBlur="lifetimeMonths_blur(this);"
                                />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </ItemTemplate>
</asp:Repeater>

<insite:PageFooterContent runat="server">
<script type="text/javascript">
    function lifetimeMonths_blur(txt) {
        var textValue = $(txt).val();
        var value = Number(textValue);
        var isTimeSensitive = !isNaN(value) && value > 0;

        var parentControl = $(txt.parentNode.parentNode);

        parentControl.find(".IsTimeSensitive input[type=checkbox]").prop("checked", isTimeSensitive);

        if (isTimeSensitive && $(txt).data("value") != textValue)
            parentControl.find(".IsRequired input[type=checkbox]").prop("checked", true);

        $(txt).data("value", textValue);
    }

    (function () {
        init('.select-all-planned', '.IsPlanned');
        init('.select-all-required', '.IsRequired');
        init('.select-all-timesensitive', '.IsTimeSensitive');

        function init(selectAllSelector, checkboxSelector) {
            document.querySelectorAll(selectAllSelector).forEach(function (selectAll) {
                if (!selectAll.id.startsWith('<%= ClientID %>_'))
                    return;

                selectAll._checkboxes = selectAll.closest('.folder-container').querySelectorAll(checkboxSelector);
                selectAll.addEventListener('change', onSelectAllChange);

                selectAll._checkboxes.forEach(function (checkbox) {
                    checkbox._selectAll = selectAll;
                    checkbox.addEventListener('change', onCheckboxChange);
                });

                updateSelectAllState(selectAll);
            });
        }

        function onSelectAllChange() {
            const isChecked = this.checked;
            this._checkboxes.forEach(function (checkbox) {
                checkbox.checked = isChecked;
            });
        }

        function onCheckboxChange() {
            updateSelectAllState(this._selectAll);
        }

        function updateSelectAllState(selectAll) {
            const checkboxes = selectAll._checkboxes;
            if (checkboxes.length == 0)
                return;

            let isChecked = true;

            for (let i = 0; i < checkboxes.length; i++) {
                const checkbox = checkboxes[i];
                if (!checkbox.checked) {
                    isChecked = false;
                    break;
                }
            }

            selectAll.checked = isChecked;
        }
    })();

</script>
</insite:PageFooterContent>