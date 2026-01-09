<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Payments.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section class="pb-5 mb-md-2">

	    <div class="row">
		    <div class="col-lg-6">

			    <h2 class="h4 mb-3">Payment</h2>

			    <div class="card border-0 shadow-lg">
				    <div class="card-body">

						<div class="row">
                            <div class="col-6">
                                <div class="form-group mb-3">
                                    <asp:Label ID="ProductNameLabel" runat="server" Text="Product Name" CssClass="form-label" AssociatedControlID="ProductName" />
                                    <div><asp:Literal runat="server" ID="ProductName" /></div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="PaymentAmountLabel" runat="server" Text="Payment Amount" CssClass="form-label" AssociatedControlID="PaymentAmount" />
                                    <div><asp:Literal runat="server" ID="PaymentAmount" /></div>
                                    <div class="form-text">The amount of this payment.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="PaymentStatusLabel" runat="server" Text="Payment Status" CssClass="form-label" AssociatedControlID="PaymentStatus" />
                                    <div><asp:Literal runat="server" ID="PaymentStatus" /></div>
                                    <div class="form-text">The status of this payment.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="CustomerIPLabel" runat="server" Text="Customer IP" CssClass="form-label" AssociatedControlID="CustomerIP" />
                                    <div><asp:Literal runat="server" ID="CustomerIP" /></div>
                                    <div class="form-text">The IP Address of the customer/payer.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="ResultCodeLabel" runat="server" Text="Bambora Result Code" CssClass="form-label" AssociatedControlID="ResultCode" />
                                    <div><asp:Literal runat="server" ID="ResultCode" /></div>
                                    <div class="form-text">The result code from Bambora for this payment.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="ResultMessageLabel" runat="server" Text="Bambora Result Message" CssClass="form-label" AssociatedControlID="ResultMessage" />
                                    <div><asp:Literal runat="server" ID="ResultMessage" /></div>
                                    <div class="form-text">The result message from Bambora for this payment.</div>
                                </div>
                            </div>
                            <div class="col-6">
                                <div class="form-group mb-3">
                                    <asp:Label ID="PaymentStartedLabel" runat="server" Text="Payment Started" CssClass="form-label" AssociatedControlID="PaymentStarted" />
                                    <div><asp:Literal runat="server" ID="PaymentStarted" /></div>
                                    <div class="form-text">The payment has been started.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="PaymentAbortedLabel" runat="server" Text="Payment Aborted" CssClass="form-label" AssociatedControlID="PaymentAborted" />
                                    <div><asp:Literal runat="server" ID="PaymentAborted" /></div>
                                    <div class="form-text">The payment has been aborted.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="PaymentDeclinedLabel" runat="server" Text="Payment Declined" CssClass="form-label" AssociatedControlID="PaymentDeclined" />
                                    <div><asp:Literal runat="server" ID="PaymentDeclined" /></div>
                                    <div class="form-text">The payment has been declined.</div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Label ID="PaymentApprovedLabel" runat="server" Text="Payment Approved" CssClass="form-label" AssociatedControlID="PaymentApproved" />
                                    <div><asp:Literal runat="server" ID="PaymentApproved" /></div>
                                    <div class="form-text">The payment has been approved.</div>
                                </div>
                            </div>
                        </div>
				
				    </div>
			    </div>

		    </div>        
	    </div>

    </section>

</asp:Content>