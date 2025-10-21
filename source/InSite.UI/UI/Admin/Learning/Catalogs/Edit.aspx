<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Admin.Learning.Catalogs.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Catalog" />

    <section class="mb-3">

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">
                    <div class="col-lg-4">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Catalog Name
                                <insite:RequiredValidator runat="server" ControlToValidate="CatalogName" ValidationGroup="Catalog" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="CatalogName" MaxLength="100" />
                                
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Status
                            </label>
                            <div>
                                <insite:CheckBox runat="server" ID="CatalogIsHidden" Text="Hidden" />
                            </div>
                        </div>

                    </div>
                    <div class="col-lg-4">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Courses
                            </label>
                            <div class="table-responsive">
                                <table class="table table-striped">

                                <asp:Repeater runat="server" ID="CourseRepeater">
                                    <ItemTemplate>
                                        <tr>

                                            <td><%# Eval("Title") %></td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>

                                </table>
                            </div>
                        </div>

                    </div>
                    <div class="col-lg-4">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Programs
                            </label>
                            <div class="table-responsive">
                                <table class="table table-striped">

                                    <asp:Repeater runat="server" ID="ProgramRepeater">
                                        <ItemTemplate>
                                            <tr>

                                                <td><%# Eval("ProgramName") %></td>

                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </table>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Catalog" />
    <insite:DeleteButton runat="server" ID="DeleteButton" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
