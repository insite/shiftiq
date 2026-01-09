<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ErrorPageHeader.ascx.cs" Inherits="InSite.UI.Lobby.Controls.ErrorPageHeader" %>

<link rel="stylesheet" href="/library/fonts/font-awesome/7.1.0/css/all.min.css">
<insite:StyleLink runat="server" />

<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&amp;display=swap" rel="stylesheet">

<style type="text/css">
    .page-loading {
        position: fixed;
        top: 0;
        right: 0;
        bottom: 0;
        left: 0;
        width: 100%;
        height: 100%;
        -webkit-transition: all .4s .2s ease-in-out;
        transition: all .4s .2s ease-in-out;
        background-color: #fff;
        opacity: 0;
        visibility: hidden;
        z-index: 9999;
    }

    [data-bs-theme="dark"] .page-loading {
        background-color: #121519;
    }

    .page-loading.active {
        opacity: 1;
        visibility: visible;
    }

    .page-loading-inner {
        position: absolute;
        top: 50%;
        left: 0;
        width: 100%;
        text-align: center;
        -webkit-transform: translateY(-50%);
        transform: translateY(-50%);
        -webkit-transition: opacity .2s ease-in-out;
        transition: opacity .2s ease-in-out;
        opacity: 0;
    }

    .page-loading.active > .page-loading-inner {
        opacity: 1;
    }

    .page-loading-inner > span {
        display: block;
        font-family: "Inter", sans-serif;
        font-size: 1rem;
        font-weight: normal;
        color: #6f788b;
    }

    [data-bs-theme="dark"] .page-loading-inner > span {
        color: #fff;
        opacity: .6;
    }

    .page-spinner {
        display: inline-block;
        width: 2.75rem;
        height: 2.75rem;
        margin-bottom: .75rem;
        vertical-align: text-bottom;
        background-color: #d7dde2;
        border-radius: 50%;
        opacity: 0;
        -webkit-animation: spinner .75s linear infinite;
        animation: spinner .75s linear infinite;
    }

    [data-bs-theme="dark"] .page-spinner {
        background-color: rgba(255,255,255,.25);
    }

    @-webkit-keyframes spinner {
        0% {
            -webkit-transform: scale(0);
            transform: scale(0);
        }

        50% {
            opacity: 1;
            -webkit-transform: none;
            transform: none;
        }
    }

    @keyframes spinner {
        0% {
            -webkit-transform: scale(0);
            transform: scale(0);
        }

        50% {
            opacity: 1;
            -webkit-transform: none;
            transform: none;
        }
    }

    #lady {
        will-change: opacity, transform;
        transition: all .7s ease-in-out;
        transform: translateY(150px);
        opacity: 0;
    }

        #lady.show {
            transform: none;
            opacity: 1;
        }

    #bubble, #question {
        will-change: opacity, transform;
        transition: all .4s cubic-bezier(.68, -.55, .265, 1.55);
        transform: scale(.8);
        opacity: 0;
    }

    #question {
        transform: scale(.8) rotate(20deg);
    }

        #bubble.show,
        #question.show {
            transform: scale(1) rotate(0);
            opacity: 1;
        }
</style>

<script type="text/javascript">
    (function () {
        window.onload = function () {
            const preloader = document.querySelector('.page-loading')
            if (preloader) preloader.classList.remove('active')
            setTimeout(function () {
                if (preloader) preloader.remove()
            }, 1500)
            setTimeout(function () {
                document.getElementById('lady').classList.add('show')
            }, 300)
            setTimeout(function () {
                document.getElementById('question').classList.add('show')
            }, 1000)
            setTimeout(function () {
                document.getElementById('bubble').classList.add('show')
            }, 1600)
        }
    })()
</script>