<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentList.ascx.cs" Inherits="InSite.Admin.Standards.Controls.RecentList" %>

<style type="text/css">
    h1.recent-title { margin-bottom: 10px; }
    table tr td { vertical-align: top; }
    table tr td.recent-icon { width: 24px; }
    .recent-counts { margin-top: 5px; text-align: right; }
    .no-wrap { white-space: nowrap; }
</style>

<asp:Repeater runat="server" ID="StandardRepeater">
    
    <ItemTemplate>
        
        <table class="table table-striped">
            <tr>
                <td class="recent-icon">
                    <i class="<%# Eval("Icon") %>"></i>
                </td>
                <td>
                    
                    <div>
                        <a href="/ui/admin/standards/edit?id=<%= Eval("StandardIdentifier") %>">
                            <asp:Literal runat="server" ID="AssetName" />
                        </a>
                    </div>

                    <div runat="server" ID="AssetTitle">
                        <%# Eval("Title") %>
                    </div>

                    <div class="form-text">
                        <%# GetTimestampHtml((Guid)Eval("StandardIdentifier")) %>
                    </div>

                </td>
            </tr>
        </table>

    </ItemTemplate>

</asp:Repeater>
