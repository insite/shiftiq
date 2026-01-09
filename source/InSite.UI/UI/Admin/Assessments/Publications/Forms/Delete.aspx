<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Publications.Forms.Delete" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

    <style type="text/css">
        span.smart-checkbox { display: table; }
        span.smart-checkbox > input { display: table-cell; }
        span.smart-checkbox > label { display: table-cell; vertical-align: top; padding-left: 4px; }
    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Container runat="server" ID="DeleteContainer">
        <div class="row mb-3">
            <div class="col-lg-12">
                <insite:DeleteConfirmAlert runat="server" Name="Standalone Assessment" />
            </div>
        </div>

        <div class="row">

            <div class="col-lg-6">

                <div class="card shadow">
                    <div class="card-body">

                        <h5 class="card-title">Standalone Assessment</h5>

                        <dl class="row mt-3 mb-0">

                            <dt class="col-sm-3">Title</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="ResourceTitle" /></dd>

                            <insite:Container runat="server" ID="ExamFormField">
                                <dt class="col-sm-3">Exam Form</dt>
                                <dd class="col-sm-9"><asp:Literal runat="server" ID="ExamFormTitle" /></dd>
                            </insite:Container>

                            <dt class="col-sm-3">Content Link / Portal URL </dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="NavigationUrl" /></dd>

                        </dl>

                    </div>
                </div>

                <div class="card shadow mt-4">
                    <div class="card-body text-danger">
                        <h5 class="card-title">Action Required</h5>
                        <div class="card-text">
                            <asp:CheckBox runat="server" ID="DeleteCheck" CssClass="smart-checkbox" Text="Yes, I understand the consequences, delete this standalone assessment." />
                        </div>
                    </div>
                </div>

            </div>

            <div class="col-md-6">
                <div class="card shadow">
                    <div class="card-body">

                        <h5 class="card-title">Consequences</h5>

                        <insite:DeleteWarningAlert runat="server" Name="Standalone Assessment" />

                        <table class="table table-striped table-bordered table-metrics">
                            <tr>
                                <th>Data</th>
                                <th class="text-end" style="width:80px;">Items</th>
                            </tr>
                            <tr>
                                <td>Standalone Assessment</td>
                                <td class="text-end"><asp:Literal runat="server" ID="ResourcesCount" Text="1"/></td>
                            </tr>
                        </table>

                    </div>
                </div>
            </div>

        </div>
    </insite:Container>

    <div class="mt-4">
        <insite:DeleteButton runat="server" ID="DeleteButton" Enabled="false" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
