<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgressSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.ProgressSection" %>

<div class="row mb-3">

    <div class="col-12">

        <div class="card">

            <div class="card-body">

                <h4 class="card-title mb-3">Progress</h4>

                <div class="row">
                    <div class="col-6">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Profile Completion
                            </label>
                            <asp:Literal runat="server" ID="ProfileCompletionBar" />
                        </div>

                    </div>

                    <div class="col-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Education and Experience Completion
                            </label>
                            <asp:Literal runat="server" ID="ResumeCompletionBar" />
                        </div>

                    </div>

                </div>

            </div>
            
        </div>

    </div>

</div>