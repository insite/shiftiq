<%@ Control Language="C#" CodeBehind="Select.aspx.cs" Inherits="InSite.Cmds.Actions.Contact.Company.Competency.Popup.Filter" %>

<style type="text/css">
    .panel-heading h3 { margin: 0px; padding: 0px; }
</style>

<div id="desktop">

    <cmds:CmdsAlert runat="server" ID="ScreenStatus" />

    <insite:Container runat="server" ID="ContentContainer">

        <div class="form-group mb-3">
            <label class="form-label">
                Competency Settings
            </label>
            <div>
                <asp:RadioButtonList ID="SaveOption" runat="server">
                    <asp:ListItem Value="Only" Text="Apply to the selected profile in the selected departments" Selected="True" />
                    <asp:ListItem Value="All" Text="Apply to all profiles in the selected departments" />
                </asp:RadioButtonList>
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Departments
            </label>
            <div runat="server" id="NoDepartments" visible="false">
                No departments for specified tier of levels and profile <asp:Literal ID="ProfileName" runat="server" />.
            </div>
            <insite:Container runat="server" ID="DepartmentPanel" Visible="false">
                <div>
                    <asp:CheckBoxList ID="Departments" runat="server" RepeatColumns="2" />
                    <insite:Button runat="server" Icon="far fa-undo" Text="Clear Selection" ButtonStyle="OutlinePrimary" OnClientClick="uncheckCheckboxes(); return false;" />
                </div>
                <div class="form-text">
                    Must be selected at least one department but not more than 5.
                </div>
            </insite:Container>
        </div>

        <insite:SearchButton runat="server" ID="ApplyFilterButton" Text="Apply Filter" OnClientClick="return validateDepartmentSelection();" />
        
    </insite:Container>

</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        function uncheckCheckboxes() {
            var checkboxes = document.getElementById("<%= Departments.ClientID %>").getElementsByTagName("input");

            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i].type.toLowerCase() == "checkbox")
                    checkboxes[i].checked = false;
            }

            return false;
        }

        function validateDepartmentSelection() {
            var checkboxes = document.getElementById("<%= Departments.ClientID %>").getElementsByTagName("input");
            var selectedCount = 0;

            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i].type.toLowerCase() == "checkbox" && checkboxes[i].checked)
                    selectedCount++;
            }

            if (selectedCount == 0) {
                alert("Must be selected at least one department.");
                return false;
            }

            if (selectedCount > 5) {
                alert("Can be selected not more than 5 departments.");
                return false;
            }

            return true;
        }
        
    </script>
</insite:PageFooterContent>
