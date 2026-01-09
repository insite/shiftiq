<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeAchievementDetails.ascx.cs" Inherits="InSite.Custom.CMDS.User.Progressions.Controls.EmployeeAchievementDetails" %>
<%@ Register Src="AttachmentList.ascx" TagName="AttachmentList" TagPrefix="uc" %>
<%@ Register Src="EmployeeAchievementDownloadGrid.ascx" TagName="EmployeeAchievementDownloadGrid" TagPrefix="uc" %>
<%@ Register Src="AchievementSummary.ascx" TagName="AchievementSummary" TagPrefix="uc" %>

<div class="row">
    <div class="col-lg-6">

        <uc:AchievementSummary ID="AchievementSummary" runat="server" />
        
        <div runat="server" id="AdminPanel1">

            <div runat="server" ID="SettingsPanel" Visible="false" class="card border-0 shadow-lg mb-3">
                <div class="card-body">
                    <h3>CMDS Information</h3>
                    <div class="form-group mb-3">
                        <insite:CheckBox ID="IsInTrainingPlan" runat="server" Text="Training Plan" />
                    </div>
                    <div class="form-group mb-3">
                        <insite:CheckBox ID="IsRequired" runat="server" Text="Required" />
                    </div>
                    <div id="AttachmentPanel" runat="server" visible="false" class="mb-3">
                        <h6 class="mb-1">Downloads/Resources</h6>
                        <uc:AttachmentList ID="AttachmentList" runat="server" />
                    </div>
                </div>
            </div>

            <div class="card border-0 shadow-lg mb-3">
                <div class="card-body">

                    <h3>Admin Use Only</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">Type</label>
                        <div>
                            <cmds:AchievementTypeSelector ID="SubType" runat="server" />
                            <asp:Literal runat="server" ID="SubtypeLiteral" Text="Other Achievements" Visible="false" />
                        </div>
                    </div>

                    <div runat="server" ID="AchievementRow" Visible="False" class="form-group mb-3">
                        <span runat="server" id="AchievementValidForCount" class="float-end text-muted fs-sm"></span>
                        <label class="form-label">
                            Achievement
                            <asp:CustomValidator ID="UniqueAchievementValidator" runat="server" ControlToValidate="AchievementSelector" ValidationGroup="Education" ErrorMessage="The specified achievement is already assigned to this person." Display="None" />
                        </label>
                        <div>
                            <cmds:FindAchievement ID="AchievementSelector" runat="server" />
                        </div>
                    </div>

                    <div runat="server" id="AssignedField" class="form-group mb-3">
                        <label class="form-label">Assigned</label>
                        <div>
                            <asp:Literal ID="DateAssigned" runat="server" />
                        </div>
                    </div>
                    
                    <div class="form-group mb-3">
                        <label class="form-label">Completed</label>
                        <div>
                            <insite:DateSelector ID="DateCompleted" runat="server" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">Status</label>
                        <div>
                            <cmds:EmployeeAchievementStatusSelector ID="ValidationStatus" runat="server" AllowBlank="false" />
                        </div>
                    </div>

                    <div runat="server" ID="IsSuccessField" class="form-group mb-3">
                        <label class="form-label">Success</label>
                        <div>
                            <insite:CheckBox runat="server" ID="IsSuccess" />
                        </div>
                    </div>
                    <div runat="server" ID="EnableSignOffField" class="form-group mb-3">
                        <label class="form-label">Enable Sign Off</label>
                        <div>
                            <insite:CheckBox ID="EnableSignOff" runat="server" />
                        </div>
                    </div>
                    <asp:PlaceHolder ID="TimeSensitivePanel" runat="server">
                        <div class="form-group mb-3">
                            <label class="form-label">Time-Sensitive</label>
                            <div>
                                <insite:RadioButton runat="server" ID="IsTimeSensitiveYes" Text="Yes" GroupName="TimeSensitive" onclick="showHideDateExpired();" />
                                <insite:RadioButton runat="server" ID="IsTimeSensitiveNo" Text="No" GroupName="TimeSensitive" onclick="showHideDateExpired();" />
                            </div>
                        </div>
                        
                        <div runat="server" ID="trValidFor" class="form-group mb-3">
                            <label class="form-label">
                                Valid for Months
                                <asp:CustomValidator ID="ValidForCountRequired" runat="server"
                                    ControlToValidat="ValidForCount"
                                    ErrorMessage="<strong>Valid for</strong> is a required field."
                                    ValidationGroup="Education"
                                    ValidateEmptyText="true"
                                    ClientValidationFunction="ValidForCount_ClientValidate"
                                >
                                    <img src="/Images/Icons/warning.gif" alt="Required" title="Required" border="0" align="absmiddle" style="display:inline;">
                                </asp:CustomValidator>
                            </label>
                            <div>
                                <insite:NumericBox ID="ValidForCount" runat="server" NumericMode="Integer" MinValue="0.00" CssClass="w-25" />
                            </div>
                        </div>

                        <div runat="server" ID="trDateExpired" style="display:none;" class="form-group mb-3">
                            <label class="form-label">Expiry Date</label>
                            <div>
                                <asp:Literal ID="ExpirationDate" runat="server" />
                            </div>
                        </div>

                        <div runat="server" ID="trDateNotified" style="display:none;" class="form-group mb-3">
                            <label class="form-label">Last Email Notification</label>
                            <div>
                                <table class="table table-striped w-75">
                                    <tr>
                                        <td class="text-end">Expired</td><td><asp:Literal ID="Notified0" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td class="text-end">One-Month Reminder</td><td><asp:Literal ID="Notified1" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td class="text-end">Two-Month Reminder</td><td><asp:Literal ID="Notified2" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td class="text-end">Three-Month Reminder</td><td><asp:Literal ID="Notified3" runat="server" /></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                </div>
            </div>

            <div runat="server" ID="GradePanel" class="card border-0 shadow-lg mb-3">
                <div class="card-body">
                    <h3>Grade Information</h3>
                    <div class="form-group mb-3">
                        <label class="form-label">Quiz Score (%)</label>
                        <div class="text-nowrap w-25">
                            <insite:NumericBox ID="GradePercent" runat="server" DecimalPlaces="2" MaxValue="100.0" MinValue="0" CssClass="d-inline" /> %
                        </div>
                    </div>
                </div>
            </div>
            
            <div runat="server" ID="FileManagerPanel" class="card border-0 shadow-lg mb-3" Visible="false">
                <div class="card-body">
                    <h3>Files</h3>
                    <div class="form-group mb-3">
                        <div>
                            <uc:EmployeeAchievementDownloadGrid ID="DownloadGrid" runat="server" />
                        </div>
                    </div>
                </div>
            </div>

        </div>

    </div>
    <div class="col-lg-6">
        <div runat="server" id="AdminPanel2">

            <div class="card border-0 shadow-lg mb-3">
                <div class="card-body">

                    <h3>Other Information</h3>
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Title
                            <insite:RequiredValidator runat="server" FieldName="Title" ControlToValidate="Title" ValidationGroup="Education" />
                        </label>
                        <div>
                            <insite:TextBox ID="Title" runat="server" MaxLength="256" />
                        </div>
                        <div class="form-text" runat="server" id="AchievementDescription"></div>
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label">Program Hours</label>
                        <div>
                            <insite:NumericBox ID="Hours" runat="server" DecimalPlaces="2" MaxValue="999.99" MinValue="0" CssClass="w-25" />
                        </div>
                    </div>
                    <div runat="server" ID="NumberField" class="form-group mb-3" visible="false">
                        <label class="form-label">Document #</label>
                        <div>
                            <insite:TextBox ID="Number" runat="server" MaxLength="32" />
                        </div>
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label">Comment</label>
                        <div>
                            <insite:TextBox ID="Comment" runat="server" TextMode="MultiLine" />
                        </div>
                    </div>

                </div>
            </div>

            <div class="card border-0 shadow-lg mb-3">
                <div class="card-body">
                    <h3>Institution</h3>
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Institution Name
                        </label>
                        <div>
                            <insite:TextBox ID="AccreditorName" runat="server" MaxLength="50" />
                        </div>
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label">City</label>
                        <div>
                            <insite:TextBox ID="AccreditorCity" runat="server" MaxLength="256" />
                        </div>
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label">Province</label>
                        <div>
                            <insite:TextBox ID="AccreditorProvince" runat="server" MaxLength="64" />
                        </div>
                    </div>
                    <div class="form-group mb-3">
                        <label class="form-label">Country</label>
                        <div>
                            <insite:TextBox ID="AccreditorCountry" runat="server" MaxLength="64" />
                        </div>
                    </div>
                </div>
            </div>

        </div>

    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        function showHideDateExpired() {
            var chk = document.getElementById('<%= IsTimeSensitiveYes.ClientID %>');
            var trDateExpired = document.getElementById('<%= trDateExpired.ClientID %>');
            var trDateNotified = document.getElementById('<%= trDateNotified.ClientID %>');
            var trValidFor = document.getElementById('<%= trValidFor.ClientID %>');

            if (trDateExpired != null) {
                trDateExpired.style.display = chk.checked ? '' : 'none';
            }

            if (trDateNotified != null) {
                trDateNotified.style.display = chk.checked ? '' : 'none';
            }

            trValidFor.style.display = chk.checked ? '' : 'none';
        }

        function ValidForCount_ClientValidate(source, args) {
            if (!document.getElementById('<%= IsTimeSensitiveYes.ClientID %>').checked) {
                return;
            }

            var value = parseInt($('#<%= ValidForCount.ClientID %>').val());

            args.IsValid = !isNaN(value);
        }

    </script>
</insite:PageFooterContent>
