<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Sites.Pages.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Sites/Pages/Controls/PageInfo.ascx" TagName="PageDetail" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <div class="row mb-3">
        <div class="col-lg-12">
            <insite:DeleteConfirmAlert runat="server" Name="Page" />
        </div>
    </div>

    <div class="row">

        <div class="col-md-6">
            
                <div class="card shadow h-100">
                    <div class="card-body">
                        
                        <h3>Web Page</h3>
                        <uc:PageDetail id="PageDetails" runat="server" />

                    </div>
                </div>
            
        </div>

        <div class="col-md-6">
            
            <h3>Consequences</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The web page will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                <tr>
                    <td>
                        Web Page
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Contained Pages
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="ContainedPagesCount" />
                    </td>
                </tr>
            </table>

        </div>

    </div>

    <div class="row">
        <div class="col-lg-12">

            <insite:UpdatePanel runat="server">

                <ContentTemplate>

                    <div class="card shadow mt-4">
                        <div class="card-body">
                            <h5 class="card-title">Action Required</h5>
                            <div class="card-text text-danger">
                                <div>Yes, I understand the consequences:</div>
                                <div class="mb-3">
                                    <asp:CheckBox runat="server" ID="Confirm1" Text="Delete this page and all its contents and contained pages" />
                                </div>
                                <insite:DeleteButton runat="server" ID="DeleteButton" Enabled="false" />
                                <insite:CancelButton runat="server" ID="CancelButton" Identifier="Cancel" />
                            </div>
                        </div>
                    </div>

                </ContentTemplate>

            </insite:UpdatePanel>

        </div>
    </div>

</asp:Content>
