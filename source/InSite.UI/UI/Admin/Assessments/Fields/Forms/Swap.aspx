<%@ Page Language="C#" CodeBehind="Swap.aspx.cs" Inherits="InSite.Admin.Assessments.Fields.Forms.Swap" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <h2 class="h4 mb-3"><i class="far fa-exchange me-2"></i>Fields</h2>

    <div class="row mb-3">
        <asp:Repeater runat="server" ID="FieldRepeater">
            <ItemTemplate>
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3><%# Eval("FormName") %></h3>
                            <strong><%# Eval("SectionName") %></strong>

                            <div class="mb-3">
                                <%# Eval("BankSequence") %>
                            </div>

                            <div class="mb-3">
                                <span class='badge bg-primary'><%# Eval("FormIndex") %></span>
                            </div>

                            <div class="mb-3">
                                <%# Eval("Title") %>
                            </div>

                            <asp:Repeater runat="server" ID="OptionRepeater">
                                <HeaderTemplate>
                                    <table class="table table-striped">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# GetOptionIcon((decimal)Eval("Points")) %></td>
                                        <td><%# Eval("Letter") %>.</td>
                                        <td>
                                            <%# Eval("Title") %>
                                            <span class="text-body-secondary">&bull; <%# (decimal)Eval("Points") %> points</span>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate></table></FooterTemplate>
                            </asp:Repeater>

                            <%# DisplayStandard(Eval("Question")) %>

                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <div class="row">
        <div class="col-md-12">
            <insite:Button runat="server" ID="SwapButton" ButtonStyle="Success" Text="Swap" Icon="far fa fa-exchange" />
        </div>
    </div>

</asp:Content>
