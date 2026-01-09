<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityEditVideo.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ActivityEditVideo" %>

<%@ Register Src="ActivitySetupTab.ascx" TagName="ActivitySetup" TagPrefix="uc" %>

<insite:Alert runat="server" ID="ScreenStatus" />

<insite:Nav runat="server">

    <insite:NavItem runat="server" Title="Video Setup">
        <div class="row">
            <div class="col-md-12">
                
                <div class="form-group mb-3">
                    <label class="form-label">
                        Multilingual
                    </label>
                    <div>
                        <asp:RadioButtonList runat="server" ID="ActivityIsMultilingual">
                            <asp:ListItem Value="true" Text="Yes" />
                            <asp:ListItem Value="false" Text="No" />
                        </asp:RadioButtonList>
                    </div>
                </div>
                
                <div class="form-group mb-3">
                    <div class="float-end">
                        <asp:HyperLink runat="server" ID="VideoUrlLink" Target="_blank" Visible="false">
                            <i class="fas fa-external-link"></i>
                        </asp:HyperLink>
                    </div>
                    <label class="form-label">
                        Video URL
                        <insite:RequiredValidator runat="server" FieldName="Video URL" ControlToValidate="VideoUrl" ValidationGroup="CourseConfig" />
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="VideoUrl" MaxLength="500" />
                        <div class="form-text">
                            <asp:Repeater runat="server" ID="OtherLanguageUrlRepeater">
                                <HeaderTemplate>
                                    <div>
                                        Other languages:
                                    </div>
                                    <ul>
                                </HeaderTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <li>
                                        <%# Container.DataItem %>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Video Target
                    </label>
                    <div>
                        <insite:ComboBox runat="server" ID="VideoTarget" Width="100%">
                            <Items>
                                <insite:ComboBoxOption Value="_blank" Text="Opens in a new window or tab" Selected="true" />
                                <insite:ComboBoxOption Value="_self" Text="Opens in the same window as it was clicked" />
                                <insite:ComboBoxOption Value="_top" Text="Opens in the full body of the window" />
                                <insite:ComboBoxOption Value="_embed" Text="Opens in an embedded frame" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Video Preview</label>
                    <div>
                        <asp:Literal runat="server" ID="VideoViewer" />
                    </div>
                </div>

            </div>
        </div>
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