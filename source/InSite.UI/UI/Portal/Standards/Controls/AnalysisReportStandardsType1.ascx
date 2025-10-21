<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportStandardsType1.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.AnalysisReportStandardsType1" %>

<div runat="server" id="OverlapOutputField" class="mb-3">
    <h5 class="mt-2 mb-1">
        <insite:Literal runat="server" Text="Competency Overlap" />
    </h5>
    <div>
        <asp:Literal runat="server" ID="OverlapOutput" />
    </div>
</div>

<div runat="server" id="SharedField" class="mb-3">
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

<div runat="server" id="MissingField" class="mb-3">
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