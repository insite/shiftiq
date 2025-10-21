<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApproveDetail.ascx.cs" Inherits="InSite.UI.Admin.Assets.Glossaries.Terms.Controls.ApproveDetail" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <div class="row">
            <div class="col-md-6">
                <div class="form-group mb-3">
                    <label class="form-label">Name</label>
                    <div>
                        <asp:Literal runat="server" ID="TermName" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">Title</label>
                    <div>
                        <asp:Literal runat="server" ID="TermTitle" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">Definition</label>
                    <div>
                        <asp:Literal runat="server" ID="TermDefinition" />
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label class="form-label">Status</label>
                    <div>
                        <asp:Literal runat="server" ID="TermStatus" />
                    </div>
                </div>
            </div>
        </div>

    </div>
</div>