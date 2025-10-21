<%@ Page Language="C#" CodeBehind="Recode.aspx.cs" Inherits="InSite.Admin.Events.Exams.Forms.Recode" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ExamInfoSummary.ascx" TagName="ExamInfoSummary" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Exam" />

    <section runat="server" ID="RescheduleSection" class="mb-3">

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
                
                        <div class="form-group mb-3" runat="server" id="ClassCodeField" visible="false">
                            <label class="form-label">Class/Session Code</label>
                            <div>
                                <insite:TextBox runat="server" ID="ClassCode" MaxLength="30" />
                            </div>
                            <div class="form-text">The reference number for related training programs.</div>
                        </div>
                    
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Billing Code
                                <insite:RequiredValidator runat="server" ControlToValidate="BillingCode" ValidationGroup="Exam" />
                            </label>
                            <div>
                                <insite:ItemNameComboBox runat="server" ID="BillingCode" Width="100%" />
                            </div>
                            <div class="form-text">The exam billing code</div>
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Exam" />
            <insite:CancelButton runat="server" ID="BackButton" />
        </div>
    </div>

</div>
</asp:Content>
