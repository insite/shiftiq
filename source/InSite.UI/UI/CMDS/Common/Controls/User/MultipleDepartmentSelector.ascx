<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultipleDepartmentSelector.ascx.cs" Inherits="InSite.Cmds.Controls.Reports.Criteria.MultipleDepartmentSelector" %>

<asp:Panel ID="DepartmentPanel" runat="server">

    <asp:Repeater ID="DepartmentDistricts" runat="server">

        <HeaderTemplate>
            <table id="<%= ClientID + "_Table" %>" class="check-list" style='<%# EnableDistricts ? "margin-top:-7px;": "" %> margin-bottom:5px;'>
        </HeaderTemplate>

        <FooterTemplate>
            </table>
        </FooterTemplate>

        <ItemTemplate>

            <tr runat="server" visible='<%# EnableDistricts %>'>
                <td style="padding-top:10px;">
                    <asp:CheckBox runat="server" ID="IsSelected" />
                </td>
                <td colspan="8" style="padding-top:10px;">
                    <strong><asp:Label runat="server" Text='<%# Eval("DivisionName") %>' AssociatedControlID="IsSelected" /></strong>
                </td>
            </tr>

            <asp:Repeater ID="Departments" runat="server">
                <ItemTemplate>
                    <%# (Container.ItemIndex % 2) == 0 ? "<tr>": "" %>
                        <td></td>
                        <td style="width: 24px;">
                            <asp:Literal ID="DepartmentIdentifier" runat="server" Text='<%# Eval("DepartmentIdentifier") %>' Visible="false" />
                            <asp:CheckBox runat="server" ID="IsSelected" />
                        </td>
                        <td>
                            <asp:Label runat="server" Text='<%# Eval("DepartmentName") %>' AssociatedControlID="IsSelected" />
                        </td>
                    <%# ((Container.ItemIndex + 1) % 2) == 0 ? "</tr>": "" %>
                </ItemTemplate>
                <FooterTemplate>
                    <%# ((((Repeater)Container.Parent).Items.Count + 1) % 2) == 0 ? "</tr>": "" %>
                </FooterTemplate>
            </asp:Repeater>

        </ItemTemplate>

    </asp:Repeater>

    <insite:Button runat="server" ButtonStyle="Default" Icon="far fa-check-square" Text="Select All" OnClientClick="return MultipleDepartmentSelector_selectButton_onclick(true)" />
    &bull;
    <insite:Button runat="server" ButtonStyle="Default" Icon="far fa-square" Text="Deselect All" OnClientClick="return MultipleDepartmentSelector_selectButton_onclick(false)" />

    <asp:HiddenField ID="OnClientItemsChangedField" runat="server" />

</asp:Panel>

<script type="text/javascript">
    function MultipleDepartmentSelector_Districts_onclick(chkDistrict)
    {
        var checkboxes = $get("<%= DepartmentPanel.ClientID %>").getElementsByTagName("input");
        var isStartFound = false;

        for (var i = 0; i < checkboxes.length; i++)
        {
            var chk = checkboxes[i];

            if (isStartFound)
            {
                var index = chk.parentNode.getAttribute("index");

                if (index)
                    chk.checked = chkDistrict.checked;
                else
                    break;
            }
            else if (chk == chkDistrict)
                isStartFound = true;
        }

        MultipleDepartmentSelector_execute_OnClientItemsChanged();
    }

    function MultipleDepartmentSelector_Departments_onclick()
    {
        MultipleDepartmentSelector_execute_OnClientItemsChanged();
    }

    function MultipleDepartmentSelector_selectButton_onclick(checked)
    {
        var checkboxes = $get("<%= DepartmentPanel.ClientID %>").getElementsByTagName("input");

        for (var i = 0; i < checkboxes.length; i++)
        {
            checkboxes[i].checked = checked;
        }

        MultipleDepartmentSelector_execute_OnClientItemsChanged();

        return false;
    }

    function MultipleDepartmentSelector_execute_OnClientItemsChanged()
    {
        var script = $get("<%= OnClientItemsChangedField.ClientID %>").value;

        if (script != null)
            setTimeout(script, 0);
    }

</script>