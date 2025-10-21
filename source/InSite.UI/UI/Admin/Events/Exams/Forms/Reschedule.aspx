<%@ Page Language="C#" CodeBehind="Reschedule.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.Reschedule" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ExamInfoSummary.ascx" TagName="ExamInfoSummary" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Exam" />

    <section runat="server" ID="RescheduleSection" class="mb-3">

        <div class="row">

            <div class="col-lg-5">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        
                        <uc:ExamInfoSummary ID="ExamInfoSummary" runat="server" />

                    </div>
                </div>
            </div>

            <div class="col-lg-7">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Schedule Information</h3>
                
                        <div class="row">
                            <div class="col-md-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Start Date and Time
                                        <insite:RequiredValidator runat="server" ControlToValidate="EventScheduledStart" FieldName="Start Date and Time" ValidationGroup="Exam" />
                                        <insite:CustomValidator runat="server" ID="EventScheduledStartValidator" ControlToValidate="EventScheduledStart" ValidationGroup="Exam"
                                            ErrorMessage="Start Date and Time of exam must be in future" />
                                    </label>
                                    <div>
                                        <insite:DateTimeOffsetSelector ID="EventScheduledStart" runat="server" />
                                    </div>
                                    <div class="form-text">Start date and time for the exam.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Exam Format
                                        <insite:RequiredValidator runat="server" ControlToValidate="EventFormat" Name="Event Format" ValidationGroup="Exam" />
                                    </label>
                                    <div>
                                        <insite:EventFormatComboBox runat="server" ID="EventFormat" AllowBlank="false" />
                                    </div>
                                    <div class="form-text">The format for this exam event.</div>
                                </div>

                            </div>
                            <div class="col-md-6">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Duration
                                        <insite:RequiredValidator runat="server" ControlToValidate="Duration" FieldName="Duration" ValidationGroup="Exam" />
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="Duration" NumericMode="Integer" DigitGrouping="false" Width="25%" MinValue="0" MaxValue="99999" CssClass="d-inline" />
                                        <insite:ComboBox ID="DurationUnit" runat="server" Width="73%">
                                            <Items>
                                                <insite:ComboBoxOption Value="Minute" Text="Minute" />
                                                <insite:ComboBoxOption Value="Hour" Text="Hour" />
                                                <insite:ComboBoxOption Value="Day" Text="Day" />
                                                <insite:ComboBoxOption Value="Week" Text="Week" />
                                                <insite:ComboBoxOption Value="Month" Text="Month" />
                                                <insite:ComboBoxOption Value="Year" Text="Year" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                    <div class="form-text">Maximum time limit for the exam.</div>
                                </div>

                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Scheduling Status
                                        <insite:RequiredValidator runat="server" ControlToValidate="EventSchedulingStatus" FieldName="Scheduling Status" ValidationGroup="Exam" />
                                    </label>
                                    <div>
                                        <insite:ItemNameComboBox runat="server" ID="EventSchedulingStatus" AllowBlank="false" />
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Candidate Limit (Capacity)
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="CapacityMaximum" />
                                        <small class="badge bg-danger" id="FullLabel" runat="server" visible="false">FULL</small>
                                    </div>
                                    <div class="form-text">Maximum number of candidates to register.</div>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Request Status
                                        <insite:RequiredValidator runat="server" ControlToValidate="EventRequisitionStatus" FieldName="Request Status" ValidationGroup="Exam" />
                                    </label>
                                    <div>
                                        <insite:ItemNameComboBox runat="server" ID="EventRequisitionStatus" AllowBlank="false" />
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Invigilator(s)
                                    </label>
                                    <div>
                                        <insite:NumericBox runat="server" ID="InvigilatorMinimum" />
                                    </div>
                                    <div class="form-text">Minimum number of invigilators required.</div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Exam" DisableAfterClick="true" CausesValidation="true" />
            <insite:CancelButton runat="server" ID="BackButton" />
        </div>
    </div>

</asp:Content>
