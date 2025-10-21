<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportStandardsType3.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisReportStandardsType3" %>

<div runat="server" id="MatchesField" class="mb-3">
    <h5 class="mt-2 mb-1">
        <insite:Literal runat="server" Text="Matches" />
    </h5>
    <div>
        <asp:Repeater runat="server" ID="StandardRepeater">
            <HeaderTemplate><ul></HeaderTemplate>
            <FooterTemplate></ul></FooterTemplate>
            <ItemTemplate>
                <li>
                    <i><%# Eval("Title") %></i>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>