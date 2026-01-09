<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportResults.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Reports.Controls.ExecutiveSummaryOnCompetencyStatusResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField HeaderText="AsAt">
            <ItemTemplate>
                <%# Shift.Common.TimeZones.Format((DateTimeOffset)Eval("AsAt"), User.TimeZone, true) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Department">
            <ItemTemplate>
                <%# Eval("DepartmentName") %>
                <span class="form-text"><%# Eval("DivisionName") %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Criticality">
            <ItemTemplate>
                <%# Eval("TagCriticality") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="CP">
            <ItemTemplate>
                <%# Eval("SumCP") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="EX">
            <ItemTemplate>
                <%# Eval("SumEX") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="NC">
            <ItemTemplate>
                <%# Eval("SumNC") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="NA">
            <ItemTemplate>
                <%# Eval("SumNA") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="NT">
            <ItemTemplate>
                <%# Eval("SumNT") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="SA">
            <ItemTemplate>
                <%# Eval("SumSA") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="SV">
            <ItemTemplate>
                <%# Eval("SumSV") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="VA">
            <ItemTemplate>
                <%# Eval("SumVA") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="VN">
            <ItemTemplate>
                <%# Eval("SumVN") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="RQ">
            <ItemTemplate>
                <%# Eval("SumRQ") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Score">
            <ItemTemplate>
                <%# GetComplianceScore((decimal?)Eval("AvgScore")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Progress">
            <ItemTemplate>
                <%# GetComplianceScore((decimal?)Eval("AvgProgress")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
