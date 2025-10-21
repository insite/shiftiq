<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkshopQuestionAssetTable.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.WorkshopQuestionAssetTable" %>

<%@ Import Namespace="InSite.Admin.Assessments.Forms.Models" %>
<%@ Import Namespace="InSite.Domain.Banks" %>

<table class='property-grid asset-table'>

    <tr>
        <td>Asset #</td>
        <td>
            <%# Question.Asset %>.<%# Question.AssetVersion %>
        </td>
    </tr>

    <tr>
        <td>Publication</td>
        <td><%# GetEnumDescription(Question.PublicationStatus) %></td>
    </tr>

    <tr>
        <td>Condition</td>
        <td>
            <a href="#" class="editable-input status"
                data-name='<%# ElementUpdater.ElementTypes.QuestionCondition %>'
                data-type="select"
                data-pk='<%# Question.Set.Bank.Identifier + ":" + Question.Identifier %>'
                data-value='<%# HttpUtility.HtmlEncode(Question.Condition) %>'
            >
                <%# Question.Condition %>
            </a>
        </td>
    </tr>

    <tr>
        <td>Taxonomy</td>
        <td>
            <%# GetTaxonomy(Question.Classification.Taxonomy, Question.PublicationStatus) %>
        </td>
    </tr>

    <tr>
        <td>LIG</td>
        <td>
            <a href="#" class="editable-input"
                data-name='<%# ElementUpdater.ElementTypes.QuestionLIG %>'
                data-type="text"
                data-maxlength="64"
                data-pk='<%# Question.Set.Bank.Identifier + ":" + Question.Identifier %>'
            >
                <%# Question.Classification.LikeItemGroup %>
            </a>
        </td>
    </tr>

    <tr>
        <td>Reference</td>
        <td style="word-break:break-word;">
            <a href="#" class="editable-input"
                data-name='<%# ElementUpdater.ElementTypes.QuestionReference %>'
                data-type="text"
                data-maxlength="500"
                data-pk='<%# Question.Set.Bank.Identifier + ":" + Question.Identifier %>'
            >
                <%# Question.Classification.Reference %>
            </a>
        </td>
    </tr>

    <tr>
        <td>Code</td>
        <td style="word-break:break-word;">
            <a href="#" class="editable-input"
                data-name='<%# ElementUpdater.ElementTypes.QuestionCode %>'
                data-type="text"
                data-maxlength="40"
                data-pk='<%# Question.Set.Bank.Identifier + ":" + Question.Identifier %>'
            >
                <%# Question.Classification.Code %>
            </a>
        </td>
    </tr>

    <tr>
        <td>Tag</td>
        <td>
            <a href="#" class="editable-input"
                data-name='<%# ElementUpdater.ElementTypes.QuestionTag %>'
                data-type="text"
                data-maxlength="100"
                data-pk='<%# Question.Set.Bank.Identifier + ":" + Question.Identifier %>'
            >
                <%# Question.Classification.Tag %>
            </a>
        </td>
    </tr>
    
    <tr runat="server" visible='<%# Question.Fields.Count > 0 %>'>
        <td>Form</td>
        <td><%# string.Join(
            "<div style='height:8px;'></div>",
            Question.Fields.OrderBy(x => x.Section.Form.Sequence).Select(x => string.Format(
                "<div><a{6} target='_blank' href='/ui/admin/assessments/forms/workshop?bank={0}&form={1}&question={2}&panel=questions'>{3} [{4}.{5}]</a></div>",
                x.Section.Form.Specification.Bank.Identifier, 
                x.Section.Form.Identifier, 
                x.Question.Identifier, 
                HttpUtility.HtmlEncode(x.Section.Form.Name),
                x.Section.Form.Asset,
                x.Section.Form.AssetVersion,
                x.Section.Form.Identifier == FormIdentifier ? " style='font-style:italic;'" : string.Empty
            ))
        ) %></td>
    </tr>

    <tr runat="server" visible='<%# Question.Layout.Type != OptionLayoutType.None %>'>
        <td>Layout</td>
        <td><%# Question.Layout.Type %></td>
    </tr>

    <tr>
        <td>Type</td>
        <td><%# GetEnumDescription(Question.Type) %></td>
    </tr>

    <tr>
        <td>Points</td>
        <td><%# Question.Points %></td>
    </tr>

    <tr runat="server" visible='<%# Question.CutScore != null %>'>
        <td>Cut-Score</td>
        <td><%# Question.CutScore %></td>
    </tr>

    <tr runat="server" visible='<%# Question.CalculationMethod != QuestionCalculationMethod.Default %>'>
        <td>Calculation Method</td>
        <td><%# GetEnumDescription(Question.CalculationMethod) %></td>
    </tr>

    <tr runat="server" visible='<%# Question.Classification.Difficulty != null %>'>
        <td>Difficulty</td>
        <td><%# Question.Classification.Difficulty %></td>
    </tr>

    <tr runat="server" visible='<%# Question.Randomization.Enabled %>'>
        <td>Randomize</td>
        <td>Options</td>
    </tr>

    <tr runat="server" visible='<%# Question.Source != null %>'>
        <td>Source</td>
        <td runat="server" id="SourceCell"><span class='text-danger'>Not Found</span></td>
    </tr>

</table>