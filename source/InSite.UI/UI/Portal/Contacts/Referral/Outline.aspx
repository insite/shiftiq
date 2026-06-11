<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Portal.Contacts.Referral.PersonOutline" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/PortalDocumentList.ascx" TagName="PortalDocumentList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="OutlineAlert" />

    <div class="card mb-3">
        <div class="card-body">

            <h2>Personal Information</h2>
            <div class="row">
                <div class="col-lg-3">
                    <dl>
                        <dt>Full Name</dt>
                        <dd><%= Model.FullName %></dd>
                        <dt>Email</dt>
                        <dd><%= Model.Email %></dd>
                    </dl>
                </div>
                <div class="col-lg-3">
                    <dl>
                        <dt><insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/></dt>
                        <dd><%= Model.AccountCode %></dd>
                        <dt>Phone</dt>
                        <dd><%= Model.Phone %></dd>
                    </dl>
                </div>
                <div class="col-lg-3">
                    <dl>
                        <dt>Occupation</dt>
                        <dd><%= Model.OccupationTitle %></dd>
                    </dl>
                </div>
                <div runat="server" id="CaseColumn" class="col-lg-3">
                    <dl>
                        <dt>Case Type</dt>
                        <dd><%= Model.CaseType %></dd>
                        <dt>Case Status</dt>
                        <dd><%= Model.CaseStatus %></dd>
                    </dl>
                </div>
            </div>

        </div>
    </div>

    <div class="card">
        <div class="card-body">
            <h2>Documents</h2>
            <uc:PortalDocumentList runat="server" ID="DocumentList" />
        </div>
    </div>

</asp:Content>