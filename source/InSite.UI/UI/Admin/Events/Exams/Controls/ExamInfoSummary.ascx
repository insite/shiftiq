<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExamInfoSummary.ascx.cs" Inherits="InSite.Admin.Events.Exams.Controls.ExamInfoSummary" %>
<%@ Register Src="Venue.ascx" TagName="Venue" TagPrefix="uc" %>

    <h3>Exam</h3>

    <div class="form-group mb-3" runat="server" ID="AchievementField">
        <label class="form-label">Achievement</label>
        <div>
            <asp:Literal runat="server" ID="AchievementTitle" />
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">Event Title</label>
        <div>
            <a runat="server" id="EventLink">
                <asp:Literal runat="server" ID="EventTitle" />
            </a>
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">Exam Type</label>
        <div>
            <asp:Literal runat="server" ID="ExamType" />
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">Exam Format</label>
        <div>
            <asp:Literal runat="server" ID="ExamFormat" />
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">Scheduling Status</label>
        <div>
            <asp:Literal runat="server" ID="EventStatus" />
        </div>
    </div>

    <uc:Venue runat="server" ID="Venue" />

    <div runat="server" id="VenueRoomField" class="form-group mb-3">
        <label class="form-label">Building and Room</label>
        <div>
            <asp:Literal runat="server" ID="VenueRoom" />
        </div>
    </div>

    <div class="form-group mb-3" runat="server" id="Schedule">
        <label class="form-label">Scheduled Start</label>
        <div>
            <asp:Literal runat="server" ID="EventScheduledStart" />
        </div>
    </div>
