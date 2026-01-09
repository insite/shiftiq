<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Assessments.Attachments.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server" ID="GridStyle">
    <style type="text/css">

        div.sequence {
            font-size: 27px;
            color: #265F9F;
            white-space: nowrap;
        }

    </style>
</insite:PageHeadContent>

<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <div class="row">

        <div class="col-md-6">
            <div class="settings">

                <div class="card">
                    <div class="card-body">

                        <h3>Attachment</h3>

                        <dl class="row">
                            <dt class="col-sm-3">Title</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="AttachmentTitle" /></dd>
                
                            <dt class="col-sm-3">Asset #</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="AssetNumber" /></dd>
                
                            <dt class="col-sm-3">Publication/ Status</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="PublicationStatus" /></dd>
                
                            <dt class="col-sm-3">File Name</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="FileName" /></dd>
                
                            <dt class="col-sm-3">File Size</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="FileSize" /></dd>
                
                            <dt class="col-sm-3">Timestamp</dt>
                            <dd class="col-sm-9"><asp:Literal runat="server" ID="Timestamp" /></dd>
                        </dl>   
                    </div>
                </div>

                <div class="alert alert-danger">
                    <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                    Are you sure you want to delete this attachment from the bank?
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
                        The attachment will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                        </div>

                        <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
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
                                    Attachments
                                </td>
                                <td>
                                    1
                                </td>
                            </tr>
                        </table>
                        
                        <div runat="server" id="QuestionListField" class="form-group">
                            <dl>
                                <dt>Used in Questions</dt>
                            </dl>
                    
                            <div>
                                <asp:Repeater runat="server" ID="QuestionList">
                                    <HeaderTemplate>
                                        <table class="table">
                                    </HeaderTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width:60px;">
                                                <div class="sequence">
                                                    <%# ((int)Eval("BankIndex") + 1) %>
                                                </div>
                                            </td>
                                            <td>
                                                <div>
                                                    <%# Shift.Common.Markdown.ToHtml(Eval("Content.Title") == null ? null : ((Shift.Common.MultilingualString)Eval("Content.Title")).Default) %>
                                                </div>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                        <dl class="form-group">
                            <dt>Version</dt>
                    
                            <asp:RadioButtonList runat="server" ID="VersionSelector">
                                <asp:ListItem Value="current" Text="Delete Current Version" Selected="True" />
                                <asp:ListItem Value="all" Text="Delete All Versions" />
                            </asp:RadioButtonList>
                        </dl>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
