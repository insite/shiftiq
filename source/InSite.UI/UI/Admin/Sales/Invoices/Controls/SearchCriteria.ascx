<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.Admin.Invoices.Controls.SearchCriteria" %>
<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox ID="EmployerContactName" runat="server" EmptyMessage="Employer Contact Name" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="CustomerEmail" runat="server" EmptyMessage="Customer Email" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:InvoiceStatusComboBox runat="server" ID="InvoiceStatus" EmptyMessage="Invoice Status" AllowBlank="true" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="Employer" runat="server" EmptyMessage="Employer" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="PersonCode" runat="server" EmptyMessage="Person Code" MaxLength="256" />
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
                        <insite:DateTimeOffsetSelector ID="InvoiceDraftedSince" runat="server" EmptyMessage="Invoice Drafted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="InvoiceDraftedBefore" runat="server" EmptyMessage="Invoice Drafted Before" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="InvoiceSubmittedSince" runat="server" EmptyMessage="Invoice Submitted Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="InvoiceSubmittedBefore" runat="server" EmptyMessage="Invoice Submitted Before" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox ID="TransactionID" runat="server" EmptyMessage="Transaction ID" MaxLength="50" />
                    </div>
                </div>
                <div class="col-4">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="InvoicePaidSince" runat="server" EmptyMessage="Invoice Paid Since" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="InvoicePaidBefore" runat="server" EmptyMessage="Invoice Paid Before" />
                    </div>

                    <div class="mb-2">
                        <insite:FindProduct runat="server" ID="Product" EmptyMessage="Invoice Product" />
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
