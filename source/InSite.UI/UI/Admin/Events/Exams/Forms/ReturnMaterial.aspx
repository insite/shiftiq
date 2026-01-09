<%@ Page Language="C#" CodeBehind="ReturnMaterial.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.ReturnMaterial" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="../Controls/ExamInfoSummary.ascx" TagName="ExamInfoSummary" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Exam" />

    <section runat="server" ID="GeneralSection" class="mb-3">
        <div class="row">

            <div class="col-lg-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <uc:ExamInfoSummary ID="ExamInfoSummary" runat="server" />

                    </div>
                </div>
            </div>

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>Reference Information</h3>
                
                        <div class="form-group mb-3">
                            <label class="form-label">Return Shipment Code</label>
                            <div>
                                <insite:TextBox runat="server" ID="ExamMaterialReturnShipmentCode" MaxLength="30" />
                            </div>
                            <div class="form-text">The courier tracking number for the returned exam materials.</div>
                        </div>
                    
                        <div class="form-group mb-3">
                            <label class="form-label">Return Shipment Received</label>
                            <div>
                                <insite:DateTimeOffsetSelector runat="server" ID="ExamMaterialReturnShipmentReceived" />
                            </div>
                            <div class="form-text">The actual date and time the exam materials package is returned.</div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Return Shipment Condition</label>
                            <div>
                                <asp:CheckBox runat="server" ID="ExamMaterialReturnShipmentCondition" Text="All exam materials received" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Exam" />
        <insite:CancelButton runat="server" ID="BackButton" />
    </div>

</asp:Content>
