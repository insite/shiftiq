<%@ Page Language="C#" CodeBehind="Simulate.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Simulate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Simulate" />

    <h2 class="h4 mb-3"><i class="far fa-window me-2"></i>Simulate</h2>

    <div class="row mb-3">
        <div class="col-md-6">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <h3>Settings </h3>
                    <div class="form-group mb-3">
                        <label class="form-label">
                            File Format
                            <insite:RequiredValidator runat="server" ControlToValidate="FileFormat" FieldName="File Format" Display="Dynamic" ValidationGroup="Simulate" />
                        </label>
                        <div>
                            <insite:ComboBox runat="server" ID="FileFormat" AllowBlank="false" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Desired Score
                                <insite:RequiredValidator runat="server" ControlToValidate="ScoreFrom" FieldName="Desired Score (from)" Display="Dynamic" ValidationGroup="Simulate" />
                            <insite:RequiredValidator runat="server" ControlToValidate="ScoreThru" FieldName="Desired Score (thru)" Display="Dynamic" ValidationGroup="Simulate" />
                            <insite:CompareValidator runat="server"
                                ErrorMessage="<strong>Desired Score (from)</strong> field value must be less (or equal) than <strong>Desired Score (thru)</strong> field value"
                                ValidationGroup="Simulate" Type="Integer" ControlToValidate="ScoreThru" ControlToCompare="ScoreFrom" Operator="GreaterThanEqual" Display="Dynamic" />
                        </label>
                        <div class="d-flex flex-row">
                            <div class="pe-3">
                                <insite:NumericBox runat="server" ID="ScoreFrom" NumericMode="Integer" DigitGrouping="false" MinValue="0" MaxValue="100" /></div>
                            <div class="pt-3">thru</div>
                            <div class="ps-3">
                                <insite:NumericBox runat="server" ID="ScoreThru" NumericMode="Integer" DigitGrouping="false" MinValue="0" MaxValue="100" /></div>
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Number of Users
                                <insite:RequiredValidator runat="server" ControlToValidate="UsersNumber" FieldName="Number of Users" Display="Dynamic" ValidationGroup="Simulate" />
                        </label>
                        <div>
                            <insite:NumericBox runat="server" ID="UsersNumber" NumericMode="Integer" DigitGrouping="false" MinValue="1" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Number of Attempts per User
                            <insite:RequiredValidator runat="server" ControlToValidate="AttemptsNumberFrom" FieldName="Number of Attempts (from)" Display="Dynamic" ValidationGroup="Simulate" />
                            <insite:RequiredValidator runat="server" ControlToValidate="AttemptsNumberThru" FieldName="Number of Attempts (thru)" Display="Dynamic" ValidationGroup="Simulate" />
                            <insite:CompareValidator runat="server"
                                ErrorMessage="<strong>Number of Attempts (from)</strong> field value must be less (or equal) than <strong>Number of Attempts (thru)</strong> field value"
                                ValidationGroup="Simulate" Type="Integer" ControlToValidate="AttemptsNumberThru" ControlToCompare="AttemptsNumberFrom" Operator="GreaterThanEqual" Display="Dynamic" />
                        </label>
                        <div class="d-flex flex-row">
                            <div class="pe-3">
                                <insite:NumericBox runat="server" ID="AttemptsNumberFrom" NumericMode="Integer" DigitGrouping="false" MinValue="1" /></div>
                            <div class="pt-3">thru</div>
                            <div class="ps-3">
                                <insite:NumericBox runat="server" ID="AttemptsNumberThru" NumericMode="Integer" DigitGrouping="false" MinValue="1" /></div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div class="col-md-6">
            <uc:FormDetails ID="FormDetails" runat="server" />
        </div>
    </div>

    <div class="row">
        <div class="col-lg-12">
            <div class="mt-3 mb-3">Generate a CSV file to simulate the output for answers to this exam form:</div>
            <insite:Button runat="server" ID="BuildButton" Text="Generate" Icon="fas fa-cogs" ValidationGroup="Simulate" ButtonStyle="Success" />
            <insite:CancelButton runat="server" ID="CloseButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
