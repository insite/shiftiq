<%@ Page Language="C#" CodeBehind="Reports.aspx.cs" Inherits="InSite.Custom.Ncsha.Surveys.Forms.Home" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section class="pb-4 mb-md-2">
        <h2 class="h4 mb-3"><i class="far fa-chart-bar me-2"></i>Reports</h2>
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <table class="table table-striped">
                    <asp:Repeater runat="server" ID="Reports">
                        <ItemTemplate>
                            <tr><td>
                            <asp:HyperLink runat="server" ID="ReportLink" Text='<%# Eval("Code") + ": " + Eval("Title") %>' NavigateUrl='<%# Eval("NavigateUrl") %>' />
                            </td></tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
    </section>

</asp:Content>
