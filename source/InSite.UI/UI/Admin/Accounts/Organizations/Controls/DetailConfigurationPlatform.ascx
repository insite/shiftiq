<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailConfigurationPlatform.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailConfigurationPlatform" %>

<div class="row">
    <div class="col-md-6">

        <h3>User Registration</h3>

        <div class="form-group mb-3">
            <label class="form-label">Options</label>
            <div>
                <label class="from-label">
                    Registration Mode
                </label>
                <insite:ComboBox runat="server" ID="SelfRegistrationStatus">
                    <Items>
                        <insite:ComboBoxOption Selected="true" Value="DisallowSelfRegistration" Text="Do not allow new users to self-register" />
                        <insite:ComboBoxOption Value="AllowSelfRegistrationOnLogin" Text="Allow anyone to self-register on user login screen" />
                        <insite:ComboBoxOption Value="AllowSelfRegistrationByLink" Text="Allow new users to self-register by link only" />
                    </Items>
                </insite:ComboBox>
                <div class="form-text">
                    Select first option if organization wants an admin adding all new users. Select the second if the orginazation wants the New User tab on the login screen. Select the third if the organization wants new users to sign up by invite or link only.
                </div>
                <br />
                <asp:CheckBox runat="server" ID="AutomaticApproval" Text="Automatically grant portal access to self-registered new users" />
                <div class="form-text">
                    If this is unchecked, any self-registered new users will receive the User Account Register alert, telling them they must wait for approval. Admins will receive an Access Requested alert where they can grant or deny access to the system. If this is checked, any  self-registered new users will automatically be granted learner access to the system and a portal view. Mandatory forms can be tied to self-registering groups so that additional info can be collected about these users prior to seeing the portal. 
                </div>
                <br />
                <asp:CheckBox runat="server" ID="ConvertProvinceAbbreviation" Text="Convert province abbreviation to name during upload" />
            </div>
        </div>

    </div>
    <div class="col-md-6">

        <h3>Settings</h3>

        <div class="form-group mb-3">
            <label class="form-label">Lock Published Questions</label>
            <div>
                <insite:BooleanComboBox runat="server" ID="LockPublishedQuestions" TrueText="Enabled" FalseText="Disabled" AllowBlank="false" Width="200" />
            </div>
            <div class="form-text">Enable this option if you want to disallow changes to published questions.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Automatic Competency Expiration</label>
            <div>
                <insite:ComboBox runat="server" ID="AutomaticCompetencyExpirationType" Width="200">
                    <Items>
                        <insite:ComboBoxOption Value="None" Text="None" />
                        <insite:ComboBoxOption Value="Interval" Text="Interval" />
                        <insite:ComboBoxOption Value="Date" Text="Date" />
                    </Items>
                </insite:ComboBox>
                <span runat="server" id="AutomaticCompetencyExpirationDateFields" style="display:none;">
                    <insite:TextBox runat="server" ID="AutomaticCompetencyExpirationMonth" MaxLength="2" Width="70" EmptyMessage="Month" />
                    <insite:TextBox runat="server" ID="AutomaticCompetencyExpirationDay" MaxLength="2" Width="70" EmptyMessage="Day" />
                </span>
            </div>
            <div class="form-text">
                Configure time-sensitive competencies to expire automatically on a specific date or after a specific period of time has elapsed from the date of validation.
                Select <strong>None</strong> to disable automatic expiration of time-sensitive competencies
                Select <strong>Interval</strong> to automatically expire the time sensitive competencies after a specific period of time has elapsed from the date of validation.
                Select <strong>Date</strong> to automatically expire time-sensitive competencies on a specific day of each year.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Standard Content Tags</label>
            <insite:TextBox runat="server" ID="StandardContentLabels" MaxLength="200" />
            <div class="form-text">Customer-defined content field names for standards.</div>
        </div>

        <div class="form-group mb-3">
            <asp:Repeater runat="server" ID="StandardContentLabelsRepeater">
                <HeaderTemplate>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Container Type</th>
                                <th>Content Tag</th>
                                <th style="text-align:right;">Contents</th>
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
                        <td><%# Eval("ContainerType") %></td>
                        <td><%# Eval("ContentLabel") %></td>
                        <td style="text-align:right;"><%# Eval("Contents", "{0:n0}") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                const $combo = $('#<%= AutomaticCompetencyExpirationType.ClientID %>')
                    .off('change', onComboChange)
                    .on('change', onComboChange);

                onComboChange();
            });

            function onComboChange() {
                const value = $('#<%= AutomaticCompetencyExpirationType.ClientID %>').val()
                $("#<%= AutomaticCompetencyExpirationDateFields.ClientID %>").toggle(value == "Date");
            }
        })();
    </script>
</insite:PageFooterContent>