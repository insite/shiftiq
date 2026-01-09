<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryDetail.ascx.cs" Inherits="InSite.Admin.Records.Items.Controls.CategoryDetail" %>

<%@ Register Src="AchievementPanel.ascx" TagName="AchievementPanel" TagPrefix="uc" %>
<%@ Register Src="StandardPanel.ascx" TagName="StandardPanel" TagPrefix="uc" %>

<div class="row">
                        
    <div class="col-md-6">
                                    
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="row">
                    <div class="col-lg-4">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Code
                                <insite:RequiredValidator runat="server" ControlToValidate="Code" FieldName="Code" ValidationGroup="Item" />
                                <insite:PatternValidator runat="server" ControlToValidate="Code" ValidationExpression="[a-zA-Z0-9\-\.]*" ErrorMessage="Only letters, digits, periods, and hyphens allowed for Code" ValidationGroup="Item" Display="Dynamic" />
                                <insite:CustomValidator runat="server" ID="CodeValidator" ControlToValidate="Code" ErrorMessage="Code must be unique" ValidationGroup="Item" Display="Dynamic" />
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="Code" MaxLength="100" />
                            </div>
                            <div class="form-text">The unique code for this category.</div>
                        </div>
                    </div>
                    <div class="col-lg-8">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Short Name
                            </label>
                            <div>
                                <insite:TextBox runat="server" ID="ShortName" MaxLength="100" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Name
                        <insite:RequiredValidator runat="server" ControlToValidate="Name" FieldName="Name" ValidationGroup="Item" />
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Name" MaxLength="400" />
                    </div>
                    <div class="form-text">The descriptive title for this category.</div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Parent Item
                    </label>
                    <div>
                        <insite:GradebookItemComboBox runat="server" ID="ParentItem" />
                    </div>
                    <div class="form-text">The category that the score item belongs to.</div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Include to report
                    </label>
                    <div>
                        <asp:RadioButtonList runat="server" ID="IncludeToReport" RepeatLayout="Table">
                            <asp:ListItem Text="Yes" Value="true" Selected="true" />
                            <asp:ListItem Text="No" Value="false" />
                        </asp:RadioButtonList>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Score calculation
                    </label>
                    <div>
                        <asp:RadioButtonList runat="server" ID="ScoreCalculation" RepeatLayout="Table">
                            <asp:ListItem Text="Equally weight all items" Value="Equally" Selected="true" />
                            <asp:ListItem Text="Equally weight all items and missing scores" Value="EquallyWithNulls" />
                            <asp:ListItem Text="Weight items by points" Value="Points" />
                            <asp:ListItem Text="Sum all items" Value="Sum" />
                        </asp:RadioButtonList>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Passing Score (%)
                    </label>
                    <div>
                        <insite:NumericBox runat="server" ID="PassPercent" NumericMode="Float" DecimalPlaces="1" MinValue="0" Width="80" style="text-align:right;" />
                    </div>
                </div>

            </div>

        </div>
    </div>

    <div class="col-md-6">
                                    
        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">

                <h3>Achievement Condition</h3>

                <uc:AchievementPanel runat="server" ID="AchievementPanel" />
            </div>
        </div>

        <div runat="server" id="StandardsCard" class="card border-0 shadow-lg">
            <div class="card-body">

                <h3>Standards</h3>
                <uc:StandardPanel runat="server" ID="StandardPanel" />

            </div>
        </div>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <h3>Integration</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Reference
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Reference" MaxLength="100" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Hook / Integration Code
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="Hook" MaxLength="100" />
                    </div>
                    <div class="form-text">
                        Unique code for integration with internal toolkits and external systems.
                    </div>
                </div>


            </div>
        </div>

    </div>

</div>
