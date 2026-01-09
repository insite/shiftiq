<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResultQuestionOutput.ascx.cs" Inherits="InSite.UI.Portal.Assessments.Attempts.Controls.ResultQuestionOutput" %>

<asp:MultiView runat="server" ID="MultiView">

    <asp:View runat="server" ID="ViewListBox">
        <div runat="server" class='<%# "form-group " + ListBoxGroupClass %>'>
            <table class="table table-option mb-0">

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
                            <div><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></div>
                        </th>
                    </ItemTemplate>
                </asp:Repeater>

                <tbody>
                    <asp:Repeater runat="server" ID="ListOptionRepeater">
                        <ItemTemplate>
                            <tr>
                                <td class="input lh-1">
                                    <%# GetListBoxOptionIcon("Option") %>
                                </td>

                                <asp:Repeater runat="server" ID="TableBodyRepeater">
                                    <ItemTemplate>
                                        <td class='<%# Eval("CssClass") %>' style='<%# Eval("AlignmentStyle") %>'><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></td>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <td runat="server" id="OptionText" class="text"><%# Shift.Common.Markdown.ToHtml((string)Eval("Option.OptionText")) %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:View>

    <asp:View runat="server" ID="ViewComposedEssay">
        <div class="composed-essay-input">
            <div runat="server" class="float-end ps-4 pb-4" visible='<%# Question.AnswerPoints != null %>'>
                <%# GetSquareOptionIcon(Question.AnswerPoints > 0) %>
            </div>
            <%# Shift.Common.Markdown.ToHtml(Question.AnswerText) %>
        </div>
    </asp:View>

    <asp:View runat="server" ID="ViewComposedVoice">
        <div class="composed-voice-input">
            <div runat="server" class="position-absolute end-0" visible='<%# Question.AnswerPoints != null %>'>
                <%# GetSquareOptionIcon(Question.AnswerPoints > 0) %>
            </div>
            <insite:OutputAudio runat="server"
                AttemptLimit='<%# Question.AnswerAttemptLimit ?? 0 %>'
                CurrentAttempt='<%# Question.AnswerRequestAttempt ?? 0 %>' 
                AudioURL='<%# AnswerFileUrl %>'
                Visible='<%# AnswerFileUrl != null %>' />
        </div>
    </asp:View>

    <asp:View runat="server" ID="ViewBooleanTable">
        <div class="form-group boolean-list">

            <table class="table table-hover table-boolean mb-0">
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
                            <div><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></div>
                        </th>
                    </ItemTemplate>
                </asp:Repeater>

                <tbody>
                    <asp:Repeater runat="server" ID="BooleanOptionRepeater">
                        <ItemTemplate>
                            <tr>
                                <asp:Repeater runat="server" ID="TableBodyRepeater">
                                    <ItemTemplate>
                                        <td class='<%# Eval("CssClass") %>' style='<%# Eval("AlignmentStyle") %>'><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></td>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <td runat="server" id="OptionText" class="text"><%# Shift.Common.Markdown.ToHtml((string)Eval("Option.OptionText")) %></td>

                                <td class="input">
                                    <div class="mb-2 text-nowrap">
                                        <%# GetBooleanOptionIcon(true) %>
                                        <span class="align-middle ps-1">True</span>
                                    </div>
                                    <div class="mb-2 text-nowrap">
                                        <%# GetBooleanOptionIcon(false) %>
                                        <span class="align-middle ps-1">False</label>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:View>

    <asp:View runat="server" ID="ViewMatching">
        <div class="form-group match-list">
            <table class="table table-hover table-match mb-0">
                <tbody>
                    <asp:Repeater runat="server" ID="MatchesRepeater">
                        <ItemTemplate>
                            <tr>
                                <td class="left"><%# Shift.Common.Markdown.ToHtml((string)Eval("MatchLeftText")) %></td>
                                <td class="right"><%# Eval("AnswerText") %></td>
                                <td class="answer lh-1">
                                    <%# GetMatchOptionIcon() %>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:View>

    <asp:View runat="server" ID="ViewLikert">
        <div class="form-group likert-matrix">
            <table class="table table-striped table-likert mb-0" style="visibility:hidden;">
                <thead>
                    <asp:Repeater runat="server" ID="LikertColumnRepeater">
                        <HeaderTemplate>
                            <tr>
                                <td></td>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <td class="text"><%# Shift.Common.Markdown.ToHtml((string)Eval("OptionText")) %></td>
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
                                <td class="text"><%# Shift.Common.Markdown.ToHtml((string)Eval("Question.QuestionText")) %></td>
                                <asp:Repeater runat="server" ID="LikertOptionRepeater">
                                    <ItemTemplate>
                                        <td class="input lh-1">
                                            <%# GetListBoxOptionIcon(null) %>
                                        </td>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
    </asp:View>

    <asp:View runat="server" ID="ViewHotspot">
        <div class="form-group hotspot-image" data-img='<%# Question.HotspotImage %>' data-shapes='<%# GetHotspotShapes() %>' data-pins='<%# GetHotspotPins() %>'>
        </div>
    </asp:View>

    <asp:View runat="server" ID="ViewOrdering">
        <div class="form-group ordering-list">
            <div class="position-absolute end-0">
                <%# GetSquareOptionIcon(Question.AnswerPoints > 0) %>
            </div>
            <div class="pe-5">
                <%# Shift.Common.Markdown.ToHtml(Question.QuestionTopLabel) %>
                <asp:Repeater runat="server" ID="OrderingOptionRepeater">
                    <HeaderTemplate><div class="mb-3 ordering-list-container"></HeaderTemplate>
                    <FooterTemplate></div></FooterTemplate>
                    <ItemTemplate>
                        <div class='<%# GetOrderingOptionClass() %>'>
                            <%# Shift.Common.Markdown.ToHtml((string)Eval("OptionText")) %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <%# Shift.Common.Markdown.ToHtml(Question.QuestionBottomLabel) %>
            </div>
        </div>
    </asp:View>

</asp:MultiView>

                            






