<%@ Page Language="C#" CodeBehind="Status.aspx.cs" Inherits="InSite.UI.Admin.Standards.Standards.Forms.Status" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Standard" />

    <div class="mb-3">
        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <h3>Details</h3>

                <div class="row">
                    <div class="col-lg-6">
                        <div class="card h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Status
                                    </label>
                                    <insite:RadioButtonList runat="server" ID="StatusList" CssClass="ms-1 mt-1" />
                                </div>

                            </div>
                        </div>
                    </div>
                    <div class="col-lg-6">
                        <div class="card h-100">
                            <div class="card-body">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Updated By
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="StatusUpdatedBy" ReadOnly="true" />
                                    </div>
                                    <div class="form-text">
                                        Name of the individual who last updated the status.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Date Updated
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="StatusDateUpdated" ReadOnly="true" />
                                    </div>
                                    <div class="form-text">
                                        Date the status was last updated.
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Standard" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>