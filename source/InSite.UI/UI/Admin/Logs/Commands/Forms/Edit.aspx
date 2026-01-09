<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Logs.Commands.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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

                                <uc:CommandDetails runat="server" ID="CommandDetails" IsEditor="true" />

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
                                    <label class="form-label">Status</label>
                                    <div class="readonly-value">
                                        <asp:Literal runat="server" ID="SendStatus" /></div>
                                </div>

                                <insite:Container runat="server" ID="SendContainer">
                                    <div runat="server" id="SendErrorField" class="form-group mb-3" visible="false">
                                        <label class="form-label">Error</label>
                                        <div class="readonly-value">
                                            <asp:Literal runat="server" ID="SendError" /></div>
                                    </div>

                                    <div runat="server" id="SendStartedField" class="form-group mb-3" visible="false">
                                        <label class="form-label">Started On</label>
                                        <div class="readonly-value">
                                            <asp:Literal runat="server" ID="SendStarted" /></div>
                                    </div>

                                    <div runat="server" id="SendCompletedField" class="form-group mb-3" visible="false">
                                        <label class="form-label">Completed On</label>
                                        <div class="readonly-value">
                                            <asp:Literal runat="server" ID="SendCompleted" /></div>
                                    </div>

                                    <div runat="server" id="SendCancelledField" class="form-group mb-3" visible="false">
                                        <label class="form-label">Cancelled On</label>
                                        <div class="readonly-value">
                                            <asp:Literal runat="server" ID="SendCancelled" /></div>
                                    </div>
                                </insite:Container>

                                <insite:Container runat="server" ID="BookmarkContainer">
                                    <div class="form-group mb-3">
                                        <label class="form-label">Bookmarked</label>
                                        <div class="readonly-value">
                                            <asp:Literal runat="server" ID="BookmarkAdded" /></div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Starts (or Expires)
                                            <insite:RequiredValidator runat="server" FieldName="Bookmark Expired On" ControlToValidate="BookmarkExpired" ValidationGroup="Command" />
                                        </label>
                                        <div>
                                            <insite:DateTimeOffsetSelector runat="server" ID="BookmarkExpired" />
                                        </div>
                                    </div>
                                </insite:Container>

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
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" ValidationGroup="Command" />
            <insite:Button runat="server" ID="CopyButton" Text="Duplicate" Icon="fas fa-copy" ButtonStyle="Warning" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>


    <insite:PageHeadContent runat="server">
        <style type="text/css">
            .readonly-value {
                line-height: 33px;
            }
        </style>
    </insite:PageHeadContent>
</asp:Content>
