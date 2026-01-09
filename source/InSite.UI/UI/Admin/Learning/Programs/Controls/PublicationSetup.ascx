<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PublicationSetup.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.PublicationSetup" %>

<div class="row mb-3">
    <div class="col-md-12">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <insite:Nav runat="server">

                    <insite:NavItem runat="server" ID="PublicationTab" Title="Program Publication">

                        <div class="row mb-3">
                            <div class="col-lg-6">

                                <div class="card h-100">
                                    <div class="card-body">

                                        <div class="float-end">
                                            <insite:IconLink runat="server" ID="ModifyPublicationLink1" CssClass="p-2" ToolTip="Modify publication" Name="pencil" />
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Program Slug (URL Segment)
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="ProgramSlug" />
                                            </div>
                                            <div class="form-text">
                                                The part of the URL that specifically refers to this program.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Program Icon
                                            </label>
                                            <div>
                                                <div runat="server" id="ProgramIconPreview" class="d-inline-block me-1"></div>
                                                <asp:Literal runat="server" ID="ProgramIcon" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3" runat="server" id="ProgramImageField">
                                            <label class="form-label">
                                                Program Image
                                            </label>

                                            <div style="position: relative;">
                                                <asp:Image runat="server" ID="ProgramImage" CssClass="img-responsive" />
                                            </div>
                                        </div>

                                    </div>
                                </div>

                            </div>
                            <div runat="server" id="WebPageSection" visible="false" class="col-lg-6">

                                <div class="card h-100">
                                    <div class="card-body">

                                        <div class="float-end">
                                            <insite:IconLink runat="server" ID="ModifyPublicationLink2" CssClass="p-2" ToolTip="Modify publication" Name="pencil" />
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Web Portal
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="WebSiteName" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Web Folder
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="WebFolderName" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <a runat="server" id="WebPageIdentifierEdit" href="#" target="_blank"><i class="icon fas fa-pencil"></i></a>
                                                <a runat="server" id="WebPageIdentifierView" href="#" target="_blank"><i class="icon fas fa-external-link-square"></i></a>
                                            </div>
                                            <label class="form-label">
                                                Web Page
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="WebPageName" />
                                            </div>
                                            <div runat="server" id="WebPageHelp" class="form-text"></div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Web Page Status
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="PublicationStatus" />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>

                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="TasksTab" Title="Tasks Ordering">

                        <div class="row mb-3">
                            <div class="col-lg-6">

                                <div class="card">
                                    <div class="card-body">

                                        <div class="float-end">
                                            <insite:IconLink runat="server" ID="ModifyPublicationLink3" CssClass="p-2" ToolTip="Modify publication" Name="pencil" />
                                        </div>

                                        <div runat="server" id="NoItemsMessage" class="alert alert-info" visible="false">
                                            No Tasks in Program
                                        </div>

                                        <asp:Repeater runat="server" ID="DataRepeater">
                                            <HeaderTemplate>
                                                <table class="table table-striped">
                                                    <thead>
                                                        <tr>
                                                            <th>Type</th>
                                                            <th>Title</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                            </HeaderTemplate>
                                            <FooterTemplate>
                                                    </tbody>
                                                </table>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="res-type">
                                                        <%# GetDisplayTextType((string)Eval("Type")) %>
                                                    </td>
                                                    <td class="res-name">
                                                        <%# Eval("TaskTitle") %>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                    </div>
                                </div>

                            </div>
                        </div>

                    </insite:NavItem>

                </insite:Nav>

            </div>
        </div>
    </div>
</div>


