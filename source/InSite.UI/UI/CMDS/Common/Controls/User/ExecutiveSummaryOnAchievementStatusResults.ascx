<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExecutiveSummaryOnAchievementStatusResults.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Reports.Controls.ExecutiveSummaryOnAchievementStatusResults" %>

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
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Achievement Type">
            <ItemTemplate>
                <%# Eval("AchievementType") %>
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

        <asp:TemplateField HeaderText="Score" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetComplianceScore((decimal?)Eval("AvgScore")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Progress" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetComplianceScore((decimal?)Eval("AvgProgress")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
