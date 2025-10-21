<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BankCommentaryResults.ascx.cs" Inherits="InSite.Admin.Assessments.Reports.Controls.BankCommentaryResults" %>

<asp:Literal runat="server" ID="Instructions" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Comment">
            <itemtemplate>
                <div>
                    <%# TimeZones.Format((DateTimeOffset)Eval("CommentPosted"), CurrentSessionState.Identity.User.TimeZone) %><% if (InnerFilter.IsShowAuthor) { %> &ndash; Posted by <%# Eval("AuthorName") %><% } %>: 
                </div>
                <span style="white-space: pre-wrap;"><%# Eval("CommentText") %></span>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Subject">
            <itemtemplate>
                <asp:Literal runat="server" ID="Subject" />
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Flag">
            <itemtemplate>
                <%# GetFlagDescription((string)Eval("CommentFlag")) %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Category" DataField="FeedbackCategory" />

        <asp:BoundField HeaderText="Format" DataField="EventFormat" />

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="40px">
            <itemtemplate>
                <insite:IconLink runat="server" ID="EditLink" Name="pencil" ToolTip="Edit" Visible="false" CssClass="scroll-send" />
                <insite:IconLink runat="server" ID="EntityLink" Name="reply fa-rotate-270" Visible="false" CssClass="scroll-send" Type="Regular" />
            </itemtemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
