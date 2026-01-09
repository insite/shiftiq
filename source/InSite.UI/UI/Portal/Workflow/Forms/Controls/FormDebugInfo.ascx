<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormDebugInfo.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.FormDebugInfo" %>

<div class="row">
    <div class="col-md-6">

        <div class="card">
            <div class="card-header text-danger fs-5">
                <insite:Literal runat="server" Text="Current Form" />
            </div>
            <div class="card-body">

                <dl class="row">

                    <dt class="col-sm-4">Language</dt>
                    <dd class="col-sm-8"><%= Survey.Language %></dd>

                    <dt class="col-sm-4">Submission Limit</dt>
                    <dd class="col-sm-8"><%= (Survey.ResponseLimitPerUser ?? 0) == 0 ? "Unlimited" : Survey.ResponseLimitPerUser.ToString() + " per user" %></dd>

                    <dt class="col-sm-4">Status</dt>
                    <dd class="col-sm-8"><%= Survey.Status %></dd>    

                    <dt class="col-sm-4">Metrics</dt>
                    <dd class="col-sm-8">
                        <p class="mb-2"><%= Survey.GetPages().Count %> pages</p>
                        <p class="mb-2"><%= Survey.Questions.Count %> questions</p>
                        <p class="mb-2"><%= Survey.GetBranches().Count %> branches</p>
                        <p class="mb-0"><%= Survey.GetConditions().Count %> conditions</p>
                    </dd>

                    <dt class="col-sm-4">User Identification</dt>
                    <dd class="col-sm-8"><%= Survey.RequireUserIdentification ? "Required" : "Not Required" %></dd>

                    <dt class="col-sm-4">User Authentication</dt>
                    <dd class="col-sm-8"><%= Survey.RequireUserAuthentication ? "Required" : "Not Required" %></dd>

                    <dt class="col-sm-4">User Feedback</dt>
                    <dd class="col-sm-8"><%= Survey.UserFeedback.GetDescription() %></dd>

                    <dt class="col-sm-4">Asset Number</dt>
                    <dd class="col-sm-8"><%= Survey.Asset %></dd>

                </dl>

            </div>
        </div>
        
    </div>
    <div class="col-md-6">

        <div class="card">
            <div class="card-header text-danger fs-5">Current Submission</div>
            <div class="card-body">

                <dl class="row">

                    <dt class="col-sm-4">Respondent</dt>
                    <dd class="col-sm-8"><%= Respondent.FullName %> <%= GetRespondentLabels() %></dd>

                    <dt class="col-sm-4">Sessions</dt>
                    <dd class="col-sm-8"><%= ResponseSessions.Length %></dd>

                    <dt class="col-sm-4">Current Session</dt>
                    <dd class="col-sm-8"><%= (Response != null ? ResponseSession.ResponseSessionIdentifier.ToString() : "-") %></dd>

                    <dt class="col-sm-4">Last Question</dt>
                    <dd class="col-sm-8"><asp:Literal runat="server" ID="LastQuestion" /></dd>

                    <dt class="col-sm-4">Session Status</dt>
                    <dd class="col-sm-8"><%= (Response != null ? Response.Status.ToString() : "-") %></dd>

                    <dt class="col-sm-4">Session Started</dt>
                    <dd class="col-sm-8"><%= LocalizeTime(Response != null ? ResponseSession.ResponseSessionStarted : null) %></dd>

                    <dt class="col-sm-4">Session Completed</dt>
                    <dd class="col-sm-8"><%= LocalizeTime(Response != null ? ResponseSession.ResponseSessionCompleted : null) %></dd>

                    <dt class="col-sm-4">Previous Page</dt>
                    <dd class="col-sm-8"><%= (PageNumber > 0 ? PageNumber.ToString() : "-") %></dd>

                    <dt class="col-sm-4">Current Page</dt>
                    <dd class="col-sm-8"><%= (PageNumber > 0 ? PageNumber.ToString() : "-") %></dd>

                    <dt class="col-sm-4">Next Page</dt>
                    <dd class="col-sm-8"><%= (PageNumber > 0 ? PageNumber.ToString() : "-") %></dd>

                </dl>

            </div>
        </div>

    </div>
</div>