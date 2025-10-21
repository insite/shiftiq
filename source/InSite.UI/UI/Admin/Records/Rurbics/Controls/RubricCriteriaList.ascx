<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RubricCriteriaList.ascx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Controls.RubricCriteriaList" %>

<insite:PageHeadContent runat="server">
    <style>
        .add-button {
            top:26px;
        }
    </style>
</insite:PageHeadContent>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel" ClientEvents-OnResponseEnd="rubricCriteriaList.init">
    <ContentTemplate>

        <insite:Button runat="server" ID="AddNewCriterionButton" Icon="far fa-plus-circle" Text="Add New Criterion" ButtonStyle="Default" CssClass="mb-3" />

        <div class="mb-3">
            <b>Total Rubric Points: <span runat="server" ID="CriteriaRubricPoints" /></b>
        </div>

        <asp:Repeater runat="server" ID="CriteriaRepeater">
            <ItemTemplate>

                <div class="card border-0 shadow-lg mb-2 criterion-card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-1">
                                <insite:CheckBox runat="server" ID="Range" Text="Range" Checked='<%# Eval("Range") %>' AutoPostBack="true" />
                                <insite:Button runat="server"
                                    ButtonStyle="Link"
                                    CssClass="p-0"
                                    Icon="far fa-copy"
                                    Text="Duplicate"
                                    ToolTip="Duplicate Criterion"
                                    ConfirmText="Are you sure to duplicate this criterion?"
                                    CommandName="DuplicateCriterion"
                                />
                                <insite:Button runat="server"
                                    ButtonStyle="Link"
                                    CssClass="p-0"
                                    Icon="far fa-trash-alt"
                                    Text="Delete"
                                    ToolTip="Delete Criterion"
                                    ConfirmText="Are you sure to delete this criterion?"
                                    CommandName="DeleteCriterion"
                                    Visible="<%# AllowCriterionDelete %>"
                                />
                            </div>
                            <div class="col-md-11">

                                <div class="row mb-1">
                                    <div class="col-md-4 pe-0">
                                        <asp:Literal runat="server" ID="Identifier" Text='<%# Eval("Identifier") %>' Visible="false" />
                                        <insite:TextBox runat="server" ID="Title" Text='<%# Eval("Title") %>' EmptyMessage="Criterion Title" MaxLength="100" />
                                    </div>
                                    <div class="col-md-2 pe-0">
                                        <insite:NumericBox runat="server"
                                            ID="Points"
                                            NumericMode="Float"
                                            DecimalPlaces="2"
                                            ValueAsDecimal='<%# Eval("Points") %>'
                                            ReadOnly="true"
                                        />
                                    </div>
                                    <div class="col-md-6">
                                        <insite:TextBox runat="server" ID="Description" Text='<%# Eval("Description") %>' EmptyMessage="Criterion Description" />
                                    </div>
                                </div>

                                <asp:Repeater runat="server" ID="RatingRepeater">
                                    <ItemTemplate>
                                        <div class="row mb-1">
                                            <div class="col-md-1 mt-2 pe-0">
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <insite:IconButton runat="server"
                                                            Name="plus-circle"
                                                            Title="Add New Rating"
                                                            CssClass="text-success position-relative add-button"
                                                            Visible="<%# Container.ItemIndex < RatingCount - 1 %>"
                                                            CommandName="AddRating"
                                                        />
                                                    </div>
                                                    <div class="col-md-6 text-end">
                                                        <insite:IconButton runat="server"
                                                            Name="trash-alt"
                                                            ToolTip="Delete Rating"
                                                            ConfirmText="Are you sure to delete this rating?"
                                                            Visible="<%# Container.ItemIndex != 0 && Container.ItemIndex < RatingCount - 1 %>"
                                                            CommandName="DeleteRating"
                                                        />
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3 pe-0">
                                                <asp:Literal runat="server" ID="Identifier" Text='<%# Eval("Identifier") %>' Visible="false" />
                                                <insite:TextBox runat="server" ID="Title" Text='<%# Eval("Title") %>' EmptyMessage="Rating Title" MaxLength="100" />
                                            </div>
                                            <div runat="server" id="NotRangeDiv" class="col-md-2 pe-0">
                                                <insite:NumericBox runat="server" ID="NotRangePoints" NumericMode="Float" DecimalPlaces="2" ValueAsDecimal='<%# Eval("Points") %>' />
                                            </div>
                                            <div runat="server" id="RangeDiv" class="col-md-2 pe-0">
                                                <div class="row">
                                                    <div class="col-md-7">
                                                        <insite:NumericBox runat="server" ID="RangePoints" NumericMode="Integer" ValueAsDecimal='<%# Eval("Points") %>' />
                                                    </div>
                                                    <div class="col-md-5 d-flex align-items-center p-0 range-criteria">
                                                        <%# Eval("RangeCriteria") %>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <insite:TextBox runat="server" ID="Description" Text='<%# Eval("Description") %>' EmptyMessage="Rating Description" />
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>
                        </div>

                    </div>
                </div>

            </ItemTemplate>
        </asp:Repeater>

    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageFooterContent runat="server">
    <script>
        (() => {
            const instance = window.rubricCriteriaList = window.rubricCriteriaList || {};
            const containerId = "<%= UpdatePanel.ClientID %>";
            const pointsId = "<%= CriteriaRubricPoints.ClientID %>";
            let isDocumentLoaded = false;

            window.addEventListener('DOMContentLoaded', () => {
                isDocumentLoaded = true;
                onDocumentLoaded();
            });

            instance.init = () => {
                const inputs = document.querySelectorAll("#" + containerId + " .criterion-card input.insite-numeric:not([readonly])");
                for (const input of inputs)
                    input.addEventListener("blur", numericInput_Blur);

                if (isDocumentLoaded)
                    onDocumentLoaded();
            }

            instance.init();

            function onDocumentLoaded() {
                const cards = document.querySelectorAll("#" + containerId + " .criterion-card");
                for (const card of cards) {

                    setCriterionPoints(card);
                    setRangeCriteria(card);
                }

                updateTotalPoints();
            }

            function numericInput_Blur(e) {
                const card = e.target.closest(".criterion-card");

                setCriterionPoints(card);
                setRangeCriteria(card);
                updateTotalPoints();
            }

            function setCriterionPoints(card) {
                const inputs = card.querySelectorAll("input.insite-numeric:not([readonly])");
                let maxPoints = null;

                for (const input of inputs) {
                    const points = parseFloat(input.value);
                    if (isNaN(points))
                        continue;

                    if (maxPoints == null || points > maxPoints)
                        maxPoints = points;
                }

                if (maxPoints == null)
                    maxPoints = 0;

                const criterionInput = card.querySelector("input.insite-numeric:is([readonly])");
                criterionInput.value = String(maxPoints);
                criterionInput.dispatchEvent(new Event('change'));
            }

            function setRangeCriteria(card) {
                const rangeCheckbox = card.querySelector("input[id$='Range']");
                if (!rangeCheckbox.checked) {
                    return;
                }

                const inputs = card.querySelectorAll("input.insite-numeric:not([readonly])");

                for (let i = 0; i < inputs.length - 1; i++) {
                    const div = inputs[i].closest("div[class=row]").querySelector(".range-criteria");
                    const rangeCriteria = `to >${inputs[i + 1].value}`;

                    div.innerHTML = rangeCriteria;
                }
            }

            function updateTotalPoints() {
                const output = document.querySelector("#" + pointsId);
                const inputs = document.querySelectorAll("#" + containerId + " .criterion-card input.insite-numeric[readonly]");

                let sum = 0;

                for (const input of inputs) {
                    const points = parseFloat(input.value);
                    if (!isNaN(points))
                        sum += points;
                }

                output.innerHTML = sum.toFixed(2);
                output.dispatchEvent(new CustomEvent("calculated.criterion", {
                    bubbles: true,
                    detail: { points: sum }
                }));
            }
        })();
    </script>
</insite:PageFooterContent>