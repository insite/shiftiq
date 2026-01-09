<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentList.ascx.cs" Inherits="InSite.Admin.Issues.Reports.Controls.RecentList" %>

<asp:Literal runat="server" ID="NoChanges">There are no recent changes to cases.</asp:Literal>

<asp:Repeater runat="server" ID="IssueRepeater">    
    <HeaderTemplate>
        <dl>
    </HeaderTemplate>
    <ItemTemplate>
        
        <dt>
            <a href='/ui/admin/workflow/cases/outline?case=<%# Eval("IssueIdentifier") %>'><i class="far fa-exclamation me-1"></i><%# Eval("IssueTitle") %></a>
        </dt>
        <dd class="ms-1 ps-3 pb-3">
            <div>
                <asp:Literal runat="server" ID="LastChangeTimestamp" />
            </div>
        </dd>

    </ItemTemplate>
    <FooterTemplate>
        </dl>
    </FooterTemplate>
</asp:Repeater>
