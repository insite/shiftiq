<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Portal.Standards.Documents.Create" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Accordion runat="server" ID="MainAccordion">

        <insite:AccordionPanel runat="server" Title="Document" Icon="fas fa-file-alt">

            <div class="row">
                <div class="col-md-6">

                    <div runat="server" id="DocumentTypeField" class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Document Type" />
                            <insite:RequiredValidator runat="server" ControlToValidate="DocumentType" FieldName="Document Type" ValidationGroup="Documents" />
                        </label>
                        <insite:ComboBox runat="server" ID="DocumentType" EnableTranslation="true" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="Title" />
                            <insite:RequiredValidator runat="server" ControlToValidate="TitleInput" FieldName="Title" ValidationGroup="Documents" />
                        </label>
                        <insite:TextBox runat="server" ID="TitleInput" />
                    </div>

                    <div runat="server" id="BaseStandardField" class="form-group mb-3" visible="false">
                        <label class="form-label">
                            <asp:Literal runat="server" ID="BaseStandardLabel" />
                        </label>
                        <div style="margin-bottom:8px;">
                            <insite:ComboBox runat="server" ID="BaseStandardTypeSelector" Visible="false" Width="100%" />
                        </div>
                        <insite:FindStandard runat="server" ID="BaseStandardSelector" Visible="false" EnableTranslation="true" />
                    </div>

                </div>
            </div>
            
        </insite:AccordionPanel>

    </insite:Accordion>

    <div class="row">
        <div class="col-md-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Documents" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>