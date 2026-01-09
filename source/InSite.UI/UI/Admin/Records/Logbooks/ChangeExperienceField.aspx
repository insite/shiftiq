<%@ Page Language="C#" CodeBehind="ChangeExperienceField.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.ChangeExperienceField" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Journal" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pencil-ruler me-1"></i>
            Field
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
            <div class="row">
                <div class="col-md-6">

                    <insite:DynamicControl runat="server" ID="Field" />

                </div>
            </div>
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Journal" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
