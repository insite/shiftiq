<%@ Page Language="C#" CodeBehind="CompetenciesPerProfile.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.CompetenciesPerProfile" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        table.table-report td .description {
            color: #999999;
            font-style: italic;
        }

            table.table-report td .description h1 {
                color: #999999;
                line-height: 1.2;
                font-size: 1.4em;
                font-weight: normal;
            }

    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section runat="server" ID="PreviewSection" class="mb-3">
        
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div runat="server" ID="DownloadCommandsPanel" class="mb-3">
                    
                    <insite:Button runat="server" ID="DownloadXlsx" ButtonStyle="Primary" Text="Download Excel" Icon="fa-solid fa-download" />

                    <insite:CloseButton runat="server" ID="CloseButton" />

                </div>

                <asp:Repeater runat="server" ID="DataRepeater">
                    <HeaderTemplate>
                        <table class="table table-striped table-report">
                            <thead>
                                <tr>
                                    <th class="text-start align-middle" colspan="2">Competency</th>
                                    <th class="text-end align-middle">Hours</th>
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
                              <h3><span class="badge bg-info"><%# Eval("CompetencyNumber") %></span></h3>
                            </td>
                            <td><%# Eval("CompetencyDescription") %></td>
                            <td class="text-end"><%# Eval("ProgramHours", "{0:n2}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>

   </section>

</asp:Content>