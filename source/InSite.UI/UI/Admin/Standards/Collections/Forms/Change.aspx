<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Change.aspx.cs" Inherits="InSite.UI.Admin.Standards.Collections.Forms.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Collection" />

    <div class="row mb-3">

        <div class="col-lg-6">

            <div class="card">

                <div class="card-body">

                    <h4 class="card-title mb-3">Collection</h4>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Title
                            <insite:RequiredValidator runat="server" ControlToValidate="ContentTitle" FieldName="Title" ValidationGroup="Collection" />
                        </label>
                        <insite:TextBox runat="server" ID="ContentTitle" MaxLength="400" />
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Tag
                            <insite:RequiredValidator runat="server" ControlToValidate="StandardLabel" FieldName="Tag" ValidationGroup="Collection" />
                        </label>
                        <insite:TextBox runat="server" ID="StandardLabel" MaxLength="64" />
                    </div>

                </div>
            
            </div>

        </div>

    </div>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Collection" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
