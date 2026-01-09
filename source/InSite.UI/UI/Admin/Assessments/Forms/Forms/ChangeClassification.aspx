<%@ Page Language="C#" CodeBehind="ChangeClassification.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.ChangeClassification" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-window"></i>
            Reclassify Form
        </h2>
        <div class="row">

            <div class="col-lg-6 mb-3 mb-lg-0">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Reclassify Form</h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Instrument</label>
                            <div>
                                <insite:ComboBox runat="server" ID="Instrument" Width="100%">
                                    <Items>
                                        <insite:ComboBoxOption />
                                        <insite:ComboBoxOption Text="SLA" Value="SLA" />
                                        <insite:ComboBoxOption Text="CBA" Value="CBA" />
                                    </Items>
                                </insite:ComboBox>
                            </div>
                            <div class="form-text">The type of assessment (e.g., Pre, Post).</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Theme</label>
                            <div>
                                <insite:TextBox runat="server" ID="ThemeInput" MaxLength="128" Width="100%" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:FormDetails ID="FormDetails" runat="server" />
                    </div>
                </div>
            </div>

        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Assessment" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
