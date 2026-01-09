<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentDetails.ascx.cs" Inherits="InSite.Admin.Assessments.Attachments.Controls.AttachmentDetails" %>

<div runat="server" id="AssetRow" class="row" visible="false">
    <div class="col-lg-6">
        <div class="form-group mb-3">
            <label class="form-label">
                Asset #
            </label>
            <div>
                <asp:Literal runat="server" ID="AssetNumber" />
            </div>
        </div>
    </div>

    <div class="col-lg-6">
        <div class="form-group mb-3">
            <label class="form-label">
                Current Version
            </label>
            <div>
                <div class="float-end">
                    <span runat="server" id="AssetVersionIncremented" visible="false" class="badge bg-danger">New Version!</span>
                </div>
                <asp:Literal runat="server" ID="AssetVersion" />
                <insite:IconButton runat="server" ID="AssetVersionIncrement" Name="arrow-alt-up" />
            </div>
            <div class="form-text">
                <asp:Repeater runat="server" ID="AssetVersionRepeater">
                    <ItemTemplate>
                        <a href='<%# Eval("NavigateUrl") %>'>v<%# Eval("Number") %></a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Title
        <insite:RequiredValidator runat="server" ID="AttachmentTitleValidator" ControlToValidate="AttachmentTitle" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="AttachmentTitle" />
    </div>
</div>

<div runat="server" id="FileNameField" class="form-group mb-3">
    <label class="form-label">
        File Name
        <insite:RequiredValidator runat="server" ID="FileNameRequiredValidator" ControlToValidate="FileName" />
        <insite:PatternValidator runat="server" ID="FileNamePatternValidator" FieldName="File Name" Display="Dynamic"
            ControlToValidate="FileName" ValidationExpression="[a-zA-Z0-9_\(\)\- ]*" ErrorMessage="File name contains invalid characters" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="FileName" MaxLength="128" />
    </div>
</div>

<div class="row">
    <div class="col-lg-6">
        <div class="form-group mb-3">
            <label class="form-label">
                Condition
                <insite:RequiredValidator runat="server" ID="AttachmentConditionRequiredValidator" ControlToValidate="AttachmentCondition" />
            </label>
            <div>
                <insite:ComboBox runat="server" ID="AttachmentCondition">
                    <Items>
                        <insite:ComboBoxOption Text="Duplicate" Value="Copy" />
                        <insite:ComboBoxOption Text="Edit" Value="Edit" />
                        <insite:ComboBoxOption Text="New" Value="New" />
                        <insite:ComboBoxOption Text="Purge" Value="Purge" />
                        <insite:ComboBoxOption Text="Surplus" Value="Surplus" />
                        <insite:ComboBoxOption Text="Unassigned" />
                    </Items>
                </insite:ComboBox>
            </div>
        </div>
        <div runat="server" id="FileExtensionField" class="form-group mb-3">
            <label class="form-label">
                File Extension
            </label>
            <div>
                <asp:Literal runat="server" ID="FileExtension" />
            </div>
        </div>
        <div runat="server" id="FileSizeField" class="form-group mb-3">
            <label class="form-label">
                File Size
            </label>
            <div>
                <asp:Literal runat="server" ID="FileSize" />
            </div>
        </div>
    </div>
    <div class="col-lg-6">
        <div class="form-group mb-3" runat="server" id="AttachmentPublicationStatusColumn">
            <label class="form-label">
                Publication Status
            </label>
            <div>
                <asp:Literal runat="server" ID="AttachmentPublicationStatus" />
            </div>
        </div>
        <div class="form-group mb-3">
            <label class="form-label">
                Author
            </label>
            <div>
                <asp:Literal runat="server" ID="AttachmentAuthorName" />
            </div>
        </div>
        <div class="form-group mb-3">
            <label class="form-label">
                Uploaded
            </label>
            <div>
                <asp:Literal runat="server" ID="AttachmentUploadedDate" />
            </div>
        </div>
        <div runat="server" id="ChangesCountField" class="form-group mb-3" visible="false">
            <label class="form-label">
                Changes
            </label>
            <div>
                <asp:Literal runat="server" ID="ChangesCount" />
                <insite:IconLink runat="server" ID="ViewChangesLink" style="padding:8px" ToolTip="View Changes History" Name="history" />
            </div>
        </div>
        <div runat="server" id="UsageCountField" class="form-group mb-3" visible="false">
            <label class="form-label">
                Usage
            </label>
            <div>
                <asp:Literal runat="server" ID="UsageCount" />
                <insite:IconLink runat="server" ID="ViewUsageLink" style="padding:8px" ToolTip="View Usage Statistic" Name="chart-pie" />
            </div>
        </div>
    </div>
</div>

<insite:Container runat="server" ID="AttachmentImageFieldsContainer" Visible="false">
    <div runat="server" id="ImageResolutionInputField" class="form-group mb-3" visible="false">
        <label class="form-label">
            Image Resolution
        </label>
        <div>
            <insite:NumericBox runat="server" ID="ImageResolutionInput" Width="100px" NumericMode="Integer" MinValue="0" CssClass="d-inline-block" /> DPI
        </div>
    </div>

    <div runat="server" id="ImageResolutionOutputField" class="form-group mb-3" visible="false">
        <label class="form-label">
            Image Resolution
        </label>
        <div>
            <asp:Literal runat="server" ID="ImageResolutionOutput" /> DPI
        </div>
    </div>

    <div runat="server" id="ImageActualDimensionInputField" class="form-group mb-3" visible="false">
        <label class="form-label">
            Actual Dimensions
            <insite:RequiredValidator runat="server" ID="ImageActualDimensionWidthValidator" ControlToValidate="ImageActualDimensionWidthInput" />
            <insite:RequiredValidator runat="server" ID="ImageActualDimensionHeightValidator" ControlToValidate="ImageActualDimensionHeightInput" />
        </label>
        <div>
            <insite:NumericBox runat="server" ID="ImageActualDimensionWidthInput" Width="100px" NumericMode="Integer" MinValue="1" CssClass="d-inline-block" />
            x
            <insite:NumericBox runat="server" ID="ImageActualDimensionHeightInput" Width="100px" NumericMode="Integer" MinValue="1" CssClass="d-inline-block" />
            pixels
        </div>
    </div>

    <div runat="server" id="ImageActualDimensionOutputField" class="form-group mb-3" visible="false">
        <label class="form-label">
            Actual Dimensions
        </label>
        <div>
            <asp:Literal runat="server" ID="ImageActualDimensionWidthOutput" />
            x
            <asp:Literal runat="server" ID="ImageActualDimensionHeightOutput" />
            pixels
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">
            Target Online Dimensions
        </label>
        <div>
            <insite:NumericBox runat="server" ID="ImageOnlineDimensionWidth" Width="100px" NumericMode="Integer" MinValue="1" CssClass="d-inline-block" />
            x
            <insite:NumericBox runat="server" ID="ImageOnlineDimensionHeight" Width="100px" NumericMode="Integer" MinValue="1" CssClass="d-inline-block" />
            pixels
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">
            Target Paper Dimensions
        </label>
        <div>
            <insite:NumericBox runat="server" ID="ImagePaperDimensionWidth" Width="100px" NumericMode="Integer" MinValue="1" CssClass="d-inline-block" />
            x
            <insite:NumericBox runat="server" ID="ImagePaperDimensionHeight" Width="100px" NumericMode="Integer" MinValue="1" CssClass="d-inline-block" />
            pixels
        </div>
    </div>

    <div class="form-group mb-3">
        <label class="form-label">
            Palette
        </label>
        <div>
            <insite:ComboBox runat="server" ID="ImageIsColor" Width="215px">
                <Items>
                    <insite:ComboBoxOption Text="Color" Value="True" />
                    <insite:ComboBoxOption Text="Black and White" Value="False" />
                </Items>
            </insite:ComboBox>
        </div>
    </div>
</insite:Container>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        <% if (AttachmentImageFieldsContainer.Visible && ImageAspectRatio.HasValue) { %>
        (function () {
            var ratio = <%= ImageAspectRatio.Value %>;
            var isLocked = false;

            <% if (ImageActualDimensionInputField.Visible) { %>
            var $imageActualDimensionWidth = null;
            var $imageActualDimensionHeight = null;
            <% } %>
            var $imageOnlineDimensionWidth = null;
            var $imageOnlineDimensionHeight = null;
            var $imagePaperDimensionWidth = null;
            var $imagePaperDimensionHeight = null;

            Sys.Application.add_load(function () {
                <% if (ImageActualDimensionInputField.Visible) { %>
                if ($imageActualDimensionWidth == null || !document.contains($imageActualDimensionWidth[0]))
                    $imageActualDimensionWidth = $('#<%= ImageActualDimensionWidthInput.ClientID %>').on('change', onActualWidth_ValueChanged);

                if ($imageActualDimensionHeight == null || !document.contains($imageActualDimensionHeight[0]))
                    $imageActualDimensionHeight = $('#<%= ImageActualDimensionHeightInput.ClientID %>').on('change', onActualHeight_ValueChanged);
                <% } %>

                if ($imageOnlineDimensionWidth == null || !document.contains($imageOnlineDimensionWidth[0]))
                    $imageOnlineDimensionWidth = $('#<%= ImageOnlineDimensionWidth.ClientID %>').on('change', onOnlineWidth_ValueChanged);

                if ($imageOnlineDimensionHeight == null || !document.contains($imageOnlineDimensionHeight[0]))
                    $imageOnlineDimensionHeight = $('#<%= ImageOnlineDimensionHeight.ClientID %>').on('change', onOnlineHeight_ValueChanged);

                if ($imagePaperDimensionWidth == null || !document.contains($imagePaperDimensionWidth[0]))
                    $imagePaperDimensionWidth = $('#<%= ImagePaperDimensionWidth.ClientID %>').on('change', onPaperWidth_ValueChanged);

                if ($imagePaperDimensionHeight == null || !document.contains($imagePaperDimensionHeight[0]))
                    $imagePaperDimensionHeight = $('#<%= ImagePaperDimensionHeight.ClientID %>').on('change', onPaperHeight_ValueChanged);
            });

            function setupMaxValues($srcWidth, $srcHeight, $destWidth, $destHeight) {
                var maxWidth = parseInt($srcWidth.val());
                var maxHeight = parseInt($srcHeight.val());
                var width = parseInt($destWidth.val());
                var height = parseInt($destHeight.val());

                if (!isNaN(maxWidth) && !isNaN(maxHeight)) {
                    if (!isNaN(width) && width > maxWidth)
                        $destWidth.val(maxWidth);

                    if (!isNaN(height) && height > maxHeight)
                        $destHeight.val(maxHeight);

                    $destWidth.data('baseInput').numeric.max = maxWidth;
                    $destHeight.data('baseInput').numeric.max = maxHeight;
                } else {
                    $destWidth.data('baseInput').numeric.max = null;
                    $destHeight.data('baseInput').numeric.max = null;
                }
            }

            function onActualWidth_ValueChanged() {
                onWidthChanged($imageActualDimensionHeight, this.value);

                setupMaxValues($imageActualDimensionWidth, $imageActualDimensionHeight, $imageOnlineDimensionWidth, $imageOnlineDimensionHeight);
                setupMaxValues($imageActualDimensionWidth, $imageActualDimensionHeight, $imagePaperDimensionWidth, $imagePaperDimensionHeight);
            }

            function onActualHeight_ValueChanged() {
                onHeightChanged($imageActualDimensionWidth, this.value);

                setupMaxValues($imageActualDimensionWidth, $imageActualDimensionHeight, $imageOnlineDimensionWidth, $imageOnlineDimensionHeight);
                setupMaxValues($imageActualDimensionWidth, $imageActualDimensionHeight, $imagePaperDimensionWidth, $imagePaperDimensionHeight);
            }

            function onOnlineWidth_ValueChanged() {
                onWidthChanged($imageOnlineDimensionHeight, this.value);
            }

            function onOnlineHeight_ValueChanged() {
                onHeightChanged($imageOnlineDimensionWidth, this.value);
            }

            function onPaperWidth_ValueChanged() {
                onWidthChanged($imagePaperDimensionHeight, this.value);
            }

            function onPaperHeight_ValueChanged() {
                onHeightChanged($imagePaperDimensionWidth, this.value);
            }

            function onWidthChanged($heightInput, width) {
                if (isLocked)
                    return;

                try {
                    isLocked = true;

                    width = parseInt(width);

                    if (!isNaN(width)) {
                        var height = width / ratio;
                        if (height < 1)
                            height = 1;

                        $heightInput.val(height.toFixed(0));
                    } else {
                        $heightInput.val('');
                    }
                } finally {
                    isLocked = false;
                }
            }

            function onHeightChanged($widthInput, height) {
                if (isLocked)
                    return;

                try {
                    isLocked = true;

                    height = parseInt(height);

                    if (!isNaN(height)) {
                        var width = height * ratio;
                        if (width < 1)
                            width = 1;

                        $widthInput.val(width.toFixed(0));
                    } else {
                        $widthInput.val('');
                    }
                } finally {
                    isLocked = false;
                }
            }
        })();
        <% } %>

    </script>
</insite:PageFooterContent>