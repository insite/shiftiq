<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LanguageAbilitySection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.LanguageAbilitySection" %>

<div class="card mb-3">
    <div class="card-body">

        <h4 class="card-title mb-3">Language Ability</h4>

        <div class="row">
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Language
                        <insite:RequiredValidator runat="server" ControlToValidate="Language1" ValidationGroup="Profile" Display="Dynamic" />
                    </label>
                    <insite:FindCollectionItem runat="server" ID="Language1" Enabled="false" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Language
                    </label>
                    <insite:FindCollectionItem runat="server" ID="Language2" CollectionName="Language" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Language
                    </label>
                    <insite:FindCollectionItem runat="server" ID="Language3" CollectionName="Language" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Language
                    </label>
                    <insite:FindCollectionItem runat="server" ID="Language4" CollectionName="Language" />
                </div>

            </div>
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Proficiency Level
                        <insite:RequiredValidator runat="server" ControlToValidate="LanguageLevel1" ValidationGroup="Profile" Display="Dynamic" />
                    </label>
                    <insite:LanguageLevelComboBox runat="server" ID="LanguageLevel1" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Proficiency Level
                        <insite:CustomValidator runat="server" ID="Validator2" ControlToValidate="LanguageLevel2" ValidationGroup="Profile" Display="Dynamic" />
                    </label>
                    <insite:LanguageLevelComboBox runat="server" ID="LanguageLevel2" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Proficiency Level
                        <insite:CustomValidator runat="server" ID="Validator3" ControlToValidate="LanguageLevel3" ValidationGroup="Profile" Display="Dynamic" />
                    </label>
                    <insite:LanguageLevelComboBox runat="server" ID="LanguageLevel3" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Proficiency Level
                        <insite:CustomValidator runat="server" ID="Validator4" ControlToValidate="LanguageLevel4" ValidationGroup="Profile" Display="Dynamic" />
                    </label>
                    <insite:LanguageLevelComboBox runat="server" ID="LanguageLevel4" />
                </div>

            </div>
        </div>

        <div class="row">
            <div class="col-12">
                <div class="form-group mb-3">
                    <div class="form-text">
                        <asp:Repeater runat="server" ID="ProficiencyDescriptionRepeater">
                            <ItemTemplate>
                                <div>
                                    <%# Container.DataItem %>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

            </div>
        </div>

    </div>
</div>
