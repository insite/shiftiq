<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.Admin.Events.Classes.Forms.Delete" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/ClassSummaryInfo.ascx" TagName="SummaryInfo" TagPrefix="uc" %>	
<%@ Register Src="../Controls/ClassLocationInfo.ascx" TagName="LocationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">	

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    
    <div class="row settings">
        <div class="col-lg-6">
            <uc:SummaryInfo runat="server" ID="SummaryInfo" />

            <uc:LocationInfo runat="server" ID="LocationInfo" />

            <div class="alert alert-danger" role="alert">
                 <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
                Are you sure you want to delete this class?	
            </div>

            <p>	
                <insite:DeleteButton runat="server" ID="DeleteButton" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </p>
        </div>

        <div class="col-lg-6">

            <h3>Impact</h3>

            <div class="alert alert-warning">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                This is a permanent change that cannot be undone. 
                The event will be deleted from all forms, queries, and reports.
                Here is a summary of the data that will be erased if you proceed.
            </div>

            <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                <tr>
                    <td>
                        Class Event
                    </td>
                    <td>
                        1
                    </td>
                </tr>
                <tr>
                    <td>
                        Registrations
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="RegistrationCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Seats
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="SeatCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Gradebooks
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="GradebookCount" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Comments
                    </td>
                    <td>
                        <asp:Literal runat="server" ID="CommentsCount" />
                    </td>
                </tr>
            </table>

        </div>
    </div>

</div>
</asp:Content>
