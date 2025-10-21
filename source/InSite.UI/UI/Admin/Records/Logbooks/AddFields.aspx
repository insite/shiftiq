<%@ Page Language="C#" CodeBehind="AddFields.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.AddFields" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:Alert runat="server" ID="NoFieldsAlert" />

    <section runat="server" id="MainPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-list me-1"></i>
            Fields
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row">
                    <div class="col-md-6">

                        <table class="table table-striped table-fields">
                            <thead>
                                <tr>
                                    <th>
                                        <asp:CheckBox runat="server" />
                                    </th>
                                    <th>Name</th>
                                    <th style="text-align:center;">Required Field</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="FieldRepeater">
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width:20px;">
                                                <asp:CheckBox runat="server" ID="Selected" />
                                                <asp:Literal runat="server" ID="Type" Visible="false" Text='<%# Eval("Type") %>' />
                                            </td>
                                            <td>
                                                <%# Eval("Name") %>
                                            </td>
                                            <td style="width:100px;text-align:center;">
                                                <asp:CheckBox runat="server" ID="IsRequired" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                    </div>

                </div>

            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

<insite:PageFooterContent runat="server">
<script type="text/javascript">
    (function () {
        const rootCheckbox = document.querySelector('table.table-fields thead th input[type="checkbox"]');
        const selectCheckboxes = document.querySelectorAll('table.table-fields tbody tr input[type="checkbox"]');

        for (let i = 0; i < selectCheckboxes.length; i += 2) {
            const chk = selectCheckboxes[i];
            chk.addEventListener('change', onSelectCheckChanged);
            chk.screenData = { requiredChk: selectCheckboxes[i + 1] };
            updateSelectCheck(chk);
        }

        rootCheckbox.addEventListener('change', function () {
            const checked = this.checked;
            for (let i = 0; i < selectCheckboxes.length; i += 2) {
                const chk = selectCheckboxes[i];
                chk.checked = checked;
                updateSelectCheck(chk);
            }
        });

        function onSelectCheckChanged() {
            updateSelectCheck(this);

            let isAllChecked = selectCheckboxes.length > 0;

            for (let i = 0; i < selectCheckboxes.length; i += 2) {
                if (selectCheckboxes[i].checked === false) {
                    isAllChecked = false;
                    break;
                }
            }

            rootCheckbox.checked = isAllChecked;
        }

        function updateSelectCheck(chk) {
            const requiredChk = chk.screenData?.requiredChk;
            if (requiredChk) {
                requiredChk.disabled = !chk.checked;
                requiredChk.checked = false;
            }
        }
    })();
</script>
</insite:PageFooterContent>
</asp:Content>
