<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportStandardsType1.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisReportStandardsType1" %>

<div runat="server" id="OverlapOutputField" class="form-group mb-3">
    <label class="form-label">Competency Overlap</label>
    <div>
        <asp:Literal runat="server" ID="OverlapOutput" />
    </div>
</div>

<div runat="server" id="SharedField" class="form-group mb-3">
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

<div runat="server" id="MissingField" class="form-group mb-3">
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
