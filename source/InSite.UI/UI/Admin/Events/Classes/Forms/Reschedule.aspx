<%@ Page Language="C#" CodeBehind="Reschedule.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.Reschedule" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>


<%@ Register Src="../Controls/ClassSummaryInfo.ascx" TagName="SummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="../Controls/ClassLocationInfo.ascx" TagName="LocationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Cancel" />

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
                            Schedule Information
                        </h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Start Date and Time
                                <insite:RequiredValidator runat="server" ControlToValidate="EventScheduledStart" FieldName="Start" ValidationGroup="Class" />
                            </label>
                            <insite:DateTimeOffsetSelector ID="EventScheduledStart" runat="server" Width="365px" />
                            <div class="form-text">
                                The start date and time for this class event.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                End Date and Time
                                <insite:RequiredValidator runat="server" ControlToValidate="EventScheduledEnd" FieldName="End" ValidationGroup="Class" />
                                <insite:CustomValidator runat="server" ID="EventScheduledEndValidator" ControlToValidate="EventScheduledEnd" ValidationGroup="Class" Display="None" />
                            </label>
                            <insite:DateTimeOffsetSelector ID="EventScheduledEnd" runat="server" Width="365px" />
                            <div class="form-text">
                                The end date and time for this class event.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Duration
                                <insite:RequiredValidator runat="server" ControlToValidate="Duration" FieldName="Duration" ValidationGroup="Class" />
                            </label>
                            <div>
                                <insite:NumericBox runat="server" ID="Duration" Width="110px" CssClass="d-inline-block"
                                    NumericMode="Integer" DigitGrouping="false" MinValue="0" MaxValue="99999" />

                                <insite:ComboBox ID="DurationUnit" runat="server" Width="110px" CssClass="d-inline-block align-top">
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
                            <div class="form-text">
                                Duration of this class event.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Credit Hours
                            </label>
                            <insite:NumericBox runat="server" ID="CreditHours" DecimalPlaces="2" DigitGrouping="false" Width="110px" MinValue="0" MaxValue="99999" /> 
                            <div class="form-text">
                                The number of credit hours associated with completion of the class.
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