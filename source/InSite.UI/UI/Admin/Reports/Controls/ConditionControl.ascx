<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConditionControl.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.ConditionControl" %>

<div class="row settings">

    <div class="col-md-2">

        <div class="mb-3">
            <insite:ComboBox runat="server" ID="ConditionClause" AllowBlank="false" EmptyMessage="Clause" />
        </div>

    </div>

    <div class="col-lg-12">

        <asp:Repeater runat="server" ID="ConditionGrid">
            <HeaderTemplate>
                <table class="table">
                    <thead>
                        <tr>
                            <th><insite:Literal runat="server" Text="Column Name" /></th>
                            <th><insite:Literal runat="server" Text="Condition Operator" /></th>
                            <th><insite:Literal runat="server" Text="Condition Value" /></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Literal runat="server" ID="ColumnName" Text='<%# Eval("Column.Name") %>' />
                    </td>
                    <td>
                        <insite:ComboBox runat="server" ID="ConditionOperator" AllowBlank="false" />
                    </td>
                    <td>
                        <insite:ComboBox runat="server" ID="ConditionValueBool" Visible="false">
                            <Items>
                                <insite:ComboBoxOption Text="Any" />
                                <insite:ComboBoxOption Value="True" Text="True" />
                                <insite:ComboBoxOption Value="False" Text="False" />
                            </Items>
                        </insite:ComboBox>

                        <insite:DateSelector runat="server" ID="ConditionValueDate" Visible="false" />
                        <insite:DateTimeOffsetSelector runat="server" ID="ConditionValueDateTime" Visible="false" />
                        <insite:NumericBox runat="server" ID="ConditionValueNumber" Visible="false" />
                        <insite:TextBox runat="server" ID="ConditionValueText" Visible="false" />

                        <asp:HiddenField runat="server" ID="DataType" Value='<%# Eval("Column.DataType") %>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

    </div>

</div>
