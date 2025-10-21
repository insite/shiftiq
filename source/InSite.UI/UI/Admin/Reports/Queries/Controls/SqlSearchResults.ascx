<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SqlSearchResults.ascx.cs" Inherits="InSite.Admin.Reports.Queries.Controls.SqlSearchResults" %>

<div class="float-end mt-3 mb-3">
    <insite:DropDownButton runat="server" ID="DownloadDropDown" Visible="false" />
</div>

<div class="clearfix"></div>

<div class="fixed-header">
    <insite:Grid runat="server" ID="Grid" AutoGenerateColumns="true">
    </insite:Grid>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        div.fixed-header {
            width: 100%;
            overflow: auto;
            max-height: 600px; /* More friendly height for those who doesn't have giant monitor */
        }

            div.fixed-header table {
                border-collapse: separate;
                border-spacing: 0;
            }

                div.fixed-header table > thead > tr > th {
                    position: sticky;
                    top: 0;
                    background-color: white
                }
    </style>
</insite:PageHeadContent>
