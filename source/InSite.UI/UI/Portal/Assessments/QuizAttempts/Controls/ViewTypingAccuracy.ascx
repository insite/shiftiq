<%@ Control Language="C#" CodeBehind="ViewTypingAccuracy.ascx.cs" Inherits="InSite.UI.Portal.Assessments.QuizAttempts.Controls.ViewTypingAccuracy" %>

<div class="position-relative">
    <insite:UpdatePanel runat="server" ID="QuestionPanel" >
        <ContentTemplate>
            <asp:Repeater runat="server" ID="SampleColumnRepeater">
                <HeaderTemplate>
                    <div class="row quiz-sample">
                </HeaderTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
                <ItemTemplate>
                    <div class="col mb-3">
                        <div class="card h-100">
                            <div class="card-body">

                                <asp:Repeater runat="server" ID="RowRepeater">
                                    <ItemTemplate>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                <%# Eval("Label") %>
                                            </label>
                                            <insite:TextBox runat="server" ID="Value" Text='<%# Eval("Value") %>' ReadOnly="true" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="mb-3 text-center">
                <insite:Literal runat="server" Text="Duplicate the information above in the fields below:" />
            </div>

            <asp:Repeater runat="server" ID="InputColumnRepeater">
                <HeaderTemplate>
                    <div class="row quiz-input">
                </HeaderTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
                <ItemTemplate>
                    <div class="col mb-3">
                        <div class="card h-100">
                            <div class="card-body">

                                <asp:Repeater runat="server" ID="RowRepeater">
                                    <ItemTemplate>
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                <%# Eval("Label") %>
                                            </label>
                                            <insite:TextBox runat="server" ID="Value" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="mt-3">
                <insite:Button runat="server" ID="NextButton"
                    Text="Next"
                    Icon="fas fa-arrow-alt-right"
                    IconPosition="AfterText"
                    ButtonStyle="Primary"
                    PostBackEnabled="false"
                />
                <insite:Button runat="server" ID="CompleteButton"
                    Text="Complete" 
                    Icon="fas fa-cloud-upload"
                    ButtonStyle="Success"
                    PostBackEnabled="false"
                />
            </div>
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:LoadingPanel runat="server" ID="LoadingPanel" />
    <insite:UpdatePanel runat="server" ID="UpdatePanel" />
</div>

<insite:PageHeadContent runat="server">
    <insite:ResourceBundle runat="server" Type="Css">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Portal/Assessments/QuizAttempts/Content/timer.css" />
            <insite:ResourceBundleFile Url="/UI/Portal/Assessments/QuizAttempts/Content/typing-accuracy.css" />
        </Items>
    </insite:ResourceBundle>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <insite:ResourceBundle runat="server" Type="JavaScript">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Portal/Assessments/QuizAttempts/Content/timer.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/Assessments/QuizAttempts/Content/typing-accuracy.js" />
        </Items>
    </insite:ResourceBundle>
</insite:PageFooterContent>
