<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestEquation.aspx.cs" Inherits="InSite.UI.Admin.TestEquation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script src="/UI/Layout/common/parts/plugins/jquery/jquery.min.js"></script>

    <link rel="stylesheet" href="/UI/Layout/common/parts/plugins/mathquill/mathquill.css"/>
    <script src="/UI/Layout/common/parts/plugins/mathquill/mathquill.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tr>
                <td style="vertical-align:top;">
                    <p>
                      <span id="inputControl" style="min-width:200px;">
                          y=x^2+a\cdot\left(x+1\right)
                      </span>
                    </p>

                    <p>
                        MathField.html(): <span id="equation"></span>
                    </p>

                    <p>
                        MathField.latex(): <span id="latex"></span>
                    </p>

                    <p>
                        MQ.StaticMath(): <span id="staticEquation"></span>
                    </p>

                    <p>
                        Static html: <span class="mq-math-mode"><span class="mq-root-block mq-hasCursor"><var>y</var><span class="mq-binary-operator">=</span><var>x</var><span class="mq-supsub mq-non-leaf mq-sup-only"><span class="mq-sup" mathquill-block-id="37"><span>2</span></span></span><span class="mq-binary-operator">+</span><var>a</var><span class="mq-binary-operator">·</span><span class="mq-non-leaf"><span class="mq-scaled mq-paren" style="transform: scale(1.00007, 1.2004);">(</span><span class="mq-non-leaf" mathquill-block-id="47"><var>x</var><span class="mq-binary-operator">+</span><span>1</span></span><span class="mq-scaled mq-paren" style="transform: scale(1.00007, 1.2004);">)</span></span><span class="mq-cursor">​</span></span></span>
                    </p>

                    <p>
                        <asp:Button runat="server" ID="PrintButton" Text="Print" />
                    </p>
                </td>
                <td style="vertical-align:top;">
                    <asp:TextBox runat="server" ID="MarkdownText" TextMode="MultiLine" Rows="10" Width="500" />
                    <br />

                    <asp:Button runat="server" ID="RenderButton" Text="Render" />

                    <p>
                        <asp:Literal runat="server" ID="MarkdownResult" />
                    </p>
                </td>
            </tr>
        </table>

        <asp:ScriptManager runat="server" />

        <script>
            (function () {
                var MQ = MathQuill.getInterface(2);

                var obj = MQ.MathField($('#inputControl')[0], {
                    handlers: {
                        edit: function () {
                            init();
                        }
                    }
                });

                init();

                function init() {
                    $("#equation").html(obj.html());
                    $("#staticEquation").html(obj.latex());
                    $("#latex").html(obj.latex());

                    MQ.StaticMath($('#staticEquation')[0]);
                }
            })();

            (function () {
                var MQ = MathQuill.getInterface(2);

                Sys.Application.add_load(function () {
                    $("span.math-eq").each(function (i, el) {
                        MQ.StaticMath(el);
                        $(el).removeClass("math-eq");
                    });
                });
            })();
        </script>

    </form>
</body>
</html>
