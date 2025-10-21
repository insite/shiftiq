<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Utilities.Collections.Forms.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>
<%@ Register Src="../Controls/ItemGrid.ascx" TagName="ItemGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="EditorStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Collection" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <div class="row mb-3">
        <div class="col-12">

            <section runat="server" ID="CollectionSection">
                <h2 class="h4 mb-3">
                    <i class="far fa-album-collection me-1"></i>
                    Collection
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:Detail runat="server" ID="Detail" />

                        <div class="row">
                            <div class="col-lg-12">
                                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Collection" />
                                <insite:CancelButton runat="server" ID="CancelButton" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            <section runat="server" ID="ItemsSection">
                <h2 class="h4 pt-4 mb-3">
                    <i class="far fa-album-collection me-1"></i>
                    <asp:Literal runat="server" ID="ItemsSectionTitle" Text="Collection Items" />
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:ItemGrid runat="server" ID="ItemGrid" />
                    </div>
                </div>
            </section>

        </div>
    </div>
    
</asp:Content>
