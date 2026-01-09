<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportCareerMap.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisReportCareerMap" %>

<div runat="server" id="OverlapField" class="form-group mb-3">
    <label class="form-label">Competency Overlap</label>
    <div>
        <asp:Literal runat="server" ID="CompetencyOverlap" />
    </div>
</div>

<div runat="server" id="SharedGacRepeaterField" class="form-group mb-3">
    <label class="form-label">Shared Competencies</label>
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

<div runat="server" id="MissingGacRepeaterField" class="form-group mb-3">
    <label class="form-label">Missing Competencies</label>
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

<div style="text-align:right; margin-top:15px;">
    <insite:Button runat="server" ID="DownloadButton" ButtonStyle="Primary" Text="Download (Word *.docx)" Icon="far fa-download" />
</div>
