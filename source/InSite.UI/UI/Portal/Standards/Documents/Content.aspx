<%@ Page Language="C#" CodeBehind="Content.aspx.cs" Inherits="InSite.UI.Portal.Standards.Documents.Content" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Layout/Common/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <div class="row">
        <div class="col-md-12">
            <div class="card card-hover shadow mb-3">
                <div class="card-header">
                    <h3 class="m-0 text-primary"><asp:Literal runat="server" ID="SectionTitle" /></h3>
                </div>
                <div class="card-body">
                    <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="Standard" />
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Standard" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
