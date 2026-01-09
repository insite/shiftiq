<%@ Page Language="C#" CodeBehind="ChangeUser.aspx.cs" Inherits="InSite.Admin.Records.Gradebooks.Forms.ChangeUser" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	
<%@ Register Src="~/UI/Admin/Records/Gradebooks/Controls/GradebookInfo.ascx" TagName="GradebookDetail" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />

    <section runat="server" ID="ItemSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-user me-1"></i>
            Change Student
        </h2>

            
            <div class="row">

                <div class="col-lg-4">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Gradebook User</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Note/Comment
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="UserNote" TextMode="MultiLine" Rows="6" MaxLength="400" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    User Added to Program
                                </label>
                                <div>
                                    <insite:DateTimeOffsetSelector runat="server" ID="UserAdded" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-4">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Student</h3>
                            <uc:PersonDetail runat="server" ID="PersonDetail" />

                        </div>
                    </div>
                </div>

                <div class="col-lg-4">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <h3>Gradebook</h3>
                            <uc:GradebookDetail runat="server" ID="GradebookDetails" />

                        </div>
                    </div>
                </div>

            </div>

    </section>


    <div class="mt-3">	
        <insite:SaveButton runat="server" ID="SaveButton" />	
        <insite:CancelButton runat="server" ID="CancelButton" />	
    </div>	

</asp:Content>
