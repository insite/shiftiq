<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PublicationSetup.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.PublicationSetup" %>

<insite:PageHeadContent runat="server" ID="HeaderContent">
    <style type="text/css">
        .res-cont-list {
            width: 100%;
        }

        .res-cont-list > h4:first-child {
            margin-top: 0;
        }

        .res-cont-list > h4 > span.small-print strong {
            color: #808080;
        }

        .res-cont-list > table {
            width: 100%;
        }

        .res-cont-list > table > tr > td {
            vertical-align: baseline !important;
        }

        .res-cont-list .ui-sortable {
        }

        .res-cont-list .ui-sortable > tr,
        .res-cont-list .ui-sortable > tbody > tr {
            cursor: move !important;
        }

        .res-cont-list .ui-sortable > tr > td:first-child i.move,
        .res-cont-list .ui-sortable > tbody > tr > td:first-child i.move {
            display: inline-block !important;
        }

        .res-cont-list .ui-sortable > tr:hover,
        .res-cont-list .ui-sortable > tbody > tr:hover {
            outline: 1px solid #666666;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-helper,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-helper {
            outline: 1px solid #666666;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-helper > td:first-child,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-helper > td:first-child {
            background-image: none !important;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-placeholder,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-placeholder {
            visibility: visible !important;
            outline: 1px dashed #666666 !important;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-placeholder > td:first-child,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-placeholder > td:first-child {
            background-image: none !important;
        }
    </style>
</insite:PageHeadContent>

<div class="row mb-3">
    <div class="col-md-12">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <insite:Nav runat="server">

                    <insite:NavItem runat="server" Title="Program Publication">

                        <div class="row mb-3">
                            <div class="col-lg-6">

                                <div class="card h-100">
                                    <div class="card-body">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Program Slug (URL Segment)
                                                <insite:RequiredValidator runat="server" ControlToValidate="ProgramSlug" ValidationGroup="ProgramSetup" />
                                            </label>
                                            <div>
                                                <insite:TextBox ID="ProgramSlug" runat="server" MaxLength="100" Width="100%" />
                                            </div>
                                            <div class="form-text">
                                                The part of the URL that specifically refers to this program.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div runat="server" id="ProgramIconPreview" class="float-end"></div>
                                            <label class="form-label">
                                                Program Icon
                                            </label>
                                            <div>
                                                <insite:TextBox runat="server" ID="ProgramIcon" MaxLength="30" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3" runat="server" id="ProgramImageField">
                                            <label class="form-label">
                                                Program Image
                                            </label>

                                            <div style="position: relative;">
                                                <asp:Image runat="server" ID="ProgramImage" CssClass="img-responsive" />
                                                <div style="position: absolute; top: 0px; right: 0px;">
                                                    <insite:IconButton runat="server" ID="DeleteImage" Name="trash-alt" Type="Solid" ToolTip="Delete this image" />
                                                </div>
                                            </div>

                                        </div>

                                        <div class="mb-3">
                                            <insite:FileUploadV2 runat="server" ID="ProgramImageUploadV2" LabelText="Upload New Program Image"/>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                URL overwrite
                                            </label>
                                            <insite:TextBox runat="server" ID="ProgramImageUrl" />
                                        </div>
                                    </div>

                                </div>

                            </div>
                            <div class="col-lg-6">

                                <div class="card h-100">
                                    <div class="card-body">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Web Portal
                                            </label>
                                            <insite:WebSiteComboBox runat="server" ID="WebSiteIdentifier" AllowBlank="true" />
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Web Folder
                                            </label>
                                            <insite:WebFolderComboBox runat="server" ID="WebFolderIdentifier" AllowBlank="true" />
                                        </div>

                                        <div runat="server" id="WebPagePanel" visible="false">

                                            <div class="form-group mb-3">
                                                <div class="float-end">
                                                    <insite:IconButton runat="server" ID="WebPageIdentifierAdd" Name="plus-circle" Type="Solid" />
                                                    <span runat="server" id="WebPageIdentifierLinks">
                                                        <a runat="server" id="WebPageIdentifierEdit" href="#" target="_blank"><i class="icon fas fa-fw fa-pencil"></i></a>
                                                        <a runat="server" id="WebPageIdentifierView" href="#" target="_blank"><i class="icon fas fa-fw fa-external-link-square"></i></a>
                                                    </span>
                                                </div>
                                                <label class="form-label">
                                                    Web Page
                                                </label>
                                                <div>
                                                    <insite:WebPageComboBox runat="server" ID="WebPageIdentifier" AllowBlank="true" />
                                                </div>
                                                <div runat="server" id="WebPageHelp" class="form-text"></div>
                                            </div>

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Web Page Status
                                                </label>
                                                <div>
                                                    <asp:CheckBox runat="server" ID="PublicationStatus" Text="Published" />
                                                </div>
                                            </div>

                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-md-12">
                                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="NotificationSetup" />
                                <insite:CancelButton runat="server" ID="CancelButton" />
                            </div>
                        </div>

                    </insite:NavItem>

                    <insite:NavItem runat="server" Title="Tasks Ordering">

                        <div runat="server" id="NoItemsMessage" style="margin-top: 8px; padding: 8px 16px; background-color: #f5f5f5; border: 1px solid #ccc; border-radius: 4px;" visible="false">
                            No Tasks in Program
                        </div>

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="UpdatePanel">
                            <ContentTemplate>
                                <div class="mb-3 text-end">
                                    <div id="CommandButtons" runat="server" class="reorder-trigger reorder-hide">
                                        <insite:Button runat="server" ID="ReorderButton" Text="Reorder" Icon="fas fa-sort" ButtonStyle="Default" />
                                    </div>
                                    <asp:Panel ID="ReorderCommandButtons" runat="server" CssClass="reorder-trigger reorder-visible reorder-inactive">
                                        <insite:SaveButton runat="server" OnClientClick="inSite.common.gridReorderHelper.saveReorder(true); return false;" />
                                        <insite:CancelButton runat="server" OnClientClick="inSite.common.gridReorderHelper.cancelReorder(true); return false;" />
                                    </asp:Panel>
                                </div>

                                <div runat="server" id="SectionControl">

                                    <div class="res-cont-list">
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
                            </ContentTemplate>
                        </insite:UpdatePanel>

                        <asp:Button runat="server" ID="RefreshButton" style="display:none;" />


                    </insite:NavItem>

                </insite:Nav>

            </div>
        </div>
    </div>
</div>


