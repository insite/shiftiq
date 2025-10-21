<%@ Page Language="C#" CodeBehind="Remove.aspx.cs" Inherits="InSite.Admin.Records.Programs.DeleteUser" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <section class="pb-5 mb-md-2">

        <div class="row">                       

            <div class="col-lg-6">                                   
                
                <h3>User</h3>
                
                <uc:PersonDetail runat="server" ID="PersonDetail" />

                <h3>Program</h3>

                <dl class="row">
                    <dt class="col-sm-3">Program Name:</dt>
                    <dd class="col-sm-9"><asp:Literal runat="server" ID="ProgramName" /></dd>
                </dl>

                <div>
                    <insite:DeleteButton runat="server" ID="DeleteButton" />	
                    <insite:CancelButton runat="server" ID="CancelButton" />	
                </div>
                
            </div>

            <div class="col-lg-6">

                <h3>Impact</h3>

                <div class="alert alert-warning">
                    <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                    This is a permanent change that cannot be undone. 
                    The Program's user will be deleted from all forms, queries, and reports.
                    Here is a summary of the data that will be erased if you proceed.
                </div>

                <table class="table table-striped table-bordered table-metrics" style="width: 100%;">
                    <tr>
                        <td>
                            Users
                        </td>
                        <td>
                            1
                        </td>
                    </tr>
                </table>
            </div>     
        </div>
    </section>

</asp:Content>
