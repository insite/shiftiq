<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RubricCriteriaList.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.RubricCriteriaList" %>

<asp:Repeater runat="server" ID="CriteriaRepeater">
    <ItemTemplate>

        <div class="card mb-2">
            <div class="card-body">

                <table class="table table-striped rubric-criteria">
                    <thead>
                        <tr>
                            <th class="w-25">Title</th>
                            <th class="w-25 text-end pe-7">Points</th>
                            <th class="w-50">Description</th>
                        </tr>
                    </thead>

                    <tbody>
                        <tr>
                            <td>
                                <%# Eval("Title") %>
                            </td>
                            <td class="text-end pe-7">
                                <%# Eval("Points") %>
                            </td>
                            <td colspan="2">
                                <%# Eval("Description") %>
                            </td>
                        </tr>

                        <asp:Repeater runat="server" ID="RatingRepeater">
                            <ItemTemplate>
                                <tr>
                                    <td class="ps-5">
                                        <insite:RadioButton runat="server" ID="IsSelected" Text='<%# Eval("Title") %>' DisableTranslation="true" />

                                        <span class="d-none" data-points='<%# Eval("CurrentPoints") %>'></span>
                                    </td>
                                    <td class="text-end pe-7">
                                        <%# Eval("Points") %>
                                    </td>
                                    <td>
                                        <div class="row">
                                            <div class="col-lg-9">
                                                <%# Eval("Description") %>
                                            </div>
                                            <div class="col-lg-3">
                                                <insite:NumericBox runat="server" ID="AnswerPoints" NumericMode="Float" DecimalPlaces="2" />
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>

            </div>
        </div>

    </ItemTemplate>
</asp:Repeater>

<script>
    (() => {
        document.querySelectorAll(".rubric-criteria [id$=AnswerPoints]")
            .forEach(i => {
                i.addEventListener("change", () => calculateSum());
            });

        const rbs = document.querySelectorAll(".rubric-criteria [id$=IsSelected]");

        rbs.forEach(rb => {
            rb.addEventListener("change", e => {
                const input = e.target
                    .closest("tr")
                    .querySelector("[id$=AnswerPoints]");

                const allInputs = e.target
                    .closest("table")
                    .querySelectorAll("[id$=AnswerPoints]");

                allInputs.forEach(i => {
                    i.disabled = i !== input;
                    i.value = null;
                });

                if (input) {
                    input.focus();
                }

                calculateSum();
            });
        });

        function calculateSum() {
            let sum = 0;

            rbs.forEach(rb => {
                if (!rb.checked) {
                    return;
                }

                const tr = rb.closest("tr");
                const input = tr.querySelector("[id$=AnswerPoints]");

                if (input) {
                    const points = parseFloat(input.value);
                    if (!isNaN(points)) {
                        sum += points;
                    }
                } else {
                    const points = parseFloat(tr.querySelector("[data-points]").dataset.points);
                    sum += points;
                }
            });

            <%= UpdateSumFunc %>(sum);
        }
    })();
</script>