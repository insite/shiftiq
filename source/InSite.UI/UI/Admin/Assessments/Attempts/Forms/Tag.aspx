<%@ Page Language="C#" CodeBehind="Tag.aspx.cs" Inherits="InSite.Admin.Assessments.Attempts.Forms.Tag" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    
    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="AttemptsTab" Title="Attempts" Icon="far fa-tasks" IconPosition="BeforeText">

            <div runat="server" id="NoAttempts" class="alert alert-warning" role="alert">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                There are no submissions
            </div>

            <asp:Repeater runat="server" ID="AttemptRepeater">
                <HeaderTemplate>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Exam Form</th>
                                <th>Exam Candidate</th>
                                <th>Date and Time</th>
                                <th>Attempt Tag</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("Form.FormTitle") %>
                            <div class="form-text">
                                <%# Eval("Form.FormName") %>
                                &bull;
                                Exam Form Asset #<%# Eval("Form.FormAsset") %>
                            </div>
                        </td>
                        <td>
                            <a href='/ui/admin/contacts/people/edit?<%# Eval("LearnerPerson.UserIdentifier", "contact={0}") %>'><%# Eval("LearnerPerson.UserFullName") %></a>
                            <span class="form-text"><%# Eval("LearnerPerson.PersonCode") %></span>
                            <div>
                                <%# Eval("LearnerPerson.UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
                            </div>
                        </td>
                        <td>
                            <%# FormatTime(Container.DataItem) %>
                        </td>
                        <td>
                            <asp:Literal runat="server" ID="AttemptIdentifier" Visible="false" Text='<%# Eval("AttemptIdentifier") %>' />
                            <asp:Literal runat="server" ID="OldAttemptTag" Visible="false" Text='<%# Eval("AttemptTag") %>' />
                            <insite:TextBox runat="server" ID="AttemptTag" MaxLength="64" Text='<%# Eval("AttemptTag") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>

            <div class="mt-3">
                <insite:NextButton runat="server" ID="NextButton" />
                <insite:CancelButton runat="server" ID="CancelButton1" />
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="ChangesTab" Title="Changes" Icon="far fa-tag" IconPosition="BeforeText" Visible="false">

            <div runat="server" id="NoChanges" class="alert alert-warning" role="alert">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                There are no changes
            </div>

            <asp:Repeater runat="server" ID="ChangeRepeater">
                <HeaderTemplate>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                                <th>Exam Form</th>
                                <th>Exam Candidate</th>
                                <th>Date and Time</th>
                                <th>Attempt Tag</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("Form.FormTitle") %>
                            <div class="form-text">
                                <%# Eval("Form.FormName") %>
                                &bull;
                                Exam Form Asset #<%# Eval("Form.FormAsset") %>
                            </div>
                        </td>
                        <td>
                            <%# Eval("LearnerPerson.UserFullName") %>
                            <span class="form-text"><%# Eval("LearnerPerson.PersonCode") %></span>
                            <div>
                                <%# Eval("LearnerPerson.UserEmail") %>
                            </div>
                        </td>
                        <td>
                            <%# FormatTime(Container.DataItem) %>
                        </td>
                        <td>
                            <strong><%# Eval("AttemptTag") %></strong>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>

            <div class="mt-3">
                <insite:SaveButton runat="server" ID="SaveButton" />
                <insite:CancelButton runat="server" ID="CancelButton2" />
            </div>

        </insite:NavItem>

    </insite:Nav>


</asp:Content>
