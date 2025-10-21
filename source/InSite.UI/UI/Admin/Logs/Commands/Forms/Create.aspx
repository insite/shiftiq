<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Logs.Commands.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="CommandDetails" Src="../Controls/Details.ascx" %>
<%@ Register TagPrefix="uc" TagName="RecurrenceField" Src="../Controls/RecurrenceField.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Command" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-terminal"></i>
            Command
        </h2>

        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Settings</h3>

                        <div class="col-md-12">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Organization
                                    <insite:RequiredValidator runat="server" FieldName="Organization" ControlToValidate="OrganizationIdentifier" ValidationGroup="Command" />
                                </label>
                                <div>
                                    <insite:FindOrganization runat="server" ID="OrganizationIdentifier" Enabled="false" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    User
                                        <insite:RequiredValidator runat="server" FieldName="User" ControlToValidate="UserIdentifier" ValidationGroup="Command" />
                                </label>
                                <div>
                                    <insite:FindUser runat="server" ID="UserIdentifier" Enabled="false" />
                                </div>
                            </div>

                            <uc:CommandDetails runat="server" ID="CommandDetails" />

                        </div>

                    </div>

                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Time/Duration</h3>

                        <div class="col-md-12">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Action
                                        <insite:RequiredValidator runat="server" FieldName="Action" ControlToValidate="StatusComboBox" ValidationGroup="Command" />
                                </label>
                                <div>
                                    <insite:ComboBox runat="server" ID="StatusComboBox">
                                        <Items>
                                            <insite:ComboBoxOption />
                                            <insite:ComboBoxOption Text="Schedule" Value="Scheduled" />
                                            <insite:ComboBoxOption Text="Bookmark" Value="Bookmarked" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                            </div>

                            <div runat="server" id="BookmarkExpiredField" class="form-group mb-3" visible="false">
                                <label class="form-label">
                                    Expired On
                                        <insite:RequiredValidator runat="server" FieldName="Bookmark Expired On" ControlToValidate="BookmarkExpired" ValidationGroup="Command" />
                                </label>
                                <div>
                                    <insite:DateTimeOffsetSelector runat="server" ID="BookmarkExpired" />
                                </div>
                            </div>

                            <div runat="server" id="ScheduledDateField" class="form-group mb-3" visible="false">
                                <label class="form-label">
                                    Scheduled On
                                        <insite:RequiredValidator runat="server" FieldName="Scheduled On" ControlToValidate="ScheduledDate" ValidationGroup="Command" />
                                </label>
                                <div>
                                    <insite:DateTimeOffsetSelector runat="server" ID="ScheduledDate" />
                                </div>
                            </div>

                            <uc:RecurrenceField runat="server" ID="RecurrenceField" />

                        </div>

                    </div>

                </div>
            </div>

        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" ValidationGroup="Command" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
