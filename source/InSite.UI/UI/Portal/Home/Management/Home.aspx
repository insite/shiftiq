<%@ Page Language="C#" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Portal.Home.Management.Home" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="~/UI/Admin/Foundations/Controls/AnnouncementToast.ascx" TagName="AnnouncementToast" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Foundations/Controls/MaintenanceToast.ascx" TagName="MaintenanceToast" TagPrefix="uc" %>

<%@ Register Src="./Controls/DashboardNavigation.ascx" TagName="DashboardNavigation" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        .form-check-input:checked {
            background-color: var(--ar-info);
            border-color: var(--ar-info);
        }

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

        .status-container .skills-progress {
            --ar-progress-thickness: 0.5rem;
            width: 80px;
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

    <insite:Alert runat="server" ID="HomeStatus" />
    <insite:UserLicenseCheck runat="server" />
    <insite:UserPasswordCheck runat="server" />
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

    <div class="d-flex flex-column flex-xl-row mb-3">
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
                                        <%# GetProgressHtml((int)Eval("Total"), (int)Eval("Count"), true, false) %>
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
                            <%# Eval("LearnerUserName") ?? "None" %>
                        </td>
                        <td>
                            <div class="mb-2"><%# Eval("ProductName") %></div>
                            <small><%# GetGridStatusHtml() %></small>
                        </td>
                        <td class="text-end"><%# GetGridScoreHtml() %></td>
                        <td class="text-center">
                            <insite:Button runat="server" Text="Assign" Size="ExtraSmall" ButtonStyle="Success"
                                OnClientClick="dashboardHome.assign(this); return false;"
                                Visible='<%# Eval("LearnerUserIdentifier") == null %>'
                            />
                            <insite:Container runat="server" Visible='<%# Eval("LearnerUserIdentifier") != null && Eval("AttemptImported") == null && Eval("AttemptStarted") == null %>'>
                                <insite:Button runat="server" Text="Resend" CommandName="Resend"
                                    Size="ExtraSmall" ButtonStyle="Default" CssClass="d-block mb-1" />
                                <insite:Button runat="server" Text="Cancel" CommandName="Cancel"
                                    Size="ExtraSmall" ButtonStyle="Default" CssClass="d-block" />
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

                instance.assign = function (s) {
                    const formId = s.closest('tr')?.dataset.form;
                    if (!formId)
                        return;

                    document.getElementById('<%= AssignFormIdentifier.ClientID %>').value = formId;
                    document.getElementById('<%= AssignUserIdentifier.ClientID %>').show();
                };

                instance.downloadReport = function (attemptId) {
                    const input = document.getElementById("<%= AttemptIdField.ClientID %>");
                    input.value = attemptId;

                    __doPostBack("<%= DownloadButton.UniqueID %>", "");
                };
            })();

            (function () {
                Sys.Application.add_load(init);

                function init() {
                    const statusItems = document.querySelectorAll('.status-container > div .circular-progress');
                    for (let i = 0; i < statusItems.length; i++) {
                        const item = statusItems[i];
                        item.removeEventListener('click', onStatusClick);
                        item.addEventListener('click', onStatusClick);
                    }
                }

                function onStatusClick() {
                    this.offsetParent.querySelector('input[type="checkbox"]').click();
                }
            })();

            (function () {
                const instance = window.dashboardHome;
                if (!instance)
                    return;

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

                    document.getElementById('<%= AddContactUpdatePanel.ClientID %>').ajaxRequest('init');
                    addModal.show();
                }

                instance.closeAddContact = function () {
                    input.refresh();
                    modalManager.close('<%= AddContactWindow.ClientID %>');
                };
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>