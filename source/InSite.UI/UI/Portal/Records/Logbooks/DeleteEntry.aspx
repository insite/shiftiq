<%@ Page Language="C#" CodeBehind="DeleteEntry.aspx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.DeleteEntry" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="EntryDetail" Src="Controls/EntryDetail.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Accordion runat="server" IsFlush="false">
        <insite:AccordionPanel runat="server" ID="DetailPanel" Icon="far fa-pencil-ruler" Title="Delete Entry">
            <div class="row">

                <div class="col-md-6">

                    <h3 style="border-radius:15px 15px;background:#F7F7FC;padding:15px;">Impact</h3>

                    <div class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                        This is a permanent change that cannot be undone. 
                        The entry will be deleted from all forms, queries, and reports.
                        Here is a summary of the data that will be erased if you proceed.
                    </div>

                    <table class="table table-striped table-bordered table-metrics" style="width: 250px;">
                        <tr>
                            <td>
                                Entries
                            </td>
                            <td>
                                1
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Competencies
                            </td>
                            <td>
                                <asp:Literal runat="server" ID="CompetencyCount" />
                            </td>
                        </tr>
                        </table>

                </div>

                <div class="col-md-6">
                    <h3 style="border-radius:15px 15px;background:#F7F7FC;padding:15px;">Summary</h3>
                    <uc:EntryDetail runat="server" ID="Detail" />
                </div>
    
            </div>

        </insite:AccordionPanel>
    </insite:Accordion>

    <div class="pt-3 alert alert-danger" role="alert">
         <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong> Are you sure you want to proceed?
         Are you sure you want to delete this logbook's entry?
    </div>

    <div class="pt-3">
        <insite:DeleteButton runat="server" ID="DeleteButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>