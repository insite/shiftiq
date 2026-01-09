<%@ Page Language="C#" CodeBehind="Describe.aspx.cs" Inherits="InSite.Admin.Achievements.Achievements.Forms.Describe" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Achievement" />

    <section runat="server" ID="NameSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-trophy me-1"></i>
            Describe Achievement
        </h2>

            <div class="row">
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Title
                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementTitle" FieldName="Title" ValidationGroup="Achievement" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementTitle" MaxLength="200" />
                                </div>
                                <div class="form-text">The descriptive title for this achievement.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Tag
                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementLabel" FieldName="Label" ValidationGroup="Achievement" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementLabel" MaxLength="50" />
                                </div>
                                <div class="form-text">The descriptive tag for this achievement.</div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Badge Image
                                </label>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="CustomBadge" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="true" Text="Enabled" />
                                        <asp:ListItem Value="false" Text="Disabled" Selected="True" />
                                    </asp:RadioButtonList>
                                </div>
                                <div class="form-text">
                                    You can use <a href="https://badge.design/" target="_blank">Open Badge Designer</a> to create your badge. File type supported: .svg
                                </div>
                            </div>
                            <div runat="server" id="BadgeImagePanel" visible="false">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Badge Image
                                    </label>
                                    <div>
                                        <insite:FileUploadV2 runat="server" ID="BadgeUpload" AllowedExtensions=".svg" FileUploadType="Image" />
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <asp:Literal ID="BadgeImage" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Credential Layout
                                </label>
                                <div>
                                    <insite:CertificateLayoutComboBox runat="server" ID="CertificateLayout" Width="100%" />
                                </div>
                                <div class="form-text">What layout should be used when this achievement is granted and then printed?</div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Reporting Status
                                </label>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="AchievementIsReported" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="true" Text="Enabled" />
                                        <asp:ListItem Value="false" Text="Disabled" />
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Achievement Type
                                </label>
                                <div>
                                    <insite:ItemNameComboBox runat="server" ID="AchievementType">
                                        <Settings CollectionName="Achievements/Templates/Types/Name" UseCurrentOrganization="true" />
                                    </insite:ItemNameComboBox>
                                </div>
                                <div class="form-text">
                                </div>
                            </div>
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Self-Declaration
                                </label>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="AchievementAllowSelfDeclared" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="true" Text="Allow" />
                                        <asp:ListItem Value="false" Text="Disallow" />
                                    </asp:RadioButtonList>
                                </div>
                                <div class="form-text">Is a learner permitted to self-declare completion of this achievement?</div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Description
                                </label>
                                <div>
                                   <insite:TextBox runat="server" ID="AchievementDescription" TextMode="MultiLine" Rows="9" MaxLength="1200" />
                               </div>
                                <div class="form-text">The description for this achievement.</div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Achievement" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
