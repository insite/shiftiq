<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentList.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.CommentList" %>

<div class="row" style="padding-bottom:18px;">
    <div class="col-lg-12">
        <div class="text-end">
            <insite:AddButton runat="server" ID="AddCommentLink" Text="New Comment" />
        </div>
    </div>
</div>

<div class="row">
    <div class="col-md-12">
        <asp:Repeater runat="server" ID="Repeater">
            <ItemTemplate>
                <h4 >
                    <%# Eval("AuthorName") %>
                </h4>

                <div class="form-text" >
                    <%# Eval("PostedOn") %>
                </div>

                <div class="form-text">
                    <b><asp:Literal runat="server" id="SubjectName" /></b>
                </div>

                <div style="width: 100%;">
                    <div style="float:left;">
                        <%# Eval("Text") %>
                    </div>
                    <div style="float:right;">
                        <a runat="server" id="ChangeLink" title="Change Comment"><i class="icon fas fa-pencil"></i></a>
                        <a runat="server" id="DeleteLink" title="Delete Comment"><i class="icon fas fa-trash-alt"></i></a>
                    </div>
                    <div style="clear:both" />
                </div>
                
                <hr/>

            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>