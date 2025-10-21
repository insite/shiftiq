<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Assessments.Banks.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/BankDetail.ascx" TagName="BankDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <div runat="server" id="AdminTaskPanel">

        <section class="pb-5 mb-md-2">

            <div class="mb-2">
                <asp:Label ID="DeleteTitle" runat="server"></asp:Label>
            </div>

            <div class="alert alert-danger" role="alert" runat="server" ID="AdminErrorPanel" Visible="false"></div>

            <div class="row settings">
                <div class="col-md-6">
                    <div class="card mb-3">
                        <div class="card-body">
                            <h3>Bank</h3>
                            <uc:BankDetail runat="server" id="BankDetail" />
                        </div>
                    </div>

                    <div class="alert alert-danger mb-3" role="alert">
                        <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                        Are you sure you want to delete this question bank?
                    </div>

                    <div class="mb-3">
                        <asp:CheckBox runat="server" ID="DeleteCheck" CssClass="smart-checkbox text-danger" Text="Yes, I understand the consequences, delete this assessment bank." />
                    </div>

                    <div class="mb-3">
                        <insite:DeleteButton runat="server" ID="DeleteButton" Enabled="false" />
                        <insite:CancelButton runat="server" ID="CancelButton" />
                    </div>
                </div>

                <div class="col-md-6">
                    <div class="card">
                        <div class="card-body">
                            <h3>Impact</h3>

                            <div class="alert alert-warning">
                                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                                This is a permanent change that cannot be undone. 
                                The bank will be deleted from all forms, queries, and reports.
                                Here is a summary of the data that will be erased if you proceed.
                            </div>

                            <table class="table table-striped table-bordered table-metrics">
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
                                        <asp:Literal runat="server" ID="SetCount" />
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
                                        Question Options
                                    </td>
                                    <td>
                                        <asp:Literal runat="server" ID="OptionCount" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Specifications
                                    </td>
                                    <td>
                                        <asp:Literal runat="server" ID="SpecificationCount" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Forms
                                    </td>
                                    <td>
                                        <asp:Literal runat="server" ID="FormCount" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

</asp:Content>
