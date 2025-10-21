<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementDetails.ascx.cs" Inherits="InSite.Cmds.Controls.Training.Achievements.AchievementDetails" %>
<%@ Register Src="AchievementDownloadList.ascx" TagName="AchievementDownloadList" TagPrefix="uc" %>
<%@ Register Src="AchievementHierarchy.ascx" TagName="AchievementHierarchy" TagPrefix="uc" %>

<div class="row">

    <div class="col-md-4">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Achievement Details</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Achievement Type
                        <insite:RequiredValidator runat="server" ControlToValidate="SubType" FieldName="Type" ValidationGroup="Achievement" />
                    </label>
                    <div>
                        <cmds:AchievementTypeSelector ID="SubType" runat="server" NullText="" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Achievement Title
                        <insite:RequiredValidator runat="server" ControlToValidate="Title" FieldName="Title" ValidationGroup="Achievement" />
                        <asp:CustomValidator ID="UniqueTitle" runat="server" ControlToValidate="Title" ErrorMessage="Another achievement with the same title already exists (of the same achievement type)" Display="None" ValidationGroup="Achievement" />
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Title" MaxLength="200" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Options
                    </label>
                    <div>
                        <asp:CheckBox ID="AllowSelfDeclared" runat="server" Text="Allow self-declared" />
                    </div>
                    <div>
                        <asp:CheckBox ID="IsTimeSensitive" runat="server" Text="Time-sensitive" onclick="showHideDateExpired();" />
                    </div>
                </div>
                <div runat="server" ID="RowValidForCount" style="display:none;" class="form-group mb-3">
                    <label class="form-label">
                        Valid for
                    </label>
                    <div class="row">
                        <div class="col-sm-4">
                            <insite:NumericBox ID="ValidForCount" runat="server" NumericMode="Integer" MinValue="0.00" />
                        </div>
                        <div class="col-sm-8">
                            <cmds:ValidForUnitSelector ID="ValidForUnit" runat="server" CssClass="w-75" />
                            <span runat="server" ID="TimeSensitiveImage" class="mt-1 ms-2" style="display:none;"><i class="far fa-clock"></i></span>
                        </div>
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">
                        Achievement Description
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Description" TextMode="MultiLine" Rows="5" MaxLength="1200"/>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <div class="col-md-4">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Categories</h3>

                <uc:AchievementHierarchy ID="AchievementHierarchy" runat="server" />
            </div>
        </div>
        
    </div>

    <div runat="server" ID="DownloadRow" Visible="False" class="col-md-4">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Downloads</h3>

                <div class="form-group mb-3">
                    <asp:PlaceHolder ID="DownloadEditorLinkPanel" runat="server">
                        <insite:Button runat="server" ID="AchievementEditUploadsLink" Icon="far fa-upload" Text="Upload" ButtonStyle="Default" ToolTip="Upload and attach files to this achievement" />
                        <br /><br />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="DownloadPanel" runat="server">                    
                        <uc:AchievementDownloadList ID="DownloadList" runat="server" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>

    </div>

</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        function showHideDateExpired() {
            var chk = document.getElementById('<%= IsTimeSensitive.ClientID %>');

            var rowValidForCount = document.getElementById('<%= RowValidForCount.ClientID %>');
            rowValidForCount.style.display = chk.checked ? '' : 'none';

            var rowImage = document.getElementById('<%= TimeSensitiveImage.ClientID %>');
            rowImage.style.display = chk.checked ? '' : 'none';

            if (!chk.checked) {
                $('#<%= ValidForCount.ClientID %>').val('');

                var $combo = $('#<%= ValidForUnit.ClientID %>');
                $combo.selectpicker('val', $combo.find('option:first').prop('value'));
            }
        }

    </script>
</insite:PageFooterContent>
