<%@ Page Language="C#" CodeBehind="Archive.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Archive" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <style>
        .table-metrics {
            width: 100%;
        }

            .table-metrics td + td {
                text-align: right;
                width: 80px;
            }
    </style>


    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Form" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-window"></i>
            <asp:Literal ID="ArchiveHeader" runat="server"></asp:Literal>
        </h2>

        <div class="row">

            <div class="col-lg-4 mb-3 mb-lg-0">

                <div class="row mb-3">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Settings</h3>
                            <div class="form-group mb-3">
                                <label runat="server" id="ArchiveCommandName" class="form-label"></label>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="ArchiveOption">
                                        <asp:ListItem Value="Form" Text="Form" Selected="true" />
                                        <asp:ListItem Value="FormAndQuestions" Text="Form and Questions" />
                                        <asp:ListItem Value="FormAndQuestionsAndAttachments" Text="Form and Questions and Attachments" />
                                    </asp:RadioButtonList>
                                </div>
                                <div id="ArchiveCommandHelp" runat="server" class="form-text">
                                </div>
                            </div>

                        </div>
                    </div>

                </div>

                <div class="row mb-3">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Question Publication Status</h3>

                            <table class="table table-striped table-bordered table-metrics">
                                <asp:Repeater runat="server" ID="Questions">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%# Eval("Name") %>
                                            </td>
                                            <td>
                                                <%# Eval("Count") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>

                        </div>
                    </div>

                </div>

                <div class="row">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                            <h3>Attachment Publication Status</h3>

                            <table class="table table-striped table-bordered table-metrics">
                                <asp:Repeater runat="server" ID="Attachments">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%# Eval("Name") %>
                                            </td>
                                            <td>
                                                <%# Eval("Count") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>

                        </div>
                    </div>

                </div>
            </div>

            <div class="col-lg-8">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>

                        <uc:FormDetails ID="FormDetails" runat="server" />
                    </div>

                </div>

            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:Button runat="server" ID="ArchiveButton" Text="Archive" Icon="fas fa-archive" ButtonStyle="Danger" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
