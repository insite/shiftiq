<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportJobFit2.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisReportJobFit2" %>

<asp:Repeater runat="server" ID="NosRepeater">
    <ItemTemplate>
        <h4><%# Eval("Title") %></h4>

        <div class="form-group mb-3">
            <label class="form-label">Competency Overlap</label>
            <div>
                <%# Eval("OverlapValue", "{0:p0}") %>
                <asp:Literal runat="server" ID="CompetencyOverlapOutput" />
            </div>
        </div>
                    
        <div runat="server" id="ObtainField" class="form-group mb-3">
            <label class="form-label">Competencies Still to Obtain</label>
            <div>
                <asp:Repeater runat="server" ID="GacRepeater">
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
    </ItemTemplate>
</asp:Repeater>

<div style="text-align:right; margin-top:15px;">
    <insite:Button runat="server" ID="DownloadButton" ButtonStyle="Primary" Text="Download (Word *.docx)" Icon="far fa-download" />
</div>
