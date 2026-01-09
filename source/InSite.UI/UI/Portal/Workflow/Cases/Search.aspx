<%@ Page Language="C#" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Portal.Issues.Search" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <div class="row">
        <div class="col-lg-12">

            <insite:Alert runat="server" ID="NoIssuesAlert" Visible="false" Indicator="Success" Icon="fas fa-check">
                You have no outstanding cases.
            </insite:Alert>

            <asp:Repeater runat="server" ID="IssueRepeater">
                <ItemTemplate>
                    <div class="card shadow card-hover mb-5">

                        <div class="card-body">

                            <h5 class="card-title">Case #<%# Eval("IssueNumber") %>
                                <span class="ms-1 me-1">&raquo;</span>
                                <%# Eval("IssueType") %>
                                <span class="ms-1 me-1">&raquo;</span>
                                <%# Eval("IssueStatusDisplay") %>
                            </h5>

                            <p class="card-text"><%# Eval("IssueTitle") %></p>

                            <%# Eval("IssueDescriptionHtml") %>

                            <div class="d-block">
                                <asp:HyperLink runat="server" ID="ViewIssueLink" CssClass="btn btn-sm btn-primary"><i class="fas fa-folder-open me-2"></i>View Case</asp:HyperLink>
                            </div>
                    </div>

                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

</asp:Content>
