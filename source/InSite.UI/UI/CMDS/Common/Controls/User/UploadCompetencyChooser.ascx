<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadCompetencyChooser.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.Files.UploadCompetencyChooser" %>

<style type="text/css">
    table.competency-chooser-list td { padding:0 !important; }
</style>

<asp:Label ID="AttributeList" runat="server" />

<div class="mb-3">
    <cmds:FindCompetency ID="CompetencyStandardIdentifier" runat="server" CssClass="w-75 align-middle" />
    <cmds:IconButton ID="AddButton" runat="server" IsFontIcon="true" CssClass="plus-circle" ToolTip="Add Competency" ImageAlign="AbsMiddle" />
</div>

<span class="form-text">
    (Click the Add button to assign the achievement file to multiple competencies.)
</span>

<asp:Repeater ID="CompetencyList" runat="server">
    <HeaderTemplate><table cellpadding="0" cellspacing="0" class="competency-chooser-list"></HeaderTemplate>
    <FooterTemplate></table></FooterTemplate>
    <ItemTemplate>
        <tr>
            <td style="width:80px;">
                <%# Eval("CompetencyNumber") %>
            </td>
            <td style="width:18px;">
                <cmds:IconButton runat="server" IsFontIcon="true" CssClass="trash-alt" ToolTip="Delete Competency" CommandArgument='<%# Eval("CompetencyStandardIdentifier") %>' CommandName="DeleteCompetency" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<script type="text/javascript">
    function disableCompetencyChooser(parent)
    {
        var attributeList = getAttributeList(parent);

        document.getElementById(attributeList.getAttribute("CompetencyStandardIdentifier")).disable();
        $get(attributeList.getAttribute("AddButton")).style.display = "none";
    }

    function enableCompetencyChooser(parent)
    {
        var attributeList = getAttributeList(parent);

        document.getElementById(attributeList.getAttribute("CompetencyStandardIdentifier")).enable();
        $get(attributeList.getAttribute("AddButton")).style.display = "";
    }

    function getAttributeList(parent)
    {
        var spans = parent.getElementsByTagName("span");

        for (var i = 0; i < spans.length; i++)
        {
            if (spans[i].getAttribute("IsAttributeList") == "true")
                return spans[i];
        }

        return null;
    }
</script>