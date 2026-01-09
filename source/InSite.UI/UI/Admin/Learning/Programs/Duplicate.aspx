<%@ Page Language="C#" CodeBehind="Duplicate.aspx.cs" Inherits="InSite.Admin.Records.Programs.Duplicate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="Template" />

    <section runat="server" ID="TemplateSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pencil-ruler me-1"></i>
            Program
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Department
                                <insite:RequiredValidator runat="server" ControlToValidate="DepartmentIdentifier" FieldName="Department" ValidationGroup="Template" />
                            </label>
                            <div>
                                <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" Width="570px" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Program Name
                                <insite:RequiredValidator runat="server" ControlToValidate="ProgramName" FieldName="Program Name" ValidationGroup="Template" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="ProgramName" MaxLength="256" />
                            </div>
                            <div class="form-text"></div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <section runat="server" ID="AchievementsSection" class="mb-3" visible="false">
        <h2 class="h4 mb-3">
            <i class="far fa-trophy me-1"></i>
            Achievements
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body row-resources">
                <asp:Repeater runat="server" ID="AchievementTypeRepeater">
                    <ItemTemplate>

                        <h3><%# Eval("Key") %></h3>

                        <asp:Repeater runat="server" ID="CategoryRepeater">
                            <ItemTemplate>
                                <div class="cat-title"><%# Eval("Key") %></div>

                                <div class="row">
                                    <div class="col-md-4">
                                        <asp:Repeater runat="server" ID="ItemRepeater">
                                            <ItemTemplate>
                                                <%# (bool)Eval("IsColumnSeparator") ? "</div><div class='col-md-4'>" : string.Empty %>
                                                <div class="res-item">
                                                    <%# Eval("IconHtml") %>
                                                    <%# Eval("Title") %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </section>

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Template" />
    <insite:CancelButton runat="server" ID="CancelButton" />

    <insite:PageHeadContent runat="server">
        <style type="text/css">

            .row-resources {
            }

            .row-resources h3 {
                margin-top: 16px !important;
            }

            .row-resources .cat-title {
                margin-top: 8px;
                margin-bottom: 4px;
                font-weight: bold;
            }

            .row-resources .res-item {
                padding-left: 23px;
                margin-bottom: 4px;
            }

            .row-resources .res-item > i.far {
                position: absolute;
                margin-left: -23px;
                line-height: 1.5;
            }

        </style>
    </insite:PageHeadContent>
</asp:Content>
