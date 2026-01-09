<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ZoomAchievementGrid.ascx.cs" Inherits="InSite.Custom.CMDS.Reports.Controls.ZoomAchievementGrid" %>

<insite:Grid runat="server" ID="Grid" AllowPaging="false">
    <Columns>

        <asp:TemplateField HeaderText="Statistic">
            <ItemTemplate>
                <%# Eval("AchievementIdentifier") != null ? string.Format("<a href='/ui/cmds/admin/achievements/edit?id={0}'>{1}</a>", Eval("AchievementIdentifier"), Eval("AchievementTitle")) : (string)Eval("AchievementTitle") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="CP" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrIcon(Eval("CountCP"), Eval("AchievementIdentifier") == null) %> 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="EX" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrIcon(Eval("CountEX"), Eval("AchievementIdentifier") == null) %> 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="NC" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrIcon(Eval("CountNC"), Eval("AchievementIdentifier") == null) %> 
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="NA" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrEmpty(Eval("CountNA")) %> 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="NT" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrEmpty(Eval("CountNT")) %> 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="SA" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrEmpty(Eval("CountSA")) %> 
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="SV" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrEmpty(Eval("CountSV")) %> 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="VA" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrEmpty(Eval("CountVA")) %> 
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="RQ" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <%# NumberOrEmpty(Eval("CountRQ")) %> 
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
