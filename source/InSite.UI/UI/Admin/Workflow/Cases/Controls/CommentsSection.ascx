<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentsSection.ascx.cs" Inherits="InSite.Admin.Issues.Outlines.Controls.CommentsSection" %>

    <div class="row mt-4 mb-4">
        <div class="col-lg-6">
            <insite:AddButton runat="server" ID="AddComment" Text="New Comment" />
            <insite:DownloadButton runat="server" ID="DownloadCommentsXlsx" />
        </div>
        <asp:Panel runat="server" DefaultButton="SearchInput" CssClass="col-lg-6">
            <insite:InputSearch runat="server" ID="SearchInput" />
        </asp:Panel>
    </div>

    <div runat="server" id="CommentPanel" class="row">
        <div class="col-lg-12">

            <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h3>Comments</h3>

                <asp:Repeater runat="server" ID="Repeater">
                    <HeaderTemplate>
                        <div class="accordion mb-5 position-relative">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                    <ItemTemplate>

                        <div class="accordion-item">
                            <h2 class="accordion-header" id='item-header-<%# Eval("CommentIdentifier") %>'>
                                <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target='#item-collapse-<%# Eval("CommentIdentifier") %>' aria-expanded="false" aria-controls='item-collapse-<%# Eval("CommentIdentifier") %>'>
            
                                    <%# GetCommentSubject(Container.DataItem) %>
                                    <span class="fs fs-sm text-body-secondary ms-2 fw-normal"><%# GetTimestamp(Container.DataItem) %></span>

                                    <div class="position-absolute end-0 pe-5">
                                        <%# Eval("CommentCategoryHtml") %>
                                        <%# Eval("CommentFlagHtml") %>
                                    </div>

                                </button>
                            </h2>
                            <div class="accordion-collapse collapse" id='item-collapse-<%# Eval("CommentIdentifier") %>' aria-labelledby='item-header-<%# Eval("CommentIdentifier") %>' data-bs-parent="#orders-accordion" style="">
                                <div class="accordion-body bg-secondary rounded-top-0 rounded-3">
            
                                    <div class="mb-3">
                                        <%# GetCommentBody(Container.DataItem) %>
                                    </div>

                                    <div>
                                        <a href='<%# string.Format("/ui/admin/workflow/comments/modify?case={0}&comment={1}", Eval("IssueIdentifier"), Eval("CommentIdentifier")) %>' title="Revise Comment"><i class="icon fas fa-pencil"></i></a>
                                        <a href='<%# string.Format("/ui/admin/workflow/comments/delete?case={0}&comment={1}", Eval("IssueIdentifier"), Eval("CommentIdentifier")) %>' title="Delete Comment"><i class="icon fas fa-trash-alt"></i></a>
                                    </div>

                                </div>
                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </div>

        </div>

    </div>
</div>
