<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BuildAggregate.ascx.cs" Inherits="InSite.UI.Admin.Reports.Controls.BuildAggregate" %>

<div class="row settings">
    <div class="col-lg-12">

<asp:MultiView runat="server" ID="MultiView">
    <asp:View runat="server" ID="TableView">
        <insite:Alert runat="server" ID="TableStatus" />

        <asp:Repeater runat="server" ID="AggregateRepeater">
            <HeaderTemplate>
                <table class="table">
                    <thead>
                        <tr>
                            <th><insite:Literal runat="server" Text="Name" /></th>
                            <th><insite:Literal runat="server" Text="Function" /></th>
                            <th style="width:120px;"></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="text-nowrap">
                        <asp:Literal runat="server" ID="AliasName" Text='<%# Eval("Alias") %>' />
                    </td>
                    <td>
                        <div style="white-space:pre-wrap"><%# Eval("PseudoSql") %></div>
                    </td>
                    <td class="text-nowrap text-end">
                        <insite:Button runat="server" Icon="fas fa-edit" ToolTip='<%# Translate("Edit Column") %>' CommandName="EditAggregate" />
                        <insite:Button runat="server" Icon="far fa-trash-alt" ToolTip='<%# Translate("Delete Column") %>' CommandName="DeleteAggregate" />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:View>
    <asp:View runat="server" ID="EditorView">
        <insite:RequiredValidator runat="server" ID="AggregateAliasRequiredValidator" ControlToValidate="AggregateAlias" FieldName="Column Name" RenderMode="Exclamation" Display="None" />
        <insite:CustomValidator runat="server" ID="AggregateAliasUniqueValidator" ErrorMessage="Column Name must be unique" RenderMode="Exclamation" Display="None" />
        <insite:RequiredValidator runat="server" ID="AggregateFunctionRequiredValidator" ControlToValidate="AggregateFunction" FieldName="Aggregate Function" RenderMode="Exclamation" Display="None" />
        <insite:RequiredValidator runat="server" ID="AggregateViewRequiredValidator" ControlToValidate="AggregateViewSelector" FieldName="View" RenderMode="Exclamation" Display="None" />
        <insite:RequiredValidator runat="server" ID="AggregateColumnRequiredValidator" ControlToValidate="AggregateColumnSelector" FieldName="Column" RenderMode="Exclamation" Display="None" />

        <div class="row">
            <div class="col-md-6">
                <div class="mb-3">
                    <insite:TextBox runat="server" ID="AggregateAlias" EmptyMessage="Column Name" MaxLength="50" />
                    <asp:HiddenField runat="server" ID="AggregateAutoAlias" />
                </div>

                <div class="mb-3">
                    <insite:ComboBox runat="server" ID="AggregateFunction" AllowBlank="true" EmptyMessage="Aggregate Function" ClientEvents-OnChange="buildAggregate.onFuncChanged" />
                </div>

                <insite:Container runat="server" ID="AggregateColumnContainer">
                    <div class="mb-3">
                        <insite:ComboBox runat="server" ID="AggregateViewSelector" AllowBlank="true" EmptyMessage="View" />
                    </div>
            
                    <div runat="server" id="AggregateColumnField" class="mb-3">
                        <insite:ComboBox runat="server" ID="AggregateColumnSelector" EmptyMessage="Column" ClientEvents-OnChange="buildAggregate.onColumnChanged" />
                    </div>
                </insite:Container>
            </div>
        </div>
    </asp:View>
</asp:MultiView>

    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.buildAggregate = window.buildAggregate || {};

            $(document).ready(function () {
                var $autoAlias = $('#<%= AggregateAutoAlias.ClientID %>');
                if (!$autoAlias.val())
                    $autoAlias.val(getAutoName());
            });

            instance.onFuncChanged = function () {
                setAutoName();
            };

            instance.onColumnChanged = function () {
                setAutoName();
            };

            function setAutoName() {
                var $alias = $('#<%= AggregateAlias.ClientID %>');
                var $autoAlias = $('#<%= AggregateAutoAlias.ClientID %>');
                var aliasValue = $alias.val();
                var autoValue = $autoAlias.val();

                if (!aliasValue || aliasValue == autoValue) {
                    autoValue = getAutoName();

                    $alias.val(autoValue);
                    $autoAlias.val(autoValue);
                }
            };

            function getAutoName() {
                var func = $('#<%= AggregateFunction.ClientID %>').val();
                var column = $('#<%= AggregateColumnSelector.ClientID %> option:selected').text().trim();

                if (func == '<%= InSite.Domain.Reports.ReportAggregate.FunctionType.Count.GetName() %>' || func && column)
                    return column + func;
                else
                    return '';
            }
        })();
    </script>
</insite:PageFooterContent>