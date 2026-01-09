<%@ Page Language="C#" CodeBehind="Info.aspx.cs" Inherits="InSite.Admin.Standards.Standards.Forms.Info" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<div runat="server" id="RowCommands" class="mb-3 d-flex commands" visible="false">
    <insite:Button runat="server" ID="EditButton" CssClass="me-1" data-action="edit"
        ButtonStyle="Default" Icon="fas fa-pencil me-1" ToolTip="Edit" />
    <insite:Button runat="server" ID="CreateButton" CssClass="me-1" 
        ButtonStyle="Default" Icon="fa fa-plus" ToolTip="Create" />

    <span class="sub-commands" data-parent='<%= CreateButton.ClientID %>'>
        &nbsp; &nbsp;
        <insite:Button runat="server" ID="CreateSaveButton" CssClass="me-1" CausesValidation="true" ValidationGroup="Creator"
            ButtonStyle="Success" Icon="far fa-cloud-upload" ToolTip="Save" />
        <insite:Button runat="server" ID="CreateCancelButton" CssClass="me-1" 
            ButtonStyle="Default" Icon="fa fa-ban" ToolTip="Cancel" />
    </span>

    <insite:Button runat="server" ID="ReorderButton" CssClass="me-1" 
        ButtonStyle="Default" Icon="fa fa-sort" ToolTip="Reorder" />

    <span class="sub-commands" data-parent='<%= ReorderButton.ClientID %>'>
        &nbsp; &nbsp;
        <insite:Button runat="server" ID="ReorderSaveButton" CssClass="me-1" 
            ButtonStyle="Success" Icon="far fa-cloud-upload" ToolTip="Save" />
        <insite:Button runat="server" ID="ReorderCancelButton" CssClass="me-1" 
            ButtonStyle="Default" Icon="fa fa-ban" ToolTip="Cancel" />
    </span>

    <insite:Button runat="server" ID="IndentButton" CssClass="me-1" 
        ButtonStyle="Default" Icon="fa fa-indent" ToolTip="Indent" />
    <insite:Button runat="server" ID="OutdentButton" CssClass="me-1" 
        ButtonStyle="Default" Icon="fa fa-outdent" ToolTip="Outdent" />

    <insite:Button runat="server" ID="CopyButton" CssClass="me-1" 
        ButtonStyle="Default" Icon="far fa-copy" ToolTip="Duplicate" />
    <insite:Button runat="server" ID="DeleteButton" data-action="delete" CssClass="me-1" 
        ButtonStyle="Default" Icon="fa fa-trash-alt" ToolTip="Delete" />

    <div runat="server" id="Separator1" class="d-inline-block ps-2"></div>

    <insite:Button runat="server" ID="OutlineButton" data-action="outline" CssClass="me-1" 
        ButtonStyle="Default" Icon="fa fa-sitemap" ToolTip="Outline" />

    <div class="d-inline-block ps-2"></div>

    <insite:Button runat="server" ID="DownloadButton" ButtonStyle="Primary" Icon="fa fa-download" ToolTip="Download JSON" />
</div>

<insite:Alert runat="server" ID="ScreenStatus" />

<insite:Container runat="server" ID="InfoContainer" Visible="false">
    <h4 runat="server" id="TitleOutput"></h4>

    <div runat="server" id="SummaryOutput" class="form-text" visible="false"></div>

    <div class="mt-3 ps-3">
        <div runat="server" id="GrandparentField" class="form-group mb-3" visible="false">
            <label class="form-label">Grandparent</label>
            <div class="readonly-value">
                <asp:HyperLink runat="server" ID="GrandparentLink" />
            </div>
        </div>

        <div runat="server" id="ParentField" class="form-group mb-3" visible="false">
            <label class="form-label">Parent</label>
            <div class="readonly-value">
                <asp:HyperLink runat="server" ID="ParentLink" />
            </div>
        </div>

        <div runat="server" id="PreviousSiblingField" class="form-group mb-3" visible="false">
            <label class="form-label">Previous Sibling</label>
            <div class="readonly-value">
                <asp:HyperLink runat="server" ID="PreviousSiblingLink" />
            </div>
        </div>

        <div runat="server" id="NextSiblingField" class="form-group mb-3" visible="false">
            <label class="form-label">Next Sibling</label>
            <div class="readonly-value">
                <asp:HyperLink runat="server" ID="NextSiblingLink" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Standard Type</label>
            <div runat="server" id="TypeOutput" class="readonly-value"></div>
        </div>

        <div runat="server" id="CodeField" class="form-group mb-3" visible="false">
            <label class="form-label">Code</label>
            <div runat="server" id="CodeOutput" class="readonly-value"></div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Database Key</label>
            <div runat="server" id="KeyOutput" class="readonly-value"></div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Identifier</label>
            <div runat="server" id="IdentifierOutput" class="readonly-value"></div>
        </div>
    </div>
</insite:Container>

<insite:Container runat="server" ID="CreateContainer" Visible="false">
    <h4>New Standard</h4>

    <div class="mt-3 ps-3 position-relative">
        <div class="form-group mb-3">
            <label class="form-label">
                Type
                <insite:RequiredValidator runat="server" ControlToValidate="CreateType" FieldName="Type" ValidationGroup="Creator" />
            </label>
            <div>
                <insite:StandardTypeComboBox runat="server" ID="CreateType" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Title
                <insite:RequiredValidator runat="server" ControlToValidate="CreateTitle" FieldName="Title" ValidationGroup="Creator" />
            </label>
            <div>
                <insite:TextBox runat="server" ID="CreateTitle" MaxLength="512" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Relationship</label>
            <div>
                <div class="d-flex">
                    <div class="text-nowrap pe-4" >
                        <insite:RadioButton runat="server" ID="CreateInsertIn" Text="is contained by" GroupName="CreateInsert" CssClass="m-t-0" />
                        <insite:RadioButton runat="server" ID="CreateInsertAfter" Text="is after" GroupName="CreateInsert" />
                        <insite:RadioButton runat="server" ID="CreateInsertBefore" Text="is before" GroupName="CreateInsert" />
                    </div>
                    <div runat="server" id="CreateEntityTitle" class="w-100">
                    </div>
                </div>
            </div>
        </div>
    </div>
</insite:Container>

<insite:Container runat="server" ID="ReorderContainer" Visible="false">
    <div class="mt-3 ps-3">
        <asp:Repeater runat="server" ID="ReorderRepeater">
            <HeaderTemplate><ul class="list-group" data-reorder="#<%# ReorderState.ClientID %>"></HeaderTemplate>
            <FooterTemplate></ul></FooterTemplate>
            <ItemTemplate>
                <li class="list-group-item" data-number='<%# Eval("Number") %>'>
                    <i runat="server" class='<%# Eval("Icon", "fa {0}") %>' visible='<%# !string.IsNullOrEmpty((string)Eval("Icon")) %>'></i>
                    <%# Eval("Type") %> #<%# Eval("Number") %>: <%# Eval("Title") %>
                </li>
            </ItemTemplate>
        </asp:Repeater>
        <asp:HiddenField runat="server" ID="ReorderState" />
    </div>
</insite:Container>

<insite:PageHeadContent runat="server">
    <link type="text/css" rel="stylesheet" href="/UI/Admin/standards/standards/content/styles/info.css">
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.standardInfo = window.standardInfo || {};

            var isUpdateHeight = false;

            Sys.Application.add_load(onAppLoad);

            $('.commands [data-action]').on('click', onCommandButtonClick);

            $('ul[data-reorder]').each(function () {
                var $ul = $(this);
                if ($ul.data('reorderState'))
                    return;

                var $stateInput = $($ul.data('reorder'));
                if ($stateInput.length !== 1) {
                    $ul.data('reorderState', false);
                    return;
                }

                $ul.data('reorderState', {
                    $input: $stateInput
                });

                $ul.sortable({
                    items: '> li',
                    containment: $ul.closest('div'),
                    cursor: 'grabbing',
                    opacity: 0.65,
                    tolerance: 'pointer',
                    stop: function (e, a) {
                        var data = [];
                        var $ul = a.item.closest('ul');

                        $ul.find('> li').each(function () {
                            var number = parseInt($(this).data('number'));
                            if (!isNaN(number))
                                data.push(number);
                        });

                        $ul.data('reorderState').$input.val(JSON.stringify(data));
                    },
                });
            });

            // public methods

            instance.enableSubCommands = function (id, show) {
                var $cmdButtons = $('.commands > .btn');
                var $subCmd = $('.commands > .sub-commands[data-parent=' + String(id) + ']');

                show = show !== false;

                $cmdButtons.each(function () {
                    var $this = $(this);

                    if ($this.prop('id') == id) {
                        if (show) {
                            $this.on('click', onDisabledClick)
                                .attr('disabled', 'disabled')
                                .addClass('selected')
                                .blur();
                        } else {
                            $this.off('click', onDisabledClick)
                                .removeAttr('disabled')
                                .removeClass('selected');
                        }
                    } else {
                        if (show)
                            $this.css('visibility', 'hidden');
                        else
                            $this.css('visibility', '');
                    }
                });

                if (show)
                    $subCmd.show();
                else
                    $subCmd.hide();
            };

            instance.updateWindowHeight = function () {
                isUpdateHeight = true;
            };

            // events

            function onAppLoad() {
                if (isUpdateHeight) {
                    setTimeout(modalManager.updateModalHeight, 10);
                    isUpdateHeight = false;
                }
            }

            function onDisabledClick(e) {
                e.preventDefault();
                e.stopPropagation();
            }

            function onCommandButtonClick(e) {
                e.preventDefault();

                var $this = $(this);
                if ($this.attr('disabled'))
                    return;

                var action = $this.data('action');

                if (action === 'edit') {
                    modalManager.closeModal({ action: 'redirect', url: '/ui/admin/standards/edit?id=<%= CurrentData.StandardID %>' });
                } else if (action === 'delete') {
                    var returnUrl = inSite.common.queryString.returnUrl;
                    if (!returnUrl)
                        returnUrl = window.parent.location.pathname + window.parent.location.search;

                    modalManager.closeModal({ action: 'redirect', url: '/admin/standards/delete?asset=<%= CurrentData.StandardID %>&returnUrl=' + encodeURIComponent(returnUrl) });
                } else if (action === 'outline') {
                    modalManager.closeModal({ action: 'redirect', url: '/ui/admin/standards/manage?standard=<%= CurrentData.StandardID %>' });
                }
            }
        })();
    </script>
</insite:PageFooterContent>
</asp:Content>
