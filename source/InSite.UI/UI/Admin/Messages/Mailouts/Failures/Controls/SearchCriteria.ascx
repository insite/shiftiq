<%@ Control Language="C#" CodeBehind="SearchCriteria.ascx.cs" Inherits="InSite.UI.Admin.Messages.Mailouts.Failures.Controls.SearchCriteria" %>

<%@ Register TagPrefix="uc" TagName="FilterManager" Src="~/UI/Layout/Common/Controls/SearchCriteriaFilterManager.ascx" %>

<div class="row">
    <div class="col-9">
        <div id="toolbox" class="toolbox-section">
            <h4>Criteria</h4>
            <div class="row">

                <div class="col-6">
                    <div class="mb-2">
                        <insite:MessageTypeComboBox runat="server" ID="MessageType" EmptyMessage="Message Type" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Sender" EmptyMessage="Sender" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Recipient" EmptyMessage="Recipient" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="Subject" EmptyMessage="Subject" MaxLength="256" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                    </div>
                </div>

                <div class="col-6">
                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ScheduledSince" runat="server" EmptyMessage="Scheduled &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="ScheduledBefore" runat="server" EmptyMessage="Scheduled &lt;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="FailedSince" runat="server" EmptyMessage="Failed &ge;" />
                    </div>

                    <div class="mb-2">
                        <insite:DateTimeOffsetSelector ID="FailedBefore" runat="server" EmptyMessage="Failed &lt;" />
                    </div>
                </div>

            </div>
        </div>
    </div>
    <div class="col-3">
        <div class="mb-2">
            <h4>Settings</h4>
            <insite:MultiComboBox ID="ShowColumns" runat="server" />
        </div>
        <div>
            <h4>Saved Filters</h4>
            <uc:FilterManager runat="server" ID="FilterManager" />
        </div>
    </div>
</div>
