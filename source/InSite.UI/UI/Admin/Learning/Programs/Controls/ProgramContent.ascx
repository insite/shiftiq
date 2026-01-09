<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramContent.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.ProgramContent" %>

<div class="row">
    <div class="col-md-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="float-end">
                    <insite:IconLink runat="server" ID="EditLink" CssClass="p-2" ToolTip="Modify content" Name="pencil" />
                </div>

                <insite:Nav runat="server">

                    <insite:NavItem runat="server" Title="Program Content">

                        <insite:UpdatePanel runat="server">
                            <ContentTemplate>

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
                                                    <label class="form-label"><%# Eval("Title") %></label>
                                                    <%# GetText((string)Eval("Label")) %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>

                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </insite:NavItem>

                </insite:Nav>

            </div>
        </div>

    </div>
</div>
