<%@ Page Language="C#" CodeBehind="Colors.aspx.cs" Inherits="InSite.UI.Admin.Foundations.Colors" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        .main-grid {
            display: grid;
            grid-template-columns: 1fr 1fr 1fr;
            gap: 0.75rem;
        }

        .button-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 0.5rem;
        }
        .button-grid > div {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
        }

        .text-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 0.5rem;
        }
        .text-grid > div {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
        }
        .text-grid > div > div {
            padding: 0.5rem;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <div class="main-grid">

        <div class="card border-1 shadow">
            <div class="card-body">
                <h3>Alerts</h3>

                <div class="alert d-flex alert-danger mb-2" role="alert">
                    <div>Danger</div>
                </div>
                <div class="alert d-flex alert-info mb-2" role="alert">
                    <div>Info</div>
                </div>
                <div class="alert d-flex alert-success mb-2" role="alert">
                    <div>Success</div>
                </div>
                <div class="alert d-flex alert-warning" role="alert">
                    <div>Warning</div>
                </div>

            </div>
        </div>

        <div class="card border-1 shadow">
            <div class="card-body">
                <h3>Buttons</h3>

                <div class="button-grid">
                    <div>
                        <button type="button" class="btn btn-sm btn-default">
                            Default
                        </button>
                        <button type="button" class="btn btn-sm btn-primary">
                            Primary
                        </button>
                        <button type="button" class="btn btn-sm btn-success">
                            Success
                        </button>
                        <button type="button" class="btn btn-sm btn-info">
                            Info
                        </button>
                        <button type="button" class="btn btn-sm btn-warning">
                            Warning
                        </button>
                        <button type="button" class="btn btn-sm btn-danger">
                            Danger
                        </button>
                        <button type="button" class="btn btn-sm btn-secondary">
                            Secondary
                        </button>
                        <button type="button" class="btn btn-sm btn-light">
                            Light
                        </button>
                        <button type="button" class="btn btn-sm btn-dark">
                            Dark
                        </button>
                        <button type="button" class="btn btn-sm btn-link">
                            Link
                        </button>
                    </div>
                    <div>
                        <button type="button" class="btn btn-sm btn-outline-default">
                            Outline Default
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-primary">
                            Outline Primary
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-success">
                            Outline Success
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-info">
                            Outline Info
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-warning">
                            Outline Warning
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-danger">
                            Outline Danger
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-secondary">
                            Outline Secondary
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-light">
                            Outline Light
                        </button>
                        <button type="button" class="btn btn-sm btn-outline-dark">
                            Outline Dark
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div class="card border-1 shadow">
            <div class="card-body">
                <h3>Texts &amp; Backgrounds</h3>

                <div class="text-grid">
                    <div>
                        <div class="text-primary">
                            Text Primary
                        </div>
                        <div class="text-info">
                            Text Info
                        </div>
                        <div class="text-warning">
                            Text Warning
                        </div>
                        <div class="text-danger">
                            Text Danger
                        </div>
                        <div class="text-secondary">
                            Text Secondary
                        </div>
                        <div class="text-light bg-dark">
                            Text Light
                        </div>
                        <div class="text-dark">
                            Text Dark
                        </div>
                    </div>
                    <div>
                        <div class="text-light bg-primary">
                            Background Primary
                        </div>
                        <div class="text-light bg-info">
                            Background Info
                        </div>
                        <div class="text-light bg-warning">
                            Background Warning
                        </div>
                        <div class="text-light bg-danger">
                            Background Danger
                        </div>
                        <div class="text-dark bg-secondary">
                            Background Secondary
                        </div>
                        <div class="text-dark bg-light">
                            Background Light
                        </div>
                        <div class="text-light bg-dark">
                            Background Dark
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

</asp:Content>