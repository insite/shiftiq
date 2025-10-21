<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UnitSetup.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.UnitSetup" %>

<%@ Register Src="./PrerequisiteList.ascx" TagName="PrerequisiteList" TagPrefix="uc" %>
<%@ Register Src="./PrivacySettingsGroups.ascx" TagName="PrivacySettingsGroups" TagPrefix="uc" %>

<insite:Nav runat="server">
            
    <insite:NavItem runat="server" ID="DetailsTab" Title="Unit Details">

        <div class="row">
            <div class="col-lg-6">

                <div class="card">
                    <div class="card-body">

                        <h3>Identification</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Unit Name</label>
                            <div>
                                <insite:TextBox runat="server" ID="UnitName" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Unit Code</label>
                            <div>
                                <insite:TextBox runat="server" ID="UnitCode" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Unit Settings
                            </label>
                            <div>
                                <asp:CheckBox runat="server" ID="UnitIsAdaptive" Text="Adaptive" />
                            </div>
                        </div>
    
                        <div class="form-group mb-3">
                            <div class="float-end">
                                <span class="badge bg-custom-default">Asset # <asp:Literal runat="server" ID="UnitAsset" /></span>
                            </div>
                            <label class="form-label">Unit Identifier</label>
                            <div>
                                <asp:Literal runat="server" ID="UnitThumbprint" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Prerequisites">
        <div class="row">
            <div class="col-lg-6">
                <div class="card">
                    <div class="card-body">

                        <uc:PrerequisiteList runat="server" ID="PrerequisiteList" />

                    </div>
                </div>
            </div>
        </div>
    </insite:NavItem>

    <insite:NavItem runat="server" Title="Optional Content">

        <div class="row">
            <div class="col-lg-12">
                <div class="card">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <label class="form-label">Language</label>
                            <div>
                                <insite:ComboBox runat="server" ID="Language" AllowBlank="false" />
                            </div>
                        </div>

                        <div class="row row-translate">
                            <div class="col-md-12">
                                <asp:Repeater runat="server" ID="ContentRepeater">
                                    <ItemTemplate>
                                        <div class="form-group mb-3">
                                            <insite:DynamicControl runat="server" ID="Container" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
            
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Privacy Settings">
        <div class="card">
            <div class="card-body">
                <uc:PrivacySettingsGroups runat="server" ID="PrivacySettingsGroups" />
            </div>
        </div>
    </insite:NavItem>

</insite:Nav>

<div class="mt-3">
    <insite:SaveButton runat="server" ID="UnitSaveButton" ValidationGroup="CourseSetup" />
    <insite:CancelButton runat="server" ID="UnitCancelButton" />
</div>