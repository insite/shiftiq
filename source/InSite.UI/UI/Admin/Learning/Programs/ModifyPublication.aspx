<%@ Page Language="C#" CodeBehind="ModifyPublication.aspx.cs" Inherits="InSite.Admin.Records.Programs.ModifyPublication" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Settings" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <insite:Nav runat="server">

                    <insite:NavItem runat="server" ID="PublicationTab" Title="Program Publication">

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
                                            <insite:FileUploadV2 runat="server" ID="ProgramImageUploadV2" LabelText="Upload New Program Image" FileUploadType="Image"/>
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
                                                        <a runat="server" id="WebPageIdentifierEdit" href="#" target="_blank"><i class="icon fas fa-pencil"></i></a>
                                                        <a runat="server" id="WebPageIdentifierView" href="#" target="_blank"><i class="icon fas fa-external-link-square"></i></a>
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

                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="TasksTab" Title="Tasks Ordering">

                        <div runat="server" id="NoTaskItemsMessage" style="margin-top: 8px; padding: 8px 16px; background-color: #f5f5f5; border: 1px solid #ccc; border-radius: 4px;" visible="false">
                            No Tasks in Program
                        </div>

                        <asp:Repeater runat="server" ID="TaskItemRepeater">
                            <HeaderTemplate>
                                <table id='<%= TaskItemRepeater.ClientID %>' class="table table-striped table-tasks">
                                    <thead>
                                        <tr>
                                            <th style="width:40px;"></th>
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
                                <tr data-id='<%# Eval("TaskIdentifier") %>'>
                                    <td><span class="start-move"><i class="fas fa-arrows-alt"></i></span></td>
                                    <td>
                                        <%# GetDisplayTextType((string)Eval("Type")) %>
                                    </td>
                                    <td>
                                        <%# Eval("TaskTitle") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:HiddenField runat="server" ID="TasksValues" />

                    </insite:NavItem>

                </insite:Nav>

            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Publication" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            table.table-tasks > tbody .start-move {
                cursor: grab;
                display: inline-block;
                text-align: center;
            }

            table.table-tasks > tbody.ui-sortable > tr.ui-sortable-placeholder {
                visibility: visible !important;
                outline: 1px dashed #666666 !important;
            }

            table.table-tasks > tbody > tr.ui-sortable-helper > td {
                border-bottom: 1px solid #ddd;
            }
        </style>
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                $('table#<%= TaskItemRepeater.ClientID %>:first').sortable({
                    items: 'tbody > tr',
                    containment: 'document',
                    cursor: 'grabbing',
                    forceHelperSize: true,
                    handle: 'span.start-move',
                    opacity: 0.65,
                    tolerance: 'pointer',
                    dropOnEmpty: true,
                    helper: function (e, el) {
                        var $cells = el.children();
                        return el.clone().children().each(function (index) {
                            $(this).width($cells.eq(index).outerWidth());
                        }).end();
                    },
                    stop: function (e, a) {
                        var $tbody = a.item.closest('tbody');
                        var $input = $('#<%= TasksValues.ClientID %>');
                        var result = '';

                        $tbody.find('> tr').each(function () {
                            result += ';' + String($(this).data('id'));
                        });

                        $input.val(result.length > 0 ? result.substring(1) : '');
                    },
                }).disableSelection();
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>