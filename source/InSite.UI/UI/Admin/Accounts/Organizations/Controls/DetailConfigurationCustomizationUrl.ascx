<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailConfigurationCustomizationUrl.ascx.cs" Inherits="InSite.UI.Admin.Accounts.Organizations.Controls.DetailConfigurationCustomizationUrl" %>

<div class="row">
    <div class="col-md-6">

        <h3>Application</h3>
                            
        <div class="form-group mb-3">
            <label class="form-label">Support</label>
            <insite:TextBox runat="server" ID="SupportUrl" MaxLength="128" />
            <div class="form-text">URL for the Support Request form. This can also be a mailto link.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Contact Us</label>
            <insite:TextBox runat="server" ID="ContactUrl" MaxLength="128" />
            <div class="form-text">URL for the Contact Us form.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Logo</label>
            <insite:TextBox runat="server" ID="LogoUrl" MaxLength="100" />
            <div class="form-text">
                Image URL for the organization's company logo.
                <asp:HyperLink runat="server" ID="UploadLogo">Upload</asp:HyperLink>
            </div>
        </div>
                            
        <div class="form-group mb-3">
            <label class="form-label">Wallpaper</label>
            <insite:TextBox runat="server" ID="WallpaperUrl" MaxLength="128" />
            <div class="form-text">
                Image URL for the organization's default wallpaper.
                <asp:HyperLink runat="server" ID="UploadWallpaper">Upload</asp:HyperLink>
            </div>
        </div>

    </div>
    <div class="col-md-6">

        <h3>Social Media</h3>
                            
        <div class="form-group mb-3">
            <label class="form-label">Facebook</label>
            <insite:TextBox runat="server" ID="FacebookUrl" MaxLength="128" />
            <div class="form-text">Navigation URL for the organization's Facebook page.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Twitter</label>
            <insite:TextBox runat="server" ID="TwitterUrl" MaxLength="128" />
            <div class="form-text">Navigation URL for the organization's Twitter feed.</div>
        </div>
                            
        <div class="form-group mb-3">
            <label class="form-label">LinkedIn</label>
            <insite:TextBox runat="server" ID="LinkedInUrl" MaxLength="128" />
            <div class="form-text">Navigation URL for the organization's LinkedIn profile.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Instagram</label>
            <insite:TextBox runat="server" ID="InstagramUrl" MaxLength="128" />
            <div class="form-text">Navigation URL for the organization's Instragram page.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">YouTube</label>
            <insite:TextBox runat="server" ID="YouTubeUrl" MaxLength="128" />
            <div class="form-text">Navigation URL for the organization's YouTube channel.</div>
        </div>
                            
        <div class="form-group mb-3">
            <label class="form-label">Other</label>
            <insite:TextBox runat="server" ID="OtherUrl" MaxLength="128" />
            <div class="form-text">Navigation URL for some other social media home page.</div>
        </div>

    </div>
</div>
