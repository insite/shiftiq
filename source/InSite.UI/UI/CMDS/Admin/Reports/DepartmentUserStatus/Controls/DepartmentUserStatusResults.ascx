<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentUserStatusResults.ascx.cs" Inherits="InSite.Custom.CMDS.Reports.Controls.DepartmentUserStatusResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="As At">
            <ItemTemplate>
                <%# Shift.Common.TimeZones.Format((DateTimeOffset)Eval("AsAt"), User.TimeZone, true) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Department" DataField="DepartmentName" />
        <asp:BoundField HeaderText="User" DataField="UserName" />

        <asp:TemplateField HeaderText="Statistic">
            <ItemTemplate>
                <%# Eval("DisplayItemName") %>
                <span class="form-text">(<%# Eval("ListDomain") %>)</span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="CP" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Completed">CP</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountCP") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="EX" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Expired">EX</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountEX") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="NC" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Not Completed">NC</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountNC") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="NA" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Not Applicable">NA</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountNA") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="NT" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Needs Training">NT</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountNT") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="SA" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Self-Assessed">SA</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountSA") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="SV" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Submitted for Validation">SV</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountSV") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="VA" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Validated">VA</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountVA") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="VN" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Validated as Not Applicable">VN</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountVN") %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="RQ" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <HeaderTemplate>
                <span title="Required">RQ</span>
            </HeaderTemplate>
            <ItemTemplate>
                <%# Eval("CountRQ") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Score" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetComplianceScore((decimal?)Eval("Score")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Progress" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetComplianceScore((decimal?)Eval("Progress")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <insite:IconButton runat="server" ID="ZoomIcon"
                    Name="search-plus"
                    CommandName="Zoom"
                    CommandArgument='<%# string.Join(",", Eval("DepartmentIdentifier"), Eval("UserIdentifier"), Eval("ItemName"), Eval("OrganizationIdentifier")) %>'
                    OnCommand="ZoomIcon_Command" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
