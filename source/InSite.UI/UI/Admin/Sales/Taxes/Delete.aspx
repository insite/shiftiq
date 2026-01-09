<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Sales.Taxes.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <div class="row">

        <div class="col-lg-6">
                                    
            <div class="card">

                <div class="card-body">

                    <h4 class="card-title">Taxes</h4>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Name
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="TaxName" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Country
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="CountryName" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Province / State
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="RegionName" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Tax Percent
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="TaxPercent" />
                        </div>
                    </div>

                </div>

            </div>

            <insite:DeleteConfirmAlert runat="server" Name="Tax" CssClass="mt-3" />

            <div class="mt-3">	
                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>

        </div>

        <div class="col-lg-6">
            <div class="card">
                <div class="card-body">

                    <h3>Consequences</h3>

                    <insite:DeleteWarningAlert runat="server" Name="Tax" NoSummary="true" />

                </div>
            </div>
        </div>

    </div>
    
</asp:Content>