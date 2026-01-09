<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineContentFieldList.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.OutlineContentFieldList" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>

<div class="row settings">
    <div class="col-md-12">                           

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:Button runat="server" id="ChangeLink" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
            </div>
            <asp:Repeater runat="server" ID="LabelRepeater">
                <ItemTemplate>
                    <div>
                        <h3><%# GetLabelTitle() %></h3>
                        <div class="form-text mb-3">
                            <%# Eval("Description") %>
                        </div>
                        <uc:MultilingualStringInfo runat="server" ID="Output" />
                    </div>
                    <hr class="mb-3" />
                </ItemTemplate>
            </asp:Repeater>
        </div>

    </div>
</div>