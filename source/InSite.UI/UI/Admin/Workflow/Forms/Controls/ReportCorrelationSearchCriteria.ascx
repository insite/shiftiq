<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportCorrelationSearchCriteria.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.ReportCorrelationSearchCriteria" %>

<insite:PageHeadContent runat="server">
<style type="text/css">

    .correlation-criteria table.axis-variables {
        white-space: normal !important;
    }

        .correlation-criteria table.axis-variables tr > td:first-child {
            padding-left: 3px;
        }

        .correlation-criteria table.axis-variables tr:hover > td:first-child {
            color: #1b1b1b;
        }

        .correlation-criteria table.axis-variables tr > td:last-child {
            width: 13px;
        }

        .correlation-criteria table.axis-variables a > i.fa-times {
            font-size: 13px;
        }

            .correlation-criteria table.axis-variables a > i.fa-times:before {
                margin: 0 0 0 5px !important;
            }

</style>
</insite:PageHeadContent>

<div class="row correlation-criteria">
    <div class="col-md-4">
        <div class="settings">
            
            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="XAxisUpdatePanel" />
            <insite:UpdatePanel runat="server" ID="XAxisUpdatePanel">
                <ContentTemplate>
                    <div class="form-group mb-3">
                        <label class="form-label">
                            X-Axis
                            <insite:RequiredValidator runat="server" ControlToValidate="XAxisQuestionID" FieldName="X-Axis Answer Question" Display="None" ValidationGroup="CorrelationXAxis" />
                        </label>
                        <div>
                            <asp:Repeater runat="server" ID="XAxisVariablesRepeater">
                                <HeaderTemplate>
                                    <table class="axis-variables" style="width: 100%;">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# String.IsNullOrEmpty(Eval("Title") as String) ? "(Untitled)" : Eval("Title") as String %></td>
                                        <td>
                                            <insite:IconButton runat="server" Name="times" 
                                                CommandName="remove" 
                                                CommandArgument='<%# Eval("QuestionIdentifier") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <div>
                            <insite:FormQuestionComboBox runat="server" ID="XAxisQuestionID"
                                EmptyMessage="Question"
                                QuestionType="CheckList,RadioList,Selection,Likert"
                                HasResponseAnswer="true"
                                CausesValidation="false"
                                OnClientSelectedIndexChanged="correlationCriteria.xAxisQuestionID.onSelectedIndexChanged"
                                MaxTextLength="40" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <div>
                            <insite:TextBox runat="server" ID="XAxisTitle" EmptyMessage="Title" />
                        </div>
                    </div>

                    <insite:Button runat="server" ID="AddXButton" Icon="fas fa-plus-circle" Text="Add X Variable" ValidationGroup="CorrelationXAxis" />
                </ContentTemplate>
            </insite:UpdatePanel>
        </div>
    </div>

    <div class="col-md-4">
        <div class="settings">
            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="YAxisUpdatePanel" />
            <insite:UpdatePanel runat="server" ID="YAxisUpdatePanel">
                <ContentTemplate>
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Y-Axis
                            <insite:RequiredValidator runat="server" ControlToValidate="YAxisQuestionID" FieldName="Y-Axis Answer Question" Display="None" ValidationGroup="CorrelationYAxis" />
                        </label>
                        <div>
                            <asp:Repeater runat="server" ID="YAxisVariablesRepeater">
                                <HeaderTemplate>
                                    <table class="axis-variables" style="width: 100%;">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td><%# String.IsNullOrEmpty(Eval("Title") as String) ? "(Untitled)" : Eval("Title") as String %></td>
                                        <td>
                                            <insite:IconButton runat="server" Name="times" 
                                                CommandName="remove" 
                                                CommandArgument='<%# Eval("QuestionIdentifier") %>' />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <div>
                            <insite:FormQuestionComboBox runat="server" ID="YAxisQuestionID"
                                EmptyMessage="Question"
                                QuestionType="CheckList,RadioList,Selection,Likert"
                                HasResponseAnswer="true"
                                CausesValidation="false"
                                OnClientSelectedIndexChanged="correlationCriteria.yAxisQuestionID.onSelectedIndexChanged"
                                MaxTextLength="40" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <div>
                            <insite:TextBox runat="server" ID="YAxisTitle" EmptyMessage="Title" />
                        </div>
                    </div>

                    <insite:Button runat="server" ID="AddYButton" Icon="fas fa-plus-circle" Text="Add Y Variable" ValidationGroup="CorrelationYAxis" />
                </ContentTemplate>
            </insite:UpdatePanel>

        </div>
    </div>

    <div class="col-md-4">
        <div class="settings">

            <div class="form-group mb-3">
                <label class="form-label">Displayed Values</label>
                <div>
                    <asp:CheckBox runat="server" ID="ShowFrequencies" Text="Frequencies" Checked="true" /><br />
                    <asp:CheckBox runat="server" ID="ShowRowPercentages" Text="Row %" Checked="true" /><br />
                    <asp:CheckBox runat="server" ID="ShowColumnPercentages" Text="Column %" Checked="true" />
                </div>
            </div>

        </div>
    </div>

    <div class="col-md-12">
        <div class="text-end">
            <insite:Button runat="server" ID="SearchButton" Icon="fas fa-check" Text="Submit" Width="97px" />
            <insite:ClearButton runat="server" ID="ClearButton" Width="97px" CausesValidation="false" />
        </div>
    </div>
</div>

<script type="text/javascript">
    
    var correlationCriteria = {
        xAxisQuestionID: {
            onSelectedIndexChanged: function () {
                var $option = $(this).find('option:selected');
                if ($option.length == 1)
                    $('#<%# XAxisTitle.ClientID %>').val($option.text().trim());
            },
        },
        yAxisQuestionID: {
            onSelectedIndexChanged: function (s, e) {
                var $option = $(this).find('option:selected');
                if ($option.length == 1)
                    $('#<%# YAxisTitle.ClientID %>').val($option.text().trim());
            },
        },
    };
    
</script>