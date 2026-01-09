<%@ Page Language="C#" CodeBehind="LimitCapacity.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.LimitCapacity" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ClassSummaryInfo.ascx" TagName="SummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="../Controls/ClassLocationInfo.ascx" TagName="LocationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Class" />

    <insite:CustomValidator runat="server" ID="CapacityValidator" ErrorMessage="Minimal Capacity should be lower then Maximum Capacity or equal." Display="None" ValidationGroup="Class" />

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
                            Attendance Information
                        </h4>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Minimum Capacity
                            </label>
                            <insite:NumericBox runat="server" ID="MinimalCapacity" NumericMode="Integer" DigitGrouping="false" Width="110px" MinValue="0" MaxValue="999" /> 
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Maximum Capacity
                                <insite:RequiredValidator runat="server" ID="MaximumCapacityRequired" FieldName="Maximum Capacity" ControlToValidate="MaximumCapacity" ValidationGroup="Class" />
                            </label>
                            <insite:NumericBox runat="server" ID="MaximumCapacity" NumericMode="Integer" DigitGrouping="false" Width="110px" MinValue="0" MaxValue="999" /> 
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Waitlist
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="WaitlistEnabled">
                                    <asp:ListItem Value="Enabled" Text="Enabled" />
                                    <asp:ListItem Value="Disabled" Text="Disabled" />
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Enable Bill To
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="BillingCodeEnabled" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="Yes" Text="Yes" />
                                    <asp:ListItem Value="No" Text="No" />
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                <asp:Literal runat="server" ID="PersonCodeIsRequiredLabel" />
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="PersonCodeIsRequired" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="Yes" Text="Yes" />
                                    <asp:ListItem Value="No" Text="No" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="form-text">
                                <asp:Literal runat="server" ID="PersonCodeIsRequiredHint" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Multiple Registrations Allowed
                            </label>
                            <div>
                                <asp:RadioButtonList runat="server" ID="AllowMultipleRegistrations" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="Yes" Text="Yes" />
                                    <asp:ListItem Value="No" Text="No" />
                                </asp:RadioButtonList>
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