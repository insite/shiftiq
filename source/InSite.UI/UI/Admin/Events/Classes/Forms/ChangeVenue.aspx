<%@ Page Language="C#" CodeBehind="ChangeVenue.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.ChangeVenue" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ClassSummaryInfo.ascx" TagName="SummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="../Controls/ClassLocationInfo.ascx" TagName="LocationInfo" TagPrefix="uc" %>	
<%@ Register Src="../Controls/ClassVenueInfo.ascx" TagName="VenueInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
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

                        <uc:LocationInfo runat="server" ID="LocationInfo" ShowVenue="false" />

                    </div>
                </div>

            </div>
            <div class="col-md-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Location Information
                        </h4>

                        <uc:VenueInfo runat="server" ID="VenueInfo" VenueLabel="Current Venue" AddressLabel="Physical Location of Current Venue" AddressDescription="" ShowChangeLink="false" />

                        <div runat="server" id="CurrentVenueRoomField" class="form-group mb-3">
                            <label class="form-label">
                                Current Building and Room
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="CurrentVenueRoom" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                New Venue
                                <insite:RequiredValidator runat="server" ControlToValidate="VenueGroupIdentifier" FieldName="New Venue" ValidationGroup="Class" />
                            </label>
                            <insite:FindGroup ID="VenueGroupIdentifier" runat="server" />
                            <div class="form-text">
                                The training provider, organization, or agency hosting the event.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                New Building and Room
                            </label>
                            <insite:TextBox ID="NewVenueRoom" runat="server" MaxLength="128" Width="100%" />
                            <div class="form-text">
                                The physical location within the venue where the event occurs.
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