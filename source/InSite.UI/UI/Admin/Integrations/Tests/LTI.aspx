<%@ Page Language="C#" CodeBehind="LTI.aspx.cs" Inherits="InSite.UI.Admin.Integrations.Tests.LTI" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="D365" />

    <div class="row">

        <div class="col-md-6">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <asp:MultiView runat="server" ID="MultiView" ActiveViewIndex="0">
                
                        <asp:View runat="server" ID="ViewStep1">
                            <h4 class="card-title mb-3">
                                Simulate LTI Launch Request
                            </h4>

                            <p class="card-subtitle mb-2 text-body-secondary">
                                This form simulates a basic LTI Consumer.
                            </p>

                            <p>
                                Click the Validate button to create an LTI launch request for Single-Sign-On access to Shift iQ.
                            </p>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Input Type
                                </label>
                                <div class="ms-2">
                                    <insite:RadioButtonList runat="server" ID="InputTypeList">
                                        <asp:ListItem Text="Fields" Value="Fields" Selected="True" />
                                        <asp:ListItem Text="Data Text" Value="Text" />
                                    </insite:RadioButtonList>
                                </div>
                            </div>

                            <asp:MultiView runat="server" ID="InputMultiView">
                                <asp:View runat="server" ID="InputFieldsView">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Learner Code
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="LtiLearnerCode" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Learner Email
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="LtiLearnerEmail" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Learner First Name
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="LtiLearnerNameFirst" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Learner Last Name
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="LtiLearnerNameLast" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Group Name
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="LtiGroupName" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Organization Identifier
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="LtiOrganizationIdentifier" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Organization Secret
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="LtiOrganizationSecret" />
                                        </div>
                                    </div>
                                </asp:View>
                                <asp:View runat="server" ID="InputTextView">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Data Text
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID="DataText" AllowHtml="true" />
                                        </div>
                                    </div>
                                </asp:View>
                            </asp:MultiView>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Launch URL
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="LtiLaunchUrl" />
                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:Button runat="server" ID="ValidateButton" Text="Validate" ButtonStyle="Primary"
                                    Icon="fas fa-check" CausesValidation="true" ValidationGroup="LTI" />
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="ViewStep2">
                            <h4 class="card-title mb-3">
                                LTI Tool Consumer
                            </h4>

                            <p class="card-subtitle mb-2 text-body-secondary">
                                This form simulates a basic LTI Tool Consumer.
                            </p>

                            <p>
                                Click the Launch link to send an LTI launch request message for Single-Sign-On access to Shift iQ.
                            </p>

                            <asp:Repeater runat="server" ID="ParameterRepeater">
                                <HeaderTemplate>
                                    <div id='<%# ParameterRepeater.ClientID %>'>
                                </HeaderTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                                <ItemTemplate>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            <%# Eval("Key") %>
                                        </label>
                                        <div>
                                            <insite:TextBox runat="server" ID='TextInput' Text='<%# Eval("Value") %>' data-key='<%# Eval("Key") %>' />
                                        </div>
                                    </div>

                                </ItemTemplate>
                            </asp:Repeater>

                            <div class="mt-3">
                                <insite:Button runat="server" ID="GoToStep1" Text="Prev" Icon="fas fa-arrow-alt-left" ButtonStyle="Default" />
                                <insite:Button runat="server" ID="LaunchButton" Text="Launch" ButtonStyle="Primary"
                                    Icon="fas fa-cloud-upload" CausesValidation="true" ValidationGroup="LTI" />
                            </div>

                            <insite:Container runat="server" ID="LaunchScript" Visible="false">
                                <script type="text/javascript">
                                    (function () {
                                        const container = document.getElementById('<%= ParameterRepeater.ClientID %>');

                                        const form = document.createElement('form');
                                        form.style.visibility = 'hidden';
                                        form.action = '<%= LaunchUrl %>';
                                        form.method = 'POST';
                                        form.target = '_blank';

                                        const inputs = container.querySelectorAll('input[type="text"][data-key]');
                                        for (const input of inputs) {
                                            const formInput = document.createElement('input');
                                            formInput.type = 'hidden';
                                            formInput.name = input.dataset.key;
                                            formInput.value = input.value;
                                            form.appendChild(formInput);
                                        }

                                        document.body.appendChild(form);

                                        form.submit();

                                        setTimeout(function (form) { form.remove(); }, 500, form);
                                    })();
                                </script>
                            </insite:Container>

                        </asp:View>

                    </asp:MultiView>

                </div>
            </div>
        </div>

    </div>

</asp:Content>