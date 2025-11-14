<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.MyProfile.View" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/MyJobOpportunityGrid.ascx" TagName="MyJobOpportunityGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div runat="server" id="ViewDetail" visible="false" class="row mb-4">

        <div class="col-12">

            <div class="card shadow-lg">

                <div class="card-body">

                    <h4 runat="server" id="CompanyHeading" class="card-title mb-3">Company Information</h4>

                    <div class="row">
                        <div class="col-6">
                            
                            <div runat="server" id="ParentPanel" class="row mb-3">
                                <h5 runat="server" id="ParentLabel" class="mt-2 mb-1">Parent Company</h5>
                                <div>
                                    <asp:Literal runat="server" ID="ParentName" />
                                </div>
                            </div>

                            <div runat="server" id="EmployerPanel" class="row mb-3">
                                <h5 runat="server" id="EmployerLabel" class="mt-2 mb-1">Company Name</h5>
                                <div>
                                    <div class="mb-1"><asp:Literal runat="server" ID="EmployerName"/></div>
                                    <asp:Literal runat="server" ID="EmployerTags"/>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">
                                    Account Status
                                </h5>
                                <div>
                                    <asp:Literal runat="server" ID="EmpAccountStatus" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Industry</h5>
                                <div>
                                    <asp:Literal runat="server" ID="EmpIndustry" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Number of Employees</h5>
                                <div>
                                    <asp:Literal runat="server" ID="EmpNumberText" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Description</h5>
                                <div>
                                    <asp:Literal runat="server" ID="GroupDescription" />
                                </div>
                            </div>

                            <div runat="server" id="OccupationsField" class="row mb-3">
                                <h5 class="mt-2 mb-1">Occupations We Seek</h5>
                                <div>
                                    <asp:Repeater runat="server" ID="OccupationList">
                                        <HeaderTemplate><ul></HeaderTemplate>
                                        <ItemTemplate>
                                            <li><%# Eval("Text") %></li>
                                        </ItemTemplate>
                                        <FooterTemplate></ul></FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>

                        </div>
                        <div class="col-6">

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Office Phone</h5>
                                <div>
                                    <asp:Literal runat="server" ID="EmpPhone" />
                                </div>
                            </div>

                            <div runat="server" id="AddressField" class="row mb-3">
                                <h5 class="mt-2 mb-1">Head Office</h5>
                                <div>
						            <asp:Literal runat="server" ID="EmpAddressLine" /><br />
						            <asp:Literal runat="server" ID="EmpAddressCity" />,
						            <asp:Literal runat="server" ID="EmpAddressProvince" /><br />
						            <asp:Literal runat="server" ID="EmpAddressCountry" />
                                    <asp:Literal runat="server" ID="EmpAddressPostalCode" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <h5 class="mt-2 mb-1">Website</h5>
                                <div>
                                    <asp:Literal runat="server" ID="WebSiteUrl" />
                                </div>
                            </div>

                            <div runat="server" id="SocialMediaPanel" class="row mb-3">
                                <h5 class="mt-2 mb-1">Social Media</h5>
                                <div>
                                    <asp:Literal runat="server" ID="SocialMediaUrls" />
                                </div>
                            </div>

                            <div runat="server" id="SitesPanel" class="row mb-3">
                                <h5 class="mt-2 mb-1">Sites</h5>
                                <div>
                                    <ul>
                                        <asp:Repeater runat="server" ID="SiteRepeater">
                                            <ItemTemplate>
                                                <li><a href="/ui/portal/job/employers/profile/view?group=<%# Eval("Identifier") %>"><%# Eval("Name") %></a></li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </div>
                            </div>

                        </div>
                    </div>

                    <div class="row mt-3">
                        <div class="col-12">
                            <insite:Button runat="server" ID="EditButton"
                                Text="Edit"
                                ButtonStyle="OutlinePrimary"
                                Icon="far fa-pencil"
                                
                            />
                        </div>
                    </div>

                </div>
            
            </div>

        </div>

    </div>

    <uc:MyJobOpportunityGrid runat="server" ID="MyJobOpportunityGrid" Visible="false" />

    <div runat="server" id="PhotoGalleryPanel" class="row mb-4" visible="false">

        <div class="col-12">

            <div class="card shadow-lg">

                <div class="card-body">

                    <h4 class="card-title mb-3">Photo Gallery</h4>

                    <asp:Repeater runat="server" ID="GalleryItemRepeater">
                        <HeaderTemplate><div id="<%# GalleryItemRepeater.ClientID %>" class="row"></HeaderTemplate>
                        <FooterTemplate></div></FooterTemplate>
                        <ItemTemplate>
                            <div class="col-lg-4 col-sm-6 mb-4">
                                <a href="<%# Eval("ResourceUrl") %>" class="gallery-item rounded-3<%# (bool)Eval("IsVideo") ? " gallery-video" : string.Empty %>"<%# (bool)Eval("HasCaption") ? string.Format(" data-sub-html='<h6 class=\"fs-sm text-light\">{0}</h6>'", Eval("CaptionText")) : string.Empty %>>
                                    <img src="<%# Eval("ThumbnailUrl") %>" alt="" />
                                    <%# (bool)Eval("HasCaption") ? string.Format("<span class=\"gallery-caption\">{0}</span>", Eval("CaptionText")) : string.Empty %>
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                </div>

            </div>

        </div>

    </div>

</asp:Content>
