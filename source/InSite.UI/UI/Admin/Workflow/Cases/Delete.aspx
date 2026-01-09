<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Issues.Issues.Delete" %>

<%@ Register TagName="CaseInfo" TagPrefix="uc" Src="./Controls/CaseInfo.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    
    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div class="row mb-3">
        <div class="col-lg-12">
            <insite:DeleteConfirmAlert runat="server" Name="Case" />
        </div>
    </div>

    <div class="row">

        <div class="col-lg-6"> 

            <div class="card shadow">
                <div class="card-body">
                    <uc:CaseInfo runat="server" ID="CaseInfo" />
                </div>
            </div>

        </div>

            <div class="col-lg-6">
                
                <h3>Consequences</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The case will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Case
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Attachments
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="AttachmentCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Comments
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="CommentCount" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Users
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="UserCount" />
                        </td>
                    </tr>
                </table>

            </div>
       
    </div>
   
    <div class="row">
        <div class="col-lg-12">

            <div class="card shadow mt-4">
                <div class="card-body">
                    <h5 class="card-title">Action Required</h5>
                    <div class="card-text text-danger">

                        <div>Yes, I understand the consequences:</div>
                        <div runat="server" id="Confirm1Panel">
                            <asp:CheckBox runat="server" ID="Confirm1" Text="Unassign all contacts from this case" />
                        </div>
                        <div class="mb-3">
                            <asp:CheckBox runat="server" ID="Confirm2" Text="Delete this case and all its comments and attachments" />
                        </div>

                        <insite:DeleteButton runat="server" ID="DeleteButton" CssClass="disabled" disabled="disabled" />
                        <insite:CancelButton runat="server" ID="CancelButton" />

                    </div>
                </div>
            </div>

        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script>
            (() => {
                const confirm1 = document.getElementById('<%= Confirm1.ClientID %>');
                const confirm2 = document.getElementById('<%= Confirm2.ClientID %>');
                const deleteButton = document.getElementById('<%= DeleteButton.ClientID %>');

                if (confirm1) {
                    confirm1.addEventListener("change", validateDeleteButton);
                }

                confirm2.addEventListener("change", validateDeleteButton);

                validateDeleteButton();

                function validateDeleteButton() {
                    if (confirm1 && !confirm1.checked || !confirm2.checked) {
                        deleteButton.classList.add("disabled");
                        deleteButton.setAttribute("disabled", "disabled");
                    } else {
                        deleteButton.classList.remove("disabled");
                        deleteButton.removeAttribute("disabled");
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
