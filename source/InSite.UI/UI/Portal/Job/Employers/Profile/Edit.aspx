<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.MyProfile.Edit" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div runat="server" id="EditDetail">

    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

    <insite:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>

            <div class="row mb-3">

                <div class="col-12">

                    <div class="card">

                        <div class="card-body">

                            <h4 runat="server" id="CompanyHeading" class="card-title mb-3">Company Information</h4>

                            <div class="row">
                                <div class="col-6">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            <span runat="server" id="EmployerLabel">Company Name</span>
                                            <insite:RequiredValidator runat="server" FieldName="Job" ControlToValidate="EmpCompanyName" ValidationGroup="Profile" Display="Dynamic" />
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpCompanyName" MaxLength="90" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Industry
                                        </label>
                                        <insite:IndustriesComboBox runat="server" ID="EmpIndustry" DistinctValuesOnly="true" EmptyMessage="Select industry" ClientEvents-OnChange="editMyProfile.onIndustryChanged" />

                                        <div class="mt-3">
                                            <insite:TextBox runat="server" ID="EmpIndustryDetails" MaxLength="100" EmptyMessage="Please specify other" style="display:none" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Number of Employees
                                        </label>
                                        <insite:NumberOfEmployeesComboBox runat="server" ID="EmpNumber" />
                                    </div>

                                </div>
                                <div class="col-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Description
                                        </label>
                                        <insite:TextBox runat="server" ID="GroupDescription" TextMode="MultiLine" Rows="10" />
                                    </div>

                                </div>
                            </div>

                        </div>
            
                    </div>

                </div>

            </div>

            <div class="row mb-3">

                <div class="col-12">

                    <div class="card">

                        <div class="card-body">

                            <div class="row">
                                <div class="col-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Main Phone
                                            <insite:RequiredValidator runat="server" FieldName="Office Phone" ControlToValidate="EmpPhone" ValidationGroup="Profile" Display="Dynamic" />
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpPhone" MaxLength="14" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Head Office Address
                                            <insite:RequiredValidator runat="server" FieldName="Head Office Address" ControlToValidate="EmpAddressLine" ValidationGroup="Profile" Display="Dynamic" />
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpAddressLine" MaxLength="128" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            City
                                            <insite:RequiredValidator runat="server" FieldName="City" ControlToValidate="EmpAddressCity" ValidationGroup="Profile" Display="Dynamic" />
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpAddressCity" MaxLength="128" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Country
                                            <insite:RequiredValidator runat="server" FieldName="Country" ControlToValidate="CountrySelector" ValidationGroup="Profile" Display="Dynamic" />
                                        </label>
                                        <insite:FindCountry ID="CountrySelector" runat="server" EmptyMessage="Country" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Province
                                        </label>
                                        <insite:ProvinceComboBox runat="server" ID="EmpAddressProvince" Country="Canada" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Postal Code / Zip
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpAddressPostalCode" MaxLength="16" />
                                    </div>

                                </div>
                                <div class="col-6">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Company Website
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpWebsite" MaxLength="500" EmptyMessage="https://..." />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Facebook Link
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpSocialFacebook" MaxLength="128" EmptyMessage="https://..." />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Instagram Link
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpSocialInstagram" MaxLength="128" EmptyMessage="https://..." />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Twitter Link
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpSocialTwitter" MaxLength="128" EmptyMessage="https://..." />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            YouTube Link
                                        </label>
                                        <insite:TextBox runat="server" ID="EmpSocialYouTube" MaxLength="128" EmptyMessage="https://..." />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Company Email
                                        </label>
                                        <insite:TextBox runat="server" ID="GroupEmail" MaxLength="254" />
                                        <div class="form-text">for general inquiries</div>
                                    </div>

                                    <div runat="server" id="GroupTagsField" class="form-group mb-3">
                                        <label class="form-label">
                                            Type of Service(s) Offered
                                        </label>
                                        <asp:Repeater runat="server" ID="GroupTagList">
                                            <ItemTemplate>
                                                <insite:CheckBox runat="server" ID="IsSelected"
                                                    Text='<%# Eval("ItemName") %>' Checked='<%# Eval("IsSelected") %>' />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>

                                    <div runat="server" id="OccupationsField" class="form-group mb-3">
                                        <label class="form-label">
                                            Occupations We Seek
                                        </label>
                                        <asp:Repeater runat="server" ID="OccupationList">
                                            <ItemTemplate>
                                                <insite:CheckBox runat="server" ID="IsSelected"
                                                    Text='<%# Eval("Html") %>' Checked='<%# Eval("IsSelected") %>' />
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>

                                </div>
                            </div>

                        </div>
            
                    </div>

                </div>

            </div>

        </ContentTemplate>
    </insite:UpdatePanel>

    <div class="row">
        <div class="col-10">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Profile" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

    <insite:PageFooterContent runat="server">
        <script>
            (function () {
                var instance = window.editMyProfile = window.editMyProfile || {};

                instance.onIndustryChanged = () => {
                    var industry = $('#<%= EmpIndustry.ClientID %> option:selected').text().trim();

                    if (industry && industry.toLowerCase() === "other") {
                        $("#<%= EmpIndustryDetails.ClientID %>").show();
                    } else {
                        $("#<%= EmpIndustryDetails.ClientID %>").hide();
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>

    </div>

</asp:Content>