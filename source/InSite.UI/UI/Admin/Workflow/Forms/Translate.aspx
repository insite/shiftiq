<%@ Page Language="C#" CodeBehind="Translate.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Translate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/TranslationWindow.ascx" TagName="TranslationWindow" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ComparisonStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="SurveyTranslator" />

    <section runat="server" id="TranslationSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-check-square me-1"></i>
            Translation
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="section-panel">
                    <div class="section-odd">

                        <div class="float-end">
                            <asp:Label runat="server" ID="PageCount" CssClass="badge bg-success" />
                            <asp:Label runat="server" ID="QuestionCount" CssClass="badge bg-info" />
                            <asp:Label runat="server" ID="FieldCount" CssClass="badge bg-custom-default" />
                        </div>

                        <table class="translation">
                            <tr>
                                <td class="translation-label">&nbsp;
                                </td>
                                <td class="translation-text" style="font-size: 1.4em;">
                                    <asp:Literal runat="server" ID="FromLanguage" />
                                </td>
                                <td class="translation-text">
                                    <insite:LanguageComboBox runat="server" ID="ToLanguage" Width="200" CssClass="ToLanguage" EmptyMessage="Translate To" />
                                </td>
                                <td class="translation-commands">&nbsp;
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>

                <asp:Repeater runat="server" ID="SurveyFieldRepeater">
                    <ItemTemplate>
                        <div class="section-panel">
                            <h1><%# Eval("Title") %></h1>
                            <div class="section-odd">
                                <table class="translation">
                                    <tr>
                                        <td class="translation-label"></td>
                                        <td class="translation-text">
                                            <%# Eval("FromText") %>
                                        </td>
                                        <td class="translation-text">
                                            <%# Eval("ToText") %>
                                        </td>
                                        <td class="translation-commands">
                                            <insite:IconButton runat="server" OnClientClick='<%# (string)Eval("OnClick") %>' Visible='<%# _languageSelected %>' Name="pencil" Style="vertical-align: text-bottom;" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Repeater runat="server" ID="PageRepeater">
                    <ItemTemplate>
                        <div class="section-panel">
                            <h1>Page <%# Eval("Title") %></h1>

                            <asp:Repeater runat="server" ID="QuestionRepeater">
                                <ItemTemplate>
                                    <div class='<%# Container.ItemIndex % 2 == 0 ? "section-odd" : "section" %>'>
                                        <table class="translation">

                                            <tr class="question">
                                                <td class="t-label">
                                                    <%# Eval("Title") %>
                                                </td>
                                                <td class="t-text">
                                                    <%# Eval("FromText") %>
                                                </td>
                                                <td class="t-text">
                                                    <%# Eval("ToText") %>
                                                </td>
                                                <td class="t-cmds">
                                                    <insite:IconButton runat="server" OnClientClick='<%# (string)Eval("OnClick") %>' Visible='<%# _languageSelected %>' Name="pencil" Style="vertical-align: text-bottom;" />
                                                </td>
                                            </tr>

                                            <asp:Repeater runat="server" ID="OptionListRepeater">
                                                <ItemTemplate>

                                                    <insite:Container runat="server" Visible='<%# Eval("FieldVisible") %>'>
                                                        <tr class="option-list">
                                                            <td class="t-label">
                                                                <div class="form-text">L<%# Eval("Title") %>.</div>
                                                            </td>
                                                            <td class="t-text">
                                                                <%# Eval("FromText") %>
                                                            </td>
                                                            <td class="t-text">
                                                                <%# Eval("ToText") %>
                                                            </td>
                                                            <td class="t-cmds">
                                                                <insite:IconButton runat="server" OnClientClick='<%# (string)Eval("OnClick") %>' Visible='<%# _languageSelected %>' Name="pencil" Style="vertical-align: text-bottom;" />
                                                            </td>
                                                        </tr>
                                                    </insite:Container>

                                                    <asp:Repeater runat="server" ID="OptionRepeater">
                                                        <ItemTemplate>
                                                            <tr class="option">
                                                                <td class="t-label">
                                                                    <div class="form-text">O<%# Eval("Title") %>.</div>
                                                                </td>
                                                                <td class="t-text">
                                                                    <%# Eval("FromText") %>
                                                                </td>
                                                                <td class="t-text">
                                                                    <%# Eval("ToText") %>
                                                                </td>
                                                                <td class="t-cmds">
                                                                    <insite:IconButton runat="server" OnClientClick='<%# (string)Eval("OnClick") %>' Visible='<%# _languageSelected %>' Name="pencil" Style="vertical-align: text-bottom;" />
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>

                                                </ItemTemplate>
                                            </asp:Repeater>

                                        </table>

                                        <insite:Container runat="server" ID="LikertScaleContainer" Visible="false">
                                            <h3>Likert Scales</h3>

                                            <asp:Repeater runat="server" ID="LikertScaleRepeater">
                                                <ItemTemplate>
                                                    <h5><%# Eval("Category") %></h5>

                                                    <asp:Repeater runat="server" ID="ItemRepeater">
                                                        <HeaderTemplate>
                                                            <table class="translation">
                                                        </HeaderTemplate>
                                                        <FooterTemplate></table></FooterTemplate>
                                                        <ItemTemplate>
                                                            <tr class="option">
                                                                <td class="t-label">
                                                                    <div class="form-text"><%# Eval("Title") %>.</div>
                                                                </td>
                                                                <td class="t-text">
                                                                    <%# Eval("FromText") %>
                                                                </td>
                                                                <td class="t-text">
                                                                    <%# Eval("ToText") %>
                                                                </td>
                                                                <td class="t-cmds">
                                                                    <insite:IconButton runat="server" OnClientClick='<%# (string)Eval("OnClick") %>' Visible='<%# _languageSelected %>' Name="pencil" Style="vertical-align: text-bottom;" />
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                        </insite:Container>

                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <asp:Button runat="server" ID="UpdateButton" Style="display: none;" />

    <uc:TranslationWindow runat="server" ID="TranslationWindow" OrganizationTypeVisible="false" AllowOrganizationSpecific="false" />

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            .translation-text{
                overflow-wrap: anywhere;
            }
            div.section,
            div.section-odd {
                padding: 10px;
                width: 100%;
            }
            div.section-odd {
                background-color: #F4F4F4;
            }

            .translate-button { float: right; }

            table.translation tr td {
                padding: 5px;
            }
                table.translation tr td.t-label {
                    width: 30px;
                    vertical-align: top;
                }

                table.translation tr td.t-text {
                    width: 400px;
                    vertical-align: top;
                }

                table.translation tr td.t-cmds {
                    width: 30px;
                    vertical-align: top;
                }

            table.translation tr.question td {
                font-weight: bold;
                font-size: 1.1em;
            }

            table.translation tr.option td.text {
                font-size: 0.8em !important;
                padding-left: 25px !important;
            }

            table.translation {
                border-collapse: collapse;
                width: 800px;
            }

                table.translation > tr:nth-child(odd), table.translation > tbody > tr:nth-child(odd) {
                    background-color: #f4f4f4;
                }

                table.translation > tr:nth-child(even), table.translation > tbody > tr:nth-child(even) {
                    background-color: #FFFFFF;
                }

                table.translation tr td.text {
                    border-left: solid 1px #d4d4d4;
                }
        </style>
    <link href="/UI/Admin/assets/contents/forms/translate.css" rel="stylesheet" type="text/css" />
</insite:PageHeadContent>

</asp:Content>
