<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />

    <section class="mb-3">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h4 class="card-title mb-3">
                    <i class="far fa-calendar-alt me-1"></i>
                    New Class
                </h4>

                <div class="row mb-3">
                    <div class="col-lg-4">
                        <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Class" />
                    </div>
                </div>

                <asp:MultiView runat="server" ID="MultiView">

                    <asp:View runat="server" ID="OneView">

                        <div class="row">
                            <div class="col-lg-4">

                                <h6 class="mt-3">Description</h6>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Achievement
                                    </label>
                                    <insite:FindAchievement runat="server" ID="OneAchievementIdentifier" />
                                    <div class="form-text">
                                        &nbsp;
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Class Title
                                        <insite:RequiredValidator runat="server" ControlToValidate="OneEventTitle" FieldName="Class Title" ValidationGroup="Class" />
                                    </label>
                                    <insite:TextBox runat="server" ID="OneEventTitle" MaxLength="400" />
                                    <div class="form-text">
                                        The descriptive title for this class.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Class Summary
                                    </label>
                                    <insite:TextBox runat="server" ID="OneEventSummary" TextMode="MultiLine" Rows="5" />
                                    <div class="form-text">
                                        Tagline / summary / short description for this class.
                                    </div>
                                </div>

                            </div>
                            <div class="col-lg-4">

                                <h6 class="mt-3">Schedule</h6>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Start Date and Time
                                        <insite:RequiredValidator runat="server" ControlToValidate="OneEventScheduledStart" FieldName="Start Date and Time" ValidationGroup="Class" />
                                    </label>
                                    <insite:DateTimeOffsetSelector ID="OneEventScheduledStart" runat="server" />
                                    <div class="form-text">
                                        The start date and time for this class event.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        End Date and Time
                                        <insite:RequiredValidator runat="server" ControlToValidate="OneEventScheduledEnd" FieldName="End Date and Time" ValidationGroup="Class" />
                                        <insite:CustomValidator runat="server" ID="OneEventScheduledEndValidator" ControlToValidate="OneEventScheduledEnd" ValidationGroup="Class" />
                                    </label>
                                    <insite:DateTimeOffsetSelector ID="OneEventScheduledEnd" runat="server" />
                                    <div class="form-text">
                                        The end date and time for this class event.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Duration
                                    </label>
                                    <div class="d-flex gap-1 w-50">
                                        <insite:NumericBox runat="server" ID="OneEventDuration" NumericMode="Integer" DigitGrouping="false" MinValue="0" MaxValue="99999" />
                                        <insite:ComboBox ID="OneEventDurationUnit" runat="server" />
                                    </div>
                                    <div class="form-text">
                                        If left blank, the system will calculate the duration for you in days, including weekends.
                                    </div>
                                </div>

                            </div>
                            <div class="col-lg-4">

                                <h6 class="mt-3">Location</h6>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Venue
                                        <insite:RequiredValidator runat="server" ControlToValidate="OneVenueLocationGroup" FieldName="Venue" ValidationGroup="Class" />
                                    </label>
                                    <insite:FindGroup ID="OneVenueLocationGroup" runat="server" />
                                    <div class="form-text">
                                        The training provider, organization, or agency hosting the event.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Building and Room
                                    </label>
                                    <insite:TextBox ID="OneVenueRoom" runat="server" MaxLength="128" Width="100%" />
                                    <div class="form-text">
                                        The physical location within the venue where the event occurs.
                                    </div>
                                </div>

                            </div>
                        </div>

                    </asp:View>

                    <asp:View runat="server" ID="UploadView">

                        <div class="row">
                            <div class="col-lg-4">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Select and Upload Class JSON File
                                    </label>
                                    <insite:FileUploadV1 runat="server" ID="UploadJsonFile"
                                        AllowedExtensions=".json"
                                        LabelText=""
                                        FileUploadType="Unlimited"
                                        OnClientFileUploaded="classCreate.onJsonFileUploaded" />
                                    <asp:Button runat="server" ID="UploadJsonFileUploaded" CssClass="d-none" />
                                    <div class="form-text">
                                    </div>
                                </div>

                            </div>
                            <div class="col-lg-8">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Uploaded JSON
                                        <insite:RequiredValidator runat="server" ControlToValidate="UploadJsonInput" FieldName="Uploaded JSON" Display="Dynamic" ValidationGroup="Class" />
                                    </label>
                                    <insite:TextBox runat="server" ID="UploadJsonInput" TextMode="MultiLine" Rows="15" />
                                    <div class="form-text">
                                    </div>
                                </div>

                            </div>
                        </div>

                    </asp:View>

                    <asp:View runat="server" ID="CopyView">

                        <div class="row">
                            <div class="col-lg-4">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Class
                                        <insite:RequiredValidator runat="server" ID="CopyEventValidator" ControlToValidate="CopyEventSelector" FieldName="Class" ValidationGroup="Class" />
                                    </label>
                                    <insite:FindEvent runat="server" ID="CopyEventSelector" ShowPrefix="false" />
                                    <div class="form-text">
                                        Choose the existing class you wish to copy.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Class Title
                                        <insite:RequiredValidator runat="server" ControlToValidate="CopyEventTitle" FieldName="Class Title" ValidationGroup="Class" />
                                    </label>
                                    <insite:TextBox runat="server" ID="CopyEventTitle" MaxLength="400" />
                                    <div class="form-text">
                                        The descriptive title for this class.
                                    </div>
                                </div>

                                <div runat="server" id="CopyGradebooksField" class="form-group mb-3">
                                    <label class="form-label">
                                        Copy Gradebooks
                                    </label>
                                    <div>
                                        <asp:RadioButtonList runat="server" ID="CopyGradebooks" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="Yes" Text="Yes" Selected="true" />
                                            <asp:ListItem Value="No" Text="No" />
                                        </asp:RadioButtonList>
                                    </div>
                                    <div class="form-text">
                                    </div>
                                </div>

                            </div>
                            <div class="col-lg-4">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Start Date and Time
                                        <insite:RequiredValidator runat="server" ControlToValidate="CopyEventScheduledStart" FieldName="Start Date and Time" ValidationGroup="Class" />
                                    </label>
                                    <insite:DateTimeOffsetSelector ID="CopyEventScheduledStart" runat="server" />
                                    <div class="form-text">
                                        The start date and time for this class event.
                                    </div>
                                </div>
                                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        End Date and Time
                                        <insite:RequiredValidator runat="server" ControlToValidate="CopyEventScheduledEnd" FieldName="End Date and Time" ValidationGroup="Class" />
                                        <insite:CustomValidator runat="server" ID="CopyEventScheduledEndValidator" ControlToValidate="CopyEventScheduledEnd" ValidationGroup="Class" />
                                    </label>
                                    <insite:DateTimeOffsetSelector ID="CopyEventScheduledEnd" runat="server" />
                                    <div class="form-text">
                                        The end date and time for this class event.
                                    </div>
                                </div>

                                <div runat="server" id="DurationField" class="form-group mb-3">
                                    <label class="form-label">
                                        Duration
                                    </label>
                                    <insite:CheckBox runat="server" ID="CopyDurationCheckbox" Text="Copy Duration" Checked="true" />

                                    <div class="d-none d-flex gap-1 w-50 copy-duration">
                                        <insite:NumericBox runat="server" ID="CopyEventDuration" NumericMode="Integer" DigitGrouping="false" MinValue="0" MaxValue="99999" />
                                        <insite:ComboBox ID="CopyEventDurationUnit" runat="server" />
                                    </div>
                                    <div class="d-none form-text copy-duration">
                                        If left blank, the system will calculate the duration for you in days, including weekends.
                                    </div>
                                </div>

                            </div>
                        </div>

                    </asp:View>

                </asp:MultiView>

            </div>
        </div>

    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Class" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>

    <insite:PageFooterContent runat="server">
        <script>
            (function () {
                const classCreate = window.classCreate = window.classCreate || {};

                classCreate.onJsonFileUploaded = function () {
                    __doPostBack('<%= UploadJsonFileUploaded.UniqueID %>', '')
                };

                const copyDurationCheckbox = document.getElementById("<%= CopyDurationCheckbox.ClientID %>");

                displayCopyDuration(copyDurationCheckbox?.checked ?? false);

                if (copyDurationCheckbox) {
                    copyDurationCheckbox.addEventListener("change", e => {
                        displayCopyDuration(e.target.checked);
                    });
                }

                function displayCopyDuration(hidden) {
                    document.querySelectorAll(".copy-duration").forEach(div => {
                        if (hidden) {
                            div.classList.add("d-none");
                        } else {
                            div.classList.remove("d-none");
                        }
                    });
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>