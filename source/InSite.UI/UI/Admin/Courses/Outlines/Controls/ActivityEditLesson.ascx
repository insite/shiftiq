<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityEditLesson.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ActivityEditLesson" %>

<%@ Register Src="ActivitySetupTab.ascx" TagName="ActivitySetup" TagPrefix="uc" %>

<insite:Alert runat="server" ID="ScreenStatus" />

<insite:Nav runat="server">

    <insite:NavItem runat="server" Title="Lesson Content">

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