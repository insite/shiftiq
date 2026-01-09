<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportJobFit1.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisReportJobFit1" %>

<div class="form-group mb-3">
    <label class="form-label">Competency Overlap</label>
    <div>
        <asp:Literal runat="server" ID="CompetencyOverlap" />
    </div>
</div>

<div runat="server" id="GacObtainField" class="form-group mb-3">
    <label class="form-label">Competencies Still to Obtain</label>
    <div>
        <asp:Repeater runat="server" ID="GacObtainRepeater">
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

<div runat="server" id="GacOverlapField" class="form-group mb-3">
    <label class="form-label">Competencies that Overlap</label>
    <div>
        <asp:Repeater runat="server" ID="GacOverlapRepeater">
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
