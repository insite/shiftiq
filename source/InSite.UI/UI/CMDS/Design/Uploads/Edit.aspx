<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Cmds.Design.Uploads.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementListEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">
    <div runat="server" id="GlobalAchievementPanel" class="alert alert-info alert-dismissible" role="alert">
        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>

        This file/link is related to global achievement and therefore cannot be edited.
        <br />
        You only can add competetencies and organization-specific achievements.
    </div>

    <insite:Alert runat="server" ID="EditorStatus" />

    <asp:ValidationSummary runat="server" ValidationGroup="CompanyDownload" />

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="FileSection" Title="Uploaded Achievement" Icon="far fa-paperclip" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Uploaded Achievement
                </h2>

                <div class="row">

                    <div class="col-md-6">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div runat="server" ID="FileRow" class="form-group mb-3">
                                    <label class="form-label">
                                        File Name
                                        <insite:RequiredValidator runat="server" ControlToValidate="FileName" FieldName="File Name" ValidationGroup="CompanyDownload" />
                                        <insite:PatternValidator runat="server" ControlToValidate="FileName" FieldName="File Name" ValidationGroup="CompanyDownload" ValidationExpression="[a-zA-Z0-9_\- ]*" />
                                    </label>
                                    <div>
                                        <insite:TextBox ID="FileName" runat="server" MaxLength="256" CssClass="w-75 d-inline"/>
                                        <asp:Literal ID="FileExtension" runat="server" />
                                        <insite:IconLink runat="server" ID="FileLink" Target="_blank" Type="Regular" Name="download" ToolTip="Download File" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                                <div runat="server" ID="UrlRow" class="form-group mb-3">
                                    <label class="form-label">
                                        URL
                                        <insite:RequiredValidator runat="server" ControlToValidate="NavigationUrl" FieldName="URL" ValidationGroup="CompanyDownload" />
                                        <insite:UrlValidator runat="server" ControlToValidate="NavigationUrl" ErrorMessage="Specified URL is not valid" ValidationGroup="CompanyDownload" />
                                    </label>
                                    <div>
                                        <insite:TextBox ID="NavigationUrl" runat="server" MaxLength="500" Width="500" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Title
                                        <insite:RequiredValidator runat="server" ControlToValidate="TitleInput" FieldName="Title" ValidationGroup="CompanyDownload" />
                                    </label>
                                    <div>
                                        <insite:TextBox ID="TitleInput" runat="server" MaxLength="256" CssClass="w-75" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                                <div runat="server" ID="SizeInKiloBytesRow" class="form-group mb-3">
                                    <label class="form-label">
                                        Size
                                    </label>
                                    <div>
                                        <asp:Literal ID="SizeInKiloBytes" runat="server" /> KB
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Last Updated
                                    </label>
                                    <div>
                                        <asp:Literal ID="LastUpdatedOn" runat="server" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>
                            </div>
                        </div>
                    </div>
            
                    <div class="col-md-6">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Description
                                    </label>
                                    <div>
                                        <insite:TextBox ID="Description" runat="server" TextMode="MultiLine" Rows="5" MaxLength="300"/>
                                    </div>
                                    <div class="form-text"></div>
                                </div>

                            </div>
                        </div>
                    </div>

                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CompetencySection" Title="Competencies" Icon="far fa-ruler-triangle" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">
                Competencies
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CompetenciesUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="CompetenciesUpdatePanel">
                        <ContentTemplate>
                            <insite:Nav runat="server">
                                <insite:NavItem runat="server" ID="CompetencyTab" Title="Competencies">
                                    <div runat="server" id="CompetencyPanel">
                                        <table class="check-list">
                                        <asp:Repeater ID="Competencies" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                <td>
                                                    <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                                    <asp:CheckBox runat="server" ID="Competency" />
                                                </td>
                                                <td>
                                                    <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                                                </td>
                                                <td><asp:Label runat="server" AssociatedControlID="Competency" Text='<%# Eval("Summary") %>' /></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        </table>
                                    </div>

                                    <div ID="CompetencyButtons" runat="server">
                                        <cmds:CmdsButton ID="SelectAllButton" runat="server" Text="<i class='far fa-check-square me-1'></i> Select All" />
                                        <cmds:CmdsButton ID="UnselectAllButton" runat="server" Text="<i class='far fa-square me-1'></i> Deselect All" />
                                        <insite:DeleteButton ID="DeleteCompetencyButton" runat="server" />
                                    </div>
                                </insite:NavItem>

                                <insite:NavItem runat="server" Title="Add Competencies">
                                    <p>Search for the individual competencies you want to add to this download. Check the box next to each one and click the Add button.</p>

                                    <div class="mt-2 mb-3">
                                        <insite:TextBox ID="SearchText" runat="server" EmptyMessage="Competency Number or Summary" CssClass="w-50" />
                                    </div>
                                    <div class="mb-3">
                                        <insite:FilterButton ID="FilterButton" runat="server" ButtonStyle="OutlinePrimary" DisableAfterClick="true" />
                                        <insite:ClearButton ID="ClearButton" runat="server" ButtonStyle="OutlinePrimary" DisableAfterClick="true" />

                                        <insite:PageFooterContent runat="server">
                                            <script type="text/javascript"> 
                                                (function () {
                                                    Sys.Application.add_load(function () {
                                                        $('#<%= SearchText.ClientID %>')
                                                            .off('keydown', onKeyDown)
                                                            .on('keydown', onKeyDown);
                                                    });

                                                    function onKeyDown(e) {
                                                        if (e.which === 13) {
                                                            e.preventDefault();
                                                            $('#<%= FilterButton.ClientID %>')[0].click();
                                                        }
                                                    }
                                                })();
                                            </script>
                                        </insite:PageFooterContent>
                                    </div>

                                    <p id="FoundCompetency" runat="server" visible="false"></p>

                                    <div id="CompetencyList" runat="server" visible="false">
                                        <asp:Repeater ID="NewCompetencies" runat="server">
                                            <HeaderTemplate>
                                                <table class="check-list">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                <td>
                                                    <asp:Literal ID="CompetencyStandardIdentifier" runat="server" Text='<%# Eval("CompetencyStandardIdentifier") %>' Visible="false" />
                                                    <asp:CheckBox ID="Competency" runat="server" />
                                                </td>
                                                <td>
                                                    <a href='<%# "/ui/cmds/admin/standards/competencies/edit?id=" + Eval("CompetencyStandardIdentifier") %>'><%# Eval("Number") %></a>
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" AssociatedControlID="Competency" Text='<%# Eval("Summary") %>' />
                                                </td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>

                                        <br />

                                        <div>
                                            <insite:Button ID="SelectAllButton2" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                                            <insite:Button ID="UnselectAllButton2" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                                            <insite:Button ID="AddCompetencyButton" runat="server" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Success" DisableAfterClick="true" />
                                        </div>
                                    </div>
                                </insite:NavItem>

                                <insite:NavItem runat="server" Title="Add Multiple Competencies">
                                    <p>Enter the list of competency numbers you want to add to this download then click the Add button.</p>

                                    <div class="mb-3">
                                        <insite:TextBox runat="server" ID="MultipleCompetencyNumbers" TextMode="MultiLine" Height="100px" CssClass="w-75" />
                                    </div>

                                    <insite:Button ID="AddMultipleButton" runat="server" Icon="fas fa-plus-circle" Text="Add" ButtonStyle="Success" DisableAfterClick="true" />
                                </insite:NavItem>

                            </insite:Nav>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>
        </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="AchievementSection" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Achievements <asp:Literal runat="server" ID="AchievementCount" />
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AchievementsUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="AchievementsUpdatePanel">
                        <ContentTemplate>
                            <uc:AchievementListEditor ID="AchievementEditor" runat="server" />
                        </ContentTemplate>
                    </insite:UpdatePanel>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CompanyDownload" />
    <insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete this Organization Download?" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</div>

<insite:PageHeadContent runat="server">
    <style>
        table.check-list > tbody > tr > td input[type="checkbox"] {
            margin-right: 8px;
        }

        table.check-list > tbody > tr > td label {
            margin-bottom: 0px;
            margin-left: 5px;
        }

        table.check-list > tbody > tr > td {
            padding-top: 3px;
            padding-bottom: 3px;
        }
    </style>
</insite:PageHeadContent>
</asp:Content>
