<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CredentialSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.CredentialSection" %>

<div class="row mb-3">

    <div class="col-12">

        <div class="card">

            <div class="card-body">

                <h4 class="card-title mb-3">Credentials</h4>

                <div class="row">
                    <div class="col-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Professional Certifications
                            </label>
                            <insite:TextBox runat="server" ID="ProfessionalCertifications" TextMode="MultiLine" MaxLength="1000" Rows="3" />
                            <div class="form-text">
                                Enter the name of any Professional Certifications or designations that you have received from a professional society or occupational certifying body.
                                Professional certifications identify that individuals have demonstrated a standard level of skills, experience, and expertise within their field.
                                An example of this would be EUR ING (European Engineer),
                                the title given to licensed engineers by the European Federation of National Engineering Associations (FEANI).
                                <br />
                                <b>Separate entries by hitting enter.</b>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Membership in Professional or Trade Organization(s)
                            </label>
                            <insite:TextBox runat="server" ID="MembershipProfessional" TextMode="MultiLine" MaxLength="1000" Rows="3" />
                            <div class="form-text">
                                This is usually an organisation seeking to further a particular profession and the interests of the individuals employed in that profession.
                                An example would be a union acting on behalf of all carpenters who are members of that union.
                                <br />
                                <b>Separate entries by hitting enter.</b>
                            </div>
                        </div>

                    </div>
                    <div class="col-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Volunteer Experience
                            </label>
                            <insite:TextBox runat="server" ID="VolunteerExperience" TextMode="MultiLine" MaxLength="1000" Rows="3" />
                            <div class="form-text">
                                Volunteer experience is any experience where you have provided services without pay. Often it includes skill development
                                or the opportunity to contribute your skills and experience to an organization.
                                Enter any volunteer experience you would like to profile to employers.
                                <br />
                                <b>Separate entries by hitting enter.</b>
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Have you taken any courses or training since arriving in Canada?
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="OtherTraining" TextMode="MultiLine" MaxLength="1000" Rows="3" />
                            </div>
                            <div class="form-text">
                                Enter the name of any courses, professional development or general training you have taken since arriving in BC.
                                For example, Microsoft Office Training, FoodSafe, First Aid, WHMIS, customer service training, etc.
                                <br />
                                <b>Separate entries by hitting enter.</b>
                            </div>
                        </div>

                    </div>

                </div>

            </div>
            
        </div>

    </div>

</div>