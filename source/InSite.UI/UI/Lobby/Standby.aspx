<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" EnableSessionState="false" %>

<!DOCTYPE html>
<html>
<head>

    <script runat="server">
        protected void Page_Load(object sender, EventArgs e)
        {
            var encoded = Request.QueryString["description"];
            
            var description = encoded != null 
                ? Shift.Common.StringHelper.DecodeBase64Url(Request.QueryString["description"]) 
                : "The site is temporarily offline for routine maintenance.";

            Response.StatusCode = 200;
            
            ErrorHeading.InnerHtml = "Please Wait...";
            
            ErrorBody.InnerHtml = description;
        }
    </script>

    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <title>Please Wait</title>

    <link rel="stylesheet" href="/library/fonts/font-awesome/7.1.0/css/all.min.css">
    <insite:StyleLink runat="server" />

</head>
<body style="height:100vh;">
    <main class="page-wrapper d-flex flex-column">
        <div class="container d-flex flex-column justify-content-center pt-5 mt-n6" style="flex: 1 0 auto;">
            <div class="pt-7 pb-5">
                <div class="text-center mb-2 pb-4 ps-lg-5 pe-lg-5">
                    <img class="mb-5" title="Please Wait" src="/UI/Layout/Common/Parts/img/standby.svg" />
                    <div class="d-none mb-5" title="Error 400" style="user-select:none; font-size:14em; font-weight:900; color:#e9e9f2; line-height:0.8;">400</div>
                    <h2 runat="server" id="ErrorHeading"></h2>
                    <p runat="server" id="ErrorBody" class="pb-2"></p>
                    <p><a href="/" class="btn btn-primary"><i class="fas fa-home me-2"></i> Home</a></p>
                </div>
            </div>
        </div>
    </main>
</body>
</html>
