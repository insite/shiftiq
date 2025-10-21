<%@ Page Language="C#" CodeBehind="Define.aspx.cs" Inherits="InSite.Admin.Achievements.Achievements.Forms.Define" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Records/Achievements/Controls/AchievementExpirationField.ascx" TagName="ExpirationField" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Achievement" />

    <section runat="server" ID="AchievementSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-trophy me-1"></i>
            Achievement
        </h2>

        <div class="row mb-3">
            <div class="col-md-6">
                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Achievement" />
            </div>
        </div>

        <asp:Panel runat="server" ID="NewSection">

            <div class="row">
                        
                <div class="col-md-6">
                                    
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Title
                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementTitle" FieldName="Title" ValidationGroup="Achievement" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementTitle" MaxLength="200" />
                                </div>
                                <div class="form-text">The descriptive title for this achievement.</div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Tag
                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementLabel" FieldName="Label" ValidationGroup="Achievement" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementLabel" MaxLength="50" />
                                </div>
                                <div class="form-text">The descriptive tag for this achievement.</div>
                            </div>
            
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Description
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementDescription" TextMode="MultiLine" Rows="5" MaxLength="1200"/>
                                </div>
                                <div class="form-text">The description for this achievement.</div>
                            </div>

                        </div>
                    </div>

                </div>
            
                <div class="col-md-6">
                            
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">
                       
                            <uc:ExpirationField runat="server" ID="ExpirationField" ValidationGroup="Achievement" />

                        </div>
                    </div>

                </div>

            </div>

        </asp:Panel>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Achievement" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
