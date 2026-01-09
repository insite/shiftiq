<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Events.Appointments.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>
<%@ Register Src="../../Comments/Controls/CommentPanel.ascx" TagName="CommentPanel" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Setup</h2>

        <div class="row">
            <div class="col-lg-6">

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <div class="float-start">
                            <h5 class="card-title">Schedule</h5>
                        </div>
                        <div class="float-end">
                            <insite:Button runat="server" ID="PublishButton" Text="Publish" Icon="fas fa-upload" ButtonStyle="Default" CssClass="mb-3"  ConfirmText="Are you sure to publish this appointment?"/>
                            <insite:Button runat="server" ID="UnpublishButton" Text="Unpublish" Icon="fas fa-eraser" ButtonStyle="Default" CssClass="mb-3" ConfirmText="Are you sure to unpublish this appointment?"/>
                            <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon="fas fa-history" ButtonStyle="Default" CssClass="mb-3" />
                        </div>
                        <div class="clearfix"></div>
                        <dl>

                            <dt>
                                <a runat="server" id="EventTitleLink" href="#" title="Rename appointment" class="float-end"><i class="fas fa-pencil ms-1"></i></a>
                                <span title="Descriptive title for this appointment.">Title</span>
                            </dt>
                            <dd>
                                <asp:Literal runat="server" ID="EventTitle" />
                            </dd>

                            <dt>
                                <a runat="server" id="AppointmentTypeLink" href="#" title="Change appointment type" class="float-end"><i class="fas fa-pencil ms-1"></i></a>
                                <span>Appointment Type</span>
                            </dt>
                            <dd>
                                <asp:Literal runat="server" ID="AppointmentType" />
                            </dd>

                            <dt>
                                <a runat="server" id="AppointmentCalendarColorLink" href="#" title="Change appointment calendar color" class="float-end"><i class="fas fa-pencil ms-1"></i></a>
                                <span>Calendar Color</span>
                            </dt>
                            <dd>
                                <asp:Literal runat="server" ID="AppointmentCalendarColorBox" />
                                <asp:Literal runat="server" ID="AppointmentCalendarColorName" />
                            </dd>


                            <dt>
                                <a runat="server" id="ChangeEventScheduledStart" title="Change start time" class="float-end"><i class="fas fa-pencil ms-1"></i></a>
                                <span title="The start date and time for this appointment event.">Start Time</span>
                            </dt>
                            <dd>
                                <asp:Literal runat="server" ID="EventScheduledStart" />
                            </dd>

                            <dt>
                                <a runat="server" id="ChangeEventScheduledEnd" title="Change end time" class="float-end"><i class="fas fa-pencil ms-1"></i></a>
                                <span title="The end date and time for this appointment event.">End Time</span>
                            </dt>
                            <dd>
                                <asp:Literal runat="server" ID="EventScheduledEnd" />
                            </dd>

                            <dt>
                                Status
                            </dt>
                            <dd>
                                <asp:Panel runat="server" ID="FormPublicationPanel" CssClass="float-end">
                                    <asp:Label runat="server" ID="FormPublicationStatus" />
                                </asp:Panel>
                                <asp:Literal runat="server" ID="FormPublicationStatusText" />
                            </dd>

                            <dt>
                                Event Description
                            </dt>
                            <dd>
                                <asp:Literal runat="server" ID="EventDescription" />
                            </dd>

                            <dt>
                                <span title="A globally unique identifier for this appointment.">Event Identifier</span>
                            </dt>
                            <dd>
                                <asp:Literal runat="server" ID="EventThumbprint" />
                            </dd>

                        </dl>

                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Content</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div style="position: absolute; right: 15px; z-index: 1;">
                    <div>
                        <insite:Button runat="server" ID="PreviewLink" Text="Preview" Icon="fas fa-external-link" ButtonStyle="OutlinePrimary" NavigateTarget="_blank" />
                    </div>
                </div>
                <insite:Nav runat="server" ID="ContentNavigation" List-CssClass="mb-0">

                    <insite:NavItem runat="server" ID="TitleTab" Title="Title">
                        <div class="float-end">
                            <insite:Button runat="server" ID="EditContentTitleLink" ToolTip="Revise Title" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                        </div>
                        <div class="content-string">
                            <uc:MultilingualStringInfo runat="server" ID="ContentTitle" />
                        </div>
                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="DescriptionTab" Title="Description">
                        <div class="float-end">
                            <insite:Button runat="server" ID="EditContentDescriptionLink" ToolTip="Revise Title" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                        </div>
                        <div class="content-string">
                            <uc:MultilingualStringInfo runat="server" ID="ContentDescription" />
                        </div>
                    </insite:NavItem>

                </insite:Nav>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:Button runat="server" ID="NewAppointmentLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/events/appointments/create" />
        <insite:DeleteButton runat="server" ID="DeleteLink" />
    </div>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .tab-content {
            width: 100%;
        }

        .card-body dl dd {
            margin-bottom: 1rem;
        }
    </style>
</asp:Content>
