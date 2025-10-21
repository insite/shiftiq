<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionStatisticsPanel.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionStatisticsPanel" %>

<div class="row">
    <div class="col-lg-3">

        <h3>Taxonomies</h3>

        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Taxonomy</th>
                    <th class="text-end">Questions</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="QuestionPerTaxonomyRepeater">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Taxonomy") %></td>
                            <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
                        
        <h3 class="mt-4">Difficulties</h3>

        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Difficulty</th>
                    <th class="text-end">Questions</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="QuestionPerDifficultyRepeater">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Difficulty") %></td>
                            <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

        <h3 class="mt-4">Like Item Groups</h3>

        <table class="table table-striped mb-0">
            <thead>
                <tr>
                    <th>Like Item Group</th>
                    <th class="text-end">Questions</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="QuestionPerLigRepeater">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("LikeItemGroup") %></td>
                            <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

    </div>
    <div class="col-lg-3">

        <h3 class="mt-4 mt-lg-0">Standards (GAC)</h3>

        <table class="table table-striped mb-0">
            <thead>
                <tr>
                    <th>Standard</th>
                    <th class="text-end">Questions</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="QuestionPerGacAndTaxonomyRepeater">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("Standard") %>
                            </td>
                            <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

        <h3 class="mt-4 mt-lg-0">Codes</h3>

        <table class="table table-striped mb-0">
            <thead>
                <tr>
                    <th>Code</th>
                    <th class="text-end">Questions</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="QuestionCodeRepeater">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

    </div>
    <div class="col-lg-6">

        <h3 class="mt-4 mt-lg-0">Standards (Competency)</h3>

        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Standard</th>
                    <th class="text-end">Questions</th>
                    <asp:Repeater runat="server" ID="StandardsTaxonomyRepeater">
                        <ItemTemplate>
                            <th class="text-end">Tax <%# Container.DataItem %></th>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="StandardsRepeater">
                    <ItemTemplate>
                        <tr>
                            <td><%# GetStandardCode(Eval("SetStandardCode")) %><%# GetStandardCode(Eval("QuestionStandardCode")) %></td>
                            <td class="text-end"><%# Eval("Questions", "{0:n0}") %></td>
                            <asp:Repeater runat="server" ID="TaxonomyRepeater">
                                <ItemTemplate>
                                    <td class="text-end"><%# string.Format("{0:n0}", Container.DataItem) %></td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

        <asp:Repeater runat="server" ID="SubCompetenciesRepeater">
            <HeaderTemplate>
                <h3 class="mt-4">Standards (Sub Competency)</h3>

                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Standard</th>
                            <th class="text-end">Questions</th>
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
                        <%# GetStandardCode(Eval("SetStandardCode")) %><%# GetStandardCode(Eval("QuestionStandardCode")) %><%# GetStandardCode(Eval("QuestionSubCode")) %>
                    </td>
                    <td class="text-end"><%# Eval("Questions", "{0:n0}") %></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

        <h3 class="mt-4">Tags</h3>

        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Tag</th>
                    <th class="text-end">Questions</th>
                    <asp:Repeater runat="server" ID="TagsTaxonomyRepeater">
                        <ItemTemplate>
                            <th class="text-end">Tax <%# Eval("Taxonomy") %></th>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="QuestionPerTagAndTaxonomyRepeater">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Tag") %></td>
                            <td class="text-end"><%# Eval("Questions", "{0:n0}") %></td>
                            <asp:Repeater runat="server" ID="QuestionPerTaxonomyRepeater">
                                <ItemTemplate>
                                    <td class="text-end"><%# string.Format("{0:n0}", Container.DataItem) %></td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

    </div>
</div>
