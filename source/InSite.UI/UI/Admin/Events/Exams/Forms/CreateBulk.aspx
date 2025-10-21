<%@ Page Language="C#" CodeBehind="CreateBulk.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.CreateBulk" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Exam Event" />

    <section runat="server" ID="GeneralSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-calendar-alt me-1"></i>
            Exam Events
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <asp:Repeater runat="server" ID="EventRepeater">
                    <HeaderTemplate>
                        <table class="table">
                            <thead>
                                <tr>
                                    <th style="width:35px;"></th>
                                    <th style="width:165px;">Exam Type</th>
                                    <th style="width:105px;">Exam Format</th>
                                    <th style="width:130px;">Billing Code</th>
                                    <th style="width:300px;">Start Date/Time</th>
                                    <th style="width:75px;">Capacity</th>
                                    <th>Venue</th>
                                    <th style="width:75px;"></th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="align-middle">
                                <asp:CustomValidator runat="server" ID="RowValidator" ValidationGroup="Exam Event"
                                    Text="<i class='fas fa-exclamation-circle text-danger' alt='Row Validation Failed'></i>" />
                            </td>
                            <td>
                                <insite:ExamTypeComboBox runat="server" ID="ExamType" Width="100%" DropDown-Width="250px" />
                            </td>
                            <td>
                                <insite:EventFormatComboBox runat="server" ID="EventFormat" Width="100%" />
                            </td>
                            <td>
                                <insite:ItemNameComboBox runat="server" ID="BillingCode" Width="100%" />
                            </td>
                            <td>
                                <insite:DateTimeOffsetSelector runat="server" ID="EventScheduled" Width="100%" />
                            </td>
                            <td>
                                <insite:NumericBox runat="server" ID="MaximumParticipantCount" NumericMode="Integer" MinValue="0" Width="100%" />
                            </td>
                            <td>
                                <insite:FindGroup runat="server" ID="VenueGroupIdentifier" />
                            </td>
                            <td style="text-align:right;">
                                <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Remove" 
                                    CommandName="Delete"
                                    ConfirmText="Are you sure you want to remove this exam event?" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <div class="mt-3">
                    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Exam Event" CausesValidation="true" Visible="false" />
                    <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/admin/events/reports/dashboard" />

                    <div class="float-end">
                        <insite:Button runat="server" ID="AddRowButton" CausesValidation="false"
                            Icon="fas fa-plus-circle"
                            Text="Add to Schedule" ButtonStyle="Primary" />
                    </div>
                </div>

            </div>
        </div>
    </section>

</asp:Content>