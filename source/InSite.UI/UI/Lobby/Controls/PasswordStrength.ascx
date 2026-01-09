<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordStrength.ascx.cs" Inherits="InSite.UI.Lobby.Controls.PasswordStrength" %>

<div runat="server" id="Field" class="password-strength">
    <div class="level"><div></div></div>
    <div class="level"><div></div></div>
    <div class="level"><div></div></div>
    <div class="level"><div></div></div>
    <div class="clearfix"></div>
</div>

<insite:PageHeadContent runat="server" ID="HeaderLiteral">
    <style type="text/css">
        .password-strength {
            width: 100%;
            position: relative;
            padding: 6px 0;
        }

            .password-strength > div.level {
                float: left;
                width: 25%;
            }

                .password-strength > div.level > div {
                    background-color: #eeeeee;
                    border: 1px solid #d4dbe0;
                    height: 22px;
                    border-radius: 4px;
                    transition: background-color 100ms linear;
                }

                .password-strength > div.level + div.level {
                    padding-left: 3px;
                }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var levelColors = ['#e4b9b9', '#f7ecb5', '#f7ecb5', '#c1e2b3'];

            Sys.Application.add_load(function () {
                var items = [];

                $('.password-strength').each(function () {
                    var $this = $(this);
                    if ($this.data('inited') === true)
                        return;

                    var $control = getControl($this);

                    var array = $control.data('validators');
                    if (!array)
                        $control.data('validators', array = []);

                    array.push($this);
                    items.push(this);

                    $control
                        .off('change input keyup', onPasswordUpdated)
                        .on('change input keyup', onPasswordUpdated);
                    $this.data('inited', true);
                });

                for (var i = 0; i < items.length; i++) {
                    onPasswordUpdated.call(items[i]);
                }
            });

            function getControl($field) {
                var ctrlId = $field.data('control');
                if (typeof ctrlId !== 'string' || ctrlId.length == 0)
                    throw 'Invalid control ID';

                var $control = $('#' + ctrlId);
                if ($control.length !== 1)
                    throw 'Password input not found';

                return $control;
            }

            function onPasswordUpdated() {
                var $this = $(this);

                var validators = $this.data('validators');
                if (!(validators instanceof Array) || validators.length == 0)
                    return;

                var strength = getPasswordStrength($this.val());

                for (var i = 0; i < validators.length; i++) {
                    var $validator = validators[i];
                    var $indicators = $validator.find('> .level > div');
                    var levelNum = Math.floor($indicators.length * strength);
                    var colorNum = Math.floor(levelColors.length * strength);

                    var levelColor = '';
                    if (colorNum >= 1 && colorNum <= levelColors.length)
                        levelColor = levelColors[colorNum - 1];

                    $indicators.each(function (itemIndex) {
                        var itemColor = levelColor;
                        if (itemIndex >= levelNum)
                            itemColor = '';

                        $(this).css('background-color', itemColor);
                    });
                }
            }

            function getPasswordStrength(value) {
                var points = 0;

                if (value.length >= 12)
                    points++;

                var hasUpper = false;
                var hasLower = false;
                var hasNumber = false;
                var hasOther = false;

                for (var i = 0; i < value.length; i++) {
                    var char = value[i];

                    if (char >= 'A' && char <= 'Z') {
                        if (!hasUpper) {
                            hasUpper = true;
                            points++;
                        }
                    } else if (char >= 'a' && char <= 'z') {
                        if (!hasLower) {
                            hasLower = true;
                            points++;
                        }
                    } else if (char >= '0' && char <= '9') {
                        if (!hasNumber) {
                            hasNumber = true;
                            points++;
                        }
                    } else if (char >= '!' && char <= '/' || char >= ':' && char <= '@' || char >= '[' && char <= '`' || char >= '{' && char <= '~') {
                        if (!hasOther) {
                            hasOther = true;
                            points++;
                        }
                    }

                    if (hasUpper && hasLower && hasNumber && hasOther)
                        break;
                }

                return points / 5;
            }
        })();
    </script>
</insite:PageFooterContent>
