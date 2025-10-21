<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Events.Exams.Controls.Detail" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormPopupSelectorWindow.ascx" TagName="FormPopupSelectorWindow" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Events/Classes/Controls/VenueAddressOld.ascx" TagName="VenueAddress" TagPrefix="uc" %>
<%@ Register Src="ExamMarkGrid.ascx" TagName="ExamMarkGrid" TagPrefix="uc" %>

<uc:FormPopupSelectorWindow runat="server" ID="FormPopupSelectorWindow" />

<div class="row button-group mb-3">
    <div class="col-lg-12">
        <insite:Button runat="server" ID="NewEventLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/events/exams/create" />

        <insite:ButtonSpacer runat="server" ID="Separator1" />

        <insite:Button runat="server" ID="CopyLink" Text="Duplicate" Visible="false" ButtonStyle="Default" Icon="fas fa-copy" />
        <insite:Button runat="server" ID="FormPublicationRestart" Text="Republish" Icon="fas fa-upload" ButtonStyle="Default"  />

        <insite:ButtonSpacer runat="server" ID="Separator2" />

        <insite:Button runat="server" ID="ViewHistoryLink" Visible="false"  Text="History" Icon="fas fa-history" ButtonStyle="Default" />
        <insite:DownloadButton runat="server" ID="DownloadLink" Visible="false" />
        <insite:DeleteButton runat="server" ID="DeleteEventLink" />
    </div>
</div>

<insite:Nav runat="server" ID="EventNavigation">
            
    <insite:NavItem runat="server" Title="Details">

        <div class="row">
            <div class="col-md-6 mb-3">
                <div class="card h-100">
                    <div class="card-body">
                        <h3>Schedule Information</h3>

                        <div class="row">
                            <div class="col-lg-6">
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeEventScheduledStart" ToolTip="Change Event Start" />
                                    </div>
                                    <label class="form-label">Scheduled Start Date and Time</label>
                                    <div>
                                        <asp:Literal runat="server" ID="EventScheduledStart" />
                                    </div>
                                    <div class="form-text">Start date and time for the exam event.</div>
                                </div>
                                <div class="form-group mb-3" runat="server" id="StatusField" visible="false">
                                    <div class="float-end">
                                
                                        <insite:IconLink runat="server" ID="ChangeEventSchedulingStatus" ToolTip="Change Scheduling Status" Name="pencil" />
                                    </div>
                                    <label class="form-label">Schedule Status</label>
                                    <div>
                                        <asp:Panel runat="server" ID="FormPublicationPanel" CssClass="float-end">
                                            <asp:Label runat="server" ID="FormPublicationStatus" />
                                        </asp:Panel>
                                        <%= EventSchedulingStatus.IfNullOrEmpty("None") %>
                                    </div>
                                    <div class="form-text">
                                        Current step in the exam scheduling process.
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeRequestStatus" ToolTip="Change Request Status" />
                                    </div>
                                    <label class="form-label">Request Status</label>
                                    <div>
                                        <asp:Literal runat="server" ID="RequestStatus" />
                                    </div>
                                    <div class="form-text">
                                        Current step in the exam request process.
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeExamDuration" ToolTip="Change Exam Duration" />
                                    </div>
                                    <label class="form-label">Duration</label>
                                    <div>
                                        <asp:Literal runat="server" ID="ExamDuration" />
                                    </div>
                                    <div class="form-text">Time allocated for the exam event.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="CapacityMaximumEdit" ToolTip="Change Exam Candidate Limit" />
                                    </div>
                                    <label class="form-label">Exam Candidate Limit (Capacity)</label>
                                    <div>
                                        <div class="float-end">
                                            <small class="badge bg-danger" id="FullLabel" runat="server" visible="false">Full</small>
                                        </div>
                                        <asp:Label runat="server" ID="CapacityMaximum" />
                                    </div>
                                    <div class="form-text">Maximum number of people allowed.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="InvigilatorMinimumEdit" ToolTip="Change Invigilator(s)" />
                                    </div>
                                    <label class="form-label">Invigilator(s)</label>
                                    <div>
                                        <div class="float-end">
                                            <small class="badge bg-danger" id="InvigilatorError" runat="server" visible="false"></small>
                                            <small class="badge bg-warning" id="InvigilatorWarning" runat="server" visible="false"></small>
                                        </div>
                                        <asp:Label runat="server" ID="InvigilatorMinimum" />
                                    </div>
                                    <div class="form-text">Minimum number of invigilators required.</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6 mb-3">
                <div class="card h-100">
                    <div class="card-body">

                        <h3>Venue Information</h3>
                    
                        <div class="form-group mb-3" runat="server" id="VenueDescriptionFields">
                            <div class="float-end">
                                <insite:IconLink Name="pencil" runat="server" ID="ExamChangeVenue1" ToolTip="Change the invigilating office for this exam event" />
                            </div>
                            <label class="form-label">Invigilating Office</label>
                            <div>
                                <asp:Literal runat="server" ID="VenueOfficeName" />
                            </div>
                            <div class="form-text">Identifies the office from which invigilators will be assigned.</div>
                        </div>
                
                        <div class="form-group mb-3">
                            <div class="float-end">
                                <insite:IconLink Name="pencil" runat="server" ID="ExamChangeVenue2" ToolTip="Change the location for this exam event" />
                            </div>
                            <label class="form-label">
                                Venue Location
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="VenueLocationName" />
                            </div>
                            <div class="form-text">The training provider, organization, or agency hosting the event.</div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="VenueRoomField">
                            <div class="float-end">
                                <insite:IconLink Name="pencil" runat="server" ID="ExamChangeVenue3" ToolTip="Change the building and/or room for this exam event" />
                            </div>
                            <label class="form-label">Building and Room</label>
                            <div>
                                <asp:Label runat="server" ID="VenueRoom" />
                            </div>
                            <div class="form-text">The physical location within the venue where the event occurs.</div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="EventTitleField" visible="false">
                            <label class="form-label">Title</label>
                            <div>
                                <asp:Literal runat="server" ID="EventTitle" />
                            </div>
                            <div class="form-text">
                                Venue and Location / Exam Form Title
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <div class="card h-100">
                    <div class="card-body">
                        <h3>Exam Information</h3>
                
                        <div class="row">
                            <div class="col-lg-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">Exam Type</label>
                                    <div>
                                        <%= EventExamType %>
                                    </div>
                                    <div class="form-text" runat="server" ID="EventTypeHelp"></div>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeExamFormat" ToolTip="Change Exam Format" />
                                    </div>
                                    <label class="form-label">Exam Format</label>
                                    <div>
                                        <asp:Literal runat="server" ID="EventFormat" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6 mb-3">
                <insite:Container runat="server" ID="LocationPanel" Visible="false">
                    <div class="card h-100">
                        <div class="card-body">
                            <h3>Address Information</h3>
                            <uc:VenueAddress ID="PhysicalAddress" runat="server" />
                            <uc:VenueAddress ID="ShippingAddress" runat="server" />
                        </div>
                    </div>
                </insite:Container>
            </div>
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <div class="card h-100">
                    <div class="card-body">
                        <h3>Reference Information</h3>
                
                        <div class="row">
                            <div class="col-lg-6">
                                <div class="form-group mb-3" runat="server" id="NumberField" visible="false">
                                    <label class="form-label">Event Number</label>
                                    <div>
                                        <asp:Literal runat="server" ID="EventNumber" />
                                    </div>
                                    <div class="form-text">Unique number for the exam event.</div>
                                </div>
                                <div class="form-group mb-3" runat="server" id="ClassCodeField" visible="false">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeClassCode" ToolTip="Change Class Code" />
                                    </div>
                                    <label class="form-label">Class/Session Code</label>
                                    <div>
                                        <asp:Label runat="server" ID="ClassCode" />
                                    </div>
                                    <div class="form-text">Reference number for related training programs.</div>
                                </div>
                                <div class="form-group mb-3" runat="server" id="CrmCaseNumberField" visible="false">
                                    <label class="form-label">CRM Reference</label>
                                    <div>
                                        <insite:TextBox runat="server" ID="CrmCaseNumber" MaxLength="32" Enabled="false" />
                                    </div>
                                    <div class="form-text">Reference number for customer relationship management case relevant to the event.</div>
                                </div>
                            </div>
                            <div class="col-lg-6">
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeBillingCode" ToolTip="Change Billing Code" />
                                    </div>
                                    <label class="form-label">Billing Code</label>
                                    <div>
                                        <asp:Literal runat="server" ID="BillingCode" />
                                    </div>
                                    <div class="form-text">Reference number for accounting.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="ChangeExamMaterialReturnShipment" ToolTip="Change Exam Material Tracking" />
                                    </div>
                                    <label class="form-label">Exam Material Tracking</label>
                                    <div>
                                        <div class="float-end">
                                            <asp:Literal runat="server" ID="ExamMaterialReturnShipmentCondition" />
                                        </div>
                                        <asp:Literal runat="server" ID="ExamMaterialReturnShipmentReceived" />
                                    </div>
                                    <div>
                                        <asp:Literal runat="server" ID="ExamMaterialReturnShipmentCode" />
                                    </div>
                                    <div class="form-text">Documentation for the return of exam materials.</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
            </div>
        </div>


        <div class="row">

            <div class="col-md-6">




            </div>
            
            <div class="col-md-6">

                

            </div>
        </div>

    </insite:NavItem>
            
    <insite:NavItem runat="server" ID="FormTab" Title="Forms" Visible="false">

        <div class="row">
            
            <div class="col-lg-8">

                <div class="card mb-3">
                    <div class="card-body">

                        <insite:Alert runat="server" ID="AddFormAlert" />

                        <div class="mb-3">
                            <insite:Button runat="server" ID="AddFormButton" Text="Add Form" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                            <asp:HiddenField runat="server" ID="AddFormIdentifier" />
                        </div>

                        <asp:Repeater runat="server" ID="FormRepeater">
                            <ItemTemplate>

                                <div class="mb-3">
                                    <h3>
                                        <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&form=<%# Eval("FormIdentifier") %>">
                                            <%# Eval("FormTitle") %>
                                        </a>
                                    </h3>

                                    <div class="float-end">
                                        <%# Eval("FormCode") != null ? "<span class='badge bg-success'>" + (string)Eval("FormCode") + "</span>" : "<span class='badge bg-danger'> Missing Form Code</span>" %>
                                        <%# Eval("FormType") != null ? "<span class='badge bg-success'>" + (string)Eval("FormType") + "</span>" : "<span class='badge bg-danger'> Missing Form Type</span>" %>
                                        <insite:IconButton runat="server" Name="trash-alt" 
                                            Visible="<%# CanWrite %>"
                                            CommandName="Delete" 
                                            CommandArgument='<%# Eval("FormIdentifier") %>' 
                                            ConfirmText="Are you sure you want to remove this form from this exam event?" />
                                    </div>

                                    <div>
                                        <%# Eval("FormName") %> [<%# Eval("FormAsset") %>.<%# Eval("FormAssetVersion") %>]
                                    </div>
                            
                                </div>

                                <asp:Literal ID="FormMaterials" runat="server" Visible="false" />

                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </div>

        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="DistributionTab" Title="Distribution" Visible="false">
            
        <div class="row">
            
            <div class="col-lg-6">

                <div class="card h-100">
                    <div class="card-body">
                
                        <h3>
                            Timetable
                            <insite:IconLink Name="pencil" runat="server" ID="ChangeDistributionLink" style="padding:2px 8px; font-size:16px; float:right;" ToolTip="Change Distribution" />
                        </h3>
                
                        <div class="form-group mb-3">
                            <label class="form-label">Scheduled Start Date and Time</label>
                            <div>
                                <asp:Literal runat="server" ID="EventScheduled2" />
                            </div>
                            <div class="form-text">The date and time this exam is scheduled to begin.</div>
                        </div>
                
                        <div class="form-group mb-3" runat="server" id="DistributionOrderedField">
                            <label class="form-label">Paper Distribution Ordered</label>
                            <div>
                                <asp:Literal runat="server" ID="DistributionOrdered" />
                            </div>
                            <div class="form-text">The actual date and time the order for the exam materials package is placed.</div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="DistributionExpectedField">
                            <label class="form-label">Paper Distribution Expected</label>
                            <div>
                                <asp:Literal runat="server" ID="DistributionExpected" />
                            </div>
                            <div class="form-text">
                                The requested or expected shipping date for the exam materials package (e.g. printed paper exam form, diagram book, calculator).
                            </div>
                        </div>
                
                        <div class="form-group mb-3" runat="server" id="DistributionShippedField">
                            <label class="form-label">Paper Distribution Shipped</label>
                            <div>
                                <asp:Literal runat="server" ID="DistributionShipped" />
                            </div>
                            <div class="form-text">The actual date and time the exam materials package is mailed.</div>
                        </div>
                
                        <div class="form-group mb-3">
                            <label class="form-label">Exam Written Date / Attempt Started</label>
                            <div>
                                <asp:Literal runat="server" ID="AttemptStarted" />
                            </div>
                            <div class="form-text">The actual date and time the exam was written.</div>
                        </div>

                        <div class="form-group mb-3" runat="server" id="ExamMaterialReturnShipmentReceivedField">
                            <label class="form-label">Paper Distribution Returned</label>
                            <div>
                                <asp:Literal runat="server" ID="ExamMaterialReturnShipmentReceived2" />
                            </div>
                            <div class="form-text">The actual date and time the exam materials package is returned.</div>
                        </div>
                                
                        <div class="form-group mb-3" runat="server" id="DistributionCourierField">
                            <label class="form-label">Courier Tracking Number</label>
                            <div>
                                <asp:Literal runat="server" ID="DistributionCourier" />
                            </div>
                            <div class="form-text">The courier tracking number for the returned exam materials.</div>
                        </div>

                    </div>
                </div>

            </div>
            
            <div class="col-lg-6">

                <div class="card h-100">
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
                                <asp:Literal runat="server" ID="DistributionProcessOutput" />
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

    </insite:NavItem>

    <insite:NavItem runat="server" ID="IntegrationTab" Title="Integration" Visible="false">

        <div class="row">
            <div class="col-lg-6">
                
                <div class="card">
                    <div class="card-body">

                        <h3>Direct Access</h3>
                
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Options
                            </label>
                            <div>
                                <asp:CheckBox runat="server" ID="WithholdGrades" Text="Withhold the release of exam results" />
                                <br />
                                <asp:CheckBox runat="server" ID="WithholdDistribution" Text="Withhold distribution requests for this event" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="GradeTab" Title="Grades">

        <div class="card">
            <div class="card-body">

                <asp:Repeater runat="server" ID="WarningRepeater">
                    <HeaderTemplate>
                        <div class="alert alert-warning" role="alert"><i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong><ul>
                    </HeaderTemplate>
                    <FooterTemplate></ul></div></FooterTemplate>
                    <ItemTemplate>
                        <li><%# Container.DataItem %></li>
                    </ItemTemplate>
                </asp:Repeater>

                <uc:ExamMarkGrid runat="server" ID="MarkGrid" />

            </div>
        </div>

    </insite:NavItem>
    
</insite:Nav>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            $('#<%= AddFormButton.ClientID %>').on('click', function (e) {
                e.preventDefault();

                bankFormPopupSelectorWindow.<%= FormPopupSelectorWindow.ClientID %>.open({
                    value: null,
                    onSelected: function (data) {
                        $('#<%= AddFormIdentifier.ClientID %>').val(data.id);
                        __doPostBack('<%= AddFormButton.UniqueID %>', '');
                    }
                });
            });
        })();

    </script>

</insite:PageFooterContent>
