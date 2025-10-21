<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineTranslate.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.OutlineTranslate" %>

<h4 runat="server" id="Header" style="margin-bottom:20px;"></h4>

<div class="row row-translate">
    <div class="col-md-12">
        <asp:Repeater runat="server" ID="FieldRepeater">
            <ItemTemplate>
                <div class="fw-bold mb-2"><%# Eval("Title") %></div>
                <div class="mb-4"><insite:DynamicControl runat="server" ID="Container" /></div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

<div class="mt-3 text-end">
    <insite:SaveButton runat="server" ID="SaveButton" CausesValidation="true" ValidationGroup="OutlineTranslate" />
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        .row-translate .content-field {
            position: relative;
        }

            .row-translate .content-field > .commands {
                position: absolute;
                right: 51px;
                top: -2px;
            }

                .row-translate .content-field > .commands > span.lang-out {
                    background-color: #aaa;
                }

    </style>
</insite:PageHeadContent>
