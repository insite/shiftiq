<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramCardRepeater.ascx.cs" Inherits="InSite.UI.Portal.Learning.Programs.Controls.ProgramCardRepeater" %>

<insite:PageHeadContent runat="server">
    <style>
        .submit-program-request {
            position: absolute;
            right: 0;
            top: 0;
        }

        .program-card {
            position: relative;
        }
        .program-card > .form-check {
            position: absolute;
            right: 1rem;
            top: 0.5rem;
            z-index: 4;
        }
        .program-card > div.card {
            cursor: pointer;
        }
    </style>
</insite:PageHeadContent>

<asp:HiddenField runat="server" ID="SubmittedProgramIds" />

<insite:SaveButton runat="server"
    ID="SubmitButton"
    CssClass="submit-program-request"
    Text="Submit Program Request"
/>

<div class="row">
    <asp:Repeater runat="server" ID="CardRepeater">
        <ItemTemplate>

            <div
                class="col-lg-4 col-sm-6 mb-4 program-card"
                data-id='<%# Eval("Identifier") %>'
                data-title='<%# Eval("Title") %>'
                data-description='<%# GetProgramDescription() %>'
            >
                <insite:CheckBox runat="server" />

                <div class="card card-hover card-tile border-0 shadow h-100">
                    <img runat="server"
                        visible='<%# Eval("Image") != null %>'
                        class="card-img-top"
                        src='<%# Eval("Image") %>'
                        alt='<%# Eval("Title") %>'
                    >

                    <div class="card-body text-center">
                        <asp:Literal runat="server" ID="Icon" />
                        <h3 class="h5 nav-heading mb-2">
                            <%# Eval("Title")  %>
                        </h3>
                    </div>
                </div>
            </div>

        </ItemTemplate>
    </asp:Repeater>
</div>

<insite:Modal runat="server" ID="DescriptionWindow" Title="Program Description" Width="600px">
    <ContentTemplate>
        <div class="mb-3">
            <strong runat="server" id="ProgramTitle">
            </strong>
        </div>

        <div runat="server" id="ProgramDescription" class="mb-3">
        </div>

        <insite:CancelButton runat="server" data-action="close" Text="Close" />
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            const modal = document.getElementById("<%= DescriptionWindow.ClientID %>");
            const programTitleElem = document.getElementById("<%= ProgramTitle.ClientID %>");
            const programDescriptionElem = document.getElementById("<%= ProgramDescription.ClientID %>");
            const submitButton = document.getElementById("<%= SubmitButton.ClientID %>");

            document.querySelectorAll(".program-card > .form-check input:checked").forEach(chk => chk.checked = false);

            enableSubmit();

            modal.querySelector("[data-action]").addEventListener("click", e => {
                e.preventDefault();

                modalManager.close($(modal));
            });

            document.querySelectorAll(".program-card > div.card").forEach(div => {
                div.addEventListener("click", () => {
                    programTitleElem.innerText = div.parentElement.dataset.title;
                    programDescriptionElem.innerHTML = div.parentElement.dataset.description;

                    modalManager.show($(modal));
                });
            });

            document.querySelectorAll(".program-card > .form-check input").forEach(chk => {
                chk.addEventListener("change", enableSubmit);
            });

            submitButton.addEventListener("click", e => {
                const ids = [];

                document.querySelectorAll(".program-card > .form-check input:checked").forEach(chk => {
                    ids.push(chk.closest(".program-card").dataset.id);
                });

                const text = ids.length === 1 ? "the program" : `${ids.length} programs`;

                if (confirm(`Are you sure to submit ${text}?`)) {
                    submitButton.classList.add("disabled");
                    document.getElementById("<%= SubmittedProgramIds.ClientID %>").value = ids.join(",");
                } else {
                    e.preventDefault();
                }
            });

            function enableSubmit() {
                const checked = !!document.querySelector(".program-card > .form-check input:checked");
                if (checked) {
                    submitButton.classList.remove("disabled");
                } else {
                    submitButton.classList.add("disabled");
                }

            }
        })();
    </script>
</insite:PageFooterContent>