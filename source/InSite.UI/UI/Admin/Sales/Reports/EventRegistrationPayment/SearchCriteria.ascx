<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Sales.Reports.EventRegistrationPayment.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">

        <div id="toolbox" class="toolbox-section">
            <div class="row">

                <div class="col-4">
                    <h4>Event Conditions</h4>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventDateSince" runat="server" EmptyMessage="Event Date Since"  />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="EventDateBefore" runat="server" EmptyMessage="Event Date Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="EventName" runat="server" EmptyMessage="Event Name" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="AchievementName" runat="server" EmptyMessage="Achievement Name" MaxLength="100" />
                    </div>

                    <h4 class="mt-3">Registration Conditions</h4>

                    <div class="mb-2">
                        <insite:TextBox ID="EmployerName" runat="server" EmptyMessage="Employer at Time of Registration" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="RegistrantName" runat="server" EmptyMessage="Registered By" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="LearnerName" runat="server" EmptyMessage="Registrant" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="LearnerCode" runat="server" EmptyMessage="Learner Code" MaxLength="100" />
                    </div>

                    <div class="mb-2">
	                    <insite:FilterButton ID="SearchButton" runat="server" />
	                    <insite:ClearButton ID="ClearButton" runat="server" />
                    </div>
                </div>

                <div class="col-4">
                    <h4>Invoice Conditions</h4>

                    <div class="mb-2">
                        <insite:NumericBox ID="InvoiceNumber" runat="server" EmptyMessage="Invoice #" MaxLength="100" NumericMode="Integer" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="InvoiceSubmittedSince" runat="server" EmptyMessage="Invoice Submitted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="InvoiceSubmittedBefore" runat="server" EmptyMessage="Invoice Submitted Before" />
                    </div>

                    <div>
                        <insite:InvoiceStatusComboBox runat="server" ID="InvoiceStatus" EmptyMessage="Invoice Status" />
                    </div>
                </div>

                <div class="col-4">
                    <h4>Payment Conditions</h4>

                    <div class="mb-2">
                        <insite:TextBox ID="PaymentStatus" runat="server" EmptyMessage="Payment Status" MaxLength="100" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentApprovedSince" runat="server" EmptyMessage="Payment Approved Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentApprovedBefore" runat="server" EmptyMessage="Payment Approved Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="PaymentTransactionId" runat="server" EmptyMessage="Payment Transaction ID" MaxLength="100" />
                    </div>
                </div>

            </div>

        </div>

    </div>
    <div class="col-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <div class="mb-2">
                <insite:MultiComboBox ID="ShowColumns" runat="server" />
            </div>
            <div class="mb-2">
                <insite:ComboBox ID="SortColumns" runat="server">
                    <Items>
                        <insite:ComboBoxOption Text="Sort by Payment Date" Value="TransactionDate DESC,InvoiceNumber DESC,LearnerAttendee" />
                        <insite:ComboBoxOption Text="Sort by Invoice Number" Value="InvoiceNumber DESC,LearnerAttendee" />
                        <insite:ComboBoxOption Text="Sort by Event Name" Value="EventName,InvoiceNumber DESC,LearnerAttendee" />
                        <insite:ComboBoxOption Text="Sort by Event Date" Value="EventDate DESC,InvoiceNumber DESC,LearnerAttendee" />
                        <insite:ComboBoxOption Text="Sort by Employer at time of Registration" Value="EmployerName,InvoiceNumber DESC,LearnerAttendee" />
                    </Items>
                </insite:ComboBox>
            </div>

            <div class="mb-2">
                <h4>Saved Filters</h4>
                <uc:FilterManager runat="server" ID="FilterManager" />
            </div>
        </div>
    </div>
</div>
