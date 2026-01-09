<%@ Page Language="C#" CodeBehind="ChangeExperienceCompetency.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Forms.ChangeExperienceCompetency" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Journal" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-pencil-ruler me-1"></i>
            Competency
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Competency
                            </label>
                            <div>
                                <asp:Literal runat="server" ID="CompetencyTitle" />
                            </div>
                            <div class="form-text">
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                <insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/>
                                <insite:CustomValidator runat="server" ID="HoursValidator" ControlToValidate="HoursValue" Display="None" ValidationGroup="Journal" />
                            </label>
                            <div>
                                <insite:NumericBox runat="server" ID="HoursValue" Width="250px" />
                            </div>
                            <div class="form-text">
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Journal" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
