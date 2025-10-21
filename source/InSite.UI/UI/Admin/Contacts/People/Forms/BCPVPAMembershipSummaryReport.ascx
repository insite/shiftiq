<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BCPVPAMembershipSummaryReport.ascx.cs" Inherits="InSite.Admin.Contacts.People.Forms.BCPVPAMembershipSummaryReport" %>

<style type="text/css">
    * {
        -webkit-box-sizing: border-box;
        -moz-box-sizing: border-box;
        box-sizing: border-box;
    }

    body {
        font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
        font-size: 14px;
        line-height: 1.42857143;
        color: #333;
        background-color: #fff;
    }
    
    body {
        margin: 0;
    }
    
    h1 {
        font-size: 36px;
        margin-top: 20px;
        margin-bottom: 10px;
        font-family: inherit;
        font-weight: 500;
        line-height: 1.1;
        color: inherit;
    }

    .row {
        margin-right: -15px;
        margin-left: -15px;
    }

        .row::before {
            display: table;
            content: " ";
        }
        
        .row::after {
            clear: both;
        }
        
    .col-6 {
        width: 50%;
        float: left;
        position: relative;
        min-height: 1px;
        padding-right: 15px;
        padding-left: 15px;
    }
    
    .form-group {
      margin-bottom: 15px;
    }
    
    label {
        display: inline-block;
        max-width: 100%;
        margin-bottom: 5px;
        font-weight: bold;
    }

</style>

<div>
    <h1>BCPVPA Membership Summary</h1>
    
    <div class="row settings">

    <div class="col-6">

        <div class="form-group mb-3">
            <label class="form-label">Member Number</label>
            <div>
                <asp:Literal ID="BCPVPAMemberNumber" runat="server" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Full Name
            </label>
            <div>
                <asp:Literal runat="server" ID="BCPVPAHonorific" />
                <asp:Literal runat="server" ID="BCPVPAFullName" /> 
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Email/Login Name
            </label>
            <div>
                <asp:Literal ID="BCPVPAEmail" runat="server" />
                <span>&nbsp;</span>
                <asp:Literal runat="server" ID="BCPVPALoginName" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Member Address
            </label>
            <div>
                <asp:Literal ID="BCPVPAHomeAddress" runat="server" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Shipping Preference
            </label>
            <div>
                <asp:Literal runat="server" ID="BCPVPAShippingPreference" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Phone Numbers</label>
            <div>
                <asp:Literal ID="BCPVPAPhoneNumbers" runat="server" />
            </div>
        </div>

    </div>
                       
    <div class="col-6">

        <div class="form-group mb-3" runat="server" id="BCPVPASchool">
            <label class="form-label">School</label>
            <div>
                <asp:Literal runat="server" ID="BCPVPASchoolDisctrict" />
            </div>
            <div>
                <asp:Literal runat="server" ID="BCPVPASchoolName" />
            </div>
            <div>
                <asp:Literal runat="server" ID="BCPVPASchoolNumber" />
            </div>
            <div>
                <asp:Literal runat="server" ID="BCPVPASchoolAddress" />
            </div>
        </div>                    

        <div class="form-group mb-3">
            <label class="form-label">
                School Phone Numbers
            </label>
            <div>
                <asp:Literal runat="server" ID="BCPVPASchoolPhoneNumbers" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Position
            </label>
            <div>
                <asp:Literal runat="server" ID="BCPVPAPosition" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Membership Status
            </label>
            <div>
                <asp:Literal runat="server" ID="BCPVPAMembershipStatus" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Participations
            </label>
            <div>
                <asp:Repeater runat="server" ID="ParticipationsRepeater">
                    <ItemTemplate>
                        <p><%# Eval("Group.GroupName") %></p>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

    </div>

    </div>

</div>
