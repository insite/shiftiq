<%@ Page Language="C#" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Delete"
    MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/Groups/Controls/GroupInfo.ascx" TagName="GroupDetail" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <div class="alert alert-danger" role="alert" runat="server" id="AdminErrorPanel" visible="false"></div>

    <section>

        <div class="row">

            <div class="col-lg-8">

                <div class="card border-0 shadow-lg mb-4">
                    <div class="card-body">
                        <h4 class="card-title mb-3">Summary</h4>
                        <uc:GroupDetail runat="server" ID="GroupDetail" />
                    </div>
                </div>

                <div runat="server" id="DeletePanel" class="d-flex alert alert-danger mb-4" role="alert">
                    <i class="fa-solid fa-octagon-exclamation fs-xl pe-1 me-2"></i>
                    <div>
                        <strong>This is a destructive action</strong> that <strong>cannot</strong> be undone. The group 
                        will be permanently deleted, including all its content, and including all references to it from 
                        other parts of the system.
                        <div class="mt-3 mb-2">
                            To confirm deletion, type the group name: 
                            <strong><asp:Literal runat="server" ID="GroupName1" /></strong>
                        </div>
                        <div class="input-group">
                            <input runat="server" id="DeleteInput" type="text" class="form-control" placeholder="Group name">
                            <button runat="server" id="DeleteButton" type="button" class="btn btn-danger">Delete</button>
                        </div>
                    </div>
                </div>

                <div runat="server" id="ArchivePanel" class="d-flex alert alert-warning mb-4" role="alert">
                    <i class="fa-solid fa-triangle-exclamation fs-xl pe-1 me-2"></i>
                    <div>
                        <strong>Alternatively</strong>, you can choose to archive this group. The name and email address 
                        for each contact in this group will be saved to a text file named 
                        <code><asp:Literal runat="server" ID="ArchiveFileName" /></code>,
                        which you can download later, and then the group will be deleted from the database.
                        <div class="mt-3 mb-2">
                            To confirm archival, type the group name: 
                            <strong><asp:Literal runat="server" ID="GroupName2" /></strong>
                        </div>
                        <div class="input-group">
                            <input runat="server" id="ArchiveInput" type="text" class="form-control" placeholder="Group name">
                            <button runat="server" id="ArchiveButton" type="button" class="btn btn-warning">Archive</button>
                        </div>
                    </div>
                </div>

                <div>
                    <insite:CancelButton runat="server" ID="CancelButton" />
                </div>

            </div>

        </div>

    </section>

</asp:Content>