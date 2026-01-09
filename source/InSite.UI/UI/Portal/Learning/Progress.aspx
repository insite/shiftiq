<%@ Page Language="C#" CodeBehind="Progress.aspx.cs" Inherits="InSite.UI.Portal.Learning.Progress" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <div class="float-end">
        <asp:Button runat="server" ID="DownloadButton" CssClass="btn btn-sm btn-primary" Text="Download PDF" />
    </div>

    <asp:Repeater runat="server" ID="UnitRepeater">
        <ItemTemplate>
            <section class="container mb-2 mb-sm-0 pb-sm-5">

                <h2><%# GetContentText("Content") %></h2>

                <asp:Repeater runat="server" ID="ModuleRepeater">
                    <HeaderTemplate>
                        <table class="table table-bordered table-striped mt-3">
                            <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                            
                        <tr class="<%# GetResultCssClass((Guid)Eval("Identifier")) %>">
                            <th colspan="2">
                                <%# GetContentText("Content") %>
                            </th>
                        </tr>

                        <asp:Repeater runat="server" ID="ActivityRepeater">
                            <ItemTemplate>

                                <tr class="<%# GetResultCssClass((Guid)Eval("Identifier")) %>">
                                    <td>
                                        <span class="ms-3">
                                            <%# GetResultIcon((Guid)Eval("Identifier")) %>
                                            <%# GetContentText("Content") %>
                                        </span>
                                    </td>
                                    <td style="width:100px; text-align:right;">
                                        <%# GetResultScore((Guid)Eval("Identifier")) %>
                                    </td>
                                </tr>

                            </ItemTemplate>
                        </asp:Repeater>

                    </ItemTemplate>
                </asp:Repeater>

            </section>
        </ItemTemplate>
    </asp:Repeater>

</asp:Content>