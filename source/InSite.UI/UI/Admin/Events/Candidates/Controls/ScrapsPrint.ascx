<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScrapsPrint.ascx.cs" Inherits="InSite.Admin.Events.Candidates.Controls.ScrapsPrint" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="us-ascii">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>ITA-003</title>

    <asp:Literal runat="server" ID="StyleLink" />

    <style type="text/css">
        body {
            font-family: Calibri, Helvetica, Arial;
            font-size: 45px;
            width: 1200px;
        }

        * {
            box-sizing: border-box;
        }

        .candidate {
            position: relative;
            width: 100%;
        }

        .candidate > table.header {
            width: 100%;
            padding-bottom: 3px;
            margin-bottom: 15px;
            border-bottom: 1px solid #eee;
        }

        .candidate > table.header td {
            vertical-align: baseline;
        }

        .candidate > table.header td.logo {
            height: 115px;
            width: 20%;
        }

        .candidate > table.header td.logo img {
            max-width: 100%;
        }

        .candidate > table.header td.title {
            padding-left: 3%;
            width: 25%;
        }

        .candidate > table.header td.name {
            text-align: right;
            font-size: 0.75em;
        }

        .candidate > table.header td.code {
            padding-left: 1%;
            width: 8%;
            font-size: 0.5em;
            text-align: right;
        }

        .candidate > .subtitle {
            font-size: 0.3em;
        }

        .candidate > .password {
            font-size: 0.5em;
            text-align: right;
            position: absolute;
            bottom: 0;
            right: 0;
        }
    </style>
</head>
<body>
    <asp:Repeater runat="server" ID="CandidateRepeater">
        <ItemTemplate>
            <div class="candidate" style='<%# (bool)Eval("IsLast") ? null : "page-break-after: always;" %>'>
                <table class="header">
                    <tr>
                        <td class="logo"><img alt="" src="<%# Logo %>" /></td>
                        <td class="title">Scrap Paper</td>
                        <td class="name"><%# Eval("Name") %></td>
                        <td class="code"><%# Eval("Code") %></td>
                    </tr>
                </table>

                <div class="subtitle">
                    This must be returned to the invigilator upon completion of the exam.
                </div>

                <div class="password"><%# Eval("Password") %></div>

            </div>
        </ItemTemplate>
    </asp:Repeater>

    <script type="text/javascript">

        (function () {

            setTimeout(init, 0);

            function init() {
                var items = document.querySelectorAll('.candidate');
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    item.style.height = Math.floor(item.offsetWidth * 1.2941) + 'px';
                }
            }
        })();

    </script>
</body>
</html>