<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Contacts.People.Outline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="PersonDetail" Src="./Controls/PersonDetail.ascx" %> 
<%@ Register TagPrefix="uc" TagName="PersonAchievements" Src="./Controls/PersonAchievements.ascx" %> 
<%@ Register TagPrefix="uc" TagName="PersonRecords" Src="./Controls/PersonRecords.ascx" %>
<%@ Register TagPrefix="uc" TagName="PersonRegistrations" Src="./Controls/PersonRegistrations.ascx" %> 

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="DetailPanel" Title="Person" Icon="far fa-user" IconPosition="BeforeText">
            <uc:PersonDetail runat="server" ID="Detail" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="AchievementSection" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">
            <uc:PersonAchievements runat="server" ID="Achievements" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="RegistrationSection" Title="Registrations" Icon="far fa-id-card" IconPosition="BeforeText">
            <uc:PersonRegistrations runat="server" ID="Registrations" />
        </insite:NavItem>
        <insite:NavItem runat="server" ID="RecordSection" Title="Records" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="GradebookUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="GradebookUpdatePanel">
                <ContentTemplate>
                    <uc:PersonRecords runat="server" ID="Records" />
                </ContentTemplate>
            </insite:UpdatePanel>
        </insite:NavItem>
    </insite:Nav>

</asp:Content>