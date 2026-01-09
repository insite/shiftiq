<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <link href="/UI/Admin/Assessments/Outlines/Forms/Outline.css" rel="stylesheet" />
</insite:PageHeadContent>

<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <div class="row">
        <div class="col-md-6">
            <div class="settings">
                <div class="card">
                    <div class="card-body">
                        <uc:FormDetails id="FormDetails" runat="server" />
                    </div>
                </div>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this form from the bank?
                </div>

                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

        <div class="col-md-6">
            <div class="settings">
                <div class="card">
                    <div class="card-body">
                        <h3>Impact</h3>

                        <div class="alert alert-warning">
                                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The form will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                        </div>

                        <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                            <tr>
                                <td>
                                    Type
                                </td>
                                <td>
                                    Rows
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Forms
                                </td>
                                <td>
                                    1
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Form Sections
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="SectionCount" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Form Fields
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="FieldCount" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
