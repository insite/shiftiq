<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineEdit.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.OutlineEdit" %>

<insite:Container runat="server" ID="EditorContainer" Visible="false">
    <div class="card">
        <div class="card-body">
            <h3 runat="server" id="Header" class="mb-4"></h3>

            <insite:Alert runat="server" ID="EditorStatus" />

            <div class="form-group mb-3">
                <label class="form-label">
                    Standard Type
                    <insite:RequiredValidator runat="server" ControlToValidate="StandardType" FieldName="Standard Type" ValidationGroup="OutlineEditor" />
                </label>
                <div>
                    <insite:StandardTypeComboBox runat="server" ID="StandardType" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Title
                    <insite:RequiredValidator runat="server" ControlToValidate="Title" FieldName="Title" ValidationGroup="OutlineEditor" />
                </label>
                <div>
                    <insite:TextBox runat="server" ID="Title" MaxLength="256" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Summary
                </label>
                <div>
                    <insite:TextBox runat="server" ID="Summary" TextMode="MultiLine" Rows="4" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Code
                </label>
                <div>
                    <insite:TextBox runat="server" ID="Code" MaxLength="32" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Properties
                </label>
                <div>
                    <insite:CheckBox runat="server" ID="IsTheory" Text="Theory" />
                    <insite:CheckBox runat="server" ID="IsPractical" Text="Practical" />
                </div>
                <div class="clearfix"></div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Credit Identifier
                </label>
                <div>
                    <insite:TextBox runat="server" ID="CreditIdentifier" MaxLength="32" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Level
                </label>
                <div>
                    <div class="d-inline-block w-50">
                        <insite:TextBox runat="server" ID="LevelType" MaxLength="32" EmptyMessage="Type" />
                    </div>
                    <div class="d-inline-block w-25">
                        <insite:TextBox runat="server" ID="LevelCode" MaxLength="1" EmptyMessage="Code" />
                    </div>
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    Version
                </label>
                <div>
                    <div class="d-inline-block w-25">
                        <insite:TextBox runat="server" ID="MajorVersion" MaxLength="8" EmptyMessage="Major" />
                    </div>
                    <div class="d-inline-block w-25">
                        <insite:TextBox runat="server" ID="MinorVersion" MaxLength="8" EmptyMessage="Minor" />
                    </div>
                </div>
            </div>

            <div class="form-group mb-3">
                <div>
                    <insite:Button runat="server" ID="IndentButton" ButtonStyle="Default" Icon="far fa-indent" Text="Indent" />
                    <insite:Button runat="server" ID="OutdentButton" ButtonStyle="Default" Icon="far fa-outdent" Text="Outdent" />
                    <insite:Button runat="server" ID="ReorderStartButton" ButtonStyle="Default" Icon="fas fa-sort" Text="Reorder" />
                    <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" ValidationGroup="OutlineEditor" />
                    <insite:DeleteButton runat="server" ID="DeleteButton" />
                </div>
            </div>

            <hr />

            <h4 class="mt-3 mb-4">
                Create New Standard
                <insite:RequiredValidator runat="server" ControlToValidate="CreatorTitles" FieldName="New Titles" ValidationGroup="OutlineCreator" Display="None" RenderMode="Dot" />
            </h4>

            <insite:Alert runat="server" ID="CreatorStatus" />

            <div class="form-group mb-3">
                <div class="col-lg-12 col-subtypes">
                    <asp:Repeater runat="server" ID="CreatorTypeRepeater">
                        <ItemTemplate>
                            <insite:RadioButton runat="server" ID="RadioButton"
                                GroupName="CreatorStandardType" Text='<%# Container.DataItem %>' Checked='<%# Container.ItemIndex == 0 %>' />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div class="clearfix"></div>
            </div>

            <div class="form-group mb-3">
                <div class="col-lg-12">
                    <insite:TextBox runat="server" ID="CreatorTitles" TextMode="MultiLine" Rows="6" style="resize:none;" autocomplete="off" />
                </div>
            </div>

            <div class="form-group mb-3">
                <insite:Button runat="server" ID="CreateButton" ButtonStyle="Success" Icon="fas fa-plus-circle" 
                    Text="Create" CausesValidation="true" ValidationGroup="OutlineCreator" />
            </div>
        </div>
    </div>
</insite:Container>

<insite:Container runat="server" ID="ReorderContainer" Visible="false">
    <div class="row mt-3">
        <div class="col-lg-12">
            <asp:Repeater runat="server" ID="ReorderRepeater">
                <HeaderTemplate><ul class="list-group" data-reorder="#<%# ReorderState.ClientID %>"></HeaderTemplate>
                <FooterTemplate></ul></FooterTemplate>
                <ItemTemplate>
                    <li class="list-group-item" data-number='<%# Eval("Number") %>'>
                        <i runat="server" class='<%# Eval("Icon", "fa {0}") %>' visible='<%# !string.IsNullOrEmpty((string)Eval("Icon")) %>'></i>
                        <%# Eval("Type") %> #<%# Eval("Number") %>: <%# Eval("Title") %>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div class="row mt-3">
        <div>
            <insite:CancelButton runat="server" ID="ReorderCancelButton" />
            <insite:SaveButton runat="server" ID="ReorderSaveButton" />
            <asp:HiddenField runat="server" ID="ReorderState" />
        </div>
    </div>
</insite:Container>

