<%@ Page Language="C#" CodeBehind="DefineLikertScales.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.DefineLikertScales" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section runat="server" id="QuestionSection">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-table me-1"></i>
                    Define Likert Scales
                </h4>

                <div class="row">

                    <div class="col-lg-6 mb-3 mb-lg-0">
                        <div class="card h-100">
                            <div class="card-body">

                                <h3>Question</h3>

                                <div class="form-group mb-3">
                                    <div>
                                        <asp:Literal runat="server" ID="QuestionText" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="card h-100">
                            <div class="card-body">

                                <h3>Likert Table Columns</h3>

                                <asp:Repeater runat="server" ID="ColumnDtoRepeater">
                                    <HeaderTemplate>
                                        <table class="table table-striped">
                                            <tr>
                                                <th>Text</th>
                                                <th>Points</th>
                                                <th>Category</th>
                                            </tr>
                                    </HeaderTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                    <ItemTemplate>
                                                    
                                        <tr>
                                            <td><%# Eval("Title") %></td>
                                            <td><%# Eval("Points") %></td>
                                            <td><%# Eval("Category") %></td>
                                        </tr>

                                    </ItemTemplate>
                                </asp:Repeater>

                                <div runat="server" id="NoDataMessage" class="alert alert-warning" visible="false">
                                    <i class="fas fa-exclamation-triangle"></i> No Data.
                                </div>

                            </div>
                        </div>
                    </div>

                    <asp:Repeater runat="server" ID="ScaleDtoRepeater">
                        <ItemTemplate>

                            <div class="col-lg-12 mt-3">
                                <div class="card">
                                    <div class="card-body">

                                        <h3><%# Eval("Category") %></h3>

                                        <%# Eval("RowsHtml") %>

                                        <table class="table table-striped mt-3">
                                            <tr>
                                                <td style="width:135px;"><insite:NumericBox runat="server" ID="MinimumA" EmptyMessage="Minimum" /></td>
                                                <td style="width:135px;"><insite:NumericBox runat="server" ID="MaximumA" EmptyMessage="Maximum" /></td>
                                                <td style="width:135px;">
                                                    <insite:TextBox runat="server" ID="GradeA" EmptyMessage="Grade" />
                                                    <div style="margin-top:15px;">
                                                    <insite:ComboBox runat="server" ID="CalculationA">
                                                        <Items>
                                                            <insite:ComboBoxOption />
                                                            <insite:ComboBoxOption Value="Sum" Text="Score" />
                                                            <insite:ComboBoxOption Value="Average" Text="Mean" />
                                                        </Items>
                                                    </insite:ComboBox>
                                                    </div>
                                                </td>
                                                <td><insite:TextBox runat="server" ID="FeedbackA" EmptyMessage="Feedback" TextMode="MultiLine" Rows="4" AllowHtml="true" /></td>
                                            </tr>
                                            <tr>
                                                <td><insite:NumericBox runat="server" ID="MinimumB" EmptyMessage="Minimum" /></td>
                                                <td><insite:NumericBox runat="server" ID="MaximumB" EmptyMessage="Maximum" /></td>
                                                <td>
                                                    <insite:TextBox runat="server" ID="GradeB" EmptyMessage="Grade" />
                                                    <div style="margin-top:15px;">
                                                    <insite:ComboBox runat="server" ID="CalculationB">
                                                        <Items>
                                                            <insite:ComboBoxOption />
                                                            <insite:ComboBoxOption Value="Sum" Text="Score" />
                                                            <insite:ComboBoxOption Value="Average" Text="Mean" />
                                                        </Items>
                                                    </insite:ComboBox>
                                                    </div>
                                                </td>
                                                <td><insite:TextBox runat="server" ID="FeedbackB" EmptyMessage="Feedback" TextMode="MultiLine" Rows="4" AllowHtml="true" /></td>
                                            </tr>
                                            <tr>
                                                <td><insite:NumericBox runat="server" ID="MinimumC" EmptyMessage="Minimum" /></td>
                                                <td><insite:NumericBox runat="server" ID="MaximumC" EmptyMessage="Maximum" /></td>
                                                <td>
                                                    <insite:TextBox runat="server" ID="GradeC" EmptyMessage="Grade" />
                                                    <div style="margin-top:15px;">
                                                    <insite:ComboBox runat="server" ID="CalculationC">
                                                        <Items>
                                                            <insite:ComboBoxOption />
                                                            <insite:ComboBoxOption Value="Sum" Text="Score" />
                                                            <insite:ComboBoxOption Value="Average" Text="Mean" />
                                                        </Items>
                                                    </insite:ComboBox>
                                                    </div>
                                                </td>
                                                <td><insite:TextBox runat="server" ID="FeedbackC" EmptyMessage="Feedback" TextMode="MultiLine" Rows="4" AllowHtml="true" /></td>
                                            </tr>
                                            <tr>
                                                <td><insite:NumericBox runat="server" ID="MinimumD" EmptyMessage="Minimum" /></td>
                                                <td><insite:NumericBox runat="server" ID="MaximumD" EmptyMessage="Maximum" /></td>
                                                <td>
                                                    <insite:TextBox runat="server" ID="GradeD" EmptyMessage="Grade" />
                                                    <div style="margin-top:15px;">
                                                    <insite:ComboBox runat="server" ID="CalculationD">
                                                        <Items>
                                                            <insite:ComboBoxOption />
                                                            <insite:ComboBoxOption Value="Sum" Text="Score" />
                                                            <insite:ComboBoxOption Value="Average" Text="Mean" />
                                                        </Items>
                                                    </insite:ComboBox>
                                                    </div>
                                                </td>
                                                <td><insite:TextBox runat="server" ID="FeedbackD" EmptyMessage="Feedback" TextMode="MultiLine" Rows="4" AllowHtml="true" /></td>
                                            </tr>
                                            <tr>
                                                <td><insite:NumericBox runat="server" ID="MinimumE" EmptyMessage="Minimum" /></td>
                                                <td><insite:NumericBox runat="server" ID="MaximumE" EmptyMessage="Maximum" /></td>
                                                <td>
                                                    <insite:TextBox runat="server" ID="GradeE" EmptyMessage="Grade" />
                                                    <div style="margin-top:15px;">
                                                    <insite:ComboBox runat="server" ID="CalculationE">
                                                        <Items>
                                                            <insite:ComboBoxOption />
                                                            <insite:ComboBoxOption Value="Sum" Text="Score" />
                                                            <insite:ComboBoxOption Value="Average" Text="Mean" />
                                                        </Items>
                                                    </insite:ComboBox>
                                                    </div>
                                                </td>
                                                <td><insite:TextBox runat="server" ID="FeedbackE" EmptyMessage="Feedback" TextMode="MultiLine" Rows="4" AllowHtml="true" /></td>
                                            </tr>
                                            <tr>
                                                <td><insite:NumericBox runat="server" ID="MinimumF" EmptyMessage="Minimum" /></td>
                                                <td><insite:NumericBox runat="server" ID="MaximumF" EmptyMessage="Maximum" /></td>
                                                <td>
                                                    <insite:TextBox runat="server" ID="GradeF" EmptyMessage="Grade" />
                                                    <div style="margin-top:15px;">
                                                    <insite:ComboBox runat="server" ID="CalculationF">
                                                        <Items>
                                                            <insite:ComboBoxOption />
                                                            <insite:ComboBoxOption Value="Sum" Text="Score" />
                                                            <insite:ComboBoxOption Value="Average" Text="Mean" />
                                                        </Items>
                                                    </insite:ComboBox>
                                                    </div>
                                                </td>
                                                <td><insite:TextBox runat="server" ID="FeedbackF" EmptyMessage="Feedback" TextMode="MultiLine" Rows="4" AllowHtml="true" /></td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>

                        </ItemTemplate>
                    </asp:Repeater>

                </div>
            </div>
        </div>

    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
