<%@ Page Language="C#" CodeBehind="Reports.aspx.cs" Inherits="InSite.Admin.UI.Desktops.Admin.Reports.Issues" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="HomeStatus" />

    <section runat="server" id="SummaryPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-file-chart-line me-1"></i>
            Summaries
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="row">
                    <div class="col-lg-12 mb-3">
                        <div class="row">
                            <insite:ComboBox runat="server" ID="StatisticDateType" Width="210px">
                                <Items>
                                    <insite:ComboBoxOption Text="Reported" Value="Reported" />
                                    <insite:ComboBoxOption Text="Opened" Value="Opened" />
                                    <insite:ComboBoxOption Text="Closed" Value="Closed" />
                                </Items>
                            </insite:ComboBox>
                            <insite:ComboBox runat="server" ID="StatisticDateShortcut" AllowBlank="false" Width="210px" />
                        </div>
                    </div>

                    <asp:PlaceHolder runat="server" ID="StatisticCustomDateInputs">
                        <div class="col-lg-6 mb-3">
                            <asp:Label runat="server" CssClass="pe-1" ID="lblStatisticDateSince" Text="Since: " AssociatedControlID="StatisticDateSince"></asp:Label>
                            <div>
                                <insite:DateSelector runat="server" ID="StatisticDateSince" />
                            </div>
                        </div>
                        <div class="col-lg-6 mb-3">
                            <asp:Label runat="server" CssClass="pe-1" ID="lblStatisticDateBefore" Text="Before: " AssociatedControlID="StatisticDateBefore"></asp:Label>
                            <div class="text-nowrap">
                                <insite:DateSelector runat="server" ID="StatisticDateBefore" />
                            </div>
                        </div>
                        <div class="col-lg-12">
                            <insite:FilterButton runat="server" ID="StatisticFilterButton" />
                        </div>
                    </asp:PlaceHolder>

                </div>

                <div class="row mt-4">
                    <asp:Repeater runat="server" ID="StatisticRepeater">
                        <ItemTemplate>
                            <div class="col-sm-3">
                                <h2 class="h4 mb-3"><%# Eval("Name") %></h2>

                                <asp:Repeater runat="server" ID="ItemsRepeater" Visible="false">
                                    <HeaderTemplate>
                                        <table class="table table-striped">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Text") %></td>
                                            <td class="text-end"><%# Eval("Count") %></td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>

                                <asp:PlaceHolder runat="server" ID="NoDataMessage" Visible="false">
                                    <p>No Data</p>
                                </asp:PlaceHolder>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

            </div>
        </div>
    </section>
</asp:Content>
