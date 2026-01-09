<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Assessments.Sets.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/SetInfo.ascx" TagName="SetDetails" TagPrefix="uc" %>

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
                        <h3>Question Set</h3>

                        <uc:SetDetails runat="server" id="SetDetails" />

                        <dl class="row">
                            <dt class="col-sm-3">
                                Bank Name
                            </dt>
                            <dd class="col-sm-9">
                                <asp:Literal runat="server" ID="BankName" />
                            </dd>
                        </dl>
                    </div>
                </div>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this question set from the bank?
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
                        The question set will be deleted from all forms, queries, and reports.
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
                                    Question Sets
                                </td>
                                <td>
                                    1
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Question Items
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="QuestionCount" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Specification Criteria
                                </td>
                                <td>
                                    <asp:Literal runat="server" ID="CriterionCount" />
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
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
