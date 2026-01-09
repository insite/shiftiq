<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImagePrint.ascx.cs" Inherits="InSite.Admin.Assessments.Attachments.Controls.ImagePrint" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="us-ascii">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title runat="server" id="PageTitle"></title>

    <link href="/library/fonts/font-awesome/7.1.0/css/all.min.css" rel="stylesheet" type="text/css">

    <style runat="server" id="ImgFitPageStyle" type="text/css" visible="false">

        body {
            width: 805px;
        }

        .image-wrapper > div > img {
            max-width: 100% !important;
            max-height: 955px !important;
        }

        .integer-part    { padding-right: 0.05em !important; }
        .fractional-part { padding-left: 0.05em !important; }

        .no-right-pad { padding-right: 0.05em !important; }
        .no-left-pad  { padding-left: 0.05em !important; }

        .small-right-pad { padding-right: 0.25em !important; }
        .small-left-pad  { padding-left: 0.25em !important; }

        .x-wide { padding-right: 5em !important; }

    </style>

    <style type="text/css">
        body {
            font-family: Arial;
            font-size: 12pt;
            margin: 0;
        }

        .image-wrapper {
            page-break-inside: avoid;
            page-break-before: always;
            overflow: hidden;
            text-align: center;
        }

            .image-wrapper > div {
                display: inline-block;
            }

                .image-wrapper > div > div {
                    margin-top: 30px;
                }

        .table-of-contents {
            margin-left: 30px;
        }

            .table-of-contents > a {
                text-decoration: none;
                color: inherit !important;
                display: block;
                position: relative;
            }

                .table-of-contents > a + a {
                    margin-top: 8px;
                }

                .table-of-contents > a > span {
                    width: 75%;
                    display: inline-block;
                }

                    .table-of-contents > a > span > span {
                        font-weight: bold;
                        background-color: #fff;
                    }

                        .table-of-contents > a > span > span > .spacer {
                            background-color: #fff;
                            display: inline-block;
                            width: 1em;
                        }

                        .table-of-contents > a > span > span > .dots {
                            font-weight: normal;
                        }

                            .table-of-contents > a > span > span > .dots::before {
                                position: absolute;
                                height: 1em;
                                display: inline-block;
                                overflow: hidden;
                                text-overflow: clip;
                                content: ". . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . ";
                            }

                        .table-of-contents > a > span > span > .page {
                            position: absolute;
                            right: 0;
                            text-align: right;
                            background-color: #fff;
                            padding-left: 1em;
                            font-weight: normal;
                        }
    </style>
</head>
<body>
    <asp:Repeater runat="server" ID="TocRepeater">
        <HeaderTemplate>
            <div style="margin:0 15px;">
                <h2 style="text-align:center;">Table of Contents</h2>

                <div class="table-of-contents">
        </HeaderTemplate>
        <FooterTemplate>
                </div>
            </div>
        </FooterTemplate>
        <ItemTemplate>
            <a href='<%# "#img" + Container.ItemIndex %>' style='<%# Container.ItemIndex != 0 && (Container.ItemIndex % PageSize) == 0 ? "page-break-before:always;" : null %>'>
                <span>
                    <span>
                        <%# Eval("Title") %>
                        <span class="spacer">&nbsp;</span>
                        <span class="dots"></span>
                        <span class="page"><%# Container.ItemIndex + PageCount + 1 %></span>
                    </span>
                </span>
            </a>
        </ItemTemplate>
    </asp:Repeater>

    <asp:Repeater runat="server" ID="ImageRepeater">
        <ItemTemplate>
            <div id='<%# "img" + Container.ItemIndex.ToString() %>' class="image-wrapper">
                <div>
                    <img alt="" src='<%# Eval("Url") %>' style='<%# Eval("ImgStyle") %>' />
                    <div style="margin-top:30px; text-align:center;"><%# Eval("Title") %></div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</body>
</html>