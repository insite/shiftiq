<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentList.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.CommentList" %>

<div class="row mb-3">
    <div class="col-lg-12">
        <insite:AddButton runat="server" ID="AddCommentLink" Text="New Comment" />
    </div>
</div>

<div class="row">
    <div class="col-md-12">
        <asp:Repeater runat="server" ID="Repeater">
            <ItemTemplate>

                <div class="commands float-end">
                    
                    <span runat="server" class="badge bg-custom-default mr-3" visible='<%# Eval("IsPrivate") %>'>
                        Private
                    </span>

                    <a runat="server" id="ChangeLink" title="Change Comment" class="scroll-send"><i class="icon fas fa-pencil"></i></a>
                    <a runat="server" id="DeleteLink"  href='<%# Eval("CommentIdentifier", "/ui/admin/records/logbooks/delete-journal-comment?comment={0}") %>' title="Delete Comment" class="scroll-send"><i class="icon fas fa-trash-alt"></i></a>
                </div>

                <h4 class="mt-0">
                    <%# Eval("AuthorName") %>
                </h4>

                <div class="form-text" style="margin-top: -10px;">
                    <%# Eval("PostedOn") %>
                </div>

                <div class="form-text">
                    <b><asp:Literal runat="server" id="SubjectName" /></b>
                </div>

                <div style="width: 100%; padding: 0 75px 0 0;">
                    <%# Eval("Text") %>
                </div>
                
                <hr/>

            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
