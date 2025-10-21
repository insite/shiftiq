<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RandomizationInput.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.RandomizationInput" %>

<insite:ComboBox runat="server" ID="EnabledInput" ClientEvents-OnChange="randomizationInput.onEnabledChanged" Width="48%" />

<span runat="server" id="CountWrapper">
    <insite:ComboBox runat="server" ID="Count" Width="48%" MaxHeight="300px" />
</span>

<insite:PageFooterContent runat="server" ID="FooterScript">
    <script type="text/javascript">

        (function () {
            var instance = window.randomizationInput = window.randomizationInput || {};

            // event handlers

            instance.onEnabledChanged = function () {
                var $this = $(this);
                var value = $this.selectpicker('val');

                $this.parent().parent().find('> span').each(function () {
                    if (!this.id.endsWith('_<%= CountWrapper.ID %>'))
                        return;

                    if (value === 'True')
                        $(this).removeClass('d-none');
                    else
                        $(this).addClass('d-none');

                    return false;
                });
            };
        })();

    </script>
</insite:PageFooterContent>
