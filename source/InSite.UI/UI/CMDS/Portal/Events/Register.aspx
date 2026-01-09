<%@ Page Language="C#" CodeBehind="Register.aspx.cs" Inherits="InSite.Custom.CMDS.User.Events.Forms.Register" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Register" />

    <section runat="server" ID="RegistrationSection" visible="false" class="mb-3">
        
        <h2 class="h4 mb-3">
            <i class="far fa-user me-1"></i>
            Registration Details
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Button runat="server" ID="RegisterAnotherUserLink" Visible="false" Text="Register Another User" Icon="fa fa-user-plus" ButtonStyle="Default" CssClass="mb-3" />

                <div class="row">
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Class
                                <insite:RequiredValidator runat="server" ControlToValidate="SelectedEventID" FieldName="Training Session" ValidationGroup="Register" />
                            </label>
                            <insite:ComboBox runat="server" ID="SelectedEventID" AllowBlank="false" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Format
                            </label>
                            <div>
                                <asp:Literal ID="Format" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Capacity
                            </label>
                            <div>
                                <asp:Literal ID="Capacity" runat="server" />
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Registrations
                            </label>
                            <div>
                                <asp:Literal ID="RegistrationCount" runat="server" />
                                <span runat="server" id="IsClosed" class="badge bg-danger">Closed</span>
                                <span runat="server" id="IsFull" class="badge bg-danger">Full</span>
                            </div>
                        </div>

                        <div runat="server" id="SeatsAvailableField" class="form-group mb-3">
                            <label class="form-label">
                                Seats Available
                            </label>
                            <div>
                                <asp:Literal ID="SeatsAvailable" runat="server" />
                            </div>
                        </div>

                    </div>
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Candidate
                                <insite:RequiredValidator runat="server" ControlToValidate="CandidateKey" FieldName="Candidate" ValidationGroup="Register" />
                            </label>
                            <insite:FindPerson runat="server" ID="CandidateKey" />
                        </div>

                        <insite:Container runat="server" ID="CandidatePanel">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Name
                                </label>
                                <div>
                                    <asp:Literal ID="CandidateName" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Email
                                </label>
                                <div>
                                    <asp:Literal ID="CandidateEmail" runat="server" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Company
                                </label>
                                <div>
                                    <asp:Literal ID="CandidateCompany" runat="server" />
                                </div>
                            </div>

                        </insite:Container>


                        <div runat="server" id="CommentField" class="form-group mb-3">
                            <label class="form-label">
                                Comment
                            </label>
                            <div>
                                <insite:TextBox ID="Comment" runat="server" MaxLength="200" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>
        </div>

        <div class="mt-3">
            <insite:Button runat="server" ID="SubmitButton" ButtonStyle="Success" Text="Register" ValidationGroup="Register" Icon="fa fa-user-plus" />
        </div>

    </section>

</asp:Content>
