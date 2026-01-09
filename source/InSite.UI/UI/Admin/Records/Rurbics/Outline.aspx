<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="OutlineAlert" />

    <insite:Nav runat="server">

        <insite:NavItem runat="server" ID="DetailsTab" Title="Details" Icon="far fa-table" IconPosition="BeforeText">

            <section class="mb-3">
                <h2 class="h4 mb-3">
                    Rubric
                </h2>

                <div class="row">
                    <div class="col-md-6">
                                    
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div class="text-end mb-3">
                                    <insite:Button runat="server" ID="CopyLink" Text="Duplicate" Icon="fas fa-copy" ButtonStyle="Default" />
                                    <insite:Button runat="server" ID="TranslateLink1" Text="Translate" Icon="fas fa-globe" ButtonStyle="Default" />
                                </div>
                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink runat="server" ID="ChangeTitleLink" Name="pencil" ToolTip="Rename Rubric" />
                                    </div>
                                    <label class="form-label">
                                        Rubric Title
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="RubricTitle" />
                                    </div>
                                    <div class="form-text">A descriptive user-friendly title for the Rubric.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink runat="server" ID="ChangeDescriptionLink" Name="pencil" ToolTip="Change Rubric Description" />
                                    </div>
                                    <label class="form-label">
                                        Rubric Description
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="RubricDescription" />
                                    </div>
                                    <div class="form-text">The rubric description.</div>
                                </div>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink runat="server" ID="ChangePointsLink" Name="pencil" ToolTip="Change Total Rubric Points" />
                                    </div>
                                    <label class="form-label">
                                        Total Rubric Points
                                    </label>
                                    <div>
                                        <asp:Literal runat="server" ID="RubricPoints" />
                                    </div>
                                    <div class="form-text"></div>
                                </div>

                            </div>
                        </div>

                    </div>
                </div>
            
            </section>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-question-circle" IconPosition="BeforeText">
            <section class="mb-3">
                <h2 class="h4 mb-3">
                    Criteria
                </h2>

                <insite:Alert runat="server" ID="NoCriteria" />

                <div class="mb-3">
                    <insite:Button runat="server" ID="EditButton" Icon="far fa-pencil" Text="Edit" ButtonStyle="Default" />
                    <insite:Button runat="server" ID="AddButton" Icon="far fa-plus-circle" Text="Add Criteria" ButtonStyle="Default" />
                    <insite:Button runat="server" ID="TranslateLink2" Text="Translate" Icon="fas fa-globe" ButtonStyle="Default" />
                </div>

                <div class="mb-3">
                    <b>Total Rubric Points: <asp:Literal runat="server" ID="CriteriaRubricPoints" /></b>
                </div>

                <asp:Repeater runat="server" ID="CriteriaRepeater">
                    <ItemTemplate>

                        <div class="card border-0 shadow-lg mb-2">
                            <div class="card-body">

                                <table class="table table-striped">
                                    <thead>
                                        <th class="w-25">Title</th>
                                        <th class="w-25 text-end pe-7">Points</th>
                                        <th class="w-50">Description</th>
                                    </thead>

                                    <tbody>
                                        <tr>
                                            <td>
                                                <%# Eval("Title") %>
                                                <div>
                                                    <%# Eval("Range") %>
                                                </div>
                                            </td>
                                            <td class="text-end pe-7">
                                                <%# Eval("Points") %>
                                            </td>
                                            <td>
                                                <%# Eval("Description") %>
                                            </td>
                                        </tr>

                                        <asp:Repeater runat="server" ID="RatingRepeater">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="ps-5">
                                                        <%# Eval("Title") %>
                                                    </td>
                                                    <td class="text-end pe-7">
                                                        <%# Eval("Points") %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("Description") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>

                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>

            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="QuestionsTab" Title="Attached Questions" Icon="far fa-question" IconPosition="BeforeText">
            <section class="mb-3">
                <h2 class="h4 mb-3">
                    Questions
                </h2>

                <div class="card border-0 shadow-lg mb-2">
                    <div class="card-body">

                        <asp:Repeater runat="server" ID="QuestionRepeater">
                            <HeaderTemplate>
                                <ul>
                            </HeaderTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                            <ItemTemplate>
                                <li class="mb-3">
                                    <a href="/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>&question=<%# Eval("QuestionIdentifier") %>"><%# Eval("QuestionAssetNumber", "Asset #{0}") %></a>
                                    <div style='white-space:pre-wrap;'><%# Eval("QuestionText") %></div>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>

           </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
