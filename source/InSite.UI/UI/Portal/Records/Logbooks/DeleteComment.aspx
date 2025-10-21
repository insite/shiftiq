<%@ Page Language="C#" CodeBehind="DeleteComment.aspx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.DeleteComment" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Accordion runat="server" IsFlush="false">
        <insite:AccordionPanel runat="server" ID="DetailPanel" Icon="far fa-pencil-ruler" Title="Delete Comment">
             <div class="row">

                <div class="col-md-6">

                    <h3 style="border-radius:15px 15px;background:#F7F7FC;padding:15px;">Impact</h3>

                    <div class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The comment will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                    </div>
                </div>

                 <div class="col-md-6">
                    <h3 style="border-radius:15px 15px;background:#F7F7FC;padding:15px;">Summary</h3>
                    

            <div class="mb-3">
                <h5 class="mt-2 mb-1">
                    <insite:Literal runat="server" Text="Posted" />
                </h5>
                <div>
                    <asp:Literal runat="server" ID="Posted" />
                </div>
            </div>

            <div class="mb-3">
                <h5 class="mt-2 mb-1">
                    <insite:Literal runat="server" Text="Text" />
                </h5>
                <div>
                    <asp:Literal runat="server" ID="Text" />
                </div>
            </div>
                </div>
            </div>

        </insite:AccordionPanel>
    </insite:Accordion>

    <div class="pt-3 alert alert-danger" role="alert">
         <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong> Are you sure you want to proceed?
         Are you sure you want to delete this comment?
    </div>

    <div class="pt-3">
        <insite:DeleteButton runat="server" ID="DeleteButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>