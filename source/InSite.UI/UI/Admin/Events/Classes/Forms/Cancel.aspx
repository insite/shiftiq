<%@ Page Language="C#" CodeBehind="Cancel.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.Cancel" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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
                            Cancellation
                        </h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Reason
                                <insite:RequiredValidator runat="server" FieldName="Reason" ControlToValidate="Reason" Display="Dynamic" ValidationGroup="Cancel" />
                            </label>
                            <insite:TextBox runat="server" ID="Reason" TextMode="MultiLine" Width="100%" Height="150px" />
                            <div class="form-text">
                                Please provide a short comment to indicate the reason why this class event is cancelled.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Registrations
                            </label>
                            <div>
                                <asp:CheckBox ID="CancelRegistrations" runat="server" Checked="false" Text="Yes, please cancel all the registrations for this event" />
                            </div>
                            <div runat="server" id="CancelRegistrationsHelp" class="form-text">
                            </div>
                        </div>

                    </div>
                </div>

            </div>
        </div>
    </section>

    <section>
        <div class="alert alert-danger" role="alert">
            <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
            Are you sure you want to cancel this class?	
        </div>	

        <insite:Button runat="server" ID="ConfirmButton" Text="Cancel class" Icon="fas fa-archive" ButtonStyle="Danger" ValidationGroup="Cancel" />	
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>
</asp:Content>