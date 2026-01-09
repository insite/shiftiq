<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentRepeater.ascx.cs" Inherits="InSite.UI.Portal.Learning.Controls.CommentRepeater" %>

<div class="row">

    <div class="col-lg-12">

        <asp:Repeater runat="server" ID="Repeater">
            <ItemTemplate>

                <div class="card border-1 mb-3">

                    <div class="card-body">

                        <div class="float-end">
                            <insite:IconButton runat="server" ID="DeleteComment" Type="Solid" Name="trash-alt" />
                        </div>

                        <strong class="mb-1">
                            <%# Eval("AuthorUserName") %><span class="fs fs-sm text-body-secondary ms-2 fw-normal"><%# GetTimestamp(Container.DataItem) %></span>
                        </strong>

                        <div>
                            <%# GetCommentHtml(Container.DataItem) %>
                        </div>

                    </div>

                </div>

            </ItemTemplate>
        </asp:Repeater>

    </div>

</div>
