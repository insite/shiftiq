<%@ Page Language="C#" CodeBehind="Organize.aspx.cs" Inherits="InSite.Admin.Contacts.Groups.Forms.Organize" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    
    <div class="card border-0 shadow-lg mb-3">
        <div class="card-body">

            <h4 class="card-title mb-3">
                <i class="far fa-users me-1"></i>
                Parents (Functional)
            </h4>

            <div class="row">
                <div class="col-md-4">
                    <asp:Repeater runat="server" ID="SupergroupRepeater">
                        <ItemTemplate>
                            <%# TryRenderHeader(Container.DataItem) %>
                            <insite:CheckBox runat="server" ID="IsSelected" Enabled='<%# Eval("Enabled") %>' Checked='<%# Eval("Selected") %>' Text='<%# Eval("Name") %>' Value='<%# Eval("GroupIdentifier") %>' />
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

        </div>
    </div>
        
    <div class="mt-3 sticky-buttons">
        <insite:SaveButton runat="server" ID="SaveButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>

