<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Invoices.Products.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="../Controls/Details.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Product" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pallet me-1"></i>
            Product
        </h2>

        <div class="row">

            <div class="col-xxl-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <div class="float-end position-absolute top-0 end-0 pe-4 pt-4">
                            <insite:Button runat="server" ID="PublishButton" Text="Publish" Icon="fas fa-upload" ButtonStyle="Default" CssClass="mb-3"  ConfirmText="Are you sure you want to publish this product?"/>
                            <insite:Button runat="server" ID="UnpublishButton" Text="Unpublish" Icon="fas fa-eraser" ButtonStyle="Default" CssClass="mb-3" ConfirmText="Are you sure you want to unpublish this product?"/>
                            <insite:Button runat="server" ID="ViewHistoryLink" Text="History" Icon="fas fa-history" ButtonStyle="Default" CssClass="mb-3" OnClientClick="editForm.historyWindow.show(); return false;" />
                        </div>
                        <div class="clearfix"></div>
                        <uc:Details runat="server" ID="ProductDetails" />
                    </div>
                </div>
            </div>

        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Product" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

    <insite:Modal runat="server" ID="HistoryWindow" Title="History" >
        <ContentTemplate>
            <div class="p-2">
                <div class="mb-3">

                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Created By
                                    </label>
                                    <div>
                                        <asp:Literal ID="CreatedBy" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Edited By
                                    </label>
                                    <div>
                                        <asp:Literal ID="ModifiedBy" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        <asp:Literal ID="PublishedByLiteral" runat="server" Text="Published By"></asp:Literal>
                                    </label>
                                    <div>
                                        <asp:Literal ID="PublishedBy" runat="server"></asp:Literal>
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Published On
                                    </label>
                                    <div>
                                        <asp:Literal ID="PublishedOn" runat="server"></asp:Literal>                                        
                                    </div>
                                </div>

                            </div>
                        </div>


                    <insite:Button runat="server" Text="Close" ButtonStyle="Success" Icon="fas fa-check" OnClientClick="editForm.historyWindow.close(); return false;"/>
                </div>
            </div>
        </ContentTemplate>
    </insite:Modal>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                var instance = window.editForm = window.editForm || {};

                instance.historyWindow = {
                    show: function () {
                        modalManager.show('<%= HistoryWindow.ClientID %>');
                    },
                    close: function () {
                        modalManager.close('<%= HistoryWindow.ClientID %>');
                    },
                };
            })();

        </script>
    </insite:PageFooterContent>

</asp:Content>
