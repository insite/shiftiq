<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudentMarksReport.ascx.cs" Inherits="InSite.Admin.Records.Reports.Gradebooks.Controls.StudentMarksReport" %>

<style type="text/css">
    * {
        -webkit-box-sizing: border-box;
        -moz-box-sizing: border-box;
        box-sizing: border-box;
    }

    body {
        font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
        font-size: 14px;
        line-height: 1.42857143;
        color: #333;
        background-color: #fff;
        margin: 0;
    }

    h1 {
        font-size: 36px;
        margin-top: 20px;
        margin-bottom: 10px;
        font-family: inherit;
        font-weight: 500;
        line-height: 1.1;
        color: inherit;
    }

    table {
        background-color: transparent;
        border-spacing: 0;
        border-collapse: collapse;
    }

    th {
        text-align: left;
    }

    .table {
        width: 100%;
        max-width: 100%;
        margin-bottom: 20px;
    }

    .table > thead > tr > th, .table > tbody > tr > th, .table > tfoot > tr > th, .table > thead > tr > td, .table > tbody > tr > td, .table > tfoot > tr > td {
        padding: 8px;
        line-height: 1.42857143;
        vertical-align: top;
        border-top: 1px solid #ddd;
    }

    .table > thead > tr > th {
        vertical-align: bottom;
        border-bottom: 2px solid #ddd;
    }

    .table > thead:first-child > tr:first-child > th {
        border-top: 0;
    }

    .table-striped > tbody > tr:nth-of-type(2n+1) {
        background-color: #f9f9f9;
    }

    .form-group {
        margin-bottom: 15px;
    }

    div, table td, table tr, table th {
        font-family: Arial;
    }

    .form-group label {
        font-weight: bold;
    }

    table td {
        padding: 5px;
        margin: 5px;
    }

    table tr.ita-item td {
        padding-bottom: 30px;
        padding-top: 30px;
    }

    .special-score {
        font-weight: bold;
    }
</style>

<div>

    <table style="width:100%;">
        <tr>
            <td style="width:40%">
                <asp:Image runat="server" ID="OrganizationLogo" style="max-height:80px;" />
            </td>
            <td style="width:60%">
                <h1>Grades Report</h1>
            </td>
        </tr>
    </table>


    <table id="ClassInfo" runat="server" style="width:100%; border:black 1px solid; font-size:larger;">
        <tr>
            <td>Class Name:</td>
            <td colspan="3"><b><asp:Literal runat="server" ID="ClassName" /></b></td>
        </tr>
        <tr>
            <td>Start date:</td>
            <td><asp:Literal runat="server" ID="ClassStartDate" /></td>
            <td>End date:</td>
            <td><asp:Literal runat="server" ID="ClassEndDate" /></td>
        </tr>
    </table>

    <table id="GradebookInfo" runat="server" style="width:100%; border:black 1px solid; font-size:larger;">
        <tr>
            <td>Gradebook Name:</td>
            <td colspan="3"><b><asp:Literal runat="server" ID="GradebookName" /></b></td>
        </tr>
    </table>

    <table id="StudentInfo" style="width:100%;">
        <tr style="text-align:center;">
            <td style="width:60%;">
                <b><insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/> <asp:Literal runat="server" ID="StudentITANumber" /></b>
            </td>
            <td>
                <b>Employer</b>
            </td>
        </tr>
        <tr>
            <td class="align-top">
                
            <div class="form-group">
                <div>
                    <b><asp:Literal runat="server" ID="StudentFullName" /> </b>
                </div>
                <div>
                    <asp:Literal runat="server" ID="StudentAddress" /> 
                </div>
            </div>
            </td>
            <td class="align-top">
                
            <div class="form-group">
                <div>
                    <asp:Literal runat="server" ID="Employer" /> 
                </div>
            </div> 
            </td>
        </tr>
    </table>

    <div  style="font-size:x-large;">
    <b>Final Grade(s)</b>

    <asp:Repeater runat="server" ID="FinalRepeater">
        <HeaderTemplate>
            <table class="table table-striped"><tbody>
        </HeaderTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
        <ItemTemplate>
            <tr class='<%# Eval("CssClass") %>'>
                <td>
                    <%# Eval("ScoreItemName") %>
                    <div style="font-size:small">
                        <%# Eval("Comment") %>
                    </div>
                </td>
                <td>
                    <div style="text-align:right;white-space:nowrap;">
                        <b><%# Eval("ScoreValue") %></b>
                    </div>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    </div>

    <asp:Repeater runat="server" ID="ScoreRepeater">
        <HeaderTemplate>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>TOPICS / UNITS OF INSTRUCTION</th>
                        <th style="text-align:right;">Score</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <div style='<%# "padding-left:" + (20 * (int)Eval("Level")) + "px" %>'>
                        <span class='<%# Eval("SpecialCssClass") %>'>
                            <%# Eval("ScoreItemName") %>
                        </span>
                        <div class="form-text">
                            <%# Eval("Comment") %>
                        </div>
                    </div>
                </td>
                <td style="text-align:right;white-space:nowrap;">
                    <span class='<%# Eval("SpecialCssClass") %>'>
                        <%# Eval("ScoreValue") %>
                    </span>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Repeater runat="server" ID="MasteryRepeater">
        <HeaderTemplate>
            <div style="page-break-before:always; font-size:x-large;">
                <b>Learning Mastery</b>
            </div>

            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Outcome</th>
                        <th style="text-align:right;">Mastery</th>
                        <th style="text-align:right;">Score</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <FooterTemplate>
            </tbody></table>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <%# Eval("StandardTitle") %>
                </td>
                <td style="text-align:right;">
                    <%# Eval("MasteryScore", "{0:n1}") %>
                </td>
                <td style='<%# "text-align:right;" + ((bool)Eval("IsMastery") ? "color:green;" : "") %>'>
                    <%# Eval("Score", "{0:n1}") %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>

</div>
