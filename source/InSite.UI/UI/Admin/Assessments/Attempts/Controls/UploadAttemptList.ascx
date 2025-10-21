<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UploadAttemptList.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.UploadAttemptList" %>

<insite:PageHeadContent runat="server">
    <style>
        table.questions {
            width: auto;
        }

            table.questions th,
            table.questions td,
            table.questions td input {
                text-align: center;
            }

            table.questions input[type=text] {
                outline: none;
                border: 1px solid #ddd;
                text-transform: uppercase;
            }
    </style>
</insite:PageHeadContent>

<div class="row">
    <div class="col-md-5">
        <asp:Repeater runat="server" ID="AttemptRepeater">
            <HeaderTemplate>
                <table id='<%# AttemptRepeater.ClientID %>' class="table table-striped attempts">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Scans</th>
                            <th class="text-center">Errors</th>
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
                        <asp:RadioButton runat="server" ID="Selected"
                            GroupName="Person"
                            Checked='<%# Container.ItemIndex == SelectedAttemptIndex %>'
                            Text='<%# Eval("LearnerName") %>'
                        />

                        <span class="form-text"><%# Eval("LearnerCode") %></span>
                        <div class="form-text">
                            <%# Eval("FormName") %>
                            (<%# Eval("Questions.Length") %> Questions)
                        </div>
                    </td>
                    <td class="text-center">
                        <%# Eval("ScanCount") %>
                    </td>
                    <td class="text-center">
                        <%# Eval("Errors") %>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div class="col-md-7">
        <asp:Repeater runat="server" ID="QuestionRepeater">
            <HeaderTemplate>
                <table id='<%# QuestionRepeater.ClientID %>' class="table table-striped questions">
                    <thead>
                        <tr>
                            <th>Question</th>

                            <asp:Repeater runat="server" ID="AnswerRepeater">
                                <ItemTemplate>
                                    <th><%# Container.DataItem %></th>
                                </ItemTemplate>
                            </asp:Repeater>

                            <th>Attempt</th>
                            <th>Mark</th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <FooterTemplate>
                </tbody></table>
            </FooterTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Sequence") %></td>
            
                    <asp:Repeater runat="server" ID="AnswerRepeater">
                        <ItemTemplate>
                            <td>
                                <%# Container.DataItem %>
                            </td>
                        </ItemTemplate>
                    </asp:Repeater>

                    <td>
                        <asp:TextBox runat="server" ID="FinalAnswer" MaxLength="1" Width="50" Text='<%# Eval("FinalAnswer") %>' data-correct='<%# (Eval("CorrectAnswer") != null ? ((string)Eval("CorrectAnswer")).ToLower() : "") %>' />
                    </td>
                    <td>
                        <i runat="server" id="CorrectIcon" class="fas fa-check valid text-success" title="Correct"></i>
                        <i runat="server" id="IncorrectIcon" class="fas fa-times invalid text-danger" title="Incorrect"></i>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script>
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initHandlers);

        function initHandlers() {
            $('table#<%= QuestionRepeater.ClientID %> tbody input[id$=FinalAnswer]')
                .on('focus', function () {
                    this.setSelectionRange(0, this.value.length);
                })
                .on('keydown', function (e) {
                    switch (e.which) {
                        case 8: //backspace
                        case 9: //tab
                        case 37: // left arrow
                        case 39: // right arrow
                        case 46: // delete
                        case 65: // a
                        case 66: // b
                        case 67: // c
                        case 68: // d
                        case 109: // -
                        case 173: // -
                        case 189: // -
                            return true;
                    }

                    e.preventDefault();
                })
                .on('keypress', function (e) {
                    
                    switch (e.key) {
                        case "a":
                        case "A":
                        case "b":
                        case "B":
                        case "c":
                        case "C":
                        case "d":
                        case "D":
                        case "-":
                            return true;
                    }

                    e.preventDefault();
                })
                .on('keyup', function (e) {
                    var $txt = $(this);
                    var correctAnswer = $txt.data('correct');
                    var finalAnswer = $txt.val();

                    var $tr = $txt.closest('tr');
                    var $validIcon = $tr.find('i.valid');
                    var $invalidIcon = $tr.find('i.invalid');
                    
                    if (finalAnswer == null || finalAnswer.toLowerCase() != correctAnswer) {
                        $validIcon.hide();
                        $invalidIcon.show();
                    } else {
                        $validIcon.show();
                        $invalidIcon.hide();
                    }

                    validateAttempt();
                });
        };

        function validateAttempt() {
            var isValid = true;

            $('table#<%= QuestionRepeater.ClientID %> tbody input[id$=FinalAnswer]').each(function () {
                var finalAnswer = $(this).val();

                if (finalAnswer == null || finalAnswer.length == 0) {
                    isValid = false;
                    return false;
                }
            });

            if (isValid) {
                $('table#<%= AttemptRepeater.ClientID %> tbody input[type=radio]:checked').parent().removeClass('invalid-attempt text-danger');
            } else {
                $('table#<%= AttemptRepeater.ClientID %> tbody input[type=radio]:checked').parent().addClass('invalid-attempt text-danger');
            }

            return isValid;
        }

        function validateAllAttempts() {
            return $('table#<%= AttemptRepeater.ClientID %> tbody .invalid-attempt').length == 0;
        }
    </script>
</insite:PageFooterContent>