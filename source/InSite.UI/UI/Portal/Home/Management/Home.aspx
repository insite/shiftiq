<%@ Page Language="C#" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Portal.Home.Management.Home" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Admin/Foundations/Controls/AnnouncementToast.ascx" TagName="AnnouncementToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Foundations/Controls/MaintenanceToast.ascx" TagName="MaintenanceToast" TagPrefix="uc" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        .navbar .nav-item:hover>.nav-link:not(.disabled), .navbar .nav-item .nav-link.show:not(.disabled) {
            color: var(--ar-info);
        }

        .dropdown-item:hover, .dropdown-item:focus {
            color: var(--ar-info) !important;
            text-decoration: none;
            background-color: var(--ar-dropdown-link-hover-bg);
        }

        .skills-progress {
            --ar-progress-thickness: 0.75rem;
            --ar-progress-font-size: 1.5rem;
            width: 110px;
        }

            .skills-progress > span {
                font-weight: bold;
            }

                .skills-progress > span[data-length="6"] {
                    font-size: calc(var(--ar-progress-font-size) * 0.9);
                }

                .skills-progress > span[data-length="7"] {
                    font-size: calc(var(--ar-progress-font-size) * 0.8);
                }

                .skills-progress > span[data-length="8"] {
                    font-size: calc(var(--ar-progress-font-size) * 0.7);
                }

                .skills-progress > span[data-length="9"] {
                    font-size: calc(var(--ar-progress-font-size) * 0.65);
                }

                .skills-progress > span[data-length="10"] {
                    font-size: calc(var(--ar-progress-font-size) * 0.55);
                }

                .skills-progress > span[data-length="11"],
                .skills-progress > span[data-length="12"] {
                    font-size: calc(var(--ar-progress-font-size) * 0.5);
                }
                
                .skills-progress > span[data-length="13"],
                .skills-progress > span[data-length="14"] {
                    font-size: calc(var(--ar-progress-font-size) * 0.45);
                }

        .status-container .skills-progress {
            --ar-progress-thickness: 0.5rem;
            width: 80px;
        }

            .status-container .skills-progress > span[data-length="4"] {
                font-size: calc(var(--ar-progress-font-size) * 0.85);
            }

            .status-container .skills-progress > span[data-length="5"] {
                font-size: calc(var(--ar-progress-font-size) * 0.8);
            }

            .status-container .skills-progress > span[data-length="6"] {
                font-size: calc(var(--ar-progress-font-size) * 0.7);
            }

            .status-container .skills-progress > span[data-length="7"] {
                font-size: calc(var(--ar-progress-font-size) * 0.6);
            }

            .status-container .skills-progress > span[data-length="8"] {
                font-size: calc(var(--ar-progress-font-size) * 0.5);
            }

            .status-container .skills-progress > span[data-length="9"] {
                font-size: calc(var(--ar-progress-font-size) * 0.45);
            }

        .status-container .status-filter-checkbox {
            right: -10px;
            top: -10px;
        }

        .status-container > div .circular-progress {
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <uc:DashboardNavigation runat="server" ID="DashboardNavigation" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:UpdatePanel runat="server" ID="HomeStatusUpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>
            <insite:Alert runat="server" ID="HomeStatus" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:UserLicenseCheck runat="server" />
    <insite:UserPasswordCheck runat="server" />
    <insite:UserEmailVerificationCheck runat="server" />
    <uc:AnnouncementToast runat="server" ID="AnnouncementToast" />
    <uc:MaintenanceToast runat="server" ID="MaintenanceToast" ShowOnEachRequest="true" />

    <insite:UpdatePanel runat="server">
        <ContentTemplate>
            <div runat="server" id="BannerPanel" class="alert alert-skillscheck d-flex bg-primary text-white mb-5" role="alert" visible="false">
                <div class="d-flex align-items-center">
                    <div class="fs-1">
                        <i class="fas fa-party-horn"></i>
                    </div>
                    <div class="px-3 fs-1 text-nowrap">
                        Congratulations!
                    </div>
                    <div runat="server" id="BannerMessage" class="mx-2">
                    </div>
                </div>
                <button runat="server" id="CloseBannerButton" type="button" 
                    class="btn-close btn-close-white ms-auto mt-2"
                    data-bs-dismiss="alert" aria-label="Close" />
            </div>
        </ContentTemplate>
    </insite:UpdatePanel>

    <div class="d-flex flex-column flex-xl-row mb-3 pt-2">
        <div class="mb-3 mb-xl-0 me-xl-2 pe-xl-1">
            <div class="d-flex flex-row align-items-center">
                <insite:UpdatePanel runat="server" ID="ProductUpdatePanel">
                    <ContentTemplate>
                        <asp:Literal runat="server" ID="ProductProgress" />
                    </ContentTemplate>
                </insite:UpdatePanel>
                <div class="ms-3">
                    <div class="fw-bold">SkillsChecks</div>
                    <div class="small mb-3">to be assigned</div>

                    <insite:UpdatePanel runat="server">
                        <ContentTemplate>
                            <insite:ComboBox runat="server" ID="ProductFilter" ButtonSize="Small" Width="150px" EmptyMessage="All SkillsChecks" />
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>
        </div>
        <div class="flex-grow-1">
            <insite:UpdatePanel runat="server" ID="StatusFiltersUpdatePanel" CssClass="d-flex flex-row align-items-center justify-content-between status-container">
                <ContentTemplate>
                    <asp:Repeater runat="server" ID="StatusRepeater">
                        <ItemTemplate>
                            <div>
                                <div class="position-relative">
                                    <div class="position-absolute status-filter-checkbox">
                                        <insite:CheckBox runat="server" ID="IsSelected" AutoPostBack="true" Value='<%# Eval("Value") %>' Checked='<%# Eval("Checked") %>' />
                                    </div>
                                    <div class="px-2">
                                        <%# GetStatusProgressHtml((int)Eval("Total"), (int)Eval("Count")) %>
                                    </div>
                                    <div class="mt-2 text-center">
                                        <%# Eval("Title") %>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </ContentTemplate>
            </insite:UpdatePanel>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="AttemptIdField" />
    <asp:Button runat="server" ID="DownloadButton" CssClass="d-none" />

    <insite:UpdatePanel runat="server" ID="DistributionUpdatePanel">
        <ContentTemplate>
            <asp:Repeater runat="server" ID="DistributionRepeater">
                <HeaderTemplate>
                    <div class="table-responsive">
                        <table class="table">
                            <thead>
                                <tr>
                                    <th>Assigned To</th>
                                    <th>SkillsCheck</th>
                                    <th class="text-end">Score</th>
                                    <th class="text-center" style="width:20px;">Action</th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <FooterTemplate>
                            </tbody>
                        </table>
                    </div>
                </FooterTemplate>
                <ItemTemplate>
                    <tr data-form="<%# Eval("CourseDistributionIdentifier") %>">
                        <td>
                            <insite:Container runat="server" Visible='<%# Eval("LearnerUserIdentifier") == null && Eval("DistributionTransferred") != null && (Guid?)Eval("DistributionTransferredFromUserIdentifier") == User.Identifier %>'>
                                <%# Eval("ManagerUserName") %>
                            </insite:Container>
                            <insite:Container runat="server" Visible='<%# Eval("LearnerUserIdentifier") != null %>'>
                                <%# Eval("LearnerUserName") ?? "None" %>
                                <insite:Container runat="server" Visible='<%# Eval("DistributionTransferred") != null && (Guid?)Eval("DistributionTransferredFromUserIdentifier") == User.Identifier %>'>
                                    <div class="fs-sm">
                                        <div class="text-muted"><%# Eval("ManagerUserName") %></div>
                                        <div><span class="badge bg-warning">Transfer</span></div>
                                    </div>
                                </insite:Container>
                            </insite:Container>
                        </td>
                        <td>
                            <div class="mb-2"><%# GetProductName() %></div>
                            <small><%# GetGridStatusHtml() %></small>
                        </td>
                        <td class="text-end"><%# GetGridScoreHtml() %></td>
                        <td class="text-center">
                            <insite:Button runat="server"
                                Text="Assign"
                                Size="ExtraSmall"
                                ButtonStyle="Success"
                                OnClientClick="dashboardHome.assign(this); return false;"
                                Visible='<%# !IsPackage() && Eval("LearnerUserIdentifier") == null %>'
                            />
                            <insite:Container runat="server" Visible='<%# IsPackage() && Eval("LearnerUserIdentifier") == null && (Eval("DistributionTransferred") == null || (Guid)Eval("ManagerUserIdentifier") == User.Identifier) %>'>
                                <insite:Button runat="server"
                                    Text="Select"
                                    Size="ExtraSmall"
                                    ButtonStyle="Success"
                                    NavigateUrl="/ui/portal/management/dashboard/catalog?chooseLater=1"
                                />
                                <insite:Button runat="server"
                                    Text="Transfer"
                                    Size="ExtraSmall"
                                    ButtonStyle="Success"
                                    OnClientClick="dashboardHome.transfer(this); return false;"
                                    Visible='<%# AllowTransfer && Eval("DistributionTransferred") == null %>'
                                />
                            </insite:Container>
                            <insite:Container runat="server" Visible='<%# Eval("LearnerUserIdentifier") != null && Eval("AttemptImported") == null && Eval("AttemptStarted") == null && (Guid)Eval("ManagerUserIdentifier") == User.Identifier %>'>
                                <insite:Button runat="server" Text="Resend" CommandName="ResendLearner"
                                    Size="ExtraSmall" ButtonStyle="Default" CssClass="d-block mb-1" />
                                <insite:Button runat="server" Text="Cancel" CommandName="CancelLearner"
                                    Size="ExtraSmall" ButtonStyle="Default" CssClass="d-block" />
                            </insite:Container>
                            <insite:Container runat="server" Visible='<%# Eval("LearnerUserIdentifier") == null && Eval("DistributionTransferred") != null && (Guid?)Eval("DistributionTransferredFromUserIdentifier") == User.Identifier %>'>
                                <insite:Button runat="server" Text="Resend" CommandName="ResendTransfer"
                                    Size="ExtraSmall" ButtonStyle="Default" CssClass="d-block mb-1"
                                    ConfirmText='<%# Eval("ManagerUserName", "Are you sure you want to resend the welcome email to {0}?") %>' />
                                <insite:Button runat="server" Text="Cancel" CommandName="CancelTransfer"
                                    Size="ExtraSmall" ButtonStyle="Default" CssClass="d-block"
                                    ConfirmText="Are you sure you want to cancel this SkillCheck transfer?" />
                            </insite:Container>
                            <insite:Container runat="server" Visible='<%# Eval("LearnerUserIdentifier") != null && Eval("AttemptGraded") != null %>'>
                                <insite:Button runat="server" Text="Report" PostBackEnabled="false"
                                    OnClientClick='<%# Eval("AttemptIdentifier", "dashboardHome.downloadReport(\"{0}\");") %>'
                                    Size="ExtraSmall" ButtonStyle="Default" CssClass="d-block" />
                            </insite:Container>
                            <insite:Container runat="server" Visible='<%# Eval("LearnerUserIdentifier") != null && (Eval("AttemptImported") != null || Eval("AttemptStarted") != null) %>'>
                                -
                            </insite:Container>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:FindPerson runat="server" ID="AssignUserIdentifier" Output="None" 
        EntityName="Contact" KeywordFieldText="Enter Contacts Name" NoItemsMessageText="No Results" />
    <asp:HiddenField runat="server" ID="AssignFormIdentifier" />

    <insite:FindPerson runat="server" ID="TransferUserIdentifier" Output="None" CloseOnSelect="false" AllowClear="false"
        EntityName="Contact" KeywordFieldText="Enter Contacts Name" NoItemsMessageText="No Results" />

    <insite:UpdatePanel runat="server" ID="TransferCountUpdatePanel" CssClass="d-none">
        <ContentTemplate>
            <div class="form-group mt-3">
                <label class="form-label fw-bold mb-1">Number of SkillsCheck to Transfer</label>
                <div class="d-flex">
                    <div class="flex-shrink-0" style="width:120px;">
                        <insite:NumericBox runat="server" ID="TransferCount" MinValue="1" NumericMode="Integer" />
                    </div>
                    <div runat="server" id="TransferCountWarning" class="alert alert-warning py-2 px-3 mb-0 ms-3 small flex-grow-1 d-none"></div>
                </div>
            </div>
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:Modal runat="server" ID="AddContactWindow" Title="Add Contact">
        <ContentTemplate>
            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AddContactUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="AddContactUpdatePanel">
                <ContentTemplate>
                    <insite:ValidationSummary runat="server" ValidationGroup="AddContact" />

                    <div class="form-group mb-3">
                        <label class="form-label">
                            First Name
                            <insite:RequiredValidator runat="server" ControlToValidate="AddContactFirstName" FieldName="First Name" ValidationGroup="AddContact" />
                        </label>
                        <insite:TextBox runat="server" ID="AddContactFirstName" MaxLength="32" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Last Name
                            <insite:RequiredValidator runat="server" ControlToValidate="AddContactLastName" FieldName="Last Name" ValidationGroup="AddContact" />
                        </label>
                        <insite:TextBox runat="server" ID="AddContactLastName" MaxLength="32" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Email
                            <insite:EmailValidator runat="server" ControlToValidate="AddContactEmail" FieldName="Email" ValidationGroup="AddContact" Display="Dynamic" />
                            <insite:RequiredValidator runat="server" ID="AddContactEmailRequiredValidator" ControlToValidate="AddContactEmail" FieldName="Email" ValidationGroup="AddContact" Display="Dynamic" />
                        </label>
                        <insite:TextBox runat="server" ID="AddContactEmail" MaxLength="128" />
                    </div>

                    <div class="mt-5 text-end">
                        <insite:Button runat="server" ID="AddContactSaveButton" Text="Add" ButtonStyle="Success" Icon="fas fa-check" DisableAfterClick="true" ValidationGroup="AddContact" />
                        <insite:CancelButton runat="server" OnClientClick="dashboardHome.closeAddContact(); return false;" />
                    </div>
                </ContentTemplate>
            </insite:UpdatePanel>
        </ContentTemplate>
    </insite:Modal>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                if (window.dashboardHome)
                    return;

                const instance = window.dashboardHome = {};

                let addContactCaller = null;

                instance.assign = function (s) {
                    const formId = s.closest('tr')?.dataset.form;
                    if (!formId)
                        return;

                    document.getElementById('<%= AssignFormIdentifier.ClientID %>').value = formId;
                    document.getElementById('<%= AssignUserIdentifier.ClientID %>').show();
                };

                instance.transfer = function (s) {
                    document.getElementById('<%= TransferUserIdentifier.ClientID %>').show();
                };

                instance.downloadReport = function (attemptId) {
                    const input = document.getElementById("<%= AttemptIdField.ClientID %>");
                    input.value = attemptId;

                    __doPostBack("<%= DownloadButton.UniqueID %>", "");
                };
                
                instance.setAddContactCaller = function (input) {
                    if (input && !addContactCaller) {
                        addContactCaller = input;
                    }
                };

                instance.closeAddContact = function () {
                    addContactCaller.refresh();
                    modalManager.close('<%= AddContactWindow.ClientID %>');
                    addContactCaller = null;
                };
            })();

            (function () {
                Sys.Application.add_load(init);

                function init() {
                    document.querySelectorAll('.status-container > div .circular-progress').forEach(progr => {
                        progr.removeEventListener('click', onStatusClick);
                        progr.addEventListener('click', onStatusClick);
                    });
                    document.querySelectorAll('.circular-progress > span').forEach(label => {
                        label.setAttribute('data-length', label.innerText.trim().length);
                    });
                }

                function onStatusClick() {
                    this.offsetParent.querySelector('input[type="checkbox"]').click();
                }
            })();

            (function () {
                const input = document.getElementById('<%= AssignUserIdentifier.ClientID %>');
                if (!input)
                    return;

                const anchorTemplate = document.createElement('template');
                anchorTemplate.innerHTML = '<a href="javascript:void(0)" class="fs-sm mt-1 ms-auto"><i class="fas fa-plus-circle ms-2 me-1"></i>Add New Person</a>';

                input.addEventListener('windows-created.findentity', initModal);

                function initModal() {
                    const modal = input?.closest('.insite-findentity')?.querySelector(':scope > .modal');
                    if (!modal)
                        return;

                    const header = modal.querySelector(':scope > .modal-dialog > .modal-content > .modal-header');
                    if (!header)
                        return;

                    const title = header.querySelector(':scope > .modal-title');
                    const createBtn = anchorTemplate.content.cloneNode(true).firstChild;
                    const closeBtn = header.querySelector(':scope > .btn-close');

                    title.after(createBtn);
                    closeBtn.classList.add('ms-0');

                    createBtn.addEventListener('click', onAddNewPerson);
                }

                function onAddNewPerson() {
                    const assignModalElement = input?.closest('.insite-findentity')?.querySelector(':scope > .modal');
                    const addModalElement = document.getElementById('<%= AddContactWindow.ClientID %>');

                    if (!assignModalElement || !addModalElement)
                        return;

                    const assignModal = bootstrap.Modal.getOrCreateInstance(assignModalElement);

                    const addModal = bootstrap.Modal.getOrCreateInstance(addModalElement);
                    addModalElement.addEventListener('show.bs.modal', function () {
                        assignModal._element.classList.add('d-none');
                        assignModal._backdrop._element.classList.add('d-none');
                    }, { once: true });
                    addModalElement.addEventListener('hide.bs.modal', function () {
                        assignModal._element.classList.remove('d-none');
                        assignModal._backdrop._element.classList.remove('d-none');
                    }, { once: true });

                    dashboardHome.setAddContactCaller(input);
                    document.getElementById('<%= AddContactUpdatePanel.ClientID %>').ajaxRequest('init');
                    addModal.show();
                }
            })();

            (function () {
                const inputUser = document.getElementById('<%= TransferUserIdentifier.ClientID %>');
                if (!inputUser) {
                    return;
                }

                const createTemplate = document.createElement('template');
                createTemplate.innerHTML = '<a href="javascript:void(0)" class="fs-sm mt-1 ms-auto"><i class="fas fa-plus-circle ms-2 me-1"></i>Add New Person</a>';

                inputUser.addEventListener('windows-created.findentity', initModal);

                {
                    let inputTransferInited = null;
                    Sys.Application.add_load(function () {
                        if (inputTransferInited && document.contains(inputTransferInited))
                            return;

                        inputTransferInited = getInputTransfer();
                        inputTransferInited.addEventListener('input', validate);
                    });
                }

                function initModal() {
                    const modal = inputUser.closest('.insite-findentity').querySelector(':scope > .modal');
                    if (!modal) {
                        return;
                    }

                    const header = modal.querySelector(':scope > .modal-dialog > .modal-content > .modal-header');
                    if (header) {
                        initModalHeader(header);
                    }

                    const body = modal.querySelector(':scope > .modal-dialog > .modal-content > .modal-body');
                    const wrapper = document.getElementById('<%= TransferCountUpdatePanel.ClientID %>');
                    if (body && wrapper) {
                        initModalBody(body, wrapper);
                    }

                    modal.addEventListener('show.bs.modal', function () {
                        const inputCount = getInputTransfer();
                        inputCount.value = inputCount.dataset.default;
                        validate();
                    });

                    validate();
                }

                function initModalHeader(header) {
                    const title = header.querySelector(':scope > .modal-title');
                    const createBtn = createTemplate.content.cloneNode(true).firstChild;
                    const closeBtn = header.querySelector(':scope > .btn-close');

                    title.after(createBtn);
                    closeBtn.classList.add('ms-0');

                    createBtn.addEventListener('click', onAddNewPerson);
                }

                function initModalBody(body, wrapper) {
                    body.append(wrapper);
                    wrapper.classList.remove('d-none');

                    body.addEventListener('click', function (e) {
                        if (wrapper.contains(e.target) || validate())
                            return;

                        e.preventDefault();
                        e.stopPropagation();
                    }, true);
                }

                function getInputTransfer() {
                    return document.getElementById('<%= TransferCount.ClientID %>');
                }

                function validate() {
                    const inputCount = getInputTransfer();
                    const available = parseInt(inputCount.dataset.available || '0');
                    const selected = parseInt(inputCount.value);
                    const isValid = Number.isInteger(selected) && selected > 0 && selected <= available;

                    const warning = document.getElementById('<%= TransferCountWarning.ClientID %>');
                    if (warning) {
                        if (isValid) {
                            warning.classList.add('d-none');
                            warning.innerHTML = '';
                        } else if (Number.isInteger(selected) && selected > 0) {
                            warning.innerHTML = `You selected <b>${selected} SkillsCheck</b>, but only <b>${available} are available</b> to choose from. Please adjust your selection to continue.`;
                            warning.classList.remove('d-none');
                        }
                    }

                    inputCount.classList.toggle('is-invalid', !isValid);

                    return isValid;
                }

                function onAddNewPerson() {
                    const transferModalElement = inputUser.closest('.insite-findentity').querySelector(':scope > .modal');
                    const addModalElement = document.getElementById('<%= AddContactWindow.ClientID %>');

                    const transferModal = bootstrap.Modal.getOrCreateInstance(transferModalElement);
                    const addModal = bootstrap.Modal.getOrCreateInstance(addModalElement);

                    addModalElement.addEventListener('show.bs.modal', function () {
                        transferModal._element.classList.add('d-none');
                        transferModal._backdrop._element.classList.add('d-none');
                    }, { once: true });
                    addModalElement.addEventListener('hide.bs.modal', function () {
                        transferModal._element.classList.remove('d-none');
                        transferModal._backdrop._element.classList.remove('d-none');
                    }, { once: true });

                    dashboardHome.setAddContactCaller(inputUser);
                    document.getElementById('<%= AddContactUpdatePanel.ClientID %>').ajaxRequest('init');
                    addModal.show();
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
