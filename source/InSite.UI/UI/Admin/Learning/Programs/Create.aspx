<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Records.Programs.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementListEditor.ascx" TagName="AchievementListEditor" TagPrefix="uc" %>
<%@ Register Src="./Controls/TaskGridEdit.ascx" TagName="TaskGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="Step1Section" Title="Program" Icon="far fa-pencil-ruler" IconPosition="BeforeText">
            <section>
                
                <div class="row mb-3">
                    <div class="col-md-6">
                        <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Program" />
                    </div>
                </div>

                <div runat="server" ID="NewSection" class="row">
                
                    <div class="col-md-6">                                    
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Program Name
                                        <insite:RequiredValidator runat="server" ControlToValidate="ProgramName" FieldName="Name" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="ProgramName" MaxLength="200" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Program Code
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="ProgramCode" MaxLength="20" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Department
                                    </label>
                                    <div>
                                        <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" />
                                    </div>
                                    <div class="mt-2">
                                        <insite:CheckBox runat="server" ID="ProgramType" Text="Achievements Only" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <div>
                                        <insite:NextButton runat="server" ID="Step1NextButton" />
                                        <insite:SaveButton runat="server" ID="Step1SaveButton" />
                                        <insite:CancelButton runat="server" ID="CancelButton" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="col-md-6">                                    
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Program Description
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="ProgramDescription" TextMode="MultiLine" Rows="8" MaxLength="500"/>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
    
                </div>

            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="Step2Section" Title="Achievements" Icon="far fa-trophy" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Achievements
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:AchievementListEditor ID="AchievementListEditor" runat="server" />
                        <div class="mt-3">
                            <insite:NextButton runat="server" ID="Step2NextButton" DisableAfterClick="true" />
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="Step3Section" Title="Settings" Icon="far fa-cog" IconPosition="BeforeText" Visible="false">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Settings
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:TaskGrid runat="server" ID="TaskGrid" />

                        <insite:SaveButton runat="server" ID="Step3SaveButton" CssClass="mt-3" ValidationGroup="Template" DisableAfterClick="true" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
