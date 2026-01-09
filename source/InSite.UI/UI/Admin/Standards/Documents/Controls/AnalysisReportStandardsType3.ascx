<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnalysisReportStandardsType3.ascx.cs" Inherits="InSite.Admin.Standards.Documents.Controls.AnalysisReportStandardsType3" %>

<div runat="server" id="MatchesField" class="form-group mb-3">
    <label class="form-label">Matches</label>
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
