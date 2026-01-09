<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencySummary.ascx.cs" Inherits="InSite.UI.CMDS.Common.Controls.User.CompetencySummary" %>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table class="table table-condensed">
            <thead>
                <tr>
                    <th style="width:200px;">Status</th>
                    <th># of Competencies</th>
                    <th style="width:65px;"></th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody>
    </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td>
                <a runat="server" href='<%# GetSelfAssessmentFinderUrl(Eval("Status")) %>'>
                    <%# Eval("Status") %>
                </a>
            </td>
            <td>
                <div class="progress" style="height:22px;">
                    <div class="progress-bar" role="progressbar" aria-valuemin="0" aria-valuemax="100"
                        style='<%# string.Format("width:{0}%; background-color:{1};", Eval("Width"), GetBarColor(Eval("Status"))) %>' 
                        aria-valuenow="<%# Eval("Width") %>"></div>
                </div>
            </td>
            <td class="text-end">
                <%# Eval("Value") %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
