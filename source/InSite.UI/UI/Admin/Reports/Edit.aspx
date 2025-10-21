<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Admin.Reports.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="PrivacyGroupManager" Src="~/UI/Admin/Reports/Controls/PrivacyGroupManager.ascx" %>
<%@ Register TagPrefix="uc" TagName="JsonEditor" Src="~/UI/Admin/Reports/Controls/JsonEditor.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Report" />

    <section class="mb-4">

        <div class="mb-3">
            <insite:Container runat="server" ID="CustomReportButtons" Visible="false">
                <insite:Button runat="server" ButtonStyle="Default" ID="DuplicateLink" Icon="fas fa-copy" Text="Duplicate" />
                <insite:Button runat="server" ButtonStyle="Primary" ID="DownloadLink" Icon="fas fa-download" Text="Download JSON" />
            </insite:Container>
            <insite:Button runat="server" ID="ExecuteButton" Text="Run Report" ButtonStyle="Primary" Icon="fas fa-bolt" CausesValidation="false" />
            <insite:Button runat="server" ID="BuildButton" Text="Build Report" ButtonStyle="Primary" Icon="fas fa-cog" CausesValidation="false" />
        </div>

		<div class="row">
			<div class="col-xl-6">

                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="Report" />
                        </h3>

                        <div class="row">
                            <div class="col-md-12">
                                <div class="settings">
                                    <div class="form-group mb-3">
                                        <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ReportTitle">Report Title</asp:Label>
                                        <insite:RequiredValidator runat="server" ControlToValidate="ReportTitle" Display="None" ValidationGroup="Report" />
                                        <div>
                                            <insite:TextBox runat="server" ID="ReportTitle" MaxLength="100" />
                                        </div>
                                    </div>

                                    <div runat="server" id="ReportTypeField" class="form-group mb-3">
                                        <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ReportType">Report Type</asp:Label>
                                        <insite:RequiredValidator runat="server" ControlToValidate="ReportType" Display="None" ValidationGroup="Report" />
                                        <div>
                                            <insite:ComboBox runat="server" ID="ReportType">
                                                <Items>
                                                    <insite:ComboBoxOption />
                                                    <insite:ComboBoxOption Value="Custom" Text="Custom Report" />
                                                    <insite:ComboBoxOption Value="Shared" Text="Shared Report" />
                                                </Items>
                                            </insite:ComboBox>
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <asp:Label runat="server" CssClass="form-label" AssociatedControlID="ReportDescription">Report Description</asp:Label>
                                        <div>
                                            <insite:TextBox runat="server" ID="ReportDescription" TextMode="MultiLine" Rows="6" MaxLength="300" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

            </div>

			<div runat="server" id="GroupsField" class="col-xl-6 mt-3 mt-xl-0">
                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="Groups" />
                        </h3>
                        <uc:PrivacyGroupManager runat="server" ID="PrivacyGroups" />
                    </div>
                </div>

            </div>
        </div>
    </section>

    <section runat="server" id="JsonSection" class="mb-3">
        <div class="row">
			<div class="col-xl-12">

                <div class="card border-0 shadow-lg">
			        <div class="card-body">
                        <h3>
                            <insite:Literal runat="server" Text="JSON" />
                        </h3>

                        <div class="row">
                            <div class="col-md-12">
                                <uc:JsonEditor runat="server" ID="ReportData" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Report" />
        <insite:CancelButton runat="server" ID="CancelButton" />
        <insite:CloseButton runat="server" ID="CloseButton" />
    </section>
</asp:Content>
