<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ValidationDetail.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.ValidationDetail" %>

<div class="row">
    <div class="col-md-6">

        <div class="form-group mb-3">
            <label class="form-label">User</label>
            <div>
                <asp:Literal runat="server" ID="UserName" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Entry Number</label>
            <div>
                <asp:Literal runat="server" ID="EntryNumber" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Entry Created</label>
            <div>
                <asp:Literal runat="server" ID="EntryCreated" />
            </div>
        </div>

        <asp:Repeater runat="server" ID="FieldRepeater">
            <ItemTemplate>
                <div class="form-group mb-3">
                    <div class="float-end">
                        <insite:IconLink ID="ChangeButton" runat="server" Name="pencil" ToolTip="Change"
                            NavigateUrl='<%# string.Format("{0}?experience={1}&field={2}", ChangeUrl, ExperienceIdentifier, Eval("FieldType")) %>'
                        />
                    </div>
                    <label class="form-label">
                        <%# Eval("LabelText") %>
                    </label>
                    <div>
                        <insite:DynamicControl runat="server" ID="FieldValue" />
                    </div>
                    <div class="form-text">
                        <%# Eval("HelpText") %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</div>
