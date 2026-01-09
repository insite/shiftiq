<%@ Page Language="C#" CodeBehind="Error.aspx.cs" Inherits="InSite.UI.Portal.Error" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <h1><insite:Literal runat="server" Text="Unexpected Error" /></h1>
    <div class="mb-5"><insite:Literal runat="server" Text="Error Please contact your administrator" />
        </div>

    <!-- Page content-->
    <div class="container">
        <div class="pb-5">
            <div class="alert d-flex alert-warning" role="alert">
              <i class="fas fa-exclamation-triangle fa-2x me-3"></i>
                <div>
                    <h1 class="text-warning"><asp:Literal runat="server" ID="ProblemTitle" Text="Problem Title" /></h1>
                    <h2 class="text-dark"><asp:Literal runat="server" ID="ProblemSource" Text="Problem Source" /></h2>
                    <div class="text-dark">
                        <asp:Literal runat="server" ID="ProblemDescription" Text="Problem description and recommended solution(s)." />
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
