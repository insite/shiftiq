<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Assessments.Criteria.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <link href="/UI/Admin/Assessments/Outlines/Forms/Outline.css" rel="stylesheet" />
</insite:PageHeadContent>

<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <div class="row">
        <div class="col-md-6">
            <div class="card">
                <div class="card-body">
                    <h3>Criterion</h3>

                    <dl class="row">
                        <dt class="col-sm-3">Criteria Name</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="SetName" /></dd>
            
                        <dt class="col-sm-3">Set Weight</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="SetWeight" /></dd>
            
                        <dt class="col-sm-3">Question Limit</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="QuestionLimit" /></dd>
            
                        <dt class="col-sm-3">Filter Type</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="FilterType" /></dd>

                        <dt class="col-sm-3">Set Names</dt>
                        <dd class="col-sm-9">
                            <asp:Repeater runat="server" ID="SetRepeater">
                                <HeaderTemplate><ul></HeaderTemplate>
                                <ItemTemplate>
                                    <li>
                                        <%# Eval("Name") %>
                                    </li>
                                </ItemTemplate>
                                <FooterTemplate></ul></FooterTemplate>
                            </asp:Repeater>
                        </dd>

                        <dt class="col-sm-3">Standards</dt>
                        <dd class="col-sm-9">
                            <asp:Repeater runat="server" ID="StandardRepeater">
                                <HeaderTemplate><ul></HeaderTemplate>
                                <ItemTemplate>
                                    <li>
                                        <assessments:AssetTitleDisplay runat="server" ID="Standard" AssetID='<%# Eval("Standard") %>' />
                                    </li>
                                </ItemTemplate>
                                <FooterTemplate></ul></FooterTemplate>
                            </asp:Repeater>
                        </dd>

                        <dt class="col-sm-3">Bank Name</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="BankName" /></dd>

                        <dt class="col-sm-3">Specification Name</dt>
                        <dd class="col-sm-9"><asp:Literal runat="server" ID="SpecificationName" /></dd>
                    </dl>
                </div>
            </div>

            <div class="alert alert-danger">
                <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this criterion from the specification?
            </div>
            <insite:DeleteButton runat="server" ID="DeleteButton" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>

        <div class="col-md-6">
            <div class="settings">
                <div class="card">
                    <div class="card-body">
                        <h3>Impact</h3>

                        <div class="alert alert-warning">
                            <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The criterion will be deleted from all forms, queries, and reports.
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
                                    Criteria
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
                                    Section Fields
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
