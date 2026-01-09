<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Payments.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox ID="CustomerName" runat="server" EmptyMessage="Customer Name" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox ID="CustomerEmail" runat="server" EmptyMessage="Customer Email" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:TextBox ID="CustomerEmployer" runat="server" EmptyMessage="Employer" MaxLength="256" />
                    </div>
                    <div class="mb-2">
                        <insite:NumericBox ID="MinAmount" runat="server" EmptyMessage="Min Amount" NumericMode="Integer" DigitGrouping="false" MinValue="0" />
                    </div>
    
                    <div class="mb-2">
                        <insite:NumericBox ID="MaxAmount" runat="server" EmptyMessage="Max Amount" NumericMode="Integer" DigitGrouping="false" MinValue="0" />
                    </div>

                    <div class="mb-2">
                        <insite:PaymentStatusComboBox ID="PaymentStatus" runat="server" EmptyMessage="Payment Status" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>
                <div class="col-4">

                    <div class="mb-2">
                        <insite:NumericBox ID="InvoiceNumber" NumericMode="Integer" runat="server" EmptyMessage="Invoice #" DigitGrouping="false"/>
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentApprovedSince" runat="server" EmptyMessage="Payment Approved Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentApprovedBefore" runat="server" EmptyMessage="Payment Approved Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentStartedSince" runat="server" EmptyMessage="Payment Started Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentStartedBefore" runat="server" EmptyMessage="Payment Started Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="TransactionID" runat="server" EmptyMessage="Transaction ID" MaxLength="50" />
                    </div>
                    <div class="mb-2">
                        <insite:FindProduct runat="server" ID="Product" EmptyMessage="Product Name" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentDeclinedSince" runat="server" EmptyMessage="Payment Declined Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentDeclinedBefore" runat="server" EmptyMessage="Payment Declined Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentAbortedSince" runat="server" EmptyMessage="Payment Aborted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="PaymentAbortedBefore" runat="server" EmptyMessage="Payment Aborted Before" />
                    </div>
                </div>
            </div> 
        </div>
    </div>
    <div class="col-3">       
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
