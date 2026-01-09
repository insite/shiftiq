<%@ Page CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Accounts.Permissions.Forms.Edit" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Permission" />

    <div class="row mb-3">
        <div class="col-md-12">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-key me-1"></i>
                        Permission
                    </h4>

                    <div class="row">
                        <div class="col-lg-6">
                            <uc:Detail ID="Detail" runat="server" />
                        </div>
                        <div class="col-lg-6">
                            <div class="form-group mb-3" runat="server" ID="ActionField" Visible="false">
                                <label class="form-label">
                                    Actions (Children)
                                </label>
					
					            <asp:Repeater runat="server" ID="ActionRepeater">
                                    <HeaderTemplate><table class="table table-striped table-sm"><tbody></HeaderTemplate>
                                    <FooterTemplate></tbody></table></FooterTemplate>
						            <ItemTemplate>
							            <tr><td>
								            <a href='/ui/admin/platform/routes/edit?id=<%# Eval("ActionIdentifier") %>'>
									            <%# Eval("ActionUrl") %>
								            </a>
							            </td><td>
							                <%# Eval("ActionName") %>
							            </td><td>
								            <a href='/ui/admin/platform/routes/edit?id=<%# Eval("ActionIdentifier") %>'>
									            <i class="far fa-pencil"></i>
								            </a>
							            </td></tr>
						            </ItemTemplate>
					            </asp:Repeater>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Permission" />
            <insite:DeleteButton runat="server" ID="DeleteButton" CausesValidation="false" ConfirmText="Are you sure you want to delete this permission?" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>
</asp:Content>
