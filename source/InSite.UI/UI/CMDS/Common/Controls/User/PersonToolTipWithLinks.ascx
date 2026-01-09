<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonToolTipWithLinks.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Persons.PersonToolTipWithLinks" %>

<table style="font-size:small;">
    <tr>
        <td class="p-1 pe-3"><a href='<%= "/ui/cmds/portal/validations/profiles/search?userID=" + UserIdentifier.ToString() %>'><i class="far fa-id-badge me-1" title="Profiles"></i>Profiles</a></td>
        <td class="p-1"><a href='<%= "/ui/cmds/portal/achievements/credentials/search?userID=" + UserIdentifier.ToString() %>'><i class="far fa-award me-1" title="Training and Education"></i>Education</a></td>
    </tr>
    <tr>
        <td class="p-1 pe-3"><a href='<%= "/ui/cmds/portal/validations/competencies/search?userID=" + UserIdentifier.ToString() %>'><i class="far fa-ruler-triangle me-1" title="Competencies"></i>Competencies</a></td>
        <td class="p-1"><a href='<%= "/ui/portal/learning/plan?userID=" + UserIdentifier.ToString() %>'><i class="far fa-map-marked-alt me-1" title="Training Plan"></i>Training Plan</a></td>
    </tr>
</table>