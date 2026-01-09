<%@ Page Language="C#" CodeBehind="Assign.aspx.cs" Inherits="InSite.Admin.Records.Programs.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ProgramTaskRepeater" Src="../Controls/ProgramTaskRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" />

    <section runat="server" id="ProgramSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-graduation-cap me-1"></i>
            Edit Programs Tasks
        </h2>

        <div class="row">
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="form-group mb-3">
                            <asp:Label ID="ProgramCodeLabel" runat="server" Text="Program Code" CssClass="form-label" AssociatedControlID="ProgramCode" />
                            <div>
                                <asp:Literal runat="server" ID="ProgramCode" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <asp:Label ID="ProgramNameLabel" runat="server" Text="Program Name" CssClass="form-label" AssociatedControlID="ProgramName" />
                            <div>
                                <asp:Literal runat="server" ID="ProgramName" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <asp:Label ID="ProgramTagLabel" runat="server" Text="Program Tag" CssClass="form-label" AssociatedControlID="ProgramTag" />
                            <div>
                                <asp:Literal runat="server" ID="ProgramTag" />
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <asp:Label ID="ProgramDescriptionLabel" runat="server" Text="Description" CssClass="form-label" AssociatedControlID="ProgramDescription" />
                            <div>
                                <span style="white-space:pre-wrap;"><asp:Literal runat="server" ID="ProgramDescription" /></span>
                            </div>
                        </div>
                        <div class="form-group mb-3">
                            <asp:Label ID="ProgramIdentifierLabel" runat="server" Text="Program Identifier" CssClass="form-label" AssociatedControlID="ProgramIdentifier" />
                            <div>
                                <asp:Literal runat="server" ID="ProgramIdentifier" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <div class="form-group mb-3">
                            <div>
                                <insite:ComboBox runat="server" ID="TaskType" AllowBlank="true" EmptyMessage="Task Type" >
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Value="Assessment" Text="Assessment" />
                                        <insite:ComboBoxOption Value="Achievement" Text="Achievement" />
                                        <insite:ComboBoxOption Value="Course" Text="Course" />
                                        <insite:ComboBoxOption Value="Logbook" Text="Logbook" />
                                        <insite:ComboBoxOption Value="Survey" Text="Form" />
                                    </Items>
                                </insite:ComboBox>
                            </div> 
                        </div>
                        <div class="form-group mb-3">
                            <div>
                                <insite:ComboBox runat="server" ID="TaskList" AllowBlank="true" EmptyMessage="Task" />
                            </div>
                        </div>
                        <div ID="TaskSubListDiv" runat="server" class="form-group mb-3">
                            <div>
                                <insite:ComboBox runat="server" ID="TaskSubList" AllowBlank="true" EmptyMessage="Assessment Form" />
                            </div>
                        </div>
                        <div runat="server" ID="HelperText" class="mb-3">
                            <asp:Label ID="HelperTextLabel" runat="server" CssClass="form-text m-2 mt-1 text-info"></asp:Label>
                        </div>
                        <div class="mb-3">
                            <insite:AddButton runat="server" id="AddTask" Text="Add Task" CssClass="float-end" />
                        </div>
                        <div class="form-group mb-3">
                            <div>
                                <asp:Repeater runat="server" ID="TaskRepeater" >
                                    <HeaderTemplate>
                                        <table class="table table-striped">
                                            <thead>
                                                <tr>
                                                    <th>Task Type</th>
                                                    <th>Task</th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div>
                                            <tr>
                                                <td>
                                                    <%# GetDisplayTextType((string)Eval("Type")) %>
                                                </td>
                                                <td>
                                                    <%# Eval("TaskTitle") %>
                                                </td>
                                                <td>
                                                    <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" CommandArgument='<%# Eval("TaskIdentifier") %>' ToolTip="Remove Item" OnClientClick="if (!confirm('Are you sure you want to remove this item?')) return false;" />
                                                </td>
                                            </tr>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tbody></table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </section>

    <insite:SaveButton runat="server" ID="SaveButton" />
    <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />

</asp:Content>
