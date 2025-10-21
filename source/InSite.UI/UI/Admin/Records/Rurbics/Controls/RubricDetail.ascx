<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RubricDetail.ascx.cs" Inherits="InSite.UI.Admin.Records.Rurbics.Controls.RubricDetail" %>

<div class="form-group mb-3">
    <label class="form-label">
        Rubric Title
        <insite:RequiredValidator runat="server" ControlToValidate="RubricTitle" FieldName="Rubric Title" ValidationGroup="Rubric" />
    </label>
    <div>
        <insite:TextBox runat="server" ID="RubricTitle" MaxLength="100" />
    </div>
    <div class="form-text">A descriptive user-friendly title for the Rubric.</div>
</div>

<div class="form-group mb-3">
    <label class="form-label">
        Rubric Description
    </label>
    <div>
        <insite:TextBox runat="server" ID="RubricDescription" TextMode="MultiLine" Rows="5" MaxLength="800"/>
    </div>
    <div class="form-text">The rubric description.</div>
</div>

<div runat="server" id="RubricPointsField" class="form-group mb-3">
    <label class="form-label">
        Total Rubric Points
    </label>
    <div>
        <insite:NumericBox runat="server" ID="RubricPoints" DecimalPlaces="2" ReadOnly="true" />
    </div>
    <div class="form-text"></div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            window.addEventListener('calculated.criterion', (e) => {
                document.getElementById('<%= RubricPoints.ClientID %>').value = e.detail.points.toFixed(2);
            });
        })();
    </script>
</insite:PageFooterContent>
