<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FieldAchievementSearchResults.ascx.cs" Inherits="InSite.Cmds.Controls.Training.Achievements.FieldAchievementSearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:BoundField HeaderText="Type" ItemStyle-Wrap="false" DataField="AchievementLabel" />

        <asp:HyperLinkField HeaderText="Title" DataTextField="AchievementTitle" DataNavigateUrlFields="AchievementIdentifier" DataNavigateUrlFormatString="/ui/cmds/design/achievements/edit?id={0}"/>
                
        <asp:BoundField HeaderText="# of Uploads" ItemStyle-Wrap="false" DataField="UploadCount" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" />

        <asp:TemplateField>
            <ItemTemplate>
                <insite:Icon runat="server" ID="TimeSensitiveIcon" Type="Regular" Name="history" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
