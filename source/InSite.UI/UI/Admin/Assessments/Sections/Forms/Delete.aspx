<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Assessments.Sections.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <link href="/UI/Admin/Assessments/Outlines/Forms/Outline.css" rel="stylesheet" />
</insite:PageHeadContent>

<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />

    <div class="row">
        <div class="col-md-6">
            <div class="settings">
                <div class="card">
                    <div class="card-body">
                        <h3>Section</h3>

                        <dl class="row">
                            <dt class="col-sm-3">Criterion Name</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="CriterionTitle" /></dd>
                
                            <dt class="col-sm-3">Section Number</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="SectionNumber" /></dd>
                
                            <dt class="col-sm-3">Standard</dt>
                            <dd class="col-sm-9"><assessments:AssetTitleDisplay runat="server" ID="FormStandard" /></dd>
                
                            <dt class="col-sm-3">Form Name</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="FormName" /></dd>

                            <dt class="col-sm-3">Specification Name</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="SpecificationName" /></dd>
                        </dl>
                    </div>
                </div>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this section from the form?
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
                        The section will be deleted from all forms, queries, and reports.
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
                            <asp:Repeater runat="server" ID="ReferenceRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("Name") %></td>
                                        <td><%# Eval("Count", "{0:n0}") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
