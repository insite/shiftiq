<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployerEditor.ascx.cs" Inherits="InSite.UI.Portal.Contacts.Groups.Controls.EmployerEditor" %>

<div class="row">

    <div class="col-md-6">

        <div class="settings">

            <h3>Company</h3>

            <div class="form-group mb-3">
                <label class="form-label">
                    <insite:Literal runat="server" Text="Name" />
                </label>
                <div>
                    <insite:TextBox ID="CompanyName" runat="server" Width="100%" />
                </div>
            </div>
            <div class="form-group mb-3">
                <label class="form-label">
                    <insite:Literal runat="server" Text="Industry" />
                </label>
                <div>
                    <insite:IndustriesComboBox runat="server" ID="IndustriesComboBox" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    <insite:Literal runat="server" Text="Sector" />
                </label>
                <div>
                    <insite:SectorComboBox runat="server" ID="SectorComboBox" AllowBlank="true" />
                </div>
            </div>

            <div class="form-group mb-3">
                <label class="form-label">
                    <insite:Literal runat="server" Text="Number of Employees" />
                </label>
                <div>
                    <insite:NumberOfEmployeesComboBox runat="server" ID="NumberOfEmployees" />
                </div>
            </div>

        </div>

    </div>

    <div class="col-md-6">

        <div class="settings">

            <h3>Contact</h3>

            <div class="form-group mb-3">
                <label class="form-label">
                    <insite:Literal runat="server" Text="Contact Phone" />
                </label>
                <div>
                    <insite:TextBox ID="Phone" runat="server" MaxLength="12" Width="100%" />
                </div>
            </div>

        </div>

    </div>

</div>