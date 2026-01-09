<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SidebarSwitch.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.Navigation.SidebarSwitch" %>

<div class="ps-4 d-flex" data-type="container">
    <i data-type="left-icon" class="fa-light fa-window fs-lg mt-1 me-2"></i>
    <insite:CheckSwitch runat="server" ID="Switch" CssClass="mb-0" />
    <i data-type="right-icon" class="fa-light fa-sidebar fs-lg mt-1"></i>
</div>

<insite:UpdatePanel runat="server" ID="UpdatePanel" />

<insite:PageHeadContent runat="server">
    <style type="text/css">
        #<%= Switch.ClientID %>:not(:indeterminate) {
            background-color: var(--ar-primary) !important;
        }
        #<%= Switch.ClientID %>:indeterminate {
            background-color: var(--ar-gray-500) !important;
        }
    </style>
</insite:PageHeadContent>

<script type="text/javascript">
    (function () {
        const stateArray = [null, true, null, false];

        const instance = window.sidebarSwitch = {
            setState: setState
        };

        setState(<%= GetJsValue() %>);

        Sys.Application.add_load(function () {
            const check = document.getElementById('<%= Switch.ClientID %>');

            if (!check._isInited) {
                check.removeEventListener('change', onSwitchChange);
                check.addEventListener('change', onSwitchChange);
                check._isInited = true;
            }

            if (check._state === undefined)
                setState(<%= GetJsValue() %>);
        });

        function setState(value) {
            const check = document.getElementById('<%= Switch.ClientID %>');

            if (value === true)
                check._state = 1;
            else if (value === false)
                check._state = 4;
            else if (check._state != 2)
                check._state = 0;

            updateSwitchState(check);
        }

        function onSwitchChange(e) {
            const check = e.target;

            check._state++;
            if (check._state >= stateArray.length)
                check._state = 0;

            updateSwitchState(check);
            sendValue(check);
        }

        function sendValue(check) {
            const checked = stateArray[check._state];
            const value = checked === true
                ? '<%= SidebarValue.Enabled %>'
                : checked === false
                    ? '<%= SidebarValue.Disabled %>'
                    : '<%= SidebarValue.Default %>';
            document.getElementById('<%= UpdatePanel.ClientID %>').ajaxRequest(value);
        }

        function updateSwitchState(check) {
            const checked = stateArray[check._state];
            const container = check.closest('[data-type="container"]');
            const leftIcon = container.querySelector('[data-type="left-icon"]');
            const rightIcon = container.querySelector('[data-type="right-icon"]');

            if (checked === null) {
                check.indeterminate = true;
                check.checked = false;
                setIconState(leftIcon, false);
                setIconState(rightIcon, false);
            } else {
                check.indeterminate = false;
                check.checked = checked;
                setIconState(leftIcon, !checked);
                setIconState(rightIcon, checked);
            }
        }

        function setIconState(icon, active) {
            if (active === true) {
                icon.classList.remove('opacity-50');
                icon.classList.add('text-primary');
            } else {
                icon.classList.add('opacity-50');
                icon.classList.remove('text-primary');
            }
        }
    })();
</script>
