<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Sites.Sites.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Sites/Sites/Controls/SiteInfo.ascx" TagName="SiteDetail" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">
    
    <div class="row settings">
        <div class="col-md-6">
            <div class="settings">
                <h3>Web Site</h3>

                <uc:SiteDetail id="SiteDetails" runat="server" />

                <div class="alert alert-danger" role="alert" runat="server" ID="ConfirmText">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this site?
                </div>

                <p>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                    <insite:CancelButton runat="server" ID="CancelButton" Identifier="Cancel" />
                </p>
            </div>
        </div>

        <div class="col-md-6">
            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The web site will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                <tr>
                    <td>
                        Web Sites
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
</div>
</asp:Content>
