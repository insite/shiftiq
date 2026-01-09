<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResultQuestionCommentsRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.ResultQuestionCommentsRepeater" %>

<insite:PageHeadContent runat="server">

    <style type="text/css">

        table.question-comments img {
            max-width: 100%;
            max-height: 500px;
        }

        table.question-comments table.table.table-comments {
            background-color: transparent;
        }

            table.question-comments table.table.table-comments td,
            table.question-comments table.table.table-comments th {
                
            }

    </style>

</insite:PageHeadContent>

<asp:Repeater runat="server" ID="ExamRepeater">
    <ItemTemplate>
        <h3><%# Eval("Title") %></h3>

        <table class="table table-striped question-comments">
    
            <thead>
                <tr>
                    <th style="width:40px;" class="text-center">#</th>
                    <th style="width:10px;"></th>
                    <th>Question</th>
                </tr>
            </thead>

            <tbody>
                <asp:Repeater runat="server" ID="QuestionRepeater">
                    <ItemTemplate>
                        <tr runat="server" id="QuestionRow">
                            <td class="text-center"><%# Eval("Sequence") %></td>
                            <td class="text-nowrap"><%# Eval("Code") %></td>
                            <td>
                                <p><%# Eval("Html") %></p>

                                <table class="table table-comments">
                                    <thead>
                                        <tr>
                                            <th>
                                                Author
                                            </th>
                                            <th>
                                                Posted
                                            </th>
                                            <th>
                                                Text
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater runat="server" ID="CommentRepeater">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="text-nowrap">
                                                        <%# Eval("AuthorName") %>
                                                    </td>
                                                    <td class="text-nowrap">
                                                        <%# FormatDate(Eval("CommentPosted")) %>
                                                    </td>
                                                    <td>
                                                        <%# Eval("CommentText") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>

                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>

        </table>
    </ItemTemplate>
</asp:Repeater>