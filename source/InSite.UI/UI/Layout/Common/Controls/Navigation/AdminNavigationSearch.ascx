<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminNavigationSearch.ascx.cs" Inherits="InSite.UI.Layout.Common.Controls.Navigation.AdminNavigationSearch" %>

<div id="admin-navigation-search" class="admin-navigation-search navbar-nav-left d-none d-lg-block">
    <label class="search-control">
        <span class="visually-hidden">Search Shift iQ</span>
        <i class="fa-solid fa-search search-icon"></i>
        <input
            type="search"
            class="form-control insite-text"
            placeholder="Search Shift iQ"
            maxlength="50"
            autocomplete="off"
            autocorrect="off"
            autocapitalize="none"
            spellcheck="false"
            role="combobox"
            aria-autocomplete="list"
            aria-expanded="false"
            aria-controls="admin-navigation-search-results" />
    </label>
</div>

<script type="text/javascript">
    inSite.common.adminNavigationSearch.init(
        'admin-navigation-search',
        <%= System.Web.HttpUtility.JavaScriptStringEncode(GetAdminNavigationSearchUrl(), true) %>
    );
</script>
