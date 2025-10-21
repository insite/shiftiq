<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentCompetencyField.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.Competencies.DepartmentCompetencyField" %>

<asp:CheckBox ID="Selected" runat="server" />
<span runat="server" id="SettingsButton" style="cursor:pointer;"><i class="fas fa-cog"></i></span>
<cmds:IconInternal runat="server" ID="AlarmIcon" IsFontIcon="true" CssClass="history" ToolTip="Time-Sensitive" />
<span class="text-danger"><cmds:IconInternal runat="server" ID="CriticalIcon" IsFontIcon="true" CssClass="flag-checkered" ToolTip="Critical" /></span>

<asp:HiddenField ID="SelectedOld" runat="server" />