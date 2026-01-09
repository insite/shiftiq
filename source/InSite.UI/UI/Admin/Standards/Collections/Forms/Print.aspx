<%@ Page Language="C#" CodeBehind="Print.aspx.cs" Inherits="InSite.Admin.Standards.Collections.Forms.Print" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="SettingsManager" Src="../Controls/PrintSettingsManager.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Collections" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-print"></i>
            Print
        </h2>

        <div class="row">

            <div class="col-lg-12">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Print Settings</h3>

                        <div class="row">
                            <div class="col-md-8">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Competency Depth
                                            <insite:RequiredValidator runat="server" ControlToValidate="CompetencyDepthFrom" FieldName="Competency Depth From" ValidationGroup="Documents" />
                                        <insite:RequiredValidator runat="server" ControlToValidate="CompetencyDepthThru" FieldName="Competency Depth Thru" ValidationGroup="Documents" />
                                    </label>
                                    <div>
                                        <div style="width: calc(50% - 30px); display: inline-block;">
                                            <insite:ComboBox runat="server" ID="CompetencyDepthFrom" Width="100%" />
                                        </div>
                                        <div style="width: 50px; display: inline-block; text-align: center;">
                                            thru
                                        </div>
                                        <div style="width: calc(50% - 30px); display: inline-block;">
                                            <insite:ComboBox runat="server" ID="CompetencyDepthThru" Width="100%" />
                                        </div>
                                    </div>
                                    <div class="form-text">
                                        Determines number of levels below Area.
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Footer Text
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="FooterText" />
                                    </div>
                                </div>

                            </div>
                            <div class="col-md-4">
                                <uc:SettingsManager runat="server" ID="SettingsManager" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group mb-3">
                                    <label class="form-label">Competency Content Fields</label>
                                    <div>
                                        <asp:CheckBoxList runat="server" ID="CompetencyFields">
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group mb-3">
                                    <label class="form-label">Competency Settings</label>
                                    <div>
                                        <asp:CheckBoxList runat="server" ID="CompetencySettings">
                                            <asp:ListItem Text="Tags" Value="Tags" Selected="True" />
                                            <asp:ListItem Text="Complexity" Value="Complexity" Selected="True" />
                                            <asp:ListItem Text="Criticality" Value="Criticality" Selected="True" />
                                            <asp:ListItem Text="Frequency" Value="Frequency" Selected="True" />
                                            <asp:ListItem Text="Recurrence" Value="Recurrence" Selected="True" />
                                            <asp:ListItem Text="Difficulty" Value="Difficulty" Selected="True" />
                                        </asp:CheckBoxList>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group mb-3">
                                    <label class="form-label">Other Settings</label>
                                    <div>
                                        <div>
                                            <insite:CheckBox runat="server" ID="IsShowFieldHeading" Text="Show Standards Headings" Checked="true" />
                                        </div>
                                        <div>
                                            <insite:CheckBox runat="server" ID="IsOrderedList" Text="Format Standards as An Ordered List" Checked="false" />
                                        </div>
                                        <div>
                                            <insite:CheckBox runat="server" ID="IsBulletedList" Text="Format Standards as A Bulleted List" Checked="false" />
                                        </div>
                                        <div>
                                            <insite:CheckBox runat="server" ID="IsPrintAsChecklist" Text="Print As Checklist" Checked="false" />
                                        </div>
                                        <div>
                                            <insite:CheckBox runat="server" ID="IsRenderPageNumbers" Text="Add Page Numbers" Checked="false" />
                                        </div>
                                        <div>
                                            <insite:CheckBox runat="server" ID="IsRenderToc" Text="Add Table Of Contents" Checked="false" />
                                        </div>
                                        <div>
                                            <insite:CheckBox runat="server" ID="IsRenderPageBreaks" Text="Page Breaks after sections" Checked="true" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                </div>

            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:Button runat="server" ID="PrintButton" ButtonStyle="Success" CausesValidation="true" ValidationGroup="Collections" Icon="far fa-download" Text="Generate Word Document" />
            <insite:CancelButton runat="server" ID="CancelLink" />
        </div>
    </div>


    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var $isOrderedList = $('#<%= IsOrderedList.ClientID %>').on('change', onIsOrderedListChanged);
            var $isBulletedList = $('#<%= IsBulletedList.ClientID %>').on('change', onIsBulletedListChanged);
            var $isPrintAsChecklist = $('#<%= IsPrintAsChecklist.ClientID %>').on('change', onIsPrintAsChecklistChanged);
            var $competencyDepthComboFrom, $competencyDepthComboThru;

            Sys.Application.add_load(onLoad);

            function onLoad() {
                $competencyDepthComboFrom = $('#<%= CompetencyDepthFrom.ClientID %>')
                    .off('change', onCompetencyDepthFromChanged)
                    .on('change', onCompetencyDepthFromChanged);

                $competencyDepthComboThru = $('#<%= CompetencyDepthThru.ClientID %>')
                    .off('change', onCompetencyDepthThruChanged)
                    .on('change', onCompetencyDepthThruChanged);

                onIsPrintAsChecklistChanged();
                onIsBulletedListChanged();
                onIsOrderedListChanged();
                onCompetencyDepthFromChanged();
                onCompetencyDepthThruChanged();
            }

            function onIsPrintAsChecklistChanged() {
                if ($isPrintAsChecklist.prop('disabled') === true)
                    return;

                var isChecked = $isPrintAsChecklist.prop('checked');

                $('#<%= CompetencyFields.ClientID %> input[type="checkbox"]').prop('disabled', isChecked);
                $('#<%= CompetencySettings.ClientID %> input[type="checkbox"]').prop('disabled', isChecked);
                $('#<%= IsShowFieldHeading.ClientID %>').prop('disabled', isChecked);
                $('#<%= IsOrderedList.ClientID %>').prop('disabled', isChecked);
                $('#<%= IsBulletedList.ClientID %>').prop('disabled', isChecked);
                $('#<%= IsRenderToc.ClientID %>').prop('disabled', isChecked);
                $('#<%= IsRenderPageBreaks.ClientID %>').prop('disabled', isChecked);

                $competencyDepthComboFrom.prop('disabled', isChecked)
                $competencyDepthComboFrom.selectpicker('refresh');

                $competencyDepthComboThru.prop('disabled', isChecked)
                $competencyDepthComboThru.selectpicker('refresh');
            }

            function onIsBulletedListChanged() {
                if ($isBulletedList.prop('disabled') === true)
                    return;

                var isChecked = $isBulletedList.prop('checked');

                $isOrderedList.prop('disabled', isChecked);

                <% if (IsPrintAsChecklist.Enabled)
            { %>
                $isPrintAsChecklist.prop('disabled', isChecked);
                <% } %>
            }

            function onIsOrderedListChanged() {
                if ($isOrderedList.prop('disabled') === true)
                    return;

                var isChecked = $isOrderedList.prop('checked');

                $isBulletedList.prop('disabled', isChecked);
                <% if (IsPrintAsChecklist.Enabled)
            { %>
                $isPrintAsChecklist.prop('disabled', isChecked);
                <% } %>
                }

                function onCompetencyDepthFromChanged() {
                    var from = parseInt($competencyDepthComboFrom.selectpicker('val'));
                    var thru = parseInt($competencyDepthComboThru.selectpicker('val'));

                    if (thru < from)
                        $competencyDepthComboThru.selectpicker('val', String(from));
                }

                function onCompetencyDepthThruChanged() {
                    var from = parseInt($competencyDepthComboFrom.selectpicker('val'));
                    var thru = parseInt($competencyDepthComboThru.selectpicker('val'));

                    if (from > thru)
                        $competencyDepthComboFrom.selectpicker('val', String(thru));
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
