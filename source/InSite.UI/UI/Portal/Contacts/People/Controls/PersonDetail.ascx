<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonDetail.ascx.cs" Inherits="InSite.UI.Portal.Contacts.People.Controls.PersonDetail" %>

<div class="row">
    <div class="col-lg-6">
        <div class="card h-100">
            <div class="card-body">

                <h3>Personal</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="PersonCode" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Full Name
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="FullName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Email
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="Email" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Birthdate
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="Birthdate" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Gender
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="Gender" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Emergency Contact
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="EmergencyContact" />
                    </div>
                </div>

            </div>
        </div>
    </div>
    <div class="col-lg-6">
        <div class="card h-100">
            <div class="card-body">
                <h3>Phone Numbers</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Preferred
                    </label>
                    <div>
                        <asp:Literal ID="Phone" runat="server" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Home
                    </label>
                    <div>
                        <asp:Literal ID="PhoneHome" runat="server" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Work
                    </label>
                    <div>
                        <asp:Literal ID="PhoneWork" runat="server" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Mobile
                    </label>
                    <div>
                        <asp:Literal ID="PhoneMobile" runat="server" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Other
                    </label>
                    <div>
                        <asp:Literal ID="PhoneOther" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>