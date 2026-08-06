<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CaseDocumentList.ascx.cs" Inherits="InSite.UI.Admin.Issues.Outlines.Controls.CaseDocumentList" %>

<asp:Label runat="server" ID="RepeaterNoItems" Text="No issue attachments" />

<asp:Repeater runat="server" ID="ListRepeater">
    <HeaderTemplate>
        <table id="<%= ListRepeater.ClientID %>" class="table table-striped">
            <thead>
                <tr>
                    <insite:Container runat="server" Visible='<%# Organization.Toolkits.Issues.CaseDocumentCopy %>'>
                        <th style="width:10px;"></th>
                    </insite:Container>
                    <th style="width:10px;"></th>
                    <th>Document Type</th>
                    <th>Status</th>
                    <th>Document</th>
                    <th>Permissions</th>
                    <th>Uploaded</th>
                    <th>Date Received</th>
                    <th>Reviewed</th>
                    <th>Approved</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <insite:Container runat="server" Visible='<%# Organization.Toolkits.Issues.CaseDocumentCopy %>'>
                <td class="text-nowrap">
                    <insite:CheckBox runat="server" ID="IsSelected" RenderMode="Input" />
                </td>
            </insite:Container>
            <td class="text-nowrap">
                <insite:IconLink runat="server"
                    Name="pencil"
                    ToolTip="Edit File Properties"
                    NavigateUrl='<%# string.Format("/ui/admin/assets/files/edit?file={0}&case={1}", Eval("FileIdentifier"), IssueIdentifier) %>'
                />
                <insite:IconLink runat="server"
                    Name="trash-alt"
                    ToolTip="Delete File"
                    NavigateUrl='<%# string.Format("/ui/admin/workflow/attachments/delete?case={0}&file={1}", IssueIdentifier, Eval("FileName")) %>'
                />
            </t>
            <td>
                <%# Eval("DocumentType") %>

                <div runat="server" visible='<%# Eval("DocumentType") == null %>'>
                    <i>N/A</i>
                </div>
            </td>
            <td>
                <%# Eval("FileStatus") %>

                <label runat="server" visible='<%# !(bool)Eval("HasFile") %>' class="badge bg-danger me-2">
                    File Not Found
                </label>
            </td>
            <td>
                <a runat="server" visible='<%# Eval("HasFile") %>' href='<%# Eval("DownloadUrl") %>' target="_blank">
                    <%# Eval("FileName") %>
                </a>
                <span runat="server" visible='<%# Eval("HasFile") %>' class="form-text text-body-secondary">
                    <%# Eval("FileSize", "({0})") %>
                </span>
                <span runat="server" visible='<%# !(bool)Eval("HasFile") %>'>
                    <%# Eval("FileName") %>
                </span>
                <p runat="server" visible='<%# (bool)Eval("HasFile") %>'>
                    <%# Eval("DocumentDescription") %>
                </p>
            </td>
            <td>
                <asp:Literal runat="server"
                    Visible='<%# Eval("HasFile") %>'
                    Text='<%# (bool)Eval("IsPublicAccess") ? "Public" : "Private" %>'
                />
                <div class="form-text text-body-secondary">
                    <%# Eval("AccessList") %>
                </div>
                <div class="form-text text-body-secondary">
                    Enable File Link for Member/Topic: <%# (bool?)Eval("AllowLearnerToView") == true ? "Yes" : "No" %>
                </div>
            </td>
            <td>
                <%# Eval("UploadedTime") %>
                <div class="form-text text-body-secondary">
                    <%# Eval("UploadedBy") %>
                </div>
            </td>
            <td>
                <%# Eval("ReceivedTime") %>
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

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            const callbackFuncName = <%= HttpUtility.JavaScriptStringEncode(ClientSelectCallback, true) %>;
            if (!callbackFuncName)
                return;

            let funcObj = null;

            document.addEventListener('DOMContentLoaded', function () {
                funcObj = inSite.common.getObjByName(callbackFuncName);
                if (typeof funcObj !== 'function')
                    funcObj = null;

                onChkChange();
            });

            const chks = document.querySelectorAll('#<%= ListRepeater.ClientID %> > tbody > tr > td input[type="checkbox"][id$="IsSelected"]');
            chks.forEach(chk => chk.addEventListener('change', onChkChange));

            function onChkChange() {
                if (funcObj == null)
                    return;

                let checkedCount = 0;

                chks.forEach(checkbox => {
                    if (checkbox.checked)
                        checkedCount++;
                });

                funcObj.call(window, { checkedCount: checkedCount });
            }
        })();
    </script>
</insite:PageFooterContent>
