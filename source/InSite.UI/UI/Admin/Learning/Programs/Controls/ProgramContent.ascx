<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramContent.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.ProgramContent" %>

<insite:Alert runat="server" ID="ScreenStatus" />

<div class="row">
    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <insite:Nav runat="server">

                    <insite:NavItem runat="server" Title="Program Content">

                        <div class="form-group mb-3">
                            <label class="form-label">Language</label>
                            <div>
                                <insite:ComboBox runat="server" ID="Language" AllowBlank="false" />
                            </div>
                        </div>

                        <div class="row row-translate">
                            <div class="col-md-12">
                                <asp:Repeater runat="server" ID="ContentRepeater">
                                    <ItemTemplate>
                                        <div class="form-group mb-3">
                                            <insite:DynamicControl runat="server" ID="Container" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                    </insite:NavItem>

                </insite:Nav>


                <div class="mt-5">
                    <insite:SaveButton runat="server" ID="ProgramContentSaveButton" />
                    <insite:CancelButton runat="server" ID="ProgramContentCancelButton" />
                </div>

            </div>
        </div>

    </div>
</div>
