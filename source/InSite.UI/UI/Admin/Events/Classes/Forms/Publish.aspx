<%@ Page Language="C#" CodeBehind="Publish.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.PublishForm" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ClassSummaryInfo.ascx" TagName="SummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="../Controls/ClassLocationInfo.ascx" TagName="LocationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Publish" />

    <section class="mb-3">
        <div class="row">
            <div class="col-md-6 mb-3 mb-md-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Summary
                        </h4>

                        <uc:SummaryInfo runat="server" ID="SummaryInfo" />

                        <div runat="server" id="RegistrationStartField" class="form-group mb-3">
                            <label class="form-label">
                                Registration Start
                                <asp:CustomValidator runat="server" ID="RegistrationDateValidator" ValidationGroup="Publish" ErrorMessage="Registration start date must be less than deadline" Display="None" />
                            </label>
                            <insite:DateTimeOffsetSelector runat="server" ID="RegistrationStart" Width="365px" />
                            <div class="form-text">
                                The date and time when registration for this event is open, after which new registrations are permitted.
                            </div>
                        </div>

                        <div runat="server" id="RegistrationDeadlineField" class="form-group mb-3">
                            <label class="form-label">
                                Registration Deadline
                            </label>
                            <insite:DateTimeOffsetSelector runat="server" ID="RegistrationDeadline" Width="365px" />
                            <div class="form-text">
                                The date and time when registration for this event is closed, after which no new registrations are permitted.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Calendar Color</label>
                            <div>
                                <insite:ColorComboBox runat="server" ID="ClassCalendarColor" CssClass="w-50" AllowBlank="false" Value="Primary"/>
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            <div class="col-md-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Location and Schedule
                        </h4>

                        <uc:LocationInfo runat="server" ID="LocationInfo" />

                    </div>
                </div>

            </div>
        </div>
    </section>

    <section>
        <insite:Button runat="server" ID="PublishButton" Text="Publish" Icon="fas fa-cloud-upload" ButtonStyle="Success" ValidationGroup="Publish" CausesValidation="true" />
        <insite:Button runat="server" ID="UnpublishButton" Text="Unpublish" Icon="fas fa-eraser" ButtonStyle="Danger" ValidationGroup="Publish" CausesValidation="true" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>
</asp:Content>