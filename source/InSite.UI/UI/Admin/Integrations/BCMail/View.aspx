<%@ Page Language="C#" CodeBehind="View.aspx.cs" Inherits="InSite.UI.Admin.Integrations.BCMail.View" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Alert" />

        <h2 class="h4 mb-3">Status</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
            
            <h2>Current Status</h2>
                    
            <div class="row">
                <div class="col-lg-12">
                    <strong runat="server" id="DistributionStatusTime"></strong>
                    <div runat="server" id="DistributionStatus"></div>
                    <div runat="server" id="DistributionErrors"></div>
                </div>
            </div>
            
            <h2 class="mt-4">History</h2>

            <asp:Repeater runat="server" ID="HistoryRepeater">
                <ItemTemplate>
                    <div class="row">
                        <div class="col-lg-12" style="margin-bottom:20px;">
                            <strong><%# Localize(Eval("Requested")) %></strong>
                            <div>
                                <%# Eval("JobStatus") %>
                            </div>
                            <div class="alert alert-danger" runat="server" id="ErrorsPanel" visible='<%# !string.IsNullOrEmpty((Eval("JobErrors") as string)) %>'>
                                <%# Eval("JobErrors") %>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            </div>
        </div>

</asp:Content>
