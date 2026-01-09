<%@ Page Language="C#" CodeBehind="Certificate.aspx.cs" Inherits="InSite.UI.Lobby.CeritifcateVerify" MasterPageFile="~/UI/Layout/Lobby/LobbyForm.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
     <style>

         .certificateContainer {
             width: 900px;
             height: 600px;
         }

         .certificateImageC1 {
             background-image: url("/UI/Layout/Common/Parts/img/cert1.jpg");
             background-repeat: no-repeat;
             background-size: 900px;
         }

         .certificateImageC2 {
             background-image: url("/UI/Layout/Common/Parts/img/cert2.jpg");
             background-repeat: no-repeat;
             background-size: 900px;
         }

     </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <h3 class="d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3">Credential</h3>

    <div class="p-4">
        This is to certify that <b id="name2" runat="server">Name</b> has successfully completed <b id="course2" runat="server">Course</b>
        on <b id="date" runat="server">yyyy/MM/dd</b> on <a runat="server" id="PlatformAnchor" href="#">Platform</a>.
        <span id="ExpSentence" runat="server">This certificate will expire on <b runat="server" id="exp">yyyy/MM/dd</b>.</span>
        The certificate indicates the entire course was completed as validated by the learner.
    </div>

    <h3 class="d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3">Certificate Holder</h3>

    <div class="p-4">

        <h5 runat="server" id="name3"></h5>
        <h4>Course:</h4>
        <h5 runat="server" id="course3"></h5>
        <h5 runat="server" id="course4"></h5>
        <h5 id="course5" runat="server"></h5>
        <h5 id="course6" runat="server"></h5>
        <asp:HiddenField runat="server" ID="fingerPrint" />
        <br />
        <br />
        <div runat="server" id="ControlPannel" style="margin: auto" class="text-center">
            <b style="color: #355C74">Share via: </b>
            <a href="javascript:void(0)" onclick="clipboardClick()">
                <i title="Copy to clipboard">
                    <svg width="24" height="24" viewBox="0 0 24 24">
                        <path fill="#355C74" fill-rule="evenodd" clip-rule="evenodd" d="M3.57 14.67c0-.57.13-1.11.38-1.6l.02-.02v-.02l.02-.02c0-.02 0-.02.02-.02.12-.26.3-.52.57-.8L7.78 9v-.02l.01-.02c.44-.41.91-.7 1.44-.85a4.87 4.87 0 0 0-1.19 2.36A5.04 5.04 0 0 0 8 11.6L6.04 13.6c-.19.19-.32.4-.38.65a2 2 0 0 0 0 .9c.08.2.2.4.38.57l1.29 1.31c.27.28.62.42 1.03.42.42 0 .78-.14 1.06-.42l1.23-1.25.79-.78 1.15-1.16c.08-.09.19-.22.28-.4.1-.2.15-.42.15-.67 0-.16-.02-.3-.06-.45l-.02-.02v-.02l-.07-.14s0-.03-.04-.06l-.06-.13-.02-.02c0-.02 0-.03-.02-.05a.6.6 0 0 0-.14-.16l-.48-.5c0-.04.02-.1.04-.15l.06-.12 1.17-1.14.09-.09.56.57c.02.04.08.1.16.18l.05.04.03.06.04.05.03.04.04.06.1.14.02.02c0 .02.01.03.03.04l.1.2v.02c.1.16.2.38.3.68a1 1 0 0 1 .04.25 3.2 3.2 0 0 1 .02 1.33 3.49 3.49 0 0 1-.95 1.87l-.66.67-.97.97-1.56 1.57a3.4 3.4 0 0 1-2.47 1.02c-.97 0-1.8-.34-2.49-1.03l-1.3-1.3a3.55 3.55 0 0 1-1-2.51v-.01h-.02v.02zm5.39-3.43c0-.19.02-.4.07-.63.13-.74.44-1.37.95-1.87l.66-.67.97-.98 1.56-1.56c.68-.69 1.5-1.03 2.47-1.03.97 0 1.8.34 2.48 1.02l1.3 1.32a3.48 3.48 0 0 1 1 2.48c0 .58-.11 1.11-.37 1.6l-.02.02v.02l-.02.04c-.14.27-.35.54-.6.8L16.23 15l-.01.02-.01.02c-.44.42-.92.7-1.43.83a4.55 4.55 0 0 0 1.23-3.52L18 10.38c.18-.21.3-.42.35-.65a2.03 2.03 0 0 0-.01-.9 1.96 1.96 0 0 0-.36-.58l-1.3-1.3a1.49 1.49 0 0 0-1.06-.42c-.42 0-.77.14-1.06.4l-1.2 1.27-.8.8-1.16 1.15c-.08.08-.18.21-.29.4a1.66 1.66 0 0 0-.08 1.12l.02.03v.02l.06.14s.01.03.05.06l.06.13.02.02.01.02.01.02c.05.08.1.13.14.16l.47.5c0 .04-.02.09-.04.15l-.06.12-1.15 1.15-.1.08-.56-.56a2.3 2.3 0 0 0-.18-.19c-.02-.01-.02-.03-.02-.04l-.02-.02a.37.37 0 0 1-.1-.12c-.03-.03-.05-.04-.05-.06l-.1-.15-.02-.02-.02-.04-.08-.17v-.02a5.1 5.1 0 0 1-.28-.69 1.03 1.03 0 0 1-.04-.26c-.06-.23-.1-.46-.1-.7v.01z"></path>
                    </svg>
                </i>
            </a>
            <asp:HyperLink runat="server" ID="FaceBook" Target="_blank"><i class='fab fa-facebook me-1' title="Share on Facebook"></i></asp:HyperLink>
            <asp:HyperLink runat="server" ID="Twitter" Target="_blank"><i class='fab fa-twitter me-1' title="Share on Twitter"></i></asp:HyperLink>
            <asp:HyperLink runat="server" ID="LinkedIn" Target="_blank"><i class='fab fa-linkedin me-1' title="Share on LinkedIn"></i></asp:HyperLink>
            <asp:HyperLink runat="server" ID="Mail" Target="_blank"><i class='fas fa-paper-plane me-1' title="Send Via Email"></i></asp:HyperLink>
        </div>

    </div>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <div class="row mainTab">
        <div class="col-lg-12">
            <div id="certificateData" class="certificateContainer certificateImageC1" runat="server">
                <div style="text-align: left;">
                    <span style="font-size: 15px"><b runat="server" id="name" style="position: absolute; margin-top: 233px; margin-left: 95px;"></b></span>
                    <!--<br /><br />-->
                    <span style="font-size: 15px"><b runat="server" id="coursTitle" style="position: absolute; margin-top: 315px; margin-left: 95px;"></b></span>
                    <span runat="server" style="font-size: 12px; position: absolute; margin-top: 403px; margin-left: 95px;" id="datetime"></span>
                    <span runat="server" style="font-size: 12px; position: absolute; margin-top: 403px; margin-left: 298px" id="exdatetime"></span>
                    <span runat="server" id="ccid" onclick="clipboardClick()" style="cursor: pointer; font-size: 10px; position: absolute; margin-top: 10px; margin-left: 10px">id</span>
                </div>
            </div>
        </div>
    </div>
    <script>
        const clipboardClick = () => {
            navigator.clipboard.writeText(window.location.href);
        };
    </script>
</asp:Content>
