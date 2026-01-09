<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnswerQuestionOutput.ascx.cs" Inherits="InSite.UI.Portal.Assessments.Attempts.Controls.AnswerQuestionOutput" %>

<asp:Literal runat="server" ID="QuestionStartComment" Text='<%# "<!--question start " + (Data.AttemptQuestion.QuestionSequence - 1) + "-->"  %>' />

<div class="card mb-4 card-question exam-locked bg-secondary" data-number="<%# Data.AttemptQuestion.QuestionNumber %>" data-question='<%# Data.AttemptQuestion.QuestionSequence %>'>
    <div class="card-header border-bottom-0">

        <div runat="server" visible='<%# Data.TagsAndLabels != null %>' class="question-tags"><%# Data.TagsAndLabels %></div>

        <div class="question-right-commands">
            <a href="#view-bookmarks" data-action="view-bookmarks" style="display: none;"><%= Translate("View Bookmarks") %></a>
        </div>

        <h3>
            <span><%= Translate("Question") %> <%# Data.AttemptQuestion.QuestionNumber %></span>

            <asp:HyperLink runat="server" href="#add-feedback" data-action="feedback-question" Visible='<%# Data.Attempt.BankForm.Publication.AllowFeedback %>' ToolTip='<%# Translate("Post Feedback") %>' CssClass="btn-icon">
                <i class="icon fas fa-comment x20"></i>
            </asp:HyperLink>
            <a href="#add-bookmark" title='<%# Translate("Add Bookmark") %>' data-action="add-bookmark" style="display:none;" class="btn-icon">
                <i class="icon far fa-bookmark x20"></i>
            </a>
            <a href="#remove-bookmark" title='<%# Translate("Remove Bookmark") %>' data-action="remove-bookmark" style="display:none;" class="btn-icon">
                <i class="icon fas fa-bookmark x20"></i>
            </a>
        </h3>
        <div class="question-text"><%# GetHtml(Data.AttemptQuestion.QuestionText) %></div>
    </div>

    <div class="card-body bg-white">

        <div runat="server" visible='<%# Data.IsRadioList || Data.IsCheckList %>' class='<%# "form-group " + (Data.IsRadioList ? "radio-list" : "checkbox-list") %>' data-limit='<%# Data.LimitAnswers %>'>
            <table class="table table-option">
                <asp:Repeater runat="server" ID="ListTableHeaderRepeater">
                    <HeaderTemplate>
                        <thead>
                            <tr>
                                <th></th>
                    </HeaderTemplate>
                    <FooterTemplate>
                            </tr>
                        </thead>
                    </FooterTemplate>
                    <ItemTemplate>
                        <th class='<%# Eval("CssClass") %>' style='<%# Eval("AlignmentStyle") %>'>
                            <div><%# GetHtml((string)Eval("Text")) %></div>
                        </th>
                    </ItemTemplate>
                </asp:Repeater>

                <tbody>
                    <asp:Repeater runat="server" ID="ListOptionRepeater">
                        <ItemTemplate>
                            <tr>
                                <td class="input">
                                    <input type='<%# Data.IsRadioList ? "radio" : "checkbox" %>' class="form-check-input" name='<%# Eval("Option.QuestionSequence", "group_{0}") %>' value='<%# Eval("Option.OptionSequence") %>' id='<%# Eval("OptionId", "option_{0}") %>' <%# (bool?)Eval("Option.AnswerIsSelected") == true ? "checked=checked" : "" %> data-validate="false" />
                                </td>

                                <asp:Repeater runat="server" ID="TableBodyRepeater">
                                    <ItemTemplate>
                                        <td class='<%# Eval("CssClass") %>' style='<%# Eval("AlignmentStyle") %>'><%# GetHtml((string)Eval("Text")) %></td>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <td runat="server" id="OptionText" class="text"><%# GetHtml((string)Eval("Option.OptionText")) %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>

            <input type="text" name='<%# "q_" + Data.AttemptQuestion.QuestionSequence %>' value="" required class="hidden-field" data-submit="true" />
        </div>

        <div runat="server" visible='<%# Data.IsComposedEssay %>' class="form-group composed-essay-input">
            <textarea class="form-control" name='<%# "q_" + Data.AttemptQuestion.QuestionSequence %>' required data-submit="true" rows="6"><%# Data.AttemptQuestion.AnswerText %></textarea>
        </div>

        <div runat="server" visible='<%# Data.IsComposedVoice %>' class="form-group composed-voice-input">
            <insite:InputAudio runat="server" AutoUpload="false" Bitrate='<%# ComposedVoiceBitrate %>'
                TimeLimit='<%# Data.AttemptQuestion.AnswerTimeLimit ?? 0 %>'
                AttemptLimit='<%# Data.AttemptQuestion.AnswerAttemptLimit ?? 0 %>'
                CurrentAttempt='<%# Data.AttemptQuestion.AnswerRequestAttempt ?? 0 %>' />
            <insite:OutputAudio runat="server" AllowDelete="true" 
                AttemptLimit='<%# Data.AttemptQuestion.AnswerAttemptLimit ?? 0 %>'
                CurrentAttempt='<%# Data.AttemptQuestion.AnswerRequestAttempt ?? 0 %>' 
                AudioURL='<%# GetFileUrl(Data.AttemptQuestion.AnswerFileIdentifier) %>' />
            <input type="text" name='<%# "q_" + Data.AttemptQuestion.QuestionSequence %>' value="" required class="hidden-field" data-submit="true" data-urgent="true" />
        </div>

        <div runat="server" visible='<%# Data.IsBooleanTable %>' class="form-group boolean-list" data-limit='<%# Data.LimitAnswers %>'>

            <table class="table table-hover table-boolean">
                <asp:Repeater runat="server" ID="BooleanTableHeaderRepeater">
                    <HeaderTemplate>
                        <thead>
                            <tr>
                                <th></th>
                    </HeaderTemplate>
                    <FooterTemplate>
                            </tr>
                        </thead>
                    </FooterTemplate>
                    <ItemTemplate>
                        <th class='<%# Eval("CssClass") %>' style='<%# Eval("AlignmentStyle") %>'>
                            <div><%# GetHtml((string)Eval("Text")) %></div>
                        </th>
                    </ItemTemplate>
                </asp:Repeater>

                <tbody>
                    <asp:Repeater runat="server" ID="BooleanOptionRepeater">
                        <ItemTemplate>
                            <tr>
                                <asp:Repeater runat="server" ID="TableBodyRepeater">
                                    <ItemTemplate>
                                        <td class='<%# Eval("CssClass") %>' style='<%# Eval("AlignmentStyle") %>'><%# GetHtml((string)Eval("Text")) %></td>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <td runat="server" id="OptionText" class="text"><%# GetHtml((string)Eval("Option.OptionText")) %></td>

                                <td class="input">
                                    <div class="form-group">
                                        <div class="mb-2">
                                            <input type="radio" class="form-check-input" name='<%# Eval("OptionId", "group_{0}") %>' value='<%# Eval("Option.OptionSequence") + ":1" %>' id='<%# Eval("OptionId", "option_{0}_1") %>' <%# (bool?)Eval("Option.AnswerIsSelected") == true ? "checked=checked" : "" %> data-validate="false" />
                                            <label for="<%# Eval("OptionId", "option_{0}_1") %>">True</label>
                                        </div>
                                        <div>
                                            <input type="radio" class="form-check-input" name='<%# Eval("OptionId", "group_{0}") %>' value='<%# Eval("Option.OptionSequence") + ":0" %>' id='<%# Eval("OptionId", "option_{0}_0") %>' <%# (bool?)Eval("Option.AnswerIsSelected") == false ? "checked=checked" : "" %> data-validate="false" />
                                            <label for="<%# Eval("OptionId", "option_{0}_0") %>">False</label>
                                        </div>
                                        <input type="text" required class="hidden-field boolean-input" />
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
            <input type="text" name='<%# "q_" + Data.AttemptQuestion.QuestionSequence %>' value="" class="hidden-field" data-submit="true" data-validate="false" />
        </div>

        <div runat="server" visible='<%# Data.IsMatching %>' class="form-group match-list">
            <table class="table table-hover table-match">
                <tbody>
                    <asp:Repeater runat="server" ID="MatchesRepeater">
                        <ItemTemplate>
                            <tr>
                                <td class="left"><%# GetHtml((string)Eval("Match.MatchLeftText")) %></td>
                                <td class="right">
                                    <div class="form-group">
                                        <select class="match-input" required>
                                            <option></option>
                                            <asp:Repeater runat="server" ID="MatchOptionsRepeater">
                                                <ItemTemplate>
                                                    <option <%# (bool)Eval("IsSelected") ? "selected=selected" : "" %>  value='<%# Eval("MatchSequence") + ":" + HttpUtility.HtmlEncode((string)Eval("Value")) %>'>
                                                        <%# Eval("Value") %>
                                                    </option>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </select>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
            <input type="text" name='<%# "q_" + Data.AttemptQuestion.QuestionSequence %>' value="" class="hidden-field" data-submit="true" data-validate="false" />
        </div>

        <div runat="server" visible='<%# Data.IsLikert %>' class="form-group likert-matrix">

            <table class="table table-striped table-likert" style="visibility:hidden;">
                <thead>
                    <asp:Repeater runat="server" ID="LikertColumnRepeater">
                        <HeaderTemplate>
                            <tr>
                                <td></td>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <td class="text"><%# GetHtml((string)Eval("OptionText")) %></td>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tr>
                        </FooterTemplate>
                    </asp:Repeater>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="LikertRowRepeater">
                        <ItemTemplate>
                            <tr>
                                <td class="text">
                                    <%# GetHtml((string)Eval("Question.QuestionText")) %>
                                    <input type="text" name='<%# Eval("Question.QuestionSequence", "q_{0}") %>' value="" required class="hidden-field" data-submit="true" />
                                </td>
                                <asp:Repeater runat="server" ID="LikertOptionRepeater">
                                    <ItemTemplate>
                                        <td class="input">
                                            <input type='<%# (bool)Eval("IsRadioList") ? "radio" : "checkbox" %>' class="form-check-input" name='<%# Eval("Option.QuestionSequence", "group_{0}") %>' value='<%# Eval("Option.OptionSequence") %>' id='<%# Eval("OptionId", "option_{0}") %>' <%# (bool?)Eval("Option.AnswerIsSelected") == true ? "checked=checked" : "" %> data-validate="false" />
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>

        </div>

        <div runat="server" visible='<%# Data.IsHotspot %>' class="form-group hotspot-image" data-pin-limit='<%# Data.AttemptQuestion.PinLimit ?? -1 %>' data-img='<%# Data.AttemptQuestion.HotspotImage %>' data-shapes='<%# GetHotspotShapes() %>' data-pins='<%# GetHotspotPins() %>'>
            <input type="text" name='<%# "q_" + Data.AttemptQuestion.QuestionSequence %>' value="" required class="hidden-field" data-submit="true" />
        </div>

        <div runat="server" visible='<%# Data.IsOrdering %>' class="form-group ordering-list">
            <%# GetHtml(Data.AttemptQuestion.QuestionTopLabel) %>
            <asp:Repeater runat="server" ID="OrderingOptionRepeater">
                <HeaderTemplate><div class="mb-3 ordering-list-container"></HeaderTemplate>
                <FooterTemplate></div></FooterTemplate>
                <ItemTemplate>
                    <div class="bg-white border rounded py-2 px-3 mb-3" data-id='<%# GetOptionId() %>'>
                        <%# GetHtml((string)Eval("OptionText")) %>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <%# GetHtml(Data.AttemptQuestion.QuestionBottomLabel) %>

            <input type="text" name='<%# "q_" + Data.AttemptQuestion.QuestionSequence %>' value="" class="hidden-field" data-submit="true" data-validate="false" />
        </div>

    </div>
</div>

<asp:Literal runat="server" ID="QuestionEndComment" Text='<%# "<!--question end-->"  %>' />