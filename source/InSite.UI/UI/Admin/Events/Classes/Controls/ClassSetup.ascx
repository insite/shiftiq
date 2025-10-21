<%@ Control Language="C#" CodeBehind="ClassSetup.ascx.cs" Inherits="InSite.UI.Admin.Events.Classes.Controls.ClassSetup" %>

<%@ Register Src="~/UI/Admin/Events/Classes/Controls/ClassVenueInfo.ascx" TagName="VenueInfo" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Events/Classes/Controls/ClassAssessmentList.ascx" TagName="ClassAssessmentList" TagPrefix="uc" %>

<div class="row mb-3">
    <div class="col-md-6 mb-3 mb-md-0">

		<div class="card mb-3">
			<div class="card-body">

                <h3>Class Information</h3>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="EventTitleLink" style="padding:8px" ToolTip="Rename Class" />
                    </div>
                    <label class="form-label">
                        <asp:Literal runat="server" ID="EventTitleLabel" Text="Event Title" />
                        <span runat="server" id="FormPublicationStatus" class="badge bg-success ms-2">
                            Published
                        </span>
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="EventTitle" />
                    </div>
                </div>

                <uc:VenueInfo runat="server" ID="VenueInfo" AddressDescription="This is the physical address of the location where the event occurs." />

                <div runat="server" id="VenueRoomField" class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="ClassChangeVenue" style="padding:8px" ToolTip="Change Class Venue" />
                    </div>
                    <asp:Label runat="server" ID="VenueRoomLabel" AssociatedControlID="VenueRoom" Text="Building and Room" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="VenueRoom" />
                    </div>
                    <div class="form-text">
                        The physical location within the venue where the event occurs.
                    </div>
                </div>

                <div class="form-group mb-3">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="HoldPanel" />
                    <insite:UpdatePanel runat="server" ID="HoldPanel">
                        <ContentTemplate>

                            <div class="float-end">
                                <insite:IconButton runat="server" ID="LockRegistrationButton"
                                    Name="lock"
                                    ToolTip="Hold open seats"
                                    ConfirmText="Are you sure to hold open seats?"
                                />
                                <insite:IconButton runat="server" ID="UnlockRegistrationButton"
                                    Name="unlock"
                                    ToolTip="Unlock Registration"
                                    ConfirmText="Are you sure to unlock class registration?"
                                />

                                <insite:IconLink Name="pencil" runat="server" id="ClassLimitAttendance" style="padding:8px" ToolTip="Limit Attendance" />
                            </div>
                            <label class="form-label">
                                <asp:Literal runat="server" Text="Event Capacity" />
                                <span runat="server" id="RegistrationLocked" class="badge bg-danger ms-2">
                                    Hold open seats
                                </span>
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="Capacity" />
                            </div>
                            <div class="form-text">
                                Held seats can only be filled by administrators or waitlist invitation links.
                            </div>

                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="WaitlistLink" style="padding:8px" ToolTip="Change Waitlist" />
                    </div>
                    <asp:Label runat="server" ID="WaitlistLabel" AssociatedControlID="Waitlist" Text="Waitlist" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="Waitlist" />
                    </div>
                    <div class="form-text">
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="EnableEventBillingCodeLink" style="padding:8px" ToolTip="Change" />
                    </div>
                    <label class="form-label">
                        Allow registrants to enter "Bill To" information instead of paying immediately?
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="EnableEventBillingCode" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="PersonCodeIsRequiredLink" style="padding:8px" ToolTip="Change" />
                    </div>
                    <label class="form-label">
                        <asp:Literal runat="server" ID="PersonCodeIsRequiredLabel" />
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="PersonCodeIsRequired" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="AllowMultipleRegistrationsLink" style="padding:8px" ToolTip="Change" />
                    </div>
                    <label class="form-label">
                        Are Multiple Registrations Allowed?
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="AllowMultipleRegistrations" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="AchievementTitleLink" style="padding:8px" ToolTip="Change Achievement" />
                    </div>
                    <asp:Label runat="server" AssociatedControlID="AchievementTitle" Text="Achievement (used to group like Classes together)" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="AchievementTitle" />                                
                    </div>
                </div>

            </div>
        </div>
                                    
		<div class="card">
			<div class="card-body">
                <h3>Assessments</h3>

                <uc:ClassAssessmentList runat="server" ID="AssessmentList" />
            </div>
        </div>

    </div>
    <div class="col-md-6">

		<div class="card mb-3">
			<div class="card-body">

                    <h3>Schedule Information</h3>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="ChangeEventScheduledStart" style="padding:8px" ToolTip="Change Event Start" />
                    </div>
                    <asp:Label runat="server" AssociatedControlID="EventScheduledStart" Text="Event Start Date and Time" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="EventScheduledStart" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="ChangeEventScheduledEnd" style="padding:8px" ToolTip="Change Event End" />
                    </div>
                    <asp:Label runat="server" AssociatedControlID="EventScheduledEnd" Text="Event End Date and Time" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="EventScheduledEnd" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="ChangeDuration" style="padding:8px" ToolTip="Change Duration" />
                    </div>
                    <asp:Label runat="server" ID="DurationLabel" AssociatedControlID="Duration" Text="Duration" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="Duration" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink Name="pencil" runat="server" id="ChangeCreditHours" style="padding:8px" ToolTip="Change Credit Hours" />
                    </div>
                    <asp:Label runat="server" AssociatedControlID="CreditHours" Text="Credit Hours associated with completion of the class" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="CreditHours" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <asp:Label runat="server" AssociatedControlID="RegistrationStart" Text="Registration open as of" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="RegistrationStart" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <asp:Label runat="server" AssociatedControlID="RegistrationDeadline" Text="Registration closed as of" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="RegistrationDeadline" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <div class="float-end">
                    </div>
                    <asp:Label runat="server" ID="EventSchedulingStatusLabel" AssociatedControlID="EventSchedulingStatus" Text="Class Status" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="EventSchedulingStatus" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <asp:Label runat="server" Text="Calendar Color when Published" CssClass="form-label" />
                    <div>
                        <asp:Literal runat="server" ID="ClassCalendarColorBox" />
                        <asp:Literal runat="server" ID="ClassCalendarColorName" />
                    </div>
                </div>

            </div>
        </div>
                                    
		<div class="card">
			<div class="card-body">

                <h3>Instructors</h3>

                <insite:Button runat="server" id="AssignInstructors" ButtonStyle="Default" Text="Assign Instructors" Icon="fas fa-plus-circle" />

                <div class="row mt-3">
                    <div class="col-lg-12">
                        <asp:Repeater runat="server" ID="InstructorRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <tbody>
                            </HeaderTemplate>
                            <FooterTemplate>
                                </tbody></table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <a href="/ui/admin/contacts/people/edit?contact=<%# Eval("UserIdentifier") %>">
                                            <%# Eval("UserFullName") %>
                                        </a>
                                    </td>
                                    <td style="width:40px;text-align:right;">
                                        <insite:IconButton runat="server" ID="RemoveCollectionButton"
                                            CommandName="Remove"
                                            CommandArgument='<%# Eval("UserIdentifier") %>'
                                            ToolTip="Delete Instructor"
                                            Name="trash-alt"
                                        />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

            </div>
        </div>

    </div>
</div>
