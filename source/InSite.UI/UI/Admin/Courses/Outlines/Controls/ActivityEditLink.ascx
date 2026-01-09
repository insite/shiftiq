<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityEditLink.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ActivityEditLink" %>

<%@ Register Src="ActivitySetupTab.ascx" TagName="ActivitySetup" TagPrefix="uc" %>

<insite:Alert runat="server" ID="ScreenStatus" />

<insite:Nav runat="server">

    <insite:NavItem runat="server" Title="Link Setup">

        <insite:UpdatePanel runat="server">

            <ContentTemplate>

            <div class="form-group mb-3">
                <label class="form-label">
                    Link Type
                </label>
                <div>
                    <insite:ComboBox runat="server" ID="LinkType">
                        <Items>
                            <insite:ComboBoxOption Value="External" Text="External Web Page" Selected="true" />
                            <insite:ComboBoxOption Value="Internal" Text="Internal Web Page" />
                            <insite:ComboBoxOption Value="SCORM" Text="SCORM Course" />
                        </Items>
                    </insite:ComboBox>
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    SCORM Platform
                </label>
                <div>
                    <insite:ComboBox runat="server" ID="ActivityPlatform">
                        <Items>
                            <insite:ComboBoxOption Value="SCORM Cloud" Text="SCORM Cloud" />
                            <insite:ComboBoxOption Value="Moodle" Text="Moodle" />                            
                            <insite:ComboBoxOption Value="Scoop" Text="Scoop" />                            
                        </Items>
                    </insite:ComboBox>
                </div>
            </div>

            <div class="form-group mb-3" runat="server" id="ActivityHookField" visible="false">
                <div class="float-end">
                    <asp:Literal runat="server" ID="ScormPackageStatus" />
                </div>
                <label class="form-label">
                    SCORM Package ID
                </label>
                <div>
                    <insite:TextBox runat="server" ID="ActivityHook" MaxLength="100" />
                </div>
                <div class="mt-2">
                    <div><asp:CheckBox runat="server" ID="LinkIsMultilingual" Text="Append a language code to SCORM Package ID" /></div>
                    <div><asp:CheckBox runat="server" ID="LinkIsPreview" Text="Launch SCORM content in Preview mode" /></div>
                    <div><asp:CheckBox runat="server" ID="LinkIsDispatch" Text="SCORM content is a dispatch package" /></div>
                    <div><asp:HyperLink runat="server" ID="ScoopLibraryUrl" Text="SCO Library" NavigateUrl="#" Visible="false" /></div>
                </div>
            </div>

            <div runat="server" id="LinkUrlField" class="form-group mb-3">
                <div class="float-end">
                    <asp:HyperLink runat="server" ID="LinkUrlTest" Target="_blank" Visible="false">
                        <i class="fas fa-external-link"></i>
                    </asp:HyperLink>
                </div>
                <label class="form-label">Link URL</label>
                <div><insite:TextBox runat="server" ID="LinkUrl" MaxLength="500" /></div>
            </div>

            <div runat="server" id="LinkTargetField" class="form-group mb-3">
                <label class="form-label">
                    Link Target
                </label>
                <div>
                    <insite:ComboBox runat="server" ID="LinkTarget">
                        <Items>
                            <insite:ComboBoxOption Value="_blank" Text="Opens in a new window or tab" Selected="true" />
                            <insite:ComboBoxOption Value="_self" Text="Opens in the same window as it was clicked" />
                            <insite:ComboBoxOption Value="_top" Text="Opens in the full body of the window" />
                            <insite:ComboBoxOption Value="_embed" Text="Opens in an embedded frame" />
                        </Items>
                    </insite:ComboBox>
                </div>
            </div>

            </ContentTemplate>

        </insite:UpdatePanel>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Optional Content">

        <div class="form-group mb-3">
            <label class="form-label">Language</label>
            <div>
                <insite:ComboBox runat="server" ID="Language" AllowBlank="false" />
            </div>
        </div>

        <div class="row row-translate">
            <div class="col-md-12">
                <asp:Repeater runat="server" ID="ContentRepeater">
                    <ItemTemplate>
                        <div class="form-group mb-3">
                            <insite:DynamicControl runat="server" ID="Container" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Activity Setup">
        <uc:ActivitySetup runat="server" ID="ActivitySetup" />
    </insite:NavItem>

</insite:Nav>

<div class="mt-5">
    <insite:SaveButton runat="server" ID="ActivitySaveButton" ValidationGroup="CourseConfig" />
    <insite:CancelButton runat="server" ID="ActivityCancelButton" />
</div>