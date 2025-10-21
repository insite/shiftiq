<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScormEventGrid.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.ScormEventGrid" %>

<h2 class="h4 mt-4 mb-3">SCORM Events</h2>

<div class="card border-0 shadow-lg">

    <div class="card-body">

        <insite:Grid runat="server" ID="Grid" DataKeyNames="EventIdentifier">
        
            <Columns>

                <asp:TemplateField HeaderText="Date/Time">
                    <ItemTemplate>
                        <%# LocalizeTime(Eval("EventWhen")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <%# Eval("Action") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Short Name">
                    <ItemTemplate>
                        <%# Eval("ShortName") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Target">
                    <ItemTemplate>
                        <%# Eval("Target") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="CMI Element">
                    <ItemTemplate>
                        <%# Eval("OtherCmiElement") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="CMI Value">
                    <ItemTemplate>
                        <%# Eval("OtherCmiValue") %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>

        </insite:Grid>

    </div>

</div>