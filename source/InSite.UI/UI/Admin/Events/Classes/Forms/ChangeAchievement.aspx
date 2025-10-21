<%@ Page Language="C#" CodeBehind="ChangeAchievement.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.ChangeAchievement" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ClassSummaryInfo.ascx" TagName="SummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="../Controls/ClassLocationInfo.ascx" TagName="LocationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Class" />

    <section class="mb-3">
        <div class="row">
            <div class="col-md-6 mb-3 mb-md-0">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Summary
                        </h4>

                        <uc:SummaryInfo runat="server" ID="SummaryInfo" />

                        <div runat="server" id="RegistrationStartField" class="form-group mb-3">
                            <label class="form-label">
                                Achievement
                                <insite:RequiredValidator runat="server" ControlToValidate="AchievementIdentifier" FieldName="Achievement" ValidationGroup="Class" />
                            </label>
                            <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                            <div class="form-text">
                                Achievement that contains the Program you want to evaluate with this class.
                            </div>
                        </div>

                    </div>
                </div>

            </div>
            <div class="col-md-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            Location and Schedule
                        </h4>

                        <uc:LocationInfo runat="server" ID="LocationInfo" />

                    </div>
                </div>

            </div>
        </div>
    </section>

    <section>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Class" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </section>
</asp:Content>