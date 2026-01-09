<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportCareerMap.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisReportCareerMap" %>

<div runat="server" id="OverlapField" class="mb-3">
    <h5 class="mt-2 mb-1">
        <insite:Literal runat="server" Text="Competency Overlap" />
    </h5>
    <div>
        <asp:Literal runat="server" ID="CompetencyOverlap" />
    </div>
</div>

<div runat="server" id="SharedGacRepeaterField" class="mb-3">
    <h5 class="mt-2 mb-1">
        <insite:Literal runat="server" Text="Shared Competencies" />
    </h5>
    <div>
        <asp:Repeater runat="server" ID="SharedGacRepeater">
            <HeaderTemplate><ul></HeaderTemplate>
            <FooterTemplate></ul></FooterTemplate>
            <ItemTemplate>
                <li>
                    <i><%# Eval("Title") %></i>
                    <asp:Repeater runat="server" ID="CompetencyRepeater">
                        <HeaderTemplate><ul></HeaderTemplate>
                        <FooterTemplate></ul></FooterTemplate>
                        <ItemTemplate>
                            <li><%# Eval("Title") %></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

<div runat="server" id="MissingGacRepeaterField" class="mb-3">
    <h5 class="mt-2 mb-1">
        <insite:Literal runat="server" Text="Missing Competencies" />
    </h5>
    <div>
        <asp:Repeater runat="server" ID="MissingGacRepeater">
            <HeaderTemplate><ul></HeaderTemplate>
            <FooterTemplate></ul></FooterTemplate>
            <ItemTemplate>
                <li>
                    <i><%# Eval("Title") %></i>
                    <asp:Repeater runat="server" ID="CompetencyRepeater">
                        <HeaderTemplate><ul></HeaderTemplate>
                        <FooterTemplate></ul></FooterTemplate>
                        <ItemTemplate>
                            <li><%# Eval("Title") %></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

<div class="mb-3 mt-3" style="text-align:right;">
    <insite:Button runat="server" ID="DownloadButton" ButtonStyle="Primary" Text="Download (Word *.docx)" Icon="far fa-download" />
</div>