<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClassAssessmentList.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.ClassAssessmentList" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormPopupSelectorWindow.ascx" TagName="FormPopupSelectorWindow" TagPrefix="uc" %>

<uc:FormPopupSelectorWindow runat="server" ID="FormPopupSelectorWindow" />

<insite:Alert runat="server" ID="AddFormAlert" />

<div class="mb-3">
    <insite:Button runat="server" ID="AddFormButton" Text="Add Form" Icon="fas fa-plus-circle" ButtonStyle="Default" />
    <asp:HiddenField runat="server" ID="AddFormIdentifier" />
</div>

<asp:Repeater runat="server" ID="FormRepeater">
    <ItemTemplate>

        <div class="mb-3">
            <h3>
                <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&form=<%# Eval("FormIdentifier") %>">
                    <%# Eval("FormTitle") %>
                </a>
            </h3>

            <div class="float-end">
                <insite:IconButton runat="server" Name="trash-alt" 
                    Visible="<%# CanWrite %>"
                    CommandName="Delete" 
                    CommandArgument='<%# Eval("FormIdentifier") %>' 
                    ConfirmText="Are you sure you want to remove this form from this class event?" />
            </div>

            <div>
                <%# Eval("FormName") %> [<%# Eval("FormAsset") %>.<%# Eval("FormAssetVersion") %>]
            </div>
                            
        </div>

        <asp:Literal ID="FormMaterials" runat="server" Visible="false" />

    </ItemTemplate>
</asp:Repeater>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            Sys.Application.add_load(function () {

                $('#<%= AddFormButton.ClientID %>').on('click', function (e) {
                    e.preventDefault();

                    bankFormPopupSelectorWindow.<%= FormPopupSelectorWindow.ClientID %>.open({
                        value: null,
                        onSelected: function (data) {
                            $('#<%= AddFormIdentifier.ClientID %>').val(data.id);
                            __doPostBack('<%= AddFormButton.UniqueID %>', '');
                        }
                    });
                });

            });
        })();

    </script>

</insite:PageFooterContent>