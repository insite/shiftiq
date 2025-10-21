<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScoreControl.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.ScoreControl" %>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        table.scores input[type=text],
        table.scores select {
            outline:none;
            border:1px solid #ddd;
            background-color:white;
            text-align:center;
            margin-bottom:2px;
        }

        .hide-comment {
            display:none;
        }
    </style>
</insite:PageHeadContent>

<insite:Alert runat="server" ID="AlertStatus" />

<div runat="server" id="NoStudentsAlert" class="alert alert-warning" role="alert">
    There are no students to score
</div>

<section runat="server" ID="StudentPanel" class="mb-3">
    <h2 class="h4 mb-3">
        <i class="far fa-user me-1"></i>
        Students
    </h2>

    <div class="card border-0 shadow-lg h-100">
        <div class="card-body">

            <div class="row">
                        
                <div class="col-md-12">

                    <div class="row">
                        <div class="col-md-6">
                            <table>
                                <tr>
                                    <td>
                                        <insite:GradebookItemComboBox runat="server" ID="GradeItem" EmptyMessage="Root" Width="400" ClientEvents-OnChange="scoresForm.clearLeaveConfirmation" />
                                    </td>
                                    <td style="width:100%;">
                                        <insite:UpdatePanel runat="server">
                                            <ContentTemplate>
                                                <asp:CheckBox runat="server" ID="HideCommentsCheckBox" Text="Hide Comments" onchange="change_HideCommentsCheckBox(); return true;" />
                                            </ContentTemplate>
                                        </insite:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div runat="server" id="PagingPanel" class="col-md-6" style="text-align:right;">
                            <asp:Literal runat="server" ID="PageNumberLiteral" />
                            of
                            <asp:Literal runat="server" ID="PageCountLiteral" />

                            <insite:Button runat="server" ID="PrevButton" Text="Prev" Icon="fas fa-arrow-alt-left" ButtonStyle="Default" OnClientClick="scoresForm.prev();" />
                            <insite:NextButton runat="server" ID="NextButton" OnClientClick="scoresForm.next();" />
                        </div>
                    </div>

                    <div runat="server" id="NoScoresAlert" class="alert alert-warning" role="alert" style="margin-top:15px;">
                        There are no score items
                    </div>

                    <asp:Panel runat="server" ID="ScorePanel">
                        <table class="table table-striped scores" style='<%= "width:" + (StudentColumnWidth + VisibleItems * ScoreColumnWidth) + "px;" %>'>
                            <thead>
                                <tr>
                                    <th style='<%= "width:" + StudentColumnWidth + "px" %>'>Student</th>
                                    <asp:Repeater runat="server" ID="ScoreItemHeaderRepeater">
                                        <ItemTemplate>
                                            <th style="<%= "width:" + ScoreColumnWidth + "px;text-align:center;" %>" title='<%# Eval("Name") %>'>
                                                <%# Eval("Header") %>
                                                <div runat="server" visible='<%# Eval("ShowMaxPoint") %>' class="form-text" style="font-weight:normal;">
                                                    Out of <%# Eval("MaxPoint") != null ? Eval("MaxPoint", "{0:n2}") : "N/A" %>
                                                </div>
                                            </th>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="StudentRepeater">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%# Eval("FullName") %>

                                                <div class='<%# HideComments ? "student-notes hide-comment" : "student-notes" %>'>
                                                    <asp:HiddenField runat="server" ID="NotesField" Value='<%# Eval("Notes") %>' />

                                                    <span id="NotesLabel" runat="server" class="form-text"><%# Eval("Notes") %></span>

                                                    <insite:IconButton runat="server" ID="NotesButton" Name="sticky-note" ToolTip="Edit Notes" OnClientClick="scoresForm.notesWindow.show(this); return false;" />
                                                </div>
                                            </td>
                                                
                                            <asp:Repeater runat="server" ID="ContactScoreRepeater">
                                                <ItemTemplate>
                                                    <td style="text-align:center;">
                                                        <asp:TextBox runat="server" ID="ScoreTextBox" Visible='<%# !(bool)Eval("IsBoolean") %>' Text='<%# Eval("Value") %>' Width="70" MaxLength="100" onchange="scoresForm.setLeaveConfirmation();" TabIndex='<%# 100 + Container.ItemIndex %>' />

                                                        <asp:DropDownList runat="server" ID="ScoreStatus" Visible='<%# Eval("IsBoolean") %>' onchange="scoresForm.setLeaveConfirmation();" TabIndex='<%# 100 + Container.ItemIndex %>'>
                                                            <asp:ListItem />
                                                            <asp:ListItem Value="Completed" Text="Completed" />
                                                            <asp:ListItem Value="Incomplete" Text="Incomplete" disabled="disabled" />
                                                            <asp:ListItem Value="Started" Text="Started" />
                                                        </asp:DropDownList>

                                                        <div class='<%# HideComments ? "score-comment hide-comment" : "score-comment" %>'>
                                                            <asp:HiddenField runat="server" ID="CommentField" Value='<%# Eval("Comment") %>' />
                                                            <insite:IconButton runat="server" ID="CommentButton" ToolTip="Edit Comment" OnClientClick="scoresForm.commentWindow.show(this); return false;" Name="comment" style="color:gray;" />
                                                        </div>
                                                    </td>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </asp:Panel>

                </div>

            </div>
        
        </div>
    </div>
</section>

<div>
    <insite:SaveButton runat="server" ID="SaveButton" Text="Save & Calculate" OnClientClick="scoresForm.clearLeaveConfirmation();" />
    <insite:CancelButton runat="server" ID="CancelButton" />
    <insite:CloseButton runat="server" ID="CloseButton" Visible="false" />
</div>

<insite:Modal runat="server" ID="NotesWindow" Title="Edit Notes">
    <ContentTemplate>
        <insite:TextBox runat="server" ID="NotesTextBox" MaxLength="50" />

        <div style="padding-top:15px;">
            <insite:SaveButton runat="server" ID="SaveNotesButton" OnClientClick='scoresForm.notesWindow.save(); return false;' Text="Save Notes" />
            <insite:CancelButton runat="server" OnClientClick="scoresForm.notesWindow.close(); return false;" />
        </div>
    </ContentTemplate>
</insite:Modal>

<insite:Modal runat="server" ID="CommentWindow" Title="Edit Comment">
    <ContentTemplate>
        <insite:TextBox runat="server" ID="CommentTextBox" TextMode="MultiLine" Rows="8" />

        <div style="padding-top:15px;">
            <insite:SaveButton runat="server" ID="SaveCommentButton" OnClientClick='scoresForm.commentWindow.save(); return false;' Text="Save Comment" />
            <insite:CancelButton runat="server" OnClientClick="scoresForm.commentWindow.close(); return false;" />
        </div>
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        function change_HideCommentsCheckBox() {
            var isChecked = $("#<%= HideCommentsCheckBox.ClientID %>").is(":checked");

            if (isChecked) {
                $(".score-comment").addClass("hide-comment");
                $(".student-notes").addClass("hide-comment");
            } else {
                $(".score-comment").removeClass("hide-comment");
                $(".student-notes").removeClass("hide-comment");
            }
        }

        (function () {
            var instance = window.scoresForm = window.scoresForm || {};
            var $commentField;
            var $commentButton;
            var $notesField;
            var $notesButton;
            var $notesLabel;

            instance.setLeaveConfirmation = function () {
                window.onbeforeunload = function () { return 'Are you sure you want to leave?'; };
            };

            instance.clearLeaveConfirmation = function () {
                window.onbeforeunload = null;
            };

            instance.next = function () {
                instance.clearLeaveConfirmation();
            };

            instance.prev = function () {
                instance.clearLeaveConfirmation();
            };

            instance.commentWindow = {
                show: function (btn) {
                    $commentButton = $(btn);
                    $commentField = $commentButton.parent().find('input[type=hidden]');

                    var comment = $commentField.val();

                    $('#<%= CommentTextBox.ClientID %>').val(comment);

                    var wnd = modalManager.show('<%= CommentWindow.ClientID %>');

                    $(wnd)
                        .on('shown.bs.modal', function (e, s, a) {
                            $('#<%= CommentTextBox.ClientID %>').focus();
                        })
                },
                close: function () {
                    modalManager.close('<%= CommentWindow.ClientID %>');
                },
                save: function () {
                    var comment = $('#<%= CommentTextBox.ClientID %>').val();
                    var hasComment = comment != null && comment != "";

                    $commentField.val(comment);

                    $commentButton.find("i").css("color", hasComment ? "#337ab7" : "gray");
                    $commentButton.prop("title", hasComment ? "Edit Comment:\n" + comment : "Edit Comment");

                    modalManager.close('<%= CommentWindow.ClientID %>');

                    scoresForm.setLeaveConfirmation();
                }
            };

            instance.notesWindow = {
                show: function (btn) {
                    $notesButton = $(btn);
                    $notesField = $notesButton.parent().find('input[type=hidden]');
                    $notesLabel = $notesButton.parent().find('span[id$=NotesLabel]');

                    var notes = $notesField.val();

                    $('#<%= NotesTextBox.ClientID %>').val(notes);

                    var wnd = modalManager.show('<%= NotesWindow.ClientID %>');

                    $(wnd)
                        .on('shown.bs.modal', function (e, s, a) {
                            $('#<%= NotesTextBox.ClientID %>').focus();
                        })
                },
                close: function () {
                    modalManager.close('<%= NotesWindow.ClientID %>');
                },
                save: function () {
                    let notes = $('#<%= NotesTextBox.ClientID %>').val();

                    $notesField.val(notes);
                    $notesLabel.html(notes);

                    modalManager.close('<%= NotesWindow.ClientID %>');

                    scoresForm.setLeaveConfirmation();
                }
            };

        })();

    </script>
</insite:PageFooterContent>
