<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementPanel.ascx.cs" Inherits="InSite.Admin.Records.Items.Controls.AchievementPanel" %>

<insite:PageHeadContent runat="server">
    <style>
        .achievements td:nth-child(odd) {
            width:130px;
        }
    </style>
</insite:PageHeadContent>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AchiecementUpdatePanel" />
<insite:UpdatePanel runat="server" ID="AchiecementUpdatePanel">
    <ContentTemplate>
        <div class="form-group mb-3">
            <label class="form-label">
                Achievement
            </label>
            <div>
                <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
            </div>
            <div class="form-text">Does successful completion of this grade item grant an achievement?</div>
        </div>

        <div runat="server" id="WarningPanel" class="alert alert-warning" role="alert" visible="false">

            Learners in this gradebook have been granted this achievement, 
            therefore you cannot change or remove it until those achievements are deleted.

            <div class="pt-3">
                <insite:Button runat="server" ID="RemoveCredentialsButton" ButtonStyle="Danger" OnClientClick="return confirm('Are you sure to delete credentials?')" Text="Delete Credentials" Icon="fas fa-trash-alt" />
            </div>
        </div>

        <asp:Panel runat="server" ID="AchievementConditionPanel" Visible="false">

            <table class="table table-striped achievements">
                <tr>
                    <td>when <b>Score</b> is</td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="WhenChange" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="Changed" Text="Changed" Selected="true" />
                            <asp:ListItem Value="Released" Text="Released" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>if <b>Percent</b> is a</td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="WhenGrade" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="Pass" Text="Pass" Selected="True" />
                            <asp:ListItem Value="Fail" Text="Fail" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td><b>then</b></td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="ThenCommand" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="Ignore" Text="Ignore" />
                            <asp:ListItem Value="Grant" Text="Grant" Selected="True" />
                            <asp:ListItem Value="Revoke" Text="Revoke" />
                            <asp:ListItem Value="Void" Text="Delete" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td><b>else</b></td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="ElseCommand" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="Ignore" Text="Ignore" />
                            <asp:ListItem Value="Grant" Text="Grant" />
                            <asp:ListItem Value="Revoke" Text="Revoke" />
                            <asp:ListItem Value="Void" Text="Delete" Selected="True" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td><b>Effective</b> as at</td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="EffectiveAsAt" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="Current" Text="Current Date" Selected="True" />
                            <asp:ListItem Value="Fixed" Text="Fixed Date" />
                        </asp:RadioButtonList>
                        <div runat="server" id="AchievementFixedDateField" visible="false">
                            <div class="row">
                                <div class="col-md-10">
                                    <insite:DateTimeOffsetSelector ID="AchievementFixedDate" runat="server" />
                                </div>
                                <div class="col-md-2">
                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementFixedDate" FieldName="Achievement Fixed Date" ValidationGroup="Item" />
                                </div>
                                
                            </div>
                        </div>
                    </td>
                </tr>
            </table>

        </asp:Panel>
    </ContentTemplate>
</insite:UpdatePanel>