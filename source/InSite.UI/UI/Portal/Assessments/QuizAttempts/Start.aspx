<%@ Page Language="C#" CodeBehind="Start.aspx.cs" Inherits="InSite.UI.Portal.Assessments.QuizAttempts.Start" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <section class="container mb-2 mb-sm-0 pb-sm-5">
        <div class="float-end ms-3">
            <div class="quiz-timer"></div>
        </div>
        <h1 runat="server" id="QuizTitle" class="mb-4 quiz-title"></h1>
        <insite:Alert runat="server" ID="ScreenStatus" ShowClose="true" />

        <insite:DynamicControl runat="server" ID="View" />
    </section>
</asp:Content>