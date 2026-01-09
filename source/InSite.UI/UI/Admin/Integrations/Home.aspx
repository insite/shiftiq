<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Integrations.Home" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section runat="server" ID="Reports" class="pb-5 mb-md-2">
        
        <h2 class="h4 mb-3">Web Services</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">               
                
                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="APIRequestCounter" class="col">
                        <a runat="server" id="APIRequestLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="APIRequestCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-plug fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>API Requests</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/admin/integrations/test/lti'>
                            <div class="card-body text-center">
                                <i class='far fa-plug fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>LTI Integration Tests</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href='/ui/admin/integrations/test/d365'>
                            <div class="card-body text-center">
                                <i class='far fa-plug fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>D365 API Tests</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>
        </div>
    </section>

</asp:Content>