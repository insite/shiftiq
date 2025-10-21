<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Contacts.MembershipReasons.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertStatus" />

    <section class="pb-5 mb-md-2">

        <div class="row">

            <div class="col-md-6">
                <div class="mb-4">
                    <h3>Reason</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Membership
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="MembershipGroupName" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Reason Type
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="ReasonType" />
                        </div>
                    </div>

                    <div runat="server" id="ReasonSubtypeField" class="form-group mb-3">
                        <label class="form-label">
                            Reason Subtype
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="ReasonSubtype" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Reason Effective Date
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="ReasonEffectiveDate" />
                        </div>
                    </div>

                    <div runat="server" id="ReasonExpiryDateField" class="form-group mb-3">
                        <label class="form-label">
                            Reason Expiry Date
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="ReasonExpiryDate" />
                        </div>
                    </div>
                </div>

                <div runat="server" id="ConfirmAlert" class="alert alert-danger" role="alert">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this reason and related data?
                </div>

                <div class="mt-3">
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </div>    
            </div>
                        
            <div class="col-md-6">
                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone.
                    The Reason will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics">
                    <thead>
                        <tr>
                            <th>Type</th>
                            <th>Rows</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>Number of Reasons</td>
                            <td>1</td>
                        </tr>
                    </tbody>
                </table>

            </div>

        </div>
    </section>
</asp:Content>