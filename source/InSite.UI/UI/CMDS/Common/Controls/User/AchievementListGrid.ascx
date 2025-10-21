<%@ Control Language="C#" CodeBehind="AchievementListGrid.ascx.cs" Inherits="InSite.UI.CMDS.Common.Controls.User.AchievementListGrid" %>

<div runat="server" id="Wrapper">

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table class="table table-striped" id='<%# ClientID %>'>
            <thead>
                <tr>
                    <th style="width:0px;">
                        <insite:CheckBox runat="server" ID="GroupCheckBox" RenderMode="Input" />
                    </th>
                    <th>Type</th>
                    <th>Achievement</th>
                    <th>Categories</th>
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
            <td>
                <insite:CheckBox runat="server" ID="AchievementSelected" RenderMode="Input" />
                <asp:HiddenField runat="server" ID="AchievementIdentifier" Value='<%# Eval("AchievementIdentifier") %>' />
            </td>
            <td><%# Eval("AchievementLabel") %></td>
            <td><%# Eval("AchievementTitle") %></td>
            <td><%# Eval("CategoryNames") %></td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                const table = document.getElementById('<%= ClientID %>');
                if (!table)
                    return;

                const groupCheck = table.tHead.querySelector('input[type="checkbox"]');
                if (!groupCheck)
                    return;

                const childChecks = table.tBodies[0].querySelectorAll('input[type="checkbox"]');

                groupCheck._childChecks = childChecks;
                groupCheck.addEventListener('change', onGroupCheckChanged);

                for (let check of childChecks) {
                    check._groupCheck = groupCheck;
                    check.addEventListener('change', onChildCheckChanged);
                }

                updateGroupCheckState(groupCheck);
            });

            function onGroupCheckChanged() {
                const checked = this.checked;
                const childChecks = this._childChecks;

                for (let check of childChecks) {
                    check.checked = checked;
                }
            }

            function onChildCheckChanged() {
                updateGroupCheckState(this._groupCheck);
            }

            function updateGroupCheckState(groupCheck) {
                const childChecks = groupCheck._childChecks;
                if (childChecks.length === 0)
                    return;

                let checked = true;

                for (let check of childChecks) {
                    if (!check.checked) {
                        checked = false;
                    }
                }

                groupCheck.checked = checked;
            }
        })();
    </script>
</insite:PageFooterContent>
