<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentList.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.DocumentList" %>

<asp:Repeater runat="server" ID="ListRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th></th>
                    <th>Type</th>
                    <th>Status</th>
                    <th>Document</th>
                    <th>Permissions</th>
                    <th runat="server" visible='<%# IsIssue %>'>
                        Case
                    </th>
                    <th runat="server" visible='<%# IsResponse %>'>
                        Source
                    </th>
                    <th runat="server" visible='<%# IsResponse %>' class="text-center">
                        Question #
                    </th>
                    <th>Uploaded</th>
                    <th>Reviewed</th>
                    <th>Approved</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td style="width:70px;">
                <insite:IconLink runat="server"
                    Visible='<%# Eval("HasFile") %>'
                    Name="pencil"
                    ToolTip="Edit File Properties"
                    NavigateUrl='<%# Eval("FileIdentifier", "/ui/admin/assets/files/edit?file={0}") %>'
                />
                <insite:IconLink runat="server"
                    Visible='<%# Eval("DeleteUrl") != null %>'
                    Name="trash-alt"
                    ToolTip="Delete File"
                    NavigateUrl='<%# Eval("DeleteUrl") %>'
                />
            </td>
            <td>
                <%# Eval("DocumentType") %>

                <div runat="server" visible='<%# Eval("DocumentType") == null %>'>
                    <i>N/A</i>
                </div>
            </td>
            <td>
                <%# Eval("FileStatus") %>
            </td>
            <td>
                <a runat="server" target="_blank" visible='<%# Eval("HasFile") %>' href='<%# Eval("DownloadUrl") %>'>
                    <i class='far fa-external-link me-1'></i>
                    <%# Eval("DocumentName") %>
                </a>
                <span runat="server" visible='<%# !(bool)Eval("HasFile") %>'>
                    <%# Eval("FileName") %>
                </span>
                <span runat="server" visible='<%# Eval("HasFile") %>' class="form-text text-body-secondary">
                    <%# Eval("FileSize", "({0})") %>
                </span>
                <p runat="server" visible='<%# Eval("HasFile") %>'>
                    <%# Eval("DocumentDescription") %>
                </p>
            </td>
            <td>
                <asp:Literal runat="server"
                    Visible='<%# Eval("HasFile") %>'
                    Text='<%# (bool)Eval("IsPublicAccess") ? "Public" : "Private" %>'
                />
            </td>
            <td runat="server" visible='<%# IsIssue %>'>
                <a href='<%# Eval("IssueUrl") %>'>
                    <%# Eval("IssueName") %>
                </a>
            </td>
            <td runat="server" visible='<%# IsResponse %>'>
                <%# Eval("ResponseSource") %>
            </td>
            <td runat="server" visible='<%# IsResponse %>' class="text-center">
                <%# Eval("ResponseQuestionNumber") %>
            </td>
            <td>
                <%# Eval("UploadedTime") %>
                <div class="form-text text-body-secondary">
                    <%# Eval("UploadedBy") %>
                </div>
            </td>
            <td>
                <div runat="server" visible='<%# Eval("ReviewedTime") != null %>'>
                    <%# Eval("ReviewedTime") %>
                    <div class="form-text text-body-secondary">
                        <%# Eval("ReviewedBy") %>
                    </div>
                </div>
            </td>
            <td>
                <div runat="server" visible='<%# Eval("ApprovedTime") != null %>'>
                    <%# Eval("ApprovedTime") %>
                    <div class="form-text text-body-secondary">
                        <%# Eval("ApprovedBy") %>
                    </div>
                </div>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
</asp:Repeater>