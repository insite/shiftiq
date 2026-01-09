<%@ Control AutoEventWireup="true" CodeBehind="TranslationField.ascx.cs" Inherits="InSite.Admin.Assets.Contents.Controls.TranslationField" Language="C#" %>
<%@ Register Src="TranslationWindow.ascx" TagName="TranslationWindow" TagPrefix="uc" %>

<uc:TranslationWindow runat="server" ID="TranslationWindow" />

<asp:Button runat="server" ID="UpdateButton" style="display:none;" />

<label class="form-label">
    <%= LabelText %>
    <insite:CustomValidator runat="server" ID="RequiredValidator" Enabled="false" />
    <insite:IconButton runat="server" ID="EditButton" Name="pencil" ToolTip="Edit" />
</label>
<div runat="server" id="ListPanel">
    <asp:Repeater runat="server" ID="Translations">
        <HeaderTemplate><table></HeaderTemplate>
        <FooterTemplate></table></FooterTemplate>
        <ItemTemplate>
            <tr>
                <td style='font-weight:bold; width:50px; padding:5px; vertical-align:top;'><%# Eval("LanguageCode") %></td>
                <td style='padding:5px;'><%# Eval("Html") %></td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</div>
<div class="form-text"><%= HelpText %></div>
