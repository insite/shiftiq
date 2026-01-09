<%@ Page Language="C#" CodeBehind="DeleteAttachment.aspx.cs" Inherits="InSite.UI.Admin.Contacts.People.Forms.DeleteAttachment" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <div class="row">

        <div class="col-md-6">
            <div class="settings">

                <h3>Attachment</h3>

                <dl class="row">
                    <dt class="col-sm-3">File Name</dt>
                    <dd class="col-sm-9"><asp:LinkButton runat="server" ID="FileName" /></dd>
                
                    <dt class="col-sm-3">File Size</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="FileSize" /></dd>
                
                    <dt class="col-sm-3">Timestamp</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="Timestamp" /></dd>
                </dl>
                
                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this attachment?
                </div>

                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

        <div class="col-md-6">
            <div class="settings">

                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The attachment will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Attachments
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
</asp:Content>
