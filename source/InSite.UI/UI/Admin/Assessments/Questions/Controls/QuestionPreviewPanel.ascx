<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPreviewPanel.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPreviewPanel" %>

<div class="card card-question mb-4 bg-secondary">
    <div class="card-header border-bottom-0">
        <asp:Repeater runat="server" ID="LabelRepeater">
            <HeaderTemplate><div class="float-end"></HeaderTemplate>
            <FooterTemplate></div></FooterTemplate>
            <ItemTemplate>
                <span class='badge bg-<%# Eval("Indicator") %>'><%# Eval("Title") %></span>
            </ItemTemplate>
        </asp:Repeater>

        <h4 runat="server" id="QuestionTitle"></h4>
        <div runat="server" id="QuestionText" class="question-text"></div>
    </div>

    <div class="card-body bg-white">
        <insite:DynamicControl runat="server" ID="QuestionContainer" />
    </div>
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .card-question img,
        .card-question iframe {
            max-width: 100% !important;
        }
    </style>
</insite:PageHeadContent>