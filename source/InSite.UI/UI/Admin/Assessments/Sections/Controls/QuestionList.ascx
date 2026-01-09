<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionList.ascx.cs" Inherits="InSite.Admin.Assessments.Sections.Controls.QuestionList" %>

<%@ Register TagPrefix="uc" TagName="QuestionRepeater" Src="../../Questions/Controls/QuestionRepeater.ascx" %>

<asp:Repeater runat="server" ID="CriterionRepeater">
    <ItemTemplate>
        <div class="row sieve-title">
            <div class="col-lg-12">
                <h3>
                    <%# Eval("Title") %>
                </h3>

                <div runat="server" id="NoQuestions" class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    There are no questions that satisfy the criteria.
                </div>

                <uc:QuestionRepeater runat="server" ID="QuestionRepeater" />
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        div.sieve-title h3 {
            color: #777;
            margin-top: 0px;
            background-color: #f5f5f5;
            padding: 5px;
            font-size: 20px;
        }
    </style>
</insite:PageHeadContent>
