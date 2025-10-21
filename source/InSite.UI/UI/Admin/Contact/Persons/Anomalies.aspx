<%@ Page Language="C#" CodeBehind="Anomalies.aspx.cs" Inherits="InSite.UI.Admin.Contact.Persons.Anomalies" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section runat="server" ID="PreviewSection" class="mb-3">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="clearfix">
                    <div runat="server" ID="DownloadCommandsPanel" class="mb-3">
                        <insite:Button runat="server" ID="DownloadXlsx" ButtonStyle="Primary" Text="Download XLSX" Icon="far fa-download" />
                    </div>
                </div>

                <asp:Repeater runat="server" ID="ProblemRepeater">
                    <HeaderTemplate>
                        <table class="table table-striped table-report mt-3">
                            <thead>
                                <tr>
                                    <th class="align-middle">Name and Email</th>
                                    <th class="align-middle">City</th>
                                    <th class="align-middle">Phone</th>
                                    <th class="text-center align-middle">Problem</th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("Name") %>
                                <div class="fs-xs"><%# Eval("Email") %></div>
                            </td>
                            <td><%# Eval("City") %></td>
                            <td><%# Eval("Phone") %></td>
                            <td class="text-center text-nowrap">
                                <%# Eval("Problem") %>
                                <div class="fs-xs text-danger"><%# Eval("Solution") %></div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>

    </section>

</asp:Content>
