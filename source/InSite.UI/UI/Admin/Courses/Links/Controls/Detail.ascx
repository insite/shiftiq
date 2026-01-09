<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Courses.Links.Controls.Detail" %>

<div class="row">
                    
    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Title
                        <insite:RequiredValidator runat="server" ControlToValidate="Title" ValidationGroup="Lti Link" Display="Dynamic" />
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Title" MaxLength="100" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Description
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Description" MaxLength="100" TextMode="MultiLine" Rows="3" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Custom Parameters
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="CustomParameters" TextMode="MultiLine" Rows="10" />
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="form-group mb-3">
                    <label class="form-label">
                        URL
                        <insite:RequiredValidator runat="server" ControlToValidate="Url" ValidationGroup="Lti Link" Display="Dynamic" />
                        <insite:UrlValidator runat="server" ControlToValidate="Url" FieldName="Url" ValidationGroup="Lti Link" Display="Dynamic" />
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Url" MaxLength="500" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Code
                        <insite:RequiredValidator runat="server" ControlToValidate="Code" ValidationGroup="Lti Link" Display="Dynamic" />
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Code" MaxLength="20" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Publisher
                                <insite:RequiredValidator runat="server" ControlToValidate="Publisher" ValidationGroup="Lti Link" Display="Dynamic" />
                            </label>
                            <div>
                                <insite:TextBox ID="Publisher" runat="server" />
                            </div>
                        </div>

                    </div>
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Type
                                <insite:RequiredValidator runat="server" ControlToValidate="Subtype" ValidationGroup="Lti Link" Display="Dynamic" />
                            </label>
                            <div>
                                <insite:ComboBox ID="Subtype" runat="server" Width="100%">
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Value="LTI" Text="LTI" />
                                        <insite:ComboBoxOption Value="PanGlobal" Text="PanGlobal" />
                                    </Items>
                                </insite:ComboBox>
                            </div>
                        </div>

                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Location
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Location" MaxLength="100" />
                    </div>
                </div>

                <div class="row">

                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Key
                                <insite:RequiredValidator runat="server" ControlToValidate="Key" ValidationGroup="Lti Link" Display="Dynamic" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="Key" MaxLength="20" />
                            </div>
                        </div>

                    </div>
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Secret
                                <insite:RequiredValidator runat="server" ControlToValidate="Secret" ValidationGroup="Lti Link" Display="Dynamic" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="Secret" MaxLength="100" />
                            </div>
                        </div>

                    </div>

                </div>

            </div>
        </div>

    </div>

</div>