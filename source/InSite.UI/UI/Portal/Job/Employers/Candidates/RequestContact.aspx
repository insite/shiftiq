<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RequestContact.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Candidates.RequestContact" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:PageHeadContent runat="server">
    
        <style type="text/css">

            input[type="checkbox"] {
                margin-top: 10px;
            }

        </style>

    </insite:PageHeadContent>

    <div class="row">

        <div class="col-8">
            <div class="form-group mb-3">
                <label class="form-label">
                    Request For:
                </label>
                <div class="h5 px-1">
                    <asp:Literal ID="CandidateFullName" runat="server"></asp:Literal>
                </div>
            </div>
        </div>

        <div runat="server" class="col-4" id="RequestHistory" visible="false">
            <div class="form-group mb-3 text-end">
                <label class="form-label">
                    Last Request on:
                </label>
                <div class="px-1 form-text">
                    <ul class="list-unstyled">
                        <asp:Repeater runat="server" ID="Repeater">
                            <ItemTemplate>
                                <li><%# Eval("Requested") %></li>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ul>
                </div>

            </div>
        </div>

    </div>

    <div class="form-group mb-3">
        <label class="form-label">
            Your Name
            <insite:RequiredValidator runat="server" ControlToValidate="RequesterName" FieldName="Requester Name" ValidationGroup="RequestContact" />
        </label>
        <insite:TextBox runat="server" ID="RequesterName" />
    </div>

    <div class="form-group mb-3">
        <label class="form-label">
            Email Address
            <insite:RequiredValidator runat="server" ControlToValidate="RequesterEmail" FieldName="Requester Email" ValidationGroup="RequestContact" />
        </label>
        <insite:TextBox runat="server" ID="RequesterEmail" Enabled="false" />
    </div>

    <div class="form-group mb-3" runat="server">
        <label class="form-label">
            Organization
            <insite:RequiredValidator runat="server" ControlToValidate="RequesterOrganization" FieldName="Requester Organization" ValidationGroup="RequestContact" />
        </label>
        <insite:TextBox runat="server" ID="RequesterOrganization" />
    </div>


    <div class="form-group mb-3">
        <label class="form-label">
            Message for Candidate (500 character limit)
        </label>
        <insite:TextBox runat="server" ID="RequesterMessage" MaxLength="500" TextMode="MultiLine" Rows="2" />
    </div>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="RequestContact" Text="Send Contact Request"/>
        <insite:CloseButton runat="server" ID="CloseButton" OnClientClick="modalManager.closeModal(); return false;" />
    </div>
</asp:Content>