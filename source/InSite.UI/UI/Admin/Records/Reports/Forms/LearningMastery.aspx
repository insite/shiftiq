<%@ Page Language="C#" CodeBehind="LearningMastery.aspx.cs" Inherits="InSite.Admin.Records.Reports.Forms.LearningMastery" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section runat="server" ID="GradebookPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Gradebook
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="row mb-3">
                    <div class="col-md-6">
                        <insite:ComboBox runat="server" ID="SelectedGradebook" Width="500" />
                    </div>
                    <div class="col-md-6 float-end text-end">
                        <insite:DownloadButton runat="server" ID="DownloadButton" Visible="false" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-12">
                        <insite:Alert runat="server" ID="NoScoreAlert" />

                        <asp:Panel runat="server" ID="StandardScorePanel" Visible="false">
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Student</th>
                                        
                                        <asp:Repeater runat="server" ID="StandardRepeater">
                                            <ItemTemplate>
                                                <th style="text-align:right;"><%# Eval("StandardTitle") %> <span class="form-text">(<%# Eval("MasteryScore", "{0:n0}") %> pts)</span></th>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tr>
                                </thead>
                                <tbody>

                                <asp:Repeater runat="server" ID="StandardScoreRepeater">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("StudentName") %></td>

                                            <asp:Repeater runat="server" ID="ScoreRepeater">
                                                <ItemTemplate>
                                                    <td style='<%# "text-align:right;" + ((bool)Eval("IsMastery") ? "color:green;" : "") %>'>
                                                        <%# Eval("Score", "{0:n1}") %>
                                                    </td>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>

                                </tbody>
                            </table>
                        </asp:Panel>
                    </div>
                </div>

            </div>
        </div>
    </section>

</asp:Content>
