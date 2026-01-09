<%@ Control Language="C#" CodeBehind="CmdsLearningSummary.ascx.cs" 
    Inherits="InSite.UI.Portal.Home.Controls.CmdsLearningSummary" %>

<div runat="server" ID="SummaryTableWrapper" class="training-panel mb-3">
    
    <h3>Learning Summary</h3>

    <asp:Repeater runat="server" ID="Repeater">
        <HeaderTemplate>
            <table class="table table-striped my-summary">
                <tbody>
        </HeaderTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td class="my-summary-flag">
                    <cmds:Flag runat="server" Type='<%# Eval("FlagType") %>' ToolTip='<%# Eval("FlagTooltip") %>' Visible='<%# Eval("HasFlagType") %>' />
                </td>
                <td class="my-summary-text"><%# Eval("Title") %></td>
                <td class="my-summary-score"><%# Eval("ProgressPercent") %></td>
                <td class="my-summary-progress">
                    <asp:HyperLink runat="server" NavigateUrl='<%# Eval("ProgressUrl") %>' Text='<%# Eval("ProgressText") %>' />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>

</div>