<%@ Page Language="C#" CodeBehind="TaxFormT2202.aspx.cs" Inherits="InSite.Admin.Contacts.Reports.Forms.TaxFormT2202" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />

    <section runat="server" ID="SearchCriteriaSection" class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <h4>Criteria</h4>

                <div class="row">
                    <div class="col-4">
                        <div class="mb-2 hstack">
                            <insite:ComboBox ID="YearSelector" runat="server" EmptyMessage="Year" CssClass="me-1" />
                            <insite:RequiredValidator runat="server" ControlToValidate="YearSelector" ValidationGroup="Report" />
                        </div>
                        <div class="mb-2">
                            <insite:Button runat="server" ID="SearchButton" Text="Build" Icon="fas fa-chart-bar" ValidationGroup="Report" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section runat="server" ID="SearchResultsSection" visible="false" class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <h4 class="h4">
                    Results
                </h4>

                <insite:DownloadButton runat="server" ID="DownloadButton" /> 
            </div>
        </div>
    </section>

</asp:Content>
