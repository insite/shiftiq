<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Exam Event" />

    <section runat="server" ID="EventSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-calendar-alt me-1"></i>
            Exam Event
        </h2>


        <div class="row mb-3">
            <div class="col-md-6">
                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Exam" />
            </div>
        </div>

        <asp:Panel runat="server" ID="NewSection">
            
            <div class="row mb-3">
                        
                <div class="col-md-6">
                                    
                    <div class="card border-0 shadow-lg mb-3">
                        <div class="card-body">

                            <h3>General Information</h3>
                
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Exam Type
                                    <insite:RequiredValidator runat="server" ControlToValidate="ExamType" Name="ExamType" ValidationGroup="Exam Event" />
                                </label>
                                <div>
                                    <insite:ExamTypeComboBox runat="server" ID="ExamType" Width="100%" />
                                </div>
                                <div class="form-text" runat="server" ID="EventTypeHelp"></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Exam Format
                                    <insite:RequiredValidator runat="server" ControlToValidate="EventFormat" Name="Event Format" ValidationGroup="Exam Event" />
                                </label>
                                <div>
                                    <insite:EventFormatComboBox runat="server" ID="EventFormat" />
                                </div>
                                <div class="form-text">The format for this exam event.</div>
                            </div>
                    
                        </div>
                    </div>
                
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Reference Information</h3>
                    
                            <div class="form-group mb-3" runat="server" id="ClassCodeField">
                                <label class="form-label">Class/Session Code</label>
                                <div>
                                    <insite:TextBox runat="server" ID="ClassCode" MaxLength="30" />
                                </div>
                                <div class="form-text">The reference number for related training programs.</div>
                            </div>
                    
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Billing Code
                                    <insite:RequiredValidator runat="server" ControlToValidate="BillingCode" ValidationGroup="Exam Event" />
                                </label>
                                <div>
                                    <insite:ItemNameComboBox runat="server" ID="BillingCode" Width="100%" />
                                </div>
                                <div class="form-text">The exam billing code</div>
                            </div>

                        </div>
                    </div>
            
                </div>
            
                <div class="col-md-6">

                    <div class="card border-0 shadow-lg mb-3">
                        <div class="card-body">

                            <h3>Schedule Information</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Scheduled Start Date and Time
                                    <insite:RequiredValidator runat="server" ControlToValidate="EventScheduled" FieldName="Scheduled Start Date and Time" ValidationGroup="Exam Event" />
                                </label>
                                <div>
                                    <insite:DateTimeOffsetSelector ID="EventScheduled" runat="server" />
                                </div>
                                <div class="form-text">The start date and time for this exam event.</div>
                            </div>
                    
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Exam Candidate Limit (Capacity)
                                    <small class="badge bg-danger" id="FullLabel" runat="server" visible="false">FULL</small>
                                </label>
                                <div>
                                    <insite:NumericBox runat="server" ID="MaximumParticipantCount" NumericMode="Integer" MinValue="0" Width="135px" />
                                </div>
                                <div class="form-text">The maximum number of people allowed to participate.</div>
                            </div>

                        </div>
                    </div>
                            
                    <div class="card border-0 shadow-lg">
                        <div class="card-body">

                            <h3>Venue Information</h3>
                
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Venue Location
                                    <insite:RequiredValidator runat="server" ControlToValidate="VenueLocationIdentifier" Name="Venue Location" ValidationGroup="Exam Event" />
                                </label>
                                <div>
                                    <insite:FindGroup ID="VenueLocationIdentifier" runat="server" Width="100%" />
                                </div>
                                <div class="form-text">The training provider, organization, or agency hosting the event.</div>
                            </div>
                    
                            <div class="form-group mb-3" runat="server" id="LocationRoomField">
                                <label class="form-label">Building and Room</label>
                                <div>
                                    <insite:TextBox ID="LocationRoom" runat="server" MaxLength="200" Width="100%" />
                                </div>
                                <div class="form-text">The physical location within the venue where the event occurs.</div>
                            </div>

                        </div>
                    </div>
                
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Exam Event" />
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </div>
            </div>
        </asp:Panel>

    </section>

</asp:Content>
