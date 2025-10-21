<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportJobFit2.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisReportJobFit2" %>

<asp:Repeater runat="server" ID="NosRepeater">
    <ItemTemplate>
        <h4><%# Eval("Title") %></h4>

        <div class="mb-3">
            <h5 class="mt-2 mb-1">
                <insite:Literal runat="server" Text="Competency Overlap" />
            </h5>
            <div>
                <%# Eval("OverlapValue", "{0:p0}") %>
                <asp:Literal runat="server" ID="CompetencyOverlapOutput" />
            </div>
        </div>
                    
        <div runat="server" id="ObtainField" class="mb-3">
            <h5 class="mt-2 mb-1">
                <insite:Literal runat="server" Text="Competencies Still to Obtain" />
            </h5>
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

<div class="mb-3 mt-3" style="text-align:right;">
    <insite:Button runat="server" ID="DownloadButton" ButtonStyle="Primary" Text="Download (Word *.docx)" Icon="far fa-download" />
</div>