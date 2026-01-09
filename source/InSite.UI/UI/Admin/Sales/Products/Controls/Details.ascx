<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.Admin.Invoices.Products.Controls.Details" %>


<div class="form-group mb-3">
    <label class="form-label">
        Name
        <insite:RequiredValidator runat="server" ControlToValidate="ProductName" FieldName="Product Name" ValidationGroup="Product" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="ProductName" MaxLength="100" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Summary
    </label>
    <div>
        <insite:TextBox runat="server" ID="ProductSummary" TextMode="MultiLine" Rows="5" MaxLength="500" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Description
    </label>
    <div>
        <insite:TextBox runat="server" ID="ProductDescription" TextMode="MultiLine" Rows="5" MaxLength="2000" />
    </div>
</div>

<div class="row">

    <div class="col-md-4">
        <div class="form-group mb-3">
            <label class="form-label">
                Industry
            </label>
            <div>
                <insite:ItemIdComboBox runat="server" ID="IndustryItemIdentifier">
                    <Settings
                        CollectionName="Sales/Industry/Name"
                        UseCurrentOrganization="true"
                        UseGlobalOrganizationIfEmpty="true"
                    />
                </insite:ItemIdComboBox>
            </div>
        </div>
    </div>

    <div class="col-md-4">
        <div class="form-group mb-3">
            <label class="form-label">
                Occupation
            </label>
            <div>
                <insite:ItemIdComboBox runat="server" ID="OccupationItemIdentifier">
                    <Settings
                        CollectionName="Sales/Occupation/Name"
                        UseCurrentOrganization="true"
                        UseGlobalOrganizationIfEmpty="true"
                    />
                </insite:ItemIdComboBox>
            </div>
        </div>
    </div>

    <div class="col-md-4">
        <div class="form-group mb-3">
            <label class="form-label">
                Level
            </label>
            <div>
                <insite:ItemIdComboBox runat="server" ID="LevelItemIdentifier">
                    <Settings
                        CollectionName="Sales/Level/Name"
                        UseCurrentOrganization="true"
                        UseGlobalOrganizationIfEmpty="true"
                    />
                </insite:ItemIdComboBox>
            </div>
        </div>
    </div>
</div>


<div class="form-group mb-3">
    <label class="form-label">
        Type
    </label>
    <div>
        <insite:ComboBox runat="server" ID="ProductType">
            <Items>
                <insite:ComboBoxOption />
                <insite:ComboBoxOption Value="Application Fee" Text="Application Fee" />
                <insite:ComboBoxOption Value="Online Assessment" Text="Online Assessment" />
                <insite:ComboBoxOption Value="Online Course" Text="Online Course" />
                <insite:ComboBoxOption Value="Reconsideration Request" Text="Reconsideration Request" />
                <insite:ComboBoxOption Value="Renewal Fee" Text="Renewal Fee" />
                <insite:ComboBoxOption Value="Rescheduling" Text="Rescheduling" />
                <insite:ComboBoxOption Value="Seat" Text="Seat" />
                <insite:ComboBoxOption Value="Package" Text="Package" />
            </Items>
        </insite:ComboBox>

        <div runat="server" id="CoursePanel" visible="false" class="pt-2">
            <insite:FindCourse runat="server" ID="CourseIdentifier" HasGradebook="true" />
        </div>

        <div runat="server" id="AssessmentPanel" visible="false" class="pt-2">
            <insite:FindAssessmentPage runat="server" ID="AssessmentIdentifier" />
        </div>

    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Price
    </label>
    <div>
        <insite:NumericBox runat="server" ID="ProductPrice" DecimalPlaces="2" MaxValue="999999.99" MinValue="0" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        API URL
    </label>
    <div>
        <insite:TextBox runat="server" ID="ProductUrl" MaxLength="2048" />
    </div>
    <div class="mt-2">
        <insite:CheckBox runat="server" ID="IsFeatured" Text="Display as a feature on the Catalog" RenderMode="Inline" />
        <insite:CheckBox runat="server" ID="IsTaxable" Text="Product is taxable" RenderMode="Inline" />
    </div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Product Image
    </label>
    <div>
        <insite:FileUploadV2 runat="server" ID="ProductImageUpload" AllowedExtensions=".jpg,.png" FileUploadType="Image" />
    </div>
</div>

<div class="form-group mb-3">
    <asp:Literal ID="ProductImage" runat="server"></asp:Literal>
</div>

