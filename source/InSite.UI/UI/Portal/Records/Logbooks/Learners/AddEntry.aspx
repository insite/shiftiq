<%@ Page Language="C#" CodeBehind="AddEntry.aspx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Learners.AddEntry" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="FieldList" Src="../Controls/FieldList.ascx" %>
<%@ Register TagPrefix="uc" TagName="CompetencyList" Src="../Controls/CompetencyList.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:ValidationSummary runat="server" ValidationGroup="Journal" />

    <insite:Nav runat="server">
        <insite:NavItem ID="InstructionTab" runat="server" Title="Instructions" Icon="far fa-book-open">

            <div class="card">
                <div class="card-body">
                    
                    <div>
                        <asp:Literal runat="server" ID="InstructionBody" />
                    </div>

                    <div class="pt-3">
                        <insite:NextButton runat="server" ID="NextButton" CausesValidation="false"/>
                        <insite:CancelButton runat="server" ID="CancelButton" ConfirmText="Are you sure you want to close page without saving any information?" />
                    </div>

                </div>
            </div>

        </insite:NavItem>
        <insite:NavItem ID="FieldsTab" runat="server" Title="Fields" Icon="far fa-list">

            <div class="card">
                <div class="card-body">

                    <div class="row">
                        <div class="col-md-7">
                            <uc:FieldList runat="server" ID="Fields" />
                        </div>
                    </div>

                    <div class="pt-3">
                        <insite:SaveButton runat="server" ID="SaveButton1" ValidationGroup="Journal" />
                        <insite:NextButton runat="server" ID="NextButton1" ValidationGroup="Journal" />
                        <insite:CancelButton runat="server" ID="CancelButton1" ConfirmText="Are you sure you want to close page without saving any information?" />
                    </div>
                </div>
            </div>

        </insite:NavItem>
        <insite:NavItem ID="CompetenciesTab" runat="server" Title="Competencies" Icon="far fa-ruler-triangle">

            <div class="card">
                <div class="card-body">

                    <div class="row">
                        <div class="col-md-12">
                            <uc:CompetencyList runat="server" ID="Competencies" />
                        </div>
                    </div>

                    <div class="pt-3">
                        <insite:Button runat="server" ID="BackButton" Text="Back" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" CausesValidation="false" />
                        <insite:SaveButton runat="server" ID="SaveButton2" ValidationGroup="Journal" />
                        <insite:CancelButton runat="server" ID="CancelButton2" ConfirmText="Are you sure you want to close page without saving any information?" />
                    </div>
                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>