<%@ Page Language="C#" CodeBehind="ModifyContent.aspx.cs" Inherits="InSite.Admin.Records.Programs.ModifyContent" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Achievement" />

    <section class="mb-3">
        <div class="row">
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="row row-translate">
                            <div class="col-md-12">
                                <asp:Repeater runat="server" ID="ContentRepeater">
                                    <ItemTemplate>
                                        <div class="form-group mb-3">
                                            <insite:DynamicControl runat="server" ID="Container" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Content" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>

</asp:Content>
