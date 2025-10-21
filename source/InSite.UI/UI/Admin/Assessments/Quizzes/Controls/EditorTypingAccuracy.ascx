<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditorTypingAccuracy.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Quizzes.Controls.EditorTypingAccuracy" %>

<asp:CustomValidator runat="server" ID="ValuesValidator" Display="None" />

<asp:Repeater runat="server" ID="QuestionRepeater">
    <ItemTemplate>

        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">

                <h3>Question #<%# Eval("QuestionNumber") %> - Quiz Fields</h3>

                <asp:Repeater runat="server" ID="ColumnRepeater">
                    <HeaderTemplate>
                        <div class="row">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                    <ItemTemplate>
                        <div class="col-xxl-4 mb-3">
                            <div class="card">
                                <div class="card-body">

                                    <h4 class="h6"><%# Eval("Letter", "Column {0}") %></h4>

                                    <asp:Repeater runat="server" ID="RowRepeater">
                                        <HeaderTemplate>
                                            <table class="table"><tbody>
                                        </HeaderTemplate>
                                        <FooterTemplate>
                                            </tbody></table>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <tr class='<%# Container.ItemIndex % 2 == 0 ? "row-strip" : null %>'>
                                                <td class="text-end align-middle border-0 pb-0" style="width:35px;">
                                                    <strong><%# Eval("RowNumber") %></strong>
                                                </td>
                                                <td class="border-0 pb-0">
                                                    <insite:RequiredValidator runat="server" ID="LabelRequiredValidator"
                                                        ControlToValidate="Label" RenderMode="Exclamation" Display="None"
                                                        FieldName='<%# string.Format("Field Label ({0}.{1})", Eval("ColumnLetter"), Eval("RowNumber")) %>' />

                                                    <insite:TextBox runat="server" ID="Label" Text='<%# Eval("Label") %>' EmptyMessage="Field Label" MaxLength="35" />
                                                </td>
                                                <td class="text-nowrap text-end pb-0 align-middle border-0" style="width:30px;">
                                                    <insite:IconButton runat="server" CommandName="DeleteRow" Name="trash-alt" ToolTip="Delete Row" />
                                                </td>
                                            </tr>
                                            <tr class='<%# Container.ItemIndex % 2 == 0 ? "row-strip" : null %>'>
                                                <td></td>
                                                <td>
                                                    <insite:RequiredValidator runat="server" ID="ValuesRequiredValidator"
                                                        ControlToValidate="Values" RenderMode="Exclamation" Display="None"
                                                        FieldName='<%# string.Format("Field Values ({0}.{1})", Eval("ColumnLetter"), Eval("RowNumber")) %>' />

                                                    <insite:TextBox runat="server" ID="Values" TextMode="MultiLine" Rows="3" EmptyMessage="Field Values" 
                                                        Text='<%# Eval("Values") %>' />
                                                </td>
                                                <td></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <div class="mt-3">
                                        <insite:Button runat="server" ID="AddRowButton" CommandName="AddRow" Icon="fas fa-plus-circle" Text="Add New Row" ButtonStyle="OutlineSecondary" CausesValidation="false" />
                                        <insite:Button runat="server" CommandName="DeleteColumn" Icon="fas fa-trash-alt" Text="Delete Column" ButtonStyle="OutlineDanger" ConfirmText='<%# Eval("Letter", "Are you sure you want to delete column {0}?") %>' CausesValidation="false" />
                                    </div>

                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <div>
                    <insite:Button runat="server"
                        ID="AddColumnButton"
                        CommandName="AddColumn"
                        Icon="fas fa-plus-circle"
                        Text="Add New Column"
                        ButtonStyle="Default"
                        CausesValidation="false"
                    />
                    <insite:Button runat="server"
                        CommandName="DeleteQuestion"
                        Icon="fas fa-trash-alt"
                        Text="Delete Question"
                        ButtonStyle="OutlineDanger"
                        ConfirmText='<%# Eval("QuestionNumber", "Are you sure you want to delete question #{0}?") %>'
                        CausesValidation="false"
                        Visible='<%# Eval("CanDelete") %>'
                    />
                    <insite:Button runat="server"
                        CommandName="AddQuestion"
                        Icon="fas fa-plus-circle"
                        Text="Add New Question"
                        ButtonStyle="Default"
                        CausesValidation="false"
                        CssClass="ms-2"
                        Visible='<%# Eval("CanAdd") %>'
                    />
                </div>

            </div>
        </div>

    </ItemTemplate>
</asp:Repeater>