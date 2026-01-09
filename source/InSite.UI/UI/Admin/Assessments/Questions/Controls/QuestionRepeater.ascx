<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionRepeater" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>
<%@ Register TagPrefix="uc" TagName="OptionRepeater" Src="../../Options/Controls/OptionReadRepeater.ascx" %>

<%@ Import Namespace="InSite.Domain.Banks" %>

<asp:Repeater runat="server" ID="Repeater">

    <HeaderTemplate>
        <table class="table question-grid">
            <tbody>
    </HeaderTemplate>

    <FooterTemplate></tbody></table></FooterTemplate>

    <ItemTemplate>
        <tr data-question='<%# Eval("Identifier") %>'>

            <td style="width:70px;">
                <div class="sequence">
                    <%# DisplayBankSequence(Container.DataItem) %>
                </div>
                <div class="mb-1">
                    <insite:IconLink Name="pencil" runat="server" ToolTip="Edit Question" style="padding:8px 0 0 0; display:block;"
                        NavigateUrl='<%# CurrentSettings.GetRedirectUrl(CurrentSettings.GetChangeUrl((Question)Container.DataItem)) %>'
                        Visible='<%# CurrentSettings.ShowEditLink((Question)Container.DataItem) %>' />
                </div>
                <div class="mb-1">
                    <insite:IconLink Name="trash-alt" runat="server" ToolTip="Remove this question from the bank" style="padding:8px 0 0 0; display:block;"
                        NavigateUrl='<%# CurrentSettings.GetRedirectUrl(CurrentSettings.GetRemoveUrl((Question)Container.DataItem)) %>'
                        Visible='<%# CurrentSettings.ShowDeleteLink((Question)Container.DataItem) %>' />
                </div>
                <div class="mb-1">
                    <insite:IconLink Name="chart-bar" runat="server" ToolTip="Question Analysis" style="padding:8px 0 0 0; display:block;" 
                        NavigateUrl='<%# CurrentSettings.GetRedirectUrl(CurrentSettings.GetAnanlysisUrl((Question)Container.DataItem)) %>'
                        visible='<%# CurrentSettings.AllowAnalyse %>' />
                </div>
                <div class="mb-1">
                    <%# DisplayFormSequence(Container.DataItem) %>
                </div>
            </td>

            <td>
                <div>
                    <%# Shift.Common.Markdown.ToHtml(Eval("Content.Title") == null ? null : ((Shift.Common.MultilingualString)Eval("Content.Title")).Default) %>
                </div>

                <div runat="server" id="OptionRepeaterSection" class="mb-3" visible="false">
                    <uc:OptionRepeater runat="server" ID="OptionRepeater" />
                </div>

                <assessments:AssetTitleDisplay runat="server" ID="StandardDisplay" 
                    AssetID='<%# Eval("Standard") %>'
                    Format="<div class='alert alert-warning'>{0}</div>"
                    Visible='<%# (Guid)Eval("Standard") != Guid.Empty %>' />

                <%# DisplayRationale(Container.DataItem) %>

            </td>

            <insite:Container runat="server" Visible='<%# CurrentSettings.ShowProperties == PropertiesVisibility.Advanced %>'>
                <td style="width:340px;">
                    <table class='property-grid'>

                        <tr>
                            <td>Asset #</td>
                            <td>
                                <%# Eval("Asset") %>.<%# Eval("AssetVersion") %>
                                <asp:LinkButton runat="server" ID="PinLink" CssClass="pin-link" Visible="false" />
                            </td>
                        </tr>

                        <tr>
                            <td>Publication</td>
                            <td><%# GetEnumDescription((PublicationStatus)Eval("PublicationStatus")) %></td>
                        </tr>

                        <tr runat="server" visible='<%# Eval("Condition") != null %>'>
                            <td>Condition</td>
                            <td><%# Eval("Condition") %></td>
                        </tr>

                        <tr runat="server" visible='<%# Eval("Classification.Taxonomy") != null %>'>
                            <td>Taxonomy</td>
                            <td><%# Eval("Classification.Taxonomy") %></td>
                        </tr>

                        <tr runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Classification.LikeItemGroup")) %>'>
                            <td>LIG</td>
                            <td><%# Eval("Classification.LikeItemGroup") %></td>
                        </tr>

                        <tr runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Classification.Reference")) %>'>
                            <td>Reference</td>
                            <td style="word-break:break-word;"><%# Eval("Classification.Reference") %></td>
                        </tr>

                        <tr runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Classification.Code")) %>'>
                            <td>Code</td>
                            <td><%# Eval("Classification.Code") %></td>
                        </tr>

                        <tr runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Classification.Tag")) %>'>
                            <td>Tag</td>
                            <td><%# Eval("Classification.Tag") %></td>
                        </tr>

                        <tr runat="server" ID="TagsField" visible='<%# ((int?)Eval("Classification.Tags.Count") ?? 0) > 0 %>'>
                            <td class="align-top">Tags</td>
                            <td><%# Shift.Common.StringHelper.JoinFormat(
                                "<div class='question-tag-collection'>{Text}</div>{Items}",
                                ((IEnumerable)Eval("Classification.Tags") ?? new object[0]).Cast<Tuple<string, List<string>>>().Select(x1 => new
                                { 
                                    Text = x1.Item1,
                                    Items = string.Concat(
                                        x1.Item2.Select(x2 => string.Format(
                                            "<div class='question-tag'>{0}</div>",
                                            x2
                                        ))
                                    )
                                })) %></td>
                        </tr>

                        <tr runat="server" visible='<%# (int)Eval("Fields.Count") > 0 %>'>
                            <td class="align-top">Form</td>
                            <td><%# string.Join(
                                "<div class='pb-2'></div>",
                                ((IEnumerable<Field>)Eval("Fields")).OrderBy(x => x.Section.Form.Sequence).Select(x => string.Format(
                                    "<div><a{6} target='_blank' href='/ui/admin/assessments/forms/workshop?bank={0}&form={1}&question={2}&panel=questions'>{3} [{4}.{5}]</a></div>",
                                    x.Section.Form.Specification.Bank.Identifier, 
                                    x.Section.Form.Identifier, 
                                    x.Question.Identifier, 
                                    HttpUtility.HtmlEncode(x.Section.Form.Name),
                                    x.Section.Form.Asset,
                                    x.Section.Form.AssetVersion,
                                    x.Section.Form.Identifier == CurrentSettings.FormIdentifier ? " style='font-style:italic;'" : string.Empty
                                ))
                            ) %></td>
                        </tr>

                        <tr runat="server" visible='<%# (OptionLayoutType)Eval("Layout.Type") != OptionLayoutType.None %>'>
                            <td>Layout</td>
                            <td><%# Eval("Layout.Type") %></td>
                        </tr>

                        <tr>
                            <td>Type</td>
                            <td><%# GetEnumDescription((QuestionItemType)Eval("Type")) %></td>
                        </tr>

                        <tr runat="server" id="RubricRow">
                            <td>Rubric</td>
                            <td>
                                <asp:Literal runat="server" ID="RubricTitle" />
                            </td>
                        </tr>

                        <tr>
                            <td>Points</td>
                            <td><%# GetPoints() %></td>
                        </tr>

                        <tr runat="server" visible='<%# Eval("CutScore") != null %>'>
                            <td>Cut-Score</td>
                            <td><%# Eval("CutScore") %></td>
                        </tr>

                        <tr runat="server" visible='<%# (QuestionCalculationMethod)Eval("CalculationMethod") != QuestionCalculationMethod.Default %>'>
                            <td>Calculation Method</td>
                            <td><%# GetEnumDescription((QuestionCalculationMethod)Eval("CalculationMethod")) %></td>
                        </tr>

                        <tr runat="server" visible='<%# (int)Eval("Comments.Count") > 0 %>'>
                            <td>Comments</td>
                            <td><%# GetCommentsSummary(Eval("Comments")) %></td>
                        </tr>

                        <tr runat="server" visible='<%# Eval("Classification.Difficulty") != null %>'>
                            <td>Difficulty</td>
                            <td><%# Eval("Classification.Difficulty") %></td>
                        </tr>

                        <tr runat="server" visible='<%# (FlagType)Eval("Flag") != FlagType.None %>'>
                            <td>Flag</td>
                            <td>
                                <%# GetFlagHtml((FlagType)Eval("Flag")) %>
                                <span class="form-text"><%# Eval("Flag") %></span>
                            </td>
                        </tr>

                        <tr runat="server" visible='<%# Eval("Randomization.Enabled") %>'>
                            <td>Randomize</td>
                            <td>Options</td>
                        </tr>

                        <tr runat="server" visible='<%# Eval("Source") != null %>'>
                            <td>Source</td>
                            <td><%# GetSourceLink() %></td>
                        </tr>

                        <tr runat="server" id="GradeItemsRow">
                            <td class="align-top">Grade Item</td>
                            <td>
                                <asp:Literal runat="server" ID="GradeItemName" />
                                <asp:Repeater runat="server" ID="GradeItemList">
                                    <HeaderTemplate>
                                        <ul>
                                    </HeaderTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                    <ItemTemplate>
                                        <li><%# Eval("Name") %></li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>

                        <tr>
                            <td>Translate</td>
                            <td><%# GetLanguages() %></td>
                        </tr>

                    </table>
                </td>
            </insite:Container>
        
            <insite:Container runat="server" Visible='<%# CurrentSettings.ShowProperties == PropertiesVisibility.Basic %>'>
                <td style="width:340px;">
                    <table class='property-grid'>

                        <tr>
                            <td>Points</td>
                            <td><%# GetPoints() %></td>
                        </tr>

                        <tr>
                            <td>Type</td>
                            <td><%# GetEnumDescription((QuestionItemType)Eval("Type")) %></td>
                        </tr>

                    </table>
                </td>
            </insite:Container>

        </tr>
    </ItemTemplate>

</asp:Repeater>

<insite:PageHeadContent runat="server" ID="GridStyle">
    <style type="text/css">

        .question-grid > tbody > tr > td {
            padding: 20px 0 20px 20px;
        }

        .question-grid img,
        .question-grid iframe{
            max-width: 100% !important;
        }

        .question-grid .labels a {
            margin-right: 3px;
            color: white !important;
        }

            .question-grid .labels a:hover {
                text-decoration: none;
            }

                .question-grid .labels a:hover span {
                    background-color: #333;
                }

            .question-grid .labels a.label {
                white-space: normal !important;
            }

        .question-grid > thead > tr > th:first-child {
            text-align: center;
        }

        .question-grid > tbody > tr > td:first-child {
            vertical-align: top;
            text-align: center;
            width: 140px;
            border-left: 1px solid transparent;
        }

            .question-grid > tbody > tr > td:first-child > div.sequence {
                font-size: 27px;
                color: #265F9F;
                white-space: nowrap;
            }

            .question-grid > tbody > tr:first-child > td {
                border-top: none;
            }

        .question-grid table.property-grid {
            width: 100%;
        }

        .question-grid table.property-grid > tbody > tr:nth-child(even) {
            background-color: #F5F5F5;
        }

        .question-grid table.property-grid > tbody > tr > td {
            border: 1px solid #ddd;
            padding: 5px !important;
            vertical-align: top;
        }

        .question-grid table.property-grid > tbody > tr > td .question-tag {
            padding-left:15px;
        }

    </style>
</insite:PageHeadContent>