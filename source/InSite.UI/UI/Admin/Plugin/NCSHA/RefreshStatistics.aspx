<%@ Page Language="C#" CodeBehind="RefreshStatistics.aspx.cs" Inherits="InSite.Custom.NCSHA.Analytics.Forms.Refresh" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        *[disabled] {
            pointer-events: none;
            cursor: not-allowed;
            opacity: 0.5;
        }

        .year-selector {
            text-align: center;
            float: left;
            padding: 0 15px 25px;
        }

        .year-selector > label {
            display: block;
        }

    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertMessage" />

    <section class="mb-3">
        
        <h2 class="h4 mb-3"><i class="far fa-sync-alt me-2"></i>Refresh</h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

            <div class="row">
                <div class="col-md-12">

                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Program Surveys</th>
                                <th style="width:85px;"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater runat="server" ID="SurveyRepeater">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("Title") %></td>
                                        <td>
                                            <asp:Literal runat="server" ID="Code" Text='<%# Eval("Code") %>' Visible="false" />

                                            <label class="switch">
                                                <asp:CheckBox runat="server" ID="IsSelected" Checked='<%# Eval("IsSelected") %>' />
                                                <div class="slider round"></div>
                                            </label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                    <br />

                    <asp:Repeater runat="server" ID="YearRepeater">
                        <ItemTemplate>
                            <div class="year-selector" <%# (bool)Eval("IsEnabled") ? null : "disabled" %>>
                                <asp:Literal runat="server" ID="Year" Text='<%# Eval("Year") %>' Visible="false" />

                                <label class="switch">
                                    <asp:CheckBox runat="server" ID="IsSelected"
                                        Checked='<%# (bool)Eval("IsEnabled") && (bool)Eval("IsSelected") %>'
                                        Enabled='<%# Eval("IsEnabled") %>'
                                    />
                                    <div class="slider round"></div>
                                </label>
                                <%# Eval("Year") %>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div class="clearfix"></div>

                    <br />

                    <div style="text-align:right;">
                        <insite:Button runat="server" ID="RefreshButton" ButtonStyle="Success" Icon="fas fa-cloud-upload" Text="Refresh" />
                        <insite:Button runat="server" ButtonStyle="Default" NavigateUrl="/ui/portal/plugin/ncsha/statistics" NavigateTarget="_blank" Text="View Chart" Icon="fa fa-chart-line" />
                    </div>
                </div>
            </div>

            </div>
        </div>
    </section>

</asp:Content>