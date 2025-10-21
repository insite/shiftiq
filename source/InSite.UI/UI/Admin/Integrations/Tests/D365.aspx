<%@ Page Language="C#" CodeBehind="D365.aspx.cs" Inherits="InSite.UI.Admin.Integrations.Tests.D365" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="D365" />

    <div class="row">

        <div class="col-md-6">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        Parameters
                    </h4>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Method
                            <insite:RequiredValidator runat="server" ControlToValidate="MethodSelector" FieldName="Method" ValidationGroup="D365" />
                        </label>
                        <div>
                            <insite:ComboBox runat="server" ID="MethodSelector">
                                <Items>
                                    <insite:ComboBoxOption Text="Add Event" Value="D365AddEvent" />
                                    <insite:ComboBoxOption Text="Cancel Event" Value="D365CancelEvent" />
                                    <insite:ComboBoxDivider />
                                    <insite:ComboBoxOption Text="Add Registration" Value="D365AddRegistration" />
                                    <insite:ComboBoxOption Text="Transfer Registration" Value="D365TransferRegistration" />
                                    <insite:ComboBoxOption Text="Cancel Registration" Value="D365CancelRegistration" />
                                </Items>
                            </insite:ComboBox>
                        </div>
                    </div>

                    <insite:UpdatePanel runat="server">
                        <ContentTemplate>
                            <insite:DynamicControl runat="server" ID="MethodContainer" />
                        </ContentTemplate>
                    </insite:UpdatePanel>

                    <div class="mt-3">
                        <insite:Button runat="server" ID="SendButton" Text="Send Request" ButtonStyle="Primary"
                            Icon="fas fa-bolt" CausesValidation="true" ValidationGroup="D365" />
                    </div>

                </div>
            </div>
        </div>

        <asp:Panel runat="server" ID="RequestPanel" CssClass="col-md-6" Visible="false">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        Request
                    </h4>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            URL
                        </label>
                        <insite:TextBox runat="server" ID="RequestUrl" ReadOnly="true" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Body
                        </label>
                        <insite:TextBox runat="server" ID="RequestBody" TextMode="MultiLine" ReadOnly="true" />
                    </div>

                    <hr class="my-3" />

                    <h4 class="card-title mb-3">
                        Response
                    </h4>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Status
                        </label>
                        <insite:TextBox runat="server" ID="ResponseStatus" ReadOnly="true" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Body
                        </label>
                        <insite:TextBox runat="server" ID="ResponseBody" TextMode="MultiLine" ReadOnly="true" Rows="10" />
                    </div>

                </div>
            </div>
        </asp:Panel>

    </div>
    
    <insite:PageHeadContent runat="server">
        <style type="text/css">

            div.json-editor {
                height: 250px;
            }

            div.json-editor > div {
                padding: 6px 12px;
                border: 1px solid #ccc;
                border-radius: 4px;
                width: 100%;
                height: 100%;
            }

        </style>
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">

        <insite:ResourceLink runat="server" Type="JavaScript" Url="/UI/Layout/common/parts/plugins/ace.cloud9/ace.js" />

        <script type="text/javascript">
            (function () {

                initJsonEditor('<%= RequestBody.ClientID %>');

                function initJsonEditor(id) {
                    const input = document.getElementById(id);
                    if (!input || input.nodeName !== 'TEXTAREA')
                        return;

                    input.classList.add('d-none');

                    const wrapper = document.createElement('div');
                    wrapper.className = 'json-editor';

                    const editor = document.createElement('div');

                    wrapper.appendChild(editor);
                    input.after(wrapper);

                    var aceEditor = ace.edit(editor, {
                        minLines: 15
                    });

                    aceEditor.$blockScrolling = Infinity;
                    aceEditor.setFontSize(15);
                    aceEditor.setShowPrintMargin(false);
                    aceEditor.session.setMode('ace/mode/json');

                    aceEditor.session.setValue(input.value);
                }
            })();
        </script>
    
    </insite:PageFooterContent>

</asp:Content>