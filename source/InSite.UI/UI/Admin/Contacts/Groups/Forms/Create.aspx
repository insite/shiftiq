<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Contacts.Groups.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Group" />

    <div class="row mb-3">
        <div class="col-lg-6">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-users me-1"></i>
                        Group
                    </h4>

                    <div class="form-group mb-3">
                        <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Group" />
                    </div>

                    <asp:MultiView runat="server" ID="CreateMultiView">
                        <asp:View runat="server" ID="ViewNewSection">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="SingleGroupName" FieldName="Group Name" ValidationGroup="Group" />
                                </label>
                                <insite:TextBox runat="server" ID="SingleGroupName" MaxLength="90" />
                                <div class="form-text">
                                    The name of the group should be unique and descriptive.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Type
                                    <insite:RequiredValidator runat="server" ControlToValidate="SingleGroupType" FieldName="Group Type" ValidationGroup="Group" />
                                </label>
                                <insite:GroupTypeComboBox runat="server" ID="SingleGroupType" AllowBlank="false" />
                                <div class="form-text">
                                    Group type determines the features available for using and managing it in the system.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Tag
                                </label>
                                <insite:TextBox runat="server" ID="SingleGroupLabel" MaxLength="100" />
                                <div class="form-text">
                                    Optional: You can tag contact groups to make them easier to find.
                                </div>
                            </div>

                        </asp:View>
                        <asp:View runat="server" ID="ViewMultipleSection">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Names
                                    <insite:RequiredValidator runat="server" ControlToValidate="MultipleGroupName" FieldName="Group Names" ValidationGroup="Group" />
                                </label>
                                <insite:TextBox runat="server" ID="MultipleGroupName" TextMode="MultiLine" Rows="10" />
                                <div class="form-text">
                                    Enter your group names with one item per line.
                                    <br />
                                    The name of the group should be unique and descriptive.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Type
                                    <insite:RequiredValidator runat="server" ControlToValidate="MultipleGroupType" FieldName="Group Type" ValidationGroup="Group" />
                                </label>
                                <insite:GroupTypeComboBox runat="server" ID="MultipleGroupType" AllowBlank="false" />
                                <div class="form-text">
                                    Group type determines the features available for using and managing it in the system.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Group Tag
                                </label>
                                <insite:TextBox runat="server" ID="MultipleGroupLabel" MaxLength="100" />
                                <div class="form-text">
                                    Optional: You can tag contact groups to make them easier to find.
                                </div>
                            </div>

                        </asp:View>
                    </asp:MultiView>

                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Group" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
