<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentCaseGrid.ascx.cs" Inherits="InSite.Admin.Reports.Dashboards.Controls.CommentCaseGrid" %>

<div class="card">
    <div class="card-body">

        <h3 class="card-title mb-3">
            <asp:Literal ID="CardTitle" runat="server"></asp:Literal>
        </h3>

        <p runat="server" id="EmptyGrid" class="help">There are no comments to display.</p>

        <insite:Grid runat="server" ID="Grid" PageSize="10">
            <Columns>
                <asp:TemplateField HeaderText="#">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" Text='<%# GetIssueNumber((Guid)Eval("IssueIdentifier")) %>'
                            NavigateUrl='<%# Eval("IssueIdentifier", "/ui/admin/workflow/cases/outline?case={0}") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comment">
                    <ItemTemplate>
                        <div class="float-end">
                            <%# Eval("CommentCategoryHtml") %>
                            <%# Eval("CommentFlagHtml") %>
                        </div>

                        <strong class="mb-1">
                            <%# LocalizeTime((DateTimeOffset?)Eval("CommentPosted")) %>
                        </strong>

                        <div>
                            <%# GetCommentHtml(Eval("CommentText")) %>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </insite:Grid>

    </div>
</div>
