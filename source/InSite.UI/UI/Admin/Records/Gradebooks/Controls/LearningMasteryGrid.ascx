<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LearningMasteryGrid.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.LearningMasteryGrid" %>

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
