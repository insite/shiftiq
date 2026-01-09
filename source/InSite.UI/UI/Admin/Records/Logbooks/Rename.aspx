<%@ Page Language="C#" CodeBehind="Rename.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.Rename" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="JournalSetup" />

    <section runat="server" ID="NameSection" class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Logbook Name
                                <insite:RequiredValidator runat="server" ControlToValidate="JournalSetupName" FieldName="Logbook Name" ValidationGroup="JournalSetup" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="JournalSetupName" MaxLength="400" />
                            </div>
                            <div class="form-text">The name that uniquely identifies the logbook for internal filing purposes.</div>
                        </div>

                    </div>

                </div>
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="JournalSetup" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>