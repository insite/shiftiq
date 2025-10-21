<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentDetail.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.CommentDetail" %>

<insite:PageHeadContent runat="server">
    
    <style type="text/css">

        input[type="checkbox"] {
            margin-top: 10px;
        }

    </style>

</insite:PageHeadContent>

<div class="row">
    
    <div class="col-12">

        <div class="form-group mb-3">
            <label class="form-label">
                Subject Contact
                <insite:RequiredValidator runat="server" ControlToValidate="CandidateID" FieldName="Subject Contact" ValidationGroup="ContactComment" Display="Dynamic" />
            </label>
            <insite:FindPerson runat="server" ID="CandidateID" Width="100%" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Text
                <insite:RequiredValidator runat="server" ControlToValidate="Text" FieldName="Text" ValidationGroup="ContactComment" />
            </label>
            <insite:TextBox runat="server" ID="Text" TextMode="MultiLine" Rows="3" />
        </div>

    </div>

</div>