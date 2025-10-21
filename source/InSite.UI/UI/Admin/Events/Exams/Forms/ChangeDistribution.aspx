<%@ Page Language="C#" CodeBehind="ChangeDistribution.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.ChangeDistribution" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="ExamDistribution" />

    <section runat="server" ID="ChangeDistributionSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-calendar-alt me-1"></i>
            Change Exam Distribution
        </h2>

            <div class="row">
            
                <div class="col-lg-6">
                
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Timetable</h3>
                
                            <div class="form-group mb-3">
                                <label class="form-label">Scheduled Start Date and Time</label>
                                <div>
                                    <asp:Literal runat="server" ID="EventScheduledStart" />
                                </div>
                                <div class="form-text">The date and time this exam is scheduled to begin.</div>
                            </div>
                
                            <div class="form-group mb-3" runat="server" id="DistributionOrderedField">
                                <label class="form-label">Paper Distribution Ordered</label>
                                <div class="w-75">
                                    <insite:DateTimeOffsetSelector runat="server" ID="DistributionOrdered" />
                                </div>
                                <div class="form-text">The actual date and time the order for the exam materials package is placed.</div>
                            </div>

                            <div class="form-group mb-3" runat="server" id="DistributionExpectedField">
                                <label class="form-label">Paper Distribution Expected</label>
                                <div class="w-75">
                                    <insite:DateTimeOffsetSelector runat="server" ID="DistributionExpected" />
                                </div>
                                <div class="form-text">
                                    The requested or expected shipping date for the exam materials package (e.g. printed paper exam form, diagram book, calculator).
                                </div>
                            </div>
                
                            <div class="form-group mb-3" runat="server" id="DistributionShippedField">
                                <label class="form-label">Paper Distribution Shipped</label>
                                <div class="w-75">
                                    <insite:DateTimeOffsetSelector runat="server" ID="DistributionShipped" />
                                </div>
                                <div class="form-text">The actual date and time the exam materials package is mailed.</div>
                            </div>
                
                            <div class="form-group mb-3">
                                <label class="form-label">Exam Written Date / Submission Started</label>
                                <div class="w-75">
                                    <insite:DateTimeOffsetSelector runat="server" ID="ExamStarted" />
                                </div>
                                <div class="form-text">The actual date and time the exam was written.</div>
                            </div>

                        </div>
                    </div>
                </div>
            
                <div class="col-lg-6">
            
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Utilities</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">Schedule Timer</label>
                                <div>
                                    <asp:Literal runat="server" ID="ScheduleTimer"></asp:Literal>
                                </div>
                                <div class="form-text">
                                    The number of days between now and the scheduled start date.
                                </div>
                            </div>
                
                            <div class="form-group mb-3" runat="server" id="DistributionProcessField">
                                <label class="form-label">Distribution Process</label>
                                <div>
                                    <insite:ComboBox runat="server" ID="DistributionProcessInput" Width="100%" AllowBlank="false" />
                                </div>
                                <div class="form-text">
                                    Select 'BC Mail' if the materials for this exam will be distributed via the standard process with BC Mail. 
                                    Otherwise, indicate if exam materials will be manually distributed due to Late Addition or Escalation.
                                </div>
                            </div>
                
                        </div>
                    </div>
                </div>

            </div>

    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ExamDistribution" />
        <insite:CancelButton runat="server" ID="BackButton" />
    </div>

</asp:Content>
