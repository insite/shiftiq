<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPrerequisiteRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPrerequisiteRepeater" %>

<div class="row mb-3">
    <div class="col-lg-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Course Activity Prerequisites</h3>
                <div>
                    <insite:Button runat="server" ID="AddPrerequisiteButton" CausesValidation="false" ButtonStyle="Default" ToolTip="Add New Prerequisite" Text="New Prerequisite" Icon="fas fa-plus-circle" />
                </div>
                <div>
                    <asp:Repeater runat="server" ID="PrerequisiteRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <thead>
                                    <tr>
                                        <th>Learner Answer</th>
                                        <th>Unlocks Course Activity <sup class="text-danger"><i class="far fa-asterisk fa-xs"></i></sup></th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>

                                    <insite:ComboBox runat="server" ID="QuestionScore">
                                        <Items>
                                            <insite:ComboBoxOption Value="QuestionAnsweredCorrectly" Text="Correct" />
                                            <insite:ComboBoxOption Value="QuestionAnsweredIncorrectly" Text="Incorrect" Selected="true" />
                                        </Items>
                                    </insite:ComboBox>

                                </td>
                                <td style="width: 300px">
                                    <asp:Literal runat="server" ID="PrerequisiteIdentifier" Text='<%# Eval("CoursePrerequisiteIdentifier") %>' Visible="false" />
                                    <insite:RequiredValidator runat="server" ID="ActivitySelectorRequiredValidator" ControlToValidate="ActivitySelector" FieldName="Prerequisite Course Activity" Display="None" RenderMode="Exclamation" />
                                    <insite:FindActivity runat="server" ID="ActivitySelector" />
                                </td>
                                <td class="text-nowrap" style="width: 30px;">
                                    <span style="line-height: 28px;">
                                        <insite:IconButton runat="server"
                                            CausesValidation="false"
                                            CommandName="Delete" Name="trash-alt" ToolTip="Delete"
                                            ConfirmText="Are you sure you want to delete this prerequisite?" />
                                    </span>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

            </div>
        </div>
    </div>
</div>
