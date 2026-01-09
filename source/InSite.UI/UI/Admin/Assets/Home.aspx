<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" 
    Inherits="InSite.UI.Admin.Assets.Dashboard" 
    MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2">    
        
        <h2 class="h4 mb-3">Content</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="ContentCard" class="col">
                        <a runat="server" id="ContentLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="ContentCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-scroll fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Contents</h3>
                            </div>
                        </a>
                    </div>
            
                    <div runat="server" id="GlossaryCard" class="col">
                        <a runat="server" id="GlossaryLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="GlossaryCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-books fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Glossaries</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="LabelCard" class="col">
                        <a runat="server" id="LabelLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="LabelCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-tag fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Labels</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="/ui/admin/assets/glossaries/terms/upload"><i class="fas fa-upload me-1"></i>Upload Glossary</a>
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2" runat="server" id="FilesSection">

        <h2 class="h4 mb-3">Files</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="UploadCard" class="col">
                        <a runat="server" id="UploadLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="UploadCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-file-alt fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Uploads</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a runat="server" id="FileBrowserLink" class="me-3" href="/ui/admin/assets/files/browse"><i class="far fa-folder-tree me-2"></i> Browse Shared Library Files</a>
                    </div>
                </div>

            </div>
        </div>

    </section>

    <section class="pb-5 mb-md-2">    
        
        <h2 class="h4 mb-3">SCORM</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row">
                    <div class="col-lg-12">
                        
                    </div>
                </div>

            </div>
        </div>

    </section>

</asp:Content>