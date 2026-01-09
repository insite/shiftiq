<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogEntryComments.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Entries.Controls.LogEntryComments" %>

<div class="row">
    <div class="col-md-12">
        <asp:Repeater runat="server" ID="Repeater">
            <ItemTemplate>

                <div class="commands float-end">
                    
                    <span runat="server" class="badge bg-custom-default mr-3" visible='<%# Eval("CommentIsPrivate") %>'>
                        Private
                    </span>

                </div>

                <h4 style="margin-top:0;">
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
