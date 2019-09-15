<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallBack.aspx.cs" Inherits="SportifyPod.CallBack" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript" src="Scripts/CommonJS.js"></script>

        <script type="text/javascript">
                window.onload = function() {               
               if (document.URL.indexOf('#') > 0)
                window.location.href = document.URL.replace('#', '?');
             }
        </script>
     
        <div>
            You are connecting with Spotify now, a moment please ...
        </div>
    </form>
</body>
</html>
