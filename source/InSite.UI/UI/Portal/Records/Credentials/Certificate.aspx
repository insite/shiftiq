<%@ Page Language="C#" CodeBehind="Certificate.aspx.cs" Inherits="InSite.UI.Portal.Records.Credentials.Certificate" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ErrorAlert" />

    <div runat="server" id="CertificatePanel" class="card" visible="false">
        <div class="card-header">
            <i class='far fa-file-certificate'></i> Certificate
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-lg-12 text-end" style="margin-bottom:30px; float:right;">
                    <insite:Button runat="server" ID="DownloadPDF" ButtonStyle="Primary" Icon="far fa-download" CssClass="btn-lg" Text="Download PDF" />
                    <insite:Button runat="server" ID="DownloadCard" ButtonStyle="Primary" Icon="far fa-download" CssClass="btn-lg" Text="Download Card" />
                </div>
                <div style="clear:both" />
                <div class="col-lg-12 text-center">
                    <asp:Literal runat="server" ID="CertificateImage" />
                </div>

            </div>
        </div>
    </div>

</asp:Content>