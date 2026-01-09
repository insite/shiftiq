<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageArray.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.ContentBlocks.ImageArray" %>

<%@ Register TagPrefix="uc" TagName="ContentBlockImage" Src="Image.ascx" %>

<div style="height:5px"></div>

<asp:Repeater runat="server" ID="ImageRepeater">
    <HeaderTemplate>
        <table id='<%# ImageRepeater.ClientID %>' class="table table-img-array"><tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td class="img-num"><strong><%# (Container.ItemIndex + 1).ToString("n0") %>.</strong></td>
            <td class="img-ctrl"><uc:ContentBlockImage runat="server" ID="Image" /></td>
            <td class="img-cmd">
                <insite:Button runat="server" title="Remove" ID="Remove" Icon="fas fa-trash-alt" ButtonStyle="Default"
                    CommandName="Delete" ConfirmText="Are you sure you want to remove this item?" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<div style="margin-top:15px">
    <insite:Button runat="server" ID="AddImage" Text="Add Image" Icon="fas fa-plus-circle" ButtonStyle="Default" />
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .table-img-array td.img-num {
            width: 40px;
            text-align: right;
            line-height: 34px;
        }

        .table-img-array td.img-ctrl {
            padding-right: 0;
        }

        .table-img-array td.img-cmd {
            width: 50px;
            white-space: nowrap;
            padding-left: 3px;
        }

    </style>
</insite:PageHeadContent>