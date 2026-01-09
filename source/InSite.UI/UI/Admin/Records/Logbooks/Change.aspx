<%@ Page Language="C#" CodeBehind="Change.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="JournalSetup" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pencil-ruler me-1"></i>
            Change Logbook's Settings
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">

                        <div class="settings">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Class
                                </label>
                                <div>
                                    <insite:FindEvent runat="server" ID="EventIdentifier" ShowPrefix="false" />
                                </div>
                                <div class="form-text">The class that includes the list of registered students.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Achievement
                                </label>
                                <div>
                                    <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                                </div>
                                <div class="form-text">The course content for this class.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Logbook Download
                                </label>
                                <div>
                                    <insite:CheckBox ID="AllowLogbookDownload" runat="server" Text="Allow Logbook Download" />
                                </div>
                                <div class="form-text">If checkbox is selected a learner will be able to download a copy of their logbook.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Framework
                                </label>
                                <div>
                                    <insite:FindStandard runat="server" ID="FrameworkStandardIdentifier" TextType="Title" />
                                </div>
                                <div class="form-text">
                                    The framework that contains the standards measured by this logbook.
                                    <div runat="server" id="ChangeFrameworkNotAllowed">
                                        Note: The logbook has entries and therefore the framework cannot be changed.
                                    </div>
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Is Validation Required for this Logbook?
                                </label>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="IsValidationRequired">
                                        <asp:ListItem Value="true" Text="Yes" />
                                        <asp:ListItem Value="false" Text="No" />
                                    </asp:RadioButtonList>
                                </div>
                                <div class="form-text"></div>
                            </div>

				            <div class="form-group mb-3">
				                <label class="form-label">Notification Message (Validator)</label>
				                <div>
                                    <insite:FindMessage runat="server" ID="ValidatorMessageIdentifier" />
                                </div>
                            </div>

				            <div class="form-group mb-3">
				                <label class="form-label">Notification Message (Learner)</label>
				                <div>
                                    <insite:FindMessage runat="server" ID="LearnerMessageIdentifier" />
                                </div>
                            </div>

				            <div class="form-group mb-3">
				                <label class="form-label">Notification Message (Learner Added)</label>
				                <div>
                                    <insite:FindMessage runat="server" ID="LearnerAddedMessageIdentifier" />
                                </div>
                            </div>

                        </div>

                    </div>

                </div>

            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="JournalSetup" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
