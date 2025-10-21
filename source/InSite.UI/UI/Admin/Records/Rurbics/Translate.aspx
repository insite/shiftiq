<%@ Page Language="C#" CodeBehind="Translate.aspx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Translate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Records/Rurbics/Controls/TranslateField.ascx" TagName="TranslateField" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/TranslationWindow.ascx" TagName="TranslationWindow" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <section class="mb-3">

        <div class="float-end">
            From
            <span runat="server" id="FromLanguageOutput" class="fw-bold ms-2 d-inline-block border py-2 px-3 rounded-2 lh-lg"></span>
            <span class="mx-2">to</span>
            <insite:LanguageComboBox runat="server" ID="ToLanguage" Width="150px" EmptyMessage="Translate To" />
        </div>

        <h2 class="h4 mb-3">Rubric</h2>

        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

        <insite:UpdatePanel runat="server" ID="UpdatePanel" CssClass="card border-0 shadow-lg">
            <ContentTemplate>
                <div class="card-body">

                    <uc:TranslateField runat="server" ID="FieldTitle" FieldName="Rubric Title" />
                    <uc:TranslateField runat="server" ID="FieldDescription" FieldName="Rubric Description" />

                    <asp:Repeater runat="server" ID="CriteriaRepeater">
                        <ItemTemplate>
                            <div class="mt-3 ms-3 mb-4 ps-4 border-start border-success-subtle border-4">
                                <h3>
                                    <span class="badge text-bg-success align-text-bottom me-1"><%# Eval("CriterionSequence") %></span>
                                    <%# Eval("CriterionTitle") %>
                                </h3>

                                <uc:TranslateField runat="server" ID="FieldTitle" FieldName="Criterion Title"
                                    AllowTranslate='<%# ToLanguageSelected %>'
                                    TextOriginal='<%# GetTitleFrom("RubricCriterionIdentifier") %>'
                                    TextTranslated='<%# GetTitleTo("RubricCriterionIdentifier") %>'
                                    OnButtonClientClick='<%# GetWindowOpenScript("RubricCriterionIdentifier", FieldLabel.Title) %>' />
                                <uc:TranslateField runat="server" ID="FieldDescription" FieldName="Criterion Description"
                                    AllowTranslate='<%# ToLanguageSelected %>'
                                    TextOriginal='<%# GetDescriptionFrom("RubricCriterionIdentifier") %>'
                                    TextTranslated='<%# GetDescriptionTo("RubricCriterionIdentifier") %>'
                                    OnButtonClientClick='<%# GetWindowOpenScript("RubricCriterionIdentifier", FieldLabel.Description) %>' />

                                <asp:Repeater runat="server" ID="RatingRepeater">
                                    <ItemTemplate>
                                        <div class="mt-3 ms-3 ps-4 border-start border-info-subtle border-4">
                                            <h3>
                                                <span class="badge text-bg-info align-text-bottom me-1"><%# $"{Eval("RubricCriterion.CriterionSequence")}.{Eval("RatingSequence")}" %></span>
                                                <%# Eval("RatingTitle") %>
                                            </h3>

                                            <uc:TranslateField runat="server" ID="FieldTitle" FieldName="Rating Title"
                                                AllowTranslate='<%# ToLanguageSelected %>'
                                                TextOriginal='<%# GetTitleFrom("RubricRatingIdentifier") %>'
                                                TextTranslated='<%# GetTitleTo("RubricRatingIdentifier") %>'
                                                OnButtonClientClick='<%# GetWindowOpenScript("RubricRatingIdentifier", FieldLabel.Title) %>' />
                                            <uc:TranslateField runat="server" ID="FieldDescription" FieldName="Rating Description"
                                                AllowTranslate='<%# ToLanguageSelected %>'
                                                TextOriginal='<%# GetDescriptionFrom("RubricRatingIdentifier") %>'
                                                TextTranslated='<%# GetDescriptionTo("RubricRatingIdentifier") %>'
                                                OnButtonClientClick='<%# GetWindowOpenScript("RubricRatingIdentifier", FieldLabel.Description) %>' />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <div></div>
                        </SeparatorTemplate>
                    </asp:Repeater>

                    <asp:Button runat="server" ID="UpdateButton" Style="display: none;" />

                </div>
            </ContentTemplate>
        </insite:UpdatePanel>

    </section>

    <div class="mt-3">
        <insite:CloseButton runat="server" ID="CloseButton" />
    </div>

    <uc:TranslationWindow runat="server" ID="TranslationWindow" OrganizationTypeVisible="false" AllowOrganizationSpecific="false" />

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            .lang-selector {
                font-size: 1.1rem;
            }
        </style>
    </insite:PageHeadContent>

</asp:Content>
