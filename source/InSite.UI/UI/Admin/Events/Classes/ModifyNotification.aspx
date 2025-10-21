<%@ Page Language="C#" CodeBehind="ModifyNotification.aspx.cs" Inherits="InSite.UI.Admin.Events.Classes.ModifyNotification" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/ClassSummaryInfo.ascx" TagName="SummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="./Controls/ClassLocationInfo.ascx" TagName="LocationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="Class" />

    <section class="mb-3">
        <div class="row">
            <div class="col-md-6 mb-3 mb-md-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Class
                        </h4>

                        <uc:SummaryInfo runat="server" ID="SummaryInfo" />

                        <uc:LocationInfo runat="server" ID="LocationInfo" />

                    </div>
                </div>

            </div>
            <div class="col-md-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Class Reminder Notifications
                        </h4>

                        <div class="form-group mb-3">
                            <label class="form-label">To Learner</label>
                            <div>
                                <insite:FindMessage runat="server" ID="ReminderLearnerMessage" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">To Instructors</label>
                            <div>
                                <insite:FindMessage runat="server" ID="ReminderInstructorMessage" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Send notification prior to start of Class (in days)</label>
                            <div>
                                <insite:NumericBox runat="server" ID="SendReminderBeforeDays" NumericMode="Integer" MinValue="1" CssClass="w-25" />
                            </div>
                        </div>
                    
                    </div>
                </div>

            </div>

        </div>
    </section>


    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Class" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>
</asp:Content>