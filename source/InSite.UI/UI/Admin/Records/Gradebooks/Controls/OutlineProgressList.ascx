<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineProgressList.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.OutlineProgressList" %>

<insite:PageHeadContent runat="server">
    <style>
        .hide-comment {
            display:none;
        }
        .hide-date {
            display:none;
        }
        .hide-ignore {
            display:none;
        }
    </style>
</insite:PageHeadContent>

<div class="mb-3">
    <insite:Button runat="server" ID="AssignPeriodButton" Text="Assign Period" Icon="fas fa-clock" ButtonStyle="Default" />
    <insite:Button runat="server" ID="ReleaseButton" Text="Issue Achievements" Icon="fas fa-award" ButtonStyle="Default" ConfirmText="Granting achievements in this way will immediately process all learners based on the settings configured for any achievements in this gradebook. It will also lock this gradebook, which will prevent additional edits. Are you sure you want to proceed?" />
    <insite:Button runat="server" ID="CalculateButton" Text="Calculate" Icon="fas fa-calculator" ButtonStyle="Default" ConfirmText="Calculating this gradebook will update all Calculated Scores.  Are you sure you want to proceed?" />
    <insite:Button runat="server" ID="ScoresLink" Text="Edit Progress" Icon="fas fa-pencil" ButtonStyle="Default" />
    <insite:Button runat="server" ID="AddStudentsLink" Text="Add Learners" Icon="fas fa-plus-circle" ButtonStyle="Default" />
    <insite:Button runat="server" ID="SummaryReportButton" Text="Summary List" Icon="fas fa-file-excel" ButtonStyle="Default" />
    <insite:Button runat="server" ID="CreateScoresButton" Text="Create Scores" Icon="fas fa-calculator" ButtonStyle="Default" ConfirmText="Creating scores for this gradebook will unlock all lessons if the Gradebook is attached to a Course. This will give learners in this Gradebook access to every lesson without satisfying prerequisites. Are you sure you want to proceed?" />
</div>

<div class="mb-3">
    <div>
        <insite:GradebookItemComboBox runat="server" ID="GradeItemIdentifier" EmptyMessage="Root" Width="380" Height="300" />
    </div>
    <div class="find-period hide-comment mt-1">
        <insite:FindPeriod runat="server" ID="PeriodIdentifier" EmptyMessage="All Periods" Width="380" />
    </div>
    <div class="mt-1">
        <insite:TextBox runat="server" ID="LearnerName" EmptyMessage="Learner" Width="380" CssClass="d-inline-block" />
        <insite:IconButton runat="server" ID="SearchButton" ToolTip="Search" Name="filter" CssClass="p-2" />
    </div>
    <div class="mt-1">
        <insite:UpdatePanel runat="server">
            <ContentTemplate>
                <asp:CheckBox runat="server" ID="HideCommentsCheckBox" Text="Hide Comments and Periods" onchange="change_HideCommentsCheckBox()" />
                <asp:CheckBox runat="server" ID="HideDatesCheckBox" Text="Hide Dates" onchange="change_HideDatesCheckBox()" />
                <asp:CheckBox runat="server" ID="HideIgnoreCheckBox" Text="Hide Ignore Score" onchange="change_HideIgnoreCheckBox()" />
            </ContentTemplate>
        </insite:UpdatePanel>
    </div>
</div>

<div runat="server" id="NoStudentsAlert" class="alert alert-warning" role="alert">
    You need to add learners to this gradebook before you can input information about their progress.
</div>

<div runat="server" id="NoScoresAlert" class="alert alert-warning mt-3" role="alert">
    You need to add at least one Grade Item to this gradebook.
</div>

<asp:Panel runat="server" ID="ScorePanel">
    <div runat="server" id="PagingPanel" style="text-align:right;">
        <asp:Literal runat="server" ID="PageNumberLiteral" />
        of
        <asp:Literal runat="server" ID="PageCountLiteral" />

        <insite:Button runat="server" ID="PrevButton" Text="Prev" Icon="fas fa-arrow-alt-left" ButtonStyle="Default" />
        <insite:NextButton runat="server" ID="NextButton" />
    </div>

    <table class="table table-striped" style='<%= "width:" + (StudentColumnWidth + VisibleItems * ScoreColumnWidth + 40) + "px;" %>'>
        <thead>
            <tr>
                <th style='<%= "width:" + StudentColumnWidth + "px" %>'>User</th>
                <asp:Repeater runat="server" ID="ScoreItemHeaderRepeater">
                    <ItemTemplate>
                        <th style="<%= "width:" + ScoreColumnWidth + "px;text-align:center;" %>" title='<%# Eval("Name") %>'>
                            <%# Eval("ShortName") ?? Eval("Abbreviation") %>
                            <div runat="server" visible='<%# Eval("ShowMaxPoint") %>' class="form-text" style="font-weight:normal;">
                                Out of <%# Eval("MaxPoint") != null ? Eval("MaxPoint", "{0:n2}") : "N/A" %>
                            </div>
                        </th>
                    </ItemTemplate>
                </asp:Repeater>
                <th></th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="StudentRepeater">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:HyperLink runat="server" Target="_blank" ToolTip='<%# Eval("EnrollmentRestartHtml") %>' Visible='<%# UserMode == UserMode.Admin %>' NavigateUrl='<%# Eval("Identifier", "/ui/admin/contacts/people/edit?contact={0}") %>'>
                                <%# Eval("FullName") %>
                            </asp:HyperLink>

                            <asp:HyperLink runat="server" Visible='<%# UserMode != UserMode.Admin %>' NavigateUrl='<%# string.Format("/ui/admin/records/gradebooks/instructors/person-outline?contact={0}&gradebook={1}", Eval("Identifier"), GradebookIdentifier) %>' >
                                <%# Eval("FullName") %>
                            </asp:HyperLink>
                            
                            <div class='<%# HideComments ? "student-notes hide-comment" : "student-notes" %>'>
                                <asp:HiddenField runat="server" ID="StudentIndex" Value='<%# Container.ItemIndex %>' />
                                <asp:HiddenField runat="server" ID="NotesValue" Value='<%# Eval("Notes") %>' />
                                <asp:HiddenField runat="server" ID="PeriodValue" Value='<%# Eval("PeriodIdentifier") %>' />

                                <div id="NotesLabel" runat="server" class="form-text"><%# Eval("Notes") %></div>                                
                                <div id="AddedLabel" runat="server" class="form-text"><%# Eval("AddedHtml") != null ? "Added: " + Eval("AddedHtml") : null %></div>
                                <div id="PeriodLabel" runat="server" class="form-text"><%# Eval("PeriodName") != null ? "Period: " + Eval("PeriodName") : null %></div>

                                <div>
                                    <insite:IconButton runat="server" ID="NotesButton" ToolTip="Edit Notes" OnClientClick="scoreList.notesWindow.show(this); return false;" Name="sticky-note" />
                                    <insite:IconButton runat="server" ID="PeriodButton" Visible="<%# !IsLocked %>" ToolTip="Edit Period" OnClientClick="scoreList.periodWindow.init(this); return false;" Name="clock" />
                                    <a
                                        href="/ui/admin/records/gradebooks/change-user?gradebook=<%# Eval("GradebookIdentifier") %> &student=<%# Eval("Identifier") %>"
                                    >
                                        <i runat="server" visible="<%# !IsLocked %>" class="icon fas fa-pencil"></i>
                                        <i runat="server" visible="<%# IsLocked %>" class="icon fas fa-search"></i>
                                    </a>
                                </div>
                            </div>
                        </td>
                                                
                        <asp:Repeater runat="server" ID="ProgressRepeater">
                            <ItemTemplate>
                                <td data-key='<%# Eval("Key") %>' style='<%# GetProgressCellStyle(Container.DataItem) %>' title='<%# GetProgressCellTooltip(Container.DataItem) %>'>
                                    <asp:Label runat="server" ID="Value" />

                                    <div runat="server" visible='<%# Eval("Graded") != null %>' class='<%# "form-text score-date" + (HideDates ? " hide-date" : "") %>' style="white-space:nowrap">
                                        <%# Eval("Graded") %>
                                    </div>

                                    <div class='<%# HideComments ? "score-comment hide-comment" : "score-comment" %>'>
                                        <asp:HiddenField runat="server" ID="CommentKey" Value='<%# Eval("Key") %>' />
                                        <asp:HiddenField runat="server" ID="CommentValue" Value='<%# Eval("Comment") %>' />

                                        <insite:IconButton runat="server" ID="CommentButton" ToolTip="Edit Comment" OnClientClick="scoreList.commentWindow.show(this); return false;" Name="comment" style="color:gray;" />
                                    </div>

                                    <div runat="server" id="IgnorePanel" class="form-text score-ignore" style="white-space:nowrap">
                                        <asp:CheckBox runat="server"
                                            Checked='<%# Eval("IsIgnored") %>'
                                            Enabled='<%# !IsLocked %>'
                                        />
                                    </div>
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>

                        <td style="width:40px; white-space:nowrap;">
                            <insite:IconButton runat="server" ID="StudentMarkReportButton" ToolTip="Print Student Grades Report" CommandName="Print" CommandArgument='<%# Eval("Identifier") %>' Name="file-alt" IconSize="Default" />
                            <insite:IconLink runat="server" ID="VoidStudent" 
                                 NavigateUrl='<%# string.Format("/admin/records/gradebooks/delete-student?gradebook={0}&student={1}", GradebookIdentifier, Eval("Identifier")) %>'
                                 ToolTip="Delete Student" Name="trash-alt"
                            />
                             
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </tbody>
    </table>
    
    <insite:Pagination runat="server" ID="StudentPagination" PageSize="30" />
</asp:Panel>

<div style="height:0; overflow:hidden;">
    <insite:UpdatePanel runat="server" ID="PeriodUpdatePanel">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="PeriodStudentIndex" />
            <insite:FindPeriod runat="server" ID="PeriodSelector" Output="None" ClientEvents-OnChange="scoreList.periodWindow.save" />
        </ContentTemplate>
    </insite:UpdatePanel>
</div>

<div style="height:0; overflow:hidden;">
    <insite:UpdatePanel runat="server">
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="IsIgnored" />
            <asp:HiddenField runat="server" ID="IgnoredScoreKey" />
            <asp:Button runat="server" ID="SaveIsIgnored" />
        </ContentTemplate>
    </insite:UpdatePanel>
</div>

<insite:Modal runat="server" ID="NotesWindow" Title="Edit Notes">
    <ContentTemplate>
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="NotesUpdatePanel" />

        <insite:UpdatePanel runat="server" ID="NotesUpdatePanel" ClientEvents-OnResponseEnd="scoreList.notesWindow.close">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="StudentIndex" />
                <insite:TextBox runat="server" ID="NotesTextBox" MaxLength="50" />

                <div class="mt-3">
                    <insite:SaveButton runat="server" ID="SaveNotesButton" OnClientClick='scoreList.notesWindow.save();' Text="Save Notes" />
                    <insite:CancelButton runat="server" ID="CancelNotesButton" OnClientClick="scoreList.notesWindow.close(); return false;" />
                    <insite:CloseButton runat="server" ID="CloseNotesButton" OnClientClick="scoreList.notesWindow.close(); return false;" />
                </div>
            </ContentTemplate>
        </insite:UpdatePanel>
    </ContentTemplate>
</insite:Modal>

<insite:Modal runat="server" ID="CommentWindow" Title="Edit Comment">
    <ContentTemplate>
        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="CommentUpdatePanel" />

        <insite:UpdatePanel runat="server" ID="CommentUpdatePanel" ClientEvents-OnResponseEnd="scoreList.commentWindow.close">
            <ContentTemplate>
                <asp:HiddenField runat="server" ID="CommentKey" />
                <insite:TextBox runat="server" ID="CommentTextBox" TextMode="MultiLine" Rows="8" />

                <div class="mt-3">
                    <insite:SaveButton runat="server" ID="SaveCommentButton" OnClientClick='scoreList.commentWindow.save();' Text="Save Comment" />
                    <insite:CancelButton runat="server" ID="CancelCommentButton" OnClientClick="scoreList.commentWindow.close(); return false;" />
                    <insite:CloseButton runat="server" ID="CloseCommentButton" OnClientClick="scoreList.commentWindow.close(); return false;" />
                </div>
            </ContentTemplate>
        </insite:UpdatePanel>
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        function change_HideCommentsCheckBox() {
            var isChecked = $("#<%= HideCommentsCheckBox.ClientID %>").is(":checked");

            if (isChecked) {
                $(".find-period").addClass("hide-comment");
                $(".score-comment").addClass("hide-comment");
                $(".student-notes").addClass("hide-comment");
            } else {
                $(".find-period").removeClass("hide-comment");
                $(".score-comment").removeClass("hide-comment");
                $(".student-notes").removeClass("hide-comment");
            }
        }

        function change_HideDatesCheckBox() {
            var isChecked = $("#<%= HideDatesCheckBox.ClientID %>").is(":checked");

            if (isChecked) {
                $(".score-date").addClass("hide-date");
            } else {
                $(".score-date").removeClass("hide-date");
            }
        }

        function change_HideIgnoreCheckBox() {
            const checkBox = document.getElementById("<%= HideIgnoreCheckBox.ClientID %>");
            if (!checkBox)
                return;

            const list = document.querySelectorAll(".score-ignore");

            if (checkBox.checked) {
                list.forEach(x => x.classList.add("hide-ignore"));
            } else {
                list.forEach(x => x.classList.remove("hide-ignore"));
            }
        }

        (function () {
            var instance = window.scoreList = window.scoreList || {};
            var $commentValue;
            var $commentButton;
            var $notesValue;
            var $notesButton;
            var $notesLabel;
            var $periodValue;
            var $periodButton;
            var $periodLabel;

            change_HideCommentsCheckBox();
            change_HideIgnoreCheckBox();

            $("#<%= LearnerName.ClientID %>").on("keyup", function (e) {
                if (e.keyCode == 13) {
                    e.preventDefault();
                    __doPostBack("<%= SearchButton.UniqueID %>", "");
                }
            });

            const scoreIgnore_change = (e) => {
                const td = e.target.closest("td");

                document.getElementById("<%= IgnoredScoreKey.ClientID %>").value = td.dataset.key;
                document.getElementById("<%= IsIgnored.ClientID %>").value = e.target.checked;

                __doPostBack("<%= SaveIsIgnored.UniqueID %>", "");
            };

            document.querySelectorAll(".score-ignore")
                .forEach(x => x.addEventListener("change", scoreIgnore_change));

            instance.commentWindow = {
                show: function (btn) {
                    $commentButton = $(btn);
                    $commentValue = $commentButton.parent().find('input[id$=CommentValue]');

                    var commentKey = $commentButton.parent().find('input[id$=CommentKey]').val();
                    var comment = $commentValue.val();

                    $('#<%= CommentKey.ClientID %>').val(commentKey);
                    $('#<%= CommentTextBox.ClientID %>').val(comment);

                    var wnd = modalManager.show('<%= CommentWindow.ClientID %>');

                    $(wnd)
                        .on('shown.bs.modal', function () {
                            $('#<%= CommentTextBox.ClientID %>').focus();
                        });
                },
                close: function () {
                    modalManager.close('<%= CommentWindow.ClientID %>');
                },
                save: function () {
                    var comment = $('#<%= CommentTextBox.ClientID %>').val();
                    var hasComment = comment != null && comment != "";

                    $commentValue.val(comment);

                    $commentButton.find("i").css("color", hasComment ? "#337ab7" : "gray");
                    $commentButton.prop("title", hasComment ? "Edit Comment:\n" + comment : "Edit Comment");
                }
            };

            instance.notesWindow = {
                show: function (btn) {
                    $notesButton = $(btn);

                    var $td = $notesButton.closest("td");

                    $notesValue = $td.find('input[id$=NotesValue]');
                    $notesLabel = $td.find('div[id$=NotesLabel]');

                    let studentIndex = $td.find('input[id$=StudentIndex]').val();
                    let notes = $notesValue.val();

                    $('#<%= StudentIndex.ClientID %>').val(studentIndex);
                    $('#<%= NotesTextBox.ClientID %>').val(notes);

                    let wnd = modalManager.show('<%= NotesWindow.ClientID %>');

                    $(wnd)
                        .on('shown.bs.modal', function () {
                            $('#<%= NotesTextBox.ClientID %>').focus();
                        });
                },
                close: function () {
                    modalManager.close('<%= NotesWindow.ClientID %>');
                },
                save: function () {
                    let notes = $('#<%= NotesTextBox.ClientID %>').val();

                    $notesValue.val(notes);
                    $notesLabel.html(notes);
                }
            };

            instance.periodWindow = {
                init: function (btn) {
                    $periodButton = $(btn);

                    var $td = $periodButton.closest("td");

                    $periodValue = $td.find('input[id$=PeriodValue]');
                    $periodLabel = $td.find('div[id$=PeriodLabel]');

                    const studentIndex = $td.find('input[id$=StudentIndex]').val();
                    const periodId = $periodValue.val();

                    $('#<%= PeriodStudentIndex.ClientID %>').val(studentIndex);

                    document.getElementById('<%= PeriodUpdatePanel.ClientID %>').ajaxRequest(periodId);
                },
                show: function () {
                    setTimeout(function () {
                        document.getElementById('<%= PeriodSelector.ClientID %>').show();
                    }, 0);
                },
                save: function () {
                    var item = document.getElementById('<%= PeriodSelector.ClientID %>').getItem();

                    $periodValue.val('');
                    $periodLabel.html('');

                    if (item != null) {
                        $periodValue.val(item.value);

                        if (item.text)
                            $periodLabel.html("Period: " + item.text);
                    }
                }
            };

        })();

    </script>
</insite:PageFooterContent>