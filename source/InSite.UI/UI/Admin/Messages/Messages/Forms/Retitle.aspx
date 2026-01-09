<%@ Page Language="C#" CodeBehind="Retitle.aspx.cs" Inherits="InSite.Admin.Messages.Messages.Forms.Retitle" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="InputSubject" Src="../Controls/FieldInputSubject.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Message" />

    <section class="mb-3">
        <div class="row">
            <div class="col-lg-6">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <h4 class="card-title mb-3">
                            <i class="far fa-paper-plane me-1"></i>
                            Change Message Subject
                        </h4>

                        <uc:InputSubject runat="server" ID="SubjectInput" ValidationGroup="Message" />

                    </div>
                </div>
            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Message" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
