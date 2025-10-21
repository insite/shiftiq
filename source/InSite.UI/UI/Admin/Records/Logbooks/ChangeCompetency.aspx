<%@ Page Language="C#" CodeBehind="ChangeCompetency.aspx.cs" Inherits="InSite.Admin.Records.Logbooks.ChangeCompetency" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Competency" />

    <section class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Competency
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="CompetencyName" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        <insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/>
                    </label>
                    <div>
                        <insite:NumericBox runat="server" ID="CompetencyHours" DecimalPlaces="2" Width="100" />
                    </div>
                    <div class="mt-2">
                        <insite:CheckSwitch runat="server" ID="IncludeHoursToArea" Text="Roll-up Number of Hours to Area Level" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Number of Log Entries
                    </label>
                    <div>
                        <insite:NumericBox runat="server" ID="JournalItems" NumericMode="Integer" Width="100" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Skill Rating
                    </label>
                    <div>
                        <insite:ComboBox runat="server" ID="SkillRating" Width="100px">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="1" Text="1" />
                                <insite:ComboBoxOption Value="2" Text="2" />
                                <insite:ComboBoxOption Value="3" Text="3" />
                                <insite:ComboBoxOption Value="4" Text="4" />
                                <insite:ComboBoxOption Value="5" Text="5" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Competency" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
