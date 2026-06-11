<%@ Page Language="C#" CodeBehind="TakerReportUpload.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.TakerReportUpload" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="StatusAlert" />

    <section>
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="form-group mb-3">
                    <label class="form-label">
                        CSV File
                    </label>
                    <div class="w-50">
                        <insite:FileUploadV2 runat="server" ID="CsvFile" AllowedExtensions=".csv" LabelText="" />
                    </div>
                </div>
            </div>
        </div>

        <div runat="server" id="AttemptPanel" class="card border-0 shadow-lg mt-3" visible="false">
            <div class="card-body">

                <table class="table table-striped">
                    <thead>
                        <tr>
                            <th>Case</th>
                            <th>Person</th>
                            <th>Birthdate</th>
                            <th>Exam Date</th>
                            <th>Exam Language</th>
                            <th>Exam Results</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater runat="server" ID="DataRepeater">
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <a target="_blank" href='<%# Eval("CaseId", "/ui/admin/workflow/cases/outline?case={0}") %>'>
                                            Case #<%# Eval("CaseNumber") %>
                                        </a>
                                        <div class="form-text">
                                            <%# Eval("CaseId") %>
                                        </div>
                                    </td>
                                    <td>
                                        <%# Eval("FirstName") %>
                                        <%# Eval("MiddleName") %>
                                        <%# Eval("LastName") %>
                                        <div class="form-text">
                                            <%# Eval("PersonCode") %>
                                        </div>
                                    </td>
                                    <td>
                                        <%# Eval("Birthdate") %>
                                    </td>
                                    <td>
                                        <%# Eval("ExamDate") %>
                                    </td>
                                    <td>
                                        <%# Eval("ExamLanguage") %>
                                    </td>
                                    <td>
                                        <ul>
                                        <asp:Repeater runat="server" ID="FrameworkStatusRepeater">
                                            <ItemTemplate>
                                                <li><%# Eval("EnglishTitle") %>: <%# Eval("Status") %></li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        </ul>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

                <insite:SaveButton runat="server" ID="SaveButton" Text="Save Reports" ButtonStyle="Success" DisableAfterClick="true" />
            </div>
        </div>

    </section>
</asp:Content>