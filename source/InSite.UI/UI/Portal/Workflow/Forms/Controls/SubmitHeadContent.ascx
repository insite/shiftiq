<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmitHeadContent.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.SubmitHeadContent" %>

<style type="text/css">

    .widget .dropdown-item { padding-left: 0px !important; padding-bottom: 0px; white-space: normal; }

    .lesson { font-size: 20px; }

    .lesson h1 { font-size: 2rem !important; }
    .lesson h2 { font-size: 1.75rem !important; }
    .lesson h3 { font-size: 1.5rem !important; }
    .lesson h4 { font-size: 1.25rem !important; }
    .lesson h5 { font-size: 1.15rem !important; }

    .lesson h2.accordion-header .accordion-button { font-size: 1.5rem; color: #737491; }
    .lesson p, .lesson ul li  { line-height: 2rem; } 
    .lesson ul li p { padding-top: 0px; }
    .lesson ul { list-style: none; }
    .lesson ul li::before {
        position: absolute;
        content: "\2022";
        /*color: #F58426;*/
        font-size: 24px;
        display: inline-block; 
        width: 1em;
        margin-left: -1em;
    }

    a.glossary-term { text-decoration: underline double; }

    .lesson table { border: #F7F7FC solid 2px; margin-bottom: 20px; font-size: 1rem !important; }
    .lesson table tr th { border: #F7F7FC solid 2px; padding: 10px; font-weight: bold; background-color: #F7F7FC; }
    .lesson table tr td { border: #F7F7FC solid 2px; padding: 10px; }

    .lesson tbody tr:nth-child(even){ background-color: #fcfcfc; }

    .form-check-label { font-size: 20px; }

</style>
