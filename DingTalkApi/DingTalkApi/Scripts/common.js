/*========== 常量定义 ===========*/

// WCF服务器URL地址
var URL_WCF = "http://localhost:44082/";  // BIS2 WCF(本地测试)
// BIS2 WCF(服务器)
if (window.location.hostname != "localhost") {
    URL_WCF = window.location.protocol + "//" + window.location.host + "/WCF/";
}
// COOKIE有效时间
var COOKIE_TIME = 1 * 24 * 60 * 60 * 1000; // 1天
//////// COOKIE名：WCF SESSION
//////var COOKIE_WCFSESSION = "BIS2WcfSession";
//////// COOKIE名：切换用户
//////var COOKIE_NAME_CHANGEUSER = "BIS2CookieChanguser";

/*========== 全局变量 ===========*/

// 登陆后的用户信息
var LoginResult = {};
// 全局变量  弹出窗计数
var PopModalCount = 0;


//全局变量  控制子类型的页面默认选择条件    条件过滤
var condition_filter = false;


/*========== WCF ===========*/

// 调用WCF服务出错处理函数 
WcfDefaultError = function (xmlRequest, textMessage, errorThrown) {
    var msg = errorThrown;
    if (!msg && textMessage)
        msg = textMessage;
    if (!msg && xmlRequest)
        msg = xmlRequest.statusText;
    alert("调用WCF服务出错：" + msg + "\n" + GobalAjaxObject.CallingCount);
};

// 获取WCF URL
function WcfGetUrl(svc, method) {
    var url = URL_WCF;
    if (svc.substring(0, 1) === "/")
        svc = svc.substring(1);
    url += svc;
    if (url.substring(url.length - 1, url.length) !== "/")
        url += "/";
    url += method;

    return url;
}

// 调用WCF服务（POST方法）
// paramter: '{"s":{"Id":100,"Name":"HH","Mark":99,"Grade":"GG"}}'
/*
 svc:        [必须] SVC的URL(OA/Xxxxx.svc) 
 method:     [必须] 方法名 
 paramter:   [可选] 参数(Json)
 success:    [必须] 成功函数(function)
 element:    [可选] 调用元(画面对象)
 error:      [可选] 失败函数(function)
 sync:       [可选] 同步(True/False)     {默认:false 异步}
 get:        [可选] GET/POST(True/False) {默认:false POST}
*/
function CallWcf(svc, method, paramter, success, element, error, sync, get) {
    if (!svc || !method) {
        alert("调用WCF服务错误：缺少参数。");
    }

    CallAjax(WcfGetUrl(svc, method), paramter, success, element, error, null, sync, get);
}

/*========== Validform ===========*/
function ValidformTip(msg, o, cssctl) {
    //msg：提示信息;
    //o:{obj:*,type:*,curform:*}, obj指向的是当前验证的表单元素或表单对象（验证表单元素时o.obj为该表单元素，全部验证通过提交表单时o.obj为该表单对象），
    //type指示提示的状态，值为1、2、3、4， 1：正在检测/提交数据，2：通过验证，3：验证失败，4：提示ignore状态, curform为当前form对象;
    //cssctl:内置的提示信息样式控制函数，该函数需传入两个参数：显示提示信息的对象 和 当前提示的状态（既形参o中的type）;

    var SHOW_TIME = 1000; // 提示信息显示时间：1秒
    var FADEOUT_TIME = 200; // 提示信息显示时间：0.2秒

    if (!o.obj.is("form")) {
        var infoObj = $(o.obj).parent().find(".info");
        if (infoObj.length == 0) {
            infoObj = $('<div class=\"info\"><span class="Validform_checktip"/><span class="dec"><s class="dec1">&#9670;</s><s class="dec2">&#9670;</s></span></div>');
            $(o.obj).parent().append(infoObj);
            $(o.obj).parent().siblings(".info").remove();
        }

        var objtip = infoObj.find(".Validform_checktip");
        cssctl(objtip, o.type);
        objtip.text(msg);

        if (o.type == 2) {
            infoObj.fadeOut(FADEOUT_TIME);
        } else {
            var left = o.obj.offset().left,
                top = o.obj.offset().top;
            left += o.obj.width() / 3;
            top -= 45;

            if (top < 0)
                top = 0;
            else if (top > ($(window).height() - infoObj.height()))
                top = $(window).height() - infoObj.height();
            if (left < 0)
                left = 0;
            else if (left > ($(window).width() - infoObj.width()))
                left = $(window).width() - infoObj.width();

            infoObj.css({
                left: left,
                top: top
            }).show().animate({
                top: top + 10
            }, FADEOUT_TIME);

            //var infos = o.curform.find(".info").not(infoObj);
            var isOver = $("input").overlaps(infoObj);
            if (isOver.length) {
                infoObj.fadeOut(SHOW_TIME);
            } else {
                isOver = $(".info").overlaps(infoObj);
                if (isOver.length) {
                    infoObj.fadeOut(SHOW_TIME);
                }
            }
        }
    }

}

/*========== 操作cookie ===========*/

//写cookies 
function SetCookie(name, value, temp) {
    var exp = new Date();
    if (temp)
        exp.setTime(exp.getTime() + 10 * 1000);
    else
        exp.setTime(exp.getTime() + COOKIE_TIME);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

//读取cookies 
function GetCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))
        return unescape(arr[2]);
    else
        return null;
}

//删除cookies 
function DelCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = GetCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

/*========== 用户认证 ===========*/

// 用户认证
function Authenticate() {
    //ShowOverlayMessage();

    //////var result = GetCookie(COOKIE_WCFSESSION);

    //////// 从cookie获取用户信息
    //////if (result) {
    //////    // 登陆后的用户信息
    //////    var cookieResult = JSON.parse(result);
    //////    if (cookieResult) {
    //////        if (cookieResult.UserId) LoginResult.UserId = cookieResult.UserId;
    //////        if (cookieResult.Token) LoginResult.Token = cookieResult.Token;
    //////    }
    //////}
    // 如没有获取域用户(cookie)
    if (!LoginResult.UserId) {
        Login();
    } else {
        LoginResult.UserId = LoginResult.UserId;
        LoginResult.Password = null;
        LoginResult.Device = 1;
        //LoginResult.Token = LoginResult.Token;
        WcfLogin(LoginResult.Device, LoginResult.UserId, LoginResult.Password, LoginResult.Token);
    }

    //WcfLogin(LoginResult.Device, LoginResult.UserId, LoginResult.Password, LoginResult.Token); // Login内重复
}

/*8进制加密*/
function EightEncode(value) {
    if (!value)
        return;

    var monyer = new Array();
    var i, s;
    for (i = 0; i < value.length; i++)
        monyer += "$" + value.charCodeAt(i).toString(8);
    return monyer;
}
/*8进制解密*/
function EightDecode(value) {
    if (!value)
        return;

    var monyer = new Array();
    var i;
    var s = value.split("$");
    for (i = 1; i < s.length; i++)
        monyer += String.fromCharCode(parseInt(s[i], 8));
    return monyer;
}

// 用户登陆
function Login() {
    return;
    var url = "/ASHX/SYS/GetLogonUser.ashx";
    url = url + "?rand=" + Math.random();
    var UserId = GetUrlParam("u");     // UserID(八进制加密)
    var Password = GetUrlParam("p");   // Password(八进制加密)
    if (UserId) {
        url = url + "&UserId=" + (UserId);
    }
    if (Password) {
        url = url + "&Password=" + (Password);
    }

    CallAjax(url, null,
    //CallAjax(url, { "rand": Math.random() },
        function (result) {
            if (result) {
                if (typeof result.UserId !== "undefined") {
                    // Login
                    var userId = result.UserId;
                    var password = result.ImpersonatePwd;
                    var device = result.Device;

                    WcfLogin(device, userId, password);
                } else {
                    ShowOverlayMessage("当前用户不是有效的域用户。", true)
                }
            } else {
                ShowOverlayMessage("当前用户不是有效的域用户。", true)
            }
        }, $("body"), null, null, true);
}

// 用户登陆(WCF)
function WcfLogin(device, userId, password, token) {
    // Wcf Login
    CallWcf("SYS/Authentication.svc", "Login", { "p": { "UserId": userId, "Password": password, "Device": device, "Token": token } },
        function (result) {
            // 登陆后的用户信息
            LoginResult = result.d;

            if (!LoginResult) {
                ShowOverlayMessage("用户登陆失败。", true);
                return false;
            }

            if (LoginResult.TokenValid) {
                //////SetCookie(COOKIE_WCFSESSION, JSON.stringify(LoginResult));

                // 页面用户信息设定
                if (typeof Init === "function") {
                    // IT用
                    NoCopyAction();

                    Init();
                }

                //HideOverlayMessage();
                setTimeout(HideLoading, 500);
            } else {
                if (token) {
                    Login();
                } else {
                    var message = LoginResult.Message;
                    if (!message)
                        message = "域用户验证出错。";
                    HideLoading();
                    ShowOverlayMessage(message, true);
                }
            }
        }, null, null, true);
}

// 用户退出
function Logout() {
    //////DelCookie(COOKIE_WCFSESSION);
    //window.opener = null;
    //window.close();
    document.location.href = "about: blank";
}

// 屏幕锁定
function LockScreen() {
    var overlay = $("#_divLockScreenOverlay");
    var divLock = $("#_divLockScreenContext");;
    if (overlay.length == 0) {
        overlay = $("<div id='_divLockScreenOverlay' class='modal-backdrop' style='z-index:9999;opacity:0.85;filter:alpha(opacity=85);'></div>");
        var maxzindex = GetMaxZindex();
        if (maxzindex > 9999)
            overlay.css("z-index", maxzindex);

        var imgurl = LoginResult.OSPhotoUrl;
        if (imgurl == null || imgurl == "") imgurl = LoginResult.PhotoUrl;
        divLock = $(' \
            <div id="_divLockScreenContext" class="lock-screen modal hide fade in"> \
              <div class="modal-body"> \
	            <div class="page-body"> \
	            	<img id="_imgLockScreenPhoto" src="{lock_photo}" alt="宝原地产"> \
	            	<div> \
	            		<h3>{lock_name}</h3> \
	            		<div>锁定时间：{lock_time}</div> \
	            		<form class="form-search"> \
                          <div class="control-group"> \
                            <div class="controls"> \
                              <input id="_txtUnlockScreenPwd" type="password" placeholder="密码"> \
                              <button id="_btnUnlockScreen" type="button" class="btn btn-primary"><i class="m-icon-swapright m-icon-white"></i>解锁</button> \
                               <div>页面已锁定，请输入密码后点击【<em>解锁</em>】按钮进行解锁</div> \
                            </div> \
                          </div> \
	            		</form> \
	            	</div> \
	            </div> \
              </div> \
            </div> \
        '.replace('{lock_name}', LoginResult.UserName).replace('{lock_time}', (new Date()).toLocaleDateString() + " " + (new Date()).toLocaleTimeString()).replace("{lock_photo}", imgurl));
        divLock.css("z-index", maxzindex + 1);
        $("body").append(divLock);
        $("body").append(overlay);
        $("#_btnUnlockScreen").click(function () { var pwd = $('#_txtUnlockScreenPwd'); UnLock(pwd.val()); pwd.val(''); });
    }
    overlay.show();
    divLock.show();
    divLock.css("top", ($(window).height() - divLock.height()) / 2);
    divLock.css("position", "fixed");
    EnableUserRefresh();
}

// 用户解锁
function UnLock(password) {
    // Wcf Login
    CallWcf("SYS/Authentication.svc", "Login", { "p": { "UserId": LoginResult.UserId, "Password": password, "Device": LoginResult.Device } },
        function (result) {
            // 登陆后的用户信息
            result = result.d;

            if (result && result.Return) {
                $("#_divLockScreenContext").hide();
                $("#_divLockScreenOverlay").hide();

                EnableUserRefresh(true);
            } else {
                alert("解锁失败，请确认登录密码。");
                return false;
            }
        });
}

// 清除Session内的PageData
// key:要清除的PageData Key(尚未支持)
function ClearSessionPageData(key) {
    if (!LoginResult || !LoginResult.Token)
        return;

    // Wcf ClearSessionPageData
    CallWcf("SYS/Authentication.svc", "ClearSessionPageData", { "p": { "Token": LoginResult.Token } });
}

// 显示加载信息（禁用、遮蔽及提示信息）
function ShowOverlayMessage(message, error, element) {
    var masked = $("body");
    if (typeof element !== "undefined") {
        if (typeof element === "string") {
            masked = $("#" + element);
        } else {
            masked = $(element);
        }
    }

    var styleOverlay = {
        "color": "red",
        "background-color": "Gray",
        //"opacity": "0.6",
        "position": "absolute",
        "background": "url(/images/alpha.png)",
        //"filter": "alpha(opacity=60)", /* IE6 */
        //"-moz-opacity": "0.5", /* Mozilla */
        //"-khtml-opacity": "0.5", /* Safari */
        "overflow": "hidden",
        "font-size": "20px",
        "text-align": "center",
        "vertical-align": "middle",
        "left": "0px",
        "top": "0px",
        "width": "100%",
        "height": "100%",

        //"filter": "progid:DXImageTransform.Microsoft.AlphaImageLoader(src='Images/Loading.gif', sizingMethod='scale')",

        "z-index": 999
    }
    var styleOverlayDivSub = {
        "position": "absolute",
        "width": "100%",
        "min-height": "180px",  /* Loading Image Height */
        "left": "0px",
        "top": "40%"
    }
    var styleOverlayDivSubP = {
        "position": "relative",
        "width": "100%",
        "top": "-40%"
    }

    if (masked) {
        var overlay = $("#_divOverlayMessage");
        if (overlay.length == 0) {
            overlay = $("<div id='_divOverlayMessage'></div>");
            overlay.css(styleOverlay);

            var divSub = $("<div></div>");
            divSub.css(styleOverlayDivSub);

            //var img = $("<img src='Images/Loading.gif' style='filter:Alpha(Opacity=50);opacity:0.5;-moz-opacity:0.5;'/>");
            var img = $("<img src='/Images/Loading.gif' style='filter:chroma(color = white);'/>"); // IE OK
            if (error)
                img.attr("src", "/Images/Error.png");
            //var img = $("<img src='Images/Loading.gif' style=''/>");
            divSub.append(img);
            img.dblclick(function (e) {
                overlay.hide();
            });

            var divSubP = $("<p id='_divOverlayMessageSpan'></p>");
            divSubP.css(styleOverlayDivSubP);
            divSub.append(divSubP);

            overlay.append(divSub);
            masked.append(overlay);
        }

        if (typeof message === "undefined")
            message = "处理中,请稍候......";

        //overlay.text(message);
        $("#_divOverlayMessageSpan").html(message);
        overlay.show();

        // Disable Scroll Bar
        ShowOverlayMessage.overflow = masked.css("overflow");
        masked.css("overflow", "hidden");

        // z-index
        var maxzindex = GetMaxZindex();
        if (maxzindex > 999)
            overlay.css("z-index", maxzindex);
    }
}

// 关闭加载信息（禁用、遮蔽及提示信息）
function HideOverlayMessage() {
    var overlay = $("#_divOverlayMessage");
    if (overlay) {
        overlay.hide();

        // Reset Scroll Bar
        if (ShowOverlayMessage.overflow)
            overlay.parent().css("overflow", ShowOverlayMessage.overflow);
    }
}
var loadingInterval_Time;
// 显示加载信息
/*
 element:    [可选] 父元素
 notOverlay: [可选] 是否覆盖父元素(True/False) {默认:true 覆盖}
*/
function ShowLoading(element, notOverlay) {
    clearInterval(loadingInterval_Time);
    var parent = $("body");
    var css = { "position": "fixed" };
    if (typeof element !== "undefined") {
        if (typeof element === "string") {
            parent = $("#" + element);
            //var target = document.getElementById(element);
            //return new Spinner(opts).spin(target);
        } else {
            parent = $(element);
        }
        css = { "position": "absolute" };
    }

    if ($("#imgbyloading").length <= 0) {
        //var loadinghtml = "<img id=\"imgbyloading\" class=\"hide\" src=\"/Images/Loading/byloading1.png\"/>";
        var loadinghtml = "<div id=\"imgbyloading\" class=\"byloading\"><p>&nbsp;</p></div>"
        parent.append(loadinghtml);
    }
    var loading = $("#imgbyloading");
    loading.css(css);
    //loading.css("zIndex", parent.css("zIndex")+10);
    //loading.css("width", 80);
    //loading.css("height", 80);
    loading.show();
    var topHeight = ($(window).height() - loading.height()) / 2;
    var leftWidth = ($(window).width() - loading.width()) / 2;
    loading.css("top", topHeight);
    loading.css("left", leftWidth);
    //var index = 1;
    ////最大值
    //var Number = 12;
    //loadingInterval_Time = setInterval(function () {
    //    if (index < Number)
    //        index = index + 1;
    //    else
    //        index = 1;
    //    loading.attr("src", "/Images/Loading/byloading" + index + ".png");
    //}, 50);
    return parent;
    //if (typeof element !== "undefined") {
    //    if (typeof element === "string") {
    //        parent = $("#" + element);
    //        //var target = document.getElementById(element);
    //        //return new Spinner(opts).spin(target);
    //    } else {
    //        parent = $(element);
    //    }
    //}

    //$.fn.spin.presets.flower = {
    //      lines: 10
    //    , length: 20
    //    , width: 8
    //    , radius: 20
    //    , direction: -1
    //    //, image: '/Images/Loading.gif'
    //    , image: '/Images/Loading/byloading1.png'
    //}

    ////parent.spin('large', '#fff')
    //parent.spin('flower', '#808080');
    //return parent;

    //var parent = $("body");
    //if (typeof element !== "undefined") {
    //    if (typeof element === "string") {
    //        parent = $("#" + element);
    //    } else {
    //        parent = $(element);
    //    }
    //}

    //var overlay = true;
    //if (notOverlay)
    //    overlay = false;

    //var styleDivMask = {
    //    "position": "absolute",
    //    "background-color": "Gray",
    //    //"opacity": "0.8",
    //    "position": "absolute",
    //    "background": "url(/images/alpha60.png)",
    //    //"filter": "alpha(opacity=80)", /* IE6 */
    //    //"-moz-opacity": "0.8", /* Mozilla */
    //    //"-khtml-opacity": "0.8", /* Safari */
    //    "overflow": "hidden",
    //    "width": "100%",
    //    "height": "100%",
    //    "left": "0px",
    //    "top": "0px",
    //    "z-index": 998
    //}

    //var styleDivSub = {
    //    "position": "absolute",
    //    "z-index": 999,
    //    "background": "#ffffff",
    //    "border": "3px solid #ccc",
    //    "padding": "5px 10px 10px 10px",
    //}

    //var idMask = "_Mask";
    //if (parent[0])
    //    idMask = parent[0].uniqueID + "_Mask";
    //var divMask;
    //if (overlay) {
    //    if ($("#" + idMask).length == 0) {
    //        divMask = $("<div></div>");
    //        divMask.attr("id", idMask);
    //        divMask.css(styleDivMask);
    //    } else {
    //        divMask = $("#" + idMask);
    //    }
    //}

    //var idLoading = "_Loading";
    //if (parent[0])
    //    idLoading = parent[0].uniqueID + "_Loading";
    //var divLoading;
    //var idImage = "_Image";
    //if (parent[0])
    //    idImage = parent[0].uniqueID + "_Image";
    //var img = $("#" + idImage);
    //if ($("#" + idLoading).length == 0) {
    //    divLoading = $("<div></div>");
    //    divLoading.attr("id", idLoading);
    //    divLoading.css(styleDivSub);
    //    //var img = $("<img src='Images/Loading.gif' style='filter:chroma(color = white);'/>"); // IE OK
    //    var img = $("<img src='Images/Loading.gif'/>");
    //    divLoading.attr("id", idImage);
    //    divLoading.append(img);
    //} else {
    //    divLoading = $("#" + idLoading);
    //}

    ////if (typeof message !== "undefined")
    ////    divLoading.text(message);

    //// Center
    //var widthOver = parent.width();
    //var heightOver = parent.height();
    //var widthLoad = img.width();
    //var heightLoad = img.height();
    //if (widthLoad <= 0)
    //    widthLoad = 100;
    //if (heightLoad <= 0)
    //    heightLoad = 100;
    //var left = 0;
    //var top = 0;
    //if (widthLoad > widthOver) {
    //    widthLoad = widthOver;
    //    img.width(widthLoad);
    //}
    //if (heightLoad > heightOver) {
    //    heightLoad = heightOver;
    //    img.height(heightLoad);
    //}
    //left = (widthOver - widthLoad) / 2;
    //top = (heightOver - heightLoad) / 2;
    //divLoading.css("left", left);
    //divLoading.css("top",top);

    //var mainDiv = divLoading;
    //if (overlay) {
    //    divMask.append(divLoading);
    //    mainDiv = divMask;
    //}
    //parent.append(mainDiv);
    //mainDiv.show();

    //// Disable Scroll Bar
    ////mainDiv.overflow = parent.css("overflow");
    ////parent.css("overflow", "hidden");
    //DisableLoadingScroll(parent);

    //return mainDiv;
}

// 关闭加载信息（禁用、遮蔽及提示信息）
function HideLoading(divLoading) {
    //clearInterval(loadingInterval_Time);
    if ($("#imgbyloading").length > 0) {
        var loading = $("#imgbyloading");
        $("#imgbyloading").hide();
        $("#imgbyloading").remove();
    }

    if (typeof divLoading !== "undefined") {
        if (typeof divLoading === "string") {
            if (divLoading) {
                ResetLoadingScroll(divLoading.parent());
                divLoading.hide();
                divLoading.remove();
            }
        }
    }
    //var parent = $("body");
    //if (typeof divLoading !== "undefined") {
    //    if (typeof divLoading === "string") {
    //        parent = $("#" + divLoading);
    //    } else {
    //        parent = divLoading;
    //    }
    //}
    //parent.spin(false);
}
$(window).resize(function () {//当浏览器大小变化时
    if ($("#imgbyloading").length > 0) {
        var loading = $("#imgbyloading");
        loading.css("position", "fixed");
        //loading.css("width", 120);
        //loading.css("height", 120);
        loading.show();
        var topHeight = ($(window).height() - loading.height()) / 2;
        var leftWidth = ($(window).width() - loading.width()) / 2;
        loading.css("top", topHeight);
        loading.css("left", leftWidth);
    }
    //设置上班打卡界面
    if (IsExitsFunction("SetSignBoxPosition")) {
        SetSignBoxPosition();
    }
});




// 关闭滚动
function DisableLoadingScroll(element) {
    if (!element)
        return;

    var isFound = false;
    var oldLoading;
    if (GobalAjaxObject.Loading) {
        for (var i in GobalAjaxObject.Loading) {
            if (GobalAjaxObject.Loading.length > 0 && element.length > 0 && GobalAjaxObject.Loading[i][0].uniqueID == element[0].uniqueID) {
                isFound = true;
                oldLoading = GobalAjaxObject.Loading[i];
                break;
            }
        }
    } else {
        GobalAjaxObject.Loading = new Array();
    }
    if (isFound) {
        oldLoading.LoadingCount++;
    } else {
        element.LoadingCount = 1;
        element.LoadingOverflow = element.css("overflow");
        // Disable Scroll Bar
        element.css("overflow", "hidden");

        GobalAjaxObject.Loading.push(element);
    }
    return element;
}

// 恢复滚动
function ResetLoadingScroll(element) {
    if (!element)
        return;

    var index = -1;
    if (GobalAjaxObject.Loading) {
        for (var i in GobalAjaxObject.Loading) {
            if (GobalAjaxObject.Loading.length > 0 && element.length > 0 && GobalAjaxObject.Loading[i][0].uniqueID == element[0].uniqueID) {
                index = i;
                break;
            }
        }
    }
    if (index >= 0) {
        oldLoading = GobalAjaxObject.Loading[index];
        oldLoading.LoadingCount--;
        if (oldLoading.LoadingCount <= 0) {
            oldLoading.css("overflow", oldLoading.LoadingOverflow);
            GobalAjaxObject.Loading.splice(index);
        }
    }
}

/*========== 浏览器兼容性 ===========*/

function CheckBrowser() {
    if (typeof $.browser !== "undefined") {
        var userAgent = navigator.userAgent.toLowerCase();
        var os = function () { };
        // Windows
        if (userAgent.indexOf("windows") > 0)
            os.windows = true;
            // Android
        else if (userAgent.indexOf("android") > 0)
            os.android = true;
            // Mac PowerPC
        else if (userAgent.indexOf("Mac_PowerPC") > 0)
            os.mac = true;
            // IOS
        else if (userAgent.indexOf("Mac OS X") > 0)
            os.ios = true;

        // IE
        if ($.browser.msie) {
            if ($.browser.version < 8) {
                var msg = "请使用IE 8以上版本。";
                alert(msg);
                document.write(msg);
                return -1;
            } else {
                return 1;
            }
        }
            // Chrome
        else if ($.browser.chrome) {
            return 1;
        }
            // Firefox
        else if ($.browser.mozilla) {
            if (os.windows || os.mac || os.android)
                return 1;
        }
            // Safari
        else if ($.browser.safari) {
            if (os.windows) {
                var msg = "本系统不支持Windows下的Safari浏览器。";
                alert(msg);
                document.write(msg);
                return -1;
            }
            if (os.ios || os.mac)
                return 1;
        }
            // Opera
        else if ($.browser.opera) {
            if (os.windows || os.mac)
                return 1;
            if (os.ios || os.android) {
                var msg = "本系统不支持移动端的Opera浏览器。";
                alert(msg);
                document.write(msg);
                return -1;
            }
        }
    }

    return 0;
}

// 获取QUERYSTRING
function GetUrlParam(name) {
    var search = document.location.search;
    var pattern = new RegExp("[?&]" + name + "\=([^&]+)", "g");
    var matcher = pattern.exec(search);
    var items = null;
    if (null != matcher) {
        try {
            items = decodeURIComponent(decodeURIComponent(matcher[1]));
        } catch (e) {
            try {
                items = decodeURIComponent(matcher[1]);
            } catch (e) {
                items = matcher[1];
            }
        }
    }
    return items;
}

/*========== AJAX调用 ===========*/
var GobalAjaxObject = function () { };
GobalAjaxObject.CallingCount = 0;
GobalAjaxObject.Loading = new Array();
//GobalAjaxObject.Url = new Array();

$.ajaxSetup({
    cache: false //关闭AJAX相应的缓存
});
$(document).ajaxStart(function () {
    //ShowLoading();
    window.status = "数据请求中……";
});
$(document).ajaxStop(function () {
    //HideLoading();
    hideTopMsg();
    window.status = "完成";
    GobalAjaxObject.CallingCount = 0;
    //GobalAjaxObject.Url = new Array();
});
$(document).ajaxSend(function (event, request, settings) {
    try {
        //if (LoginResult.IsDebug)
        //    Logger.debug("ajaxSend URL:" + settings.url + " | Token:" + LoginResult.Token + " | User:" + LoginResult.UserId);

        //var isChecked = false;
        //for (var i = 0; i < GobalAjaxObject.Url.length; i++) {
        //    if (settings.url.indexOf(GobalAjaxObject.Url[i]) == 0) {
        //        isChecked = true;
        //        GobalAjaxObject.Url.splice(i, 1);
        //        break;
        //    }
        //}
        //if (!isChecked) {
        //    GobalAjaxObject.CallingCount--;
        //    request.abort();
        //    return false;
        //}

        GobalAjaxObject.CallingCount++;

        if (!LoginResult.IsDebug && GobalAjaxObject.CallingCount > 15) {
            // IE BUG 强制停止AJAX
            var msg = "太多的AJAX请求：" + GobalAjaxObject.CallingCount;
            //settings.error = null;
            hideTopMsg();
            window.status = msg;
            //request.abort();
            event.stopPropagation();
            //Logger.debug("AJAX COUNT:" + GobalAjaxObject.CallingCount);
            GobalAjaxObject.CallingCount--;
            document.location.replace("/portal.html");
            throw msg;
        }
    } catch (ex) {
    }
});
$(document).ajaxComplete(function (event, request, settings) {
    GobalAjaxObject.CallingCount--;
});

// 调用AJAX
/*
 url:        [必须] URL(http://...) 
 paramter:   [可选] 参数(Json)
 success:    [必须] 成功函数(function)
 element:    [可选] 调用元(画面对象)
 error:      [可选] 失败函数(function)
 beforeSend: [可选] 预处理函数(function) {默认:showTopMsg}
 sync:       [可选] 同步(True/False)     {默认:false 异步}
 get:        [可选] GET/POST(True/False) {默认:false POST}
*/
function CallAjax(url, paramter, success, element, error, beforeSend, sync, get) {
    //Logger.debug("AJAX CALL:" + url);

    if (!url) {
        alert("调用AJAX错误：缺少参数。");
    }

    if (!error)
        error = function (xmlRequest, textMessage, errorThrown) {
            var msg = errorThrown;
            if (!msg && textMessage)
                msg = textMessage;
            if (!msg && xmlRequest)
                msg = xmlRequest.statusText;
            if (LoginResult.IsDebug) {
                Logger.debug(xmlRequest.responseText);
            }
            //alert("调用出错(AJAX)：" + msg + "\n" + url);
            //alert("系统错误，请将错误信息反馈给信息技术部。\n\n错误信息：" + msg + "\n" + url.replace(URL_WCF, ""));
            alert("系统错误，请将错误信息反馈给信息技术部。\n\n错误信息：" + msg + "\n发生时间：" + DateTimeFormat(new Date(), "yyyy/MM/dd HH:mm:ss") + "\nURL：" + url);
        };

    if (!beforeSend)
        beforeSend = function (xhr) {
            showTopMsg("加载中,请稍侯...", 0, true)
            //divLoading = ShowLoading(element);
        };

    var async = !sync;
    var method = "POST";
    if (get) method = "GET";
    var data = "";
    if (paramter)
        data = JSON.stringify(paramter);

    var divLoading;
    //GobalAjaxObject.Url.push(url);
    var xhr = $.ajax({
        url: url,
        async: async,
        cache: false,
        type: method,
        contentType: "application/json; charset=utf-8",
        data: data,
        dataType: "json",
        crossDomain: true,
        beforeSend: beforeSend,
        complete: function (xhr, ts) {
            //HideLoading(divLoading);
            //showTopMsg("加载成功!", 0, false);
            hideTopMsg();
        },
        success: success,
        error: error
    });
}

/*========== AJAX等待处理 ===========*/

//显示置顶信息  jianglw01 2015/3/13添加
var topMsgTime;
var topOldClass = "top-msg-box hide";
var showTopMsgFlag = true;
//参数[content:显示的内容 ,  autoHideTime:自动消失的时间(毫秒) 不填数字为不自动消失 , showLoadImg:是否显示loading图片]
function showTopMsg(content, autoHideTime, showLoadImg) {
    if (!showTopMsgFlag)
        return;

    var html = "";
    if ($("#TopMsgBox").length <= 0) {
        html = "<div id=\"TopMsgBox\" class=\"" + topOldClass + "\">";
        html += "<img src=\"/Images/loading_mini.gif\"/>";
        html += "<span></span>";
        html += "</div>";
        $("body").append(html);
    }
    var obj = $("#TopMsgBox");
    //填充内容
    obj.children("span").html(content);

    if (showLoadImg)
        obj.children("img").show();
    else
        obj.children("img").hide();

    obj.css("left", $(window).width() / 2 - obj.width() / 2);
    //显示信息
    obj.slideDown(500);
    if (autoHideTime > 0) {
        topMsgTime = setTimeout(function () {
            hideTopMsg();
        }, autoHideTime);
    }
}

var hideLongTopTime;
//显示长条提示信息 content:显示的内容  bgcolor:显示的背景色 red blue orange green
function showLongTopMsg(content, bgcolor) {
    if (bgcolor == "" || bgcolor == null)
        bgcolor = "green";

    var html = "";
    if ($("#TopLongMsgBox").length <= 0) {
        html = "<div id=\"TopLongMsgBox\" class=\"top-msg-box top-all hide bgd-" + bgcolor + "\">";
        html += "<span></span>";
        html += "</div>";
        $("body").append(html);
    }
    var obj = $("#TopLongMsgBox");
    obj.removeClass("bgd-red bgd-green bgd-blue bgd-orarnge");
    obj.addClass("bgd-" + bgcolor);
    //填充内容
    obj.children("span").html(content);

    //显示信息
    obj.slideDown(500);
    hideLongTopTime = setTimeout(function () {
        clearInterval(hideLongTopTime);
        obj.slideUp(500);
    }, 3000);
}
var hideTopTime;
//隐藏置顶信息
function hideTopMsg() {
    clearInterval(topMsgTime);
    clearInterval(hideTopTime);
    var obj = $("#TopMsgBox");
    hideTopTime = setTimeout(function () {
        obj.slideUp(500);
    }, 2000);
}

/* 数据替换HTML指定格式 jianglw01 2015-3-10
// 参数 html: 数据绑定模板 例如<span>【0】</span>&nbsp;<span>【1】</span>&nbsp;<span>【0】</span>
//      Mark: 需替换的特殊符号 例如 【】
//      DataArray: 需替换的数据源数组 例如 var arr = new Array("11", "22");
// 示例:var tempHtml = $("#temphtml").html(); //获取数据模板html
//      var newhtml = "";
//      for (i = 0; i < 100; i++) {
//          var arr = new Array("11"+i, "22"+i);
//          newhtml = replaceHtmlData(tempHtml, "【】", arr);
//          $("#divhtml").html(newhtml + "<br>");
//      }
*/
function replaceHtmlData(html, Mark, DataArray) {
    if (!html)
        return "";
    if (DataArray.length > 0) {
        for (var m = 0; m < DataArray.length; m++) {
            eval("var re = /" + Mark.substring(0, 1) + m + Mark.substring(1) + "/g;");
            html = html.replace(re, DataArray[m]);
        }
    }
    return html;
}

//获取指定元素中的checkbox选中的val 返回数组 参数[element]元素选择器 如#id .class
function getCheckBoxValArr(element) {
    var arr = new Array();
    $.each($(element + " input:checkbox"), function (i, n) {
        if ($(n).prop("checked"))
            arr.push($(n).val())
    });
    return arr;
}

//表格复选框全选事件 [allCbx] 全选按钮对象,[tagElement] 目标元素下的checkbox
function allCheckBox_Click(allCbx, tagElement) {
    var checked = $(allCbx).prop("checked");
    $.each($(tagElement + " input:checkbox"), function (i, n) {
        $(n).prop("checked", checked);
    });
}

//获取URL参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

//初始化jquery 中文dataTable插件
function initDataTable(tableElement) {
    var dataTable = $(tableElement).dataTable({
        "pagingType": "full",
        "order": [],
        "language": {
            "lengthMenu": "每页 _MENU_ 条记录",
            "zeroRecords": "没有找到记录",
            "info": "第 _PAGE_ 页 ( 总共 _PAGES_ 页 )",
            "infoEmpty": "无记录",
            "infoFiltered": "(从 _MAX_ 条记录过滤)",
            "search": "查询",
            "paginate": {
                "first": "第一页",
                "last": "尾页",
                "next": "下一页",
                "previous": "上一页"
            },
        }
    });
}

//初始化日历控件 datetimepicker
function initDatePicker(ele) {
    if (!$().datetimepicker)
        return false;

    $(ele).datetimepicker({
        format: 'yyyy-mm-dd',   //日期格式
        autoclose: true,        //选择后自动关闭
        minView: 2,             //最小视图到月
        todayHighlight: true,   //高亮当前日期
        language: "zh-CN",      //语言
    });
}

//初始化日历控件 datetimepicker
function initDatePicker2(ele, option) {
    var defaults = {
        format: 'yyyy-mm-dd',   //日期格式
        autoclose: true,        //选择后自动关闭
        minView: 2,             //最小视图到月
        todayHighlight: true,   //高亮当前日期
        language: "zh-CN",      //语言
    }
    var options = $.extend(defaults, option);
    $(ele).datetimepicker(options);
}


/*========== 动态载入JS ===========*/

//function IncludeDebugJS(sId, fileUrl, source) {
//    $.getScript("debug.js",function(){
//        newFun('"Checking new script"');//这个函数是在new.js里面的，当点击click后运行这个函数
//    });

//    if ((source != null) && (!document.getElementById(sId))) {
//        var oHead = document.getElementsByTagName('HEAD').item(0);
//        var oScript = document.createElement("script");
//        oScript.language = "javascript";
//        oScript.type = "text/javascript";
//        oScript.id = sId;
//        oScript.defer = true;
//        oScript.text = source;
//        oHead.appendChild(oScript);
//    }
//}

// 加载页面
function LoadHtml(url, element, data, callback, modal) {
    if (!url)
        return false;

    var htmlContainer = $("#divPageContent");

    if (typeof (element) === "string")
        htmlContainer = $("#" + element);

    if (!htmlContainer)
        return false;

    var loading = ShowLoading(htmlContainer);

    if (typeof callback != "function") {
        callback = function (responseTxt, statusTxt, xhr) {
            HideLoading(loading);
            if (statusTxt == "error") {
                if (modal && IsExitsFunction("hideDialog"))
                    hideDialog();
                if (IsExitsFunction("ShowAlert"))
                    ShowAlert("加载失败：" + url);
                else
                    alert("加载失败：" + url);

            } else if (statusTxt == "success")
                // Do init(url)
                if (IsExitsFunction("MyInit"))
                    MyInit();
        };
    };

    //url = url + "?RANDOM=" + (new Date().getTime() + "_" + Math.random()); // 强制刷新
    //window.status = "加载中:" + url;
    //GobalAjaxObject.Url.push(url);
    $(htmlContainer).load(url, data, callback);

    // Page Title
    if ($("#page-title").length > 0 && MenuName.length > 0) {
        var helphtml = "";
        var Menu = CurentLeftMenu.MenuPath;
        var MenuPath = "<span>EDP";
        $(".helplink").remove();
        //var Guarantee = "<span>IT报修电话：60177999*7 <a href='mailto:by.ithelpdesk@bypro.com.cn'>&nbsp;<i class='icon-envelope'></i>&nbsp;IT报修邮件</a></span>";
        var Guarantee = "";//"<span>IT报修电话：60177999*7 <a href='javascript:ShowPopEmail()'>&nbsp;<i class='icon-envelope'></i>&nbsp;向IT部发送报修信息</a></span>";
        var Kaoqin = '<p style="color:green;">考勤打卡后请及时到“人事及考勤管理&gt;考勤管理&gt;<a href="javascript:MenuItemClick(\'个人考勤\');">个人考勤</a>”核对系统记录的考勤时间</p>';

        if (Menu != null) {
            for (i = 0; i < Menu.length; i++) {
                MenuPath += "<i class=\"icon-angle-right\"></i>" + Menu[i] + "";
                //if (Menu[i] == "房源管理") {
                //    if (url.match("Modules/HRM/") || url.match("ModulesV2/HRM/"))
                //        helphtml = "<a class='helplink' target=\"_blank\" href=\"javascript:MenuItemClick('房源帮助')\"><i class=\"icon-question-sign\"></i>&nbsp;房源帮助<span></span></a>";
                //} else if (Menu[i] == "客源管理") {
                //    if (url.match("Modules/CRM/") || url.match("ModulesV2/CRM/"))
                //        helphtml = "<a class='helplink' target=\"_blank\" href=\"javascript:MenuItemClick('客源帮助')\"><i class=\"icon-question-sign\"></i>&nbsp;客源帮助<span></span></a>";
                //}
            }

            MenuPath += "<i class=\"icon-angle-right\"></i>" + MenuName + "</span>";
            //$("#pageMenuPath").html(MenuPath);
            if (MenuName != "我的工作台")
                SetNavbarTool(MenuPath);
        }
        $("#page-title").html(MenuName + Guarantee + helphtml);

        // 帮助(请在各页面自己调用SetHelper)，这里只是为了兼容老版
        if (Menu == "房源管理" && (url.match("Modules/HRM/") || url.match("ModulesV2/HRM/"))) {
            SetHelper("房源帮助");
        } else if (Menu == "客源管理" && (url.match("Modules/CRM/") || url.match("ModulesV2/CRM/"))) {
            SetHelper("客源帮助");
        }
    }
}
var MailPopWinDiv;
function ShowPopEmail() {
    MailPopWinDiv = ShowPopModal("向IT部发送报修信息", "/Modules/SYS/SendRepairMail.html", 444);
}

// 设置页面上部帮助显示
function SetHelper(menu, url, newWindow) {
    var helper = $("#page-helper");
    if (helper.length == 0) {
        helper = $('<span id="page-helper" style="margin: 0px 0px 0px 0px"></span>');
        $("#page-title").append(helper);
    }
    if (!menu && !url) {
        helper.html("");
        return false;
    }

    // Check Name
    var menuname = "";
    var Menus = LoginResult.Menus;
    for (var i = 0; i < Menus.length; i++) {
        if (url) {
            if (Menus[i].ViewUrl == url) {
                menuname = Menus[i].MenuName;
                if (!menu)
                    menu = menuname;
                break;
            }
        } else {
            if (Menus[i].MenuName == menu) {
                menuname = Menus[i].MenuName;
                break;
            }
        }
    }

    if (!menuname)
        return false;
    if (!menu)
        menu = "帮助";

    var html = "";
    if (newWindow && url) {
        html = "<a class='helplink' target=\"_blank\" href=\"" + url + "'\">";
        html += "<i class=\"icon-question-sign\"></i>&nbsp;" + menu + "<span></span></a>";
    } else {
        html = "<a class='helplink' href=\"javascript:MenuItemClick('" + menu + "')\">";
        html += "<i class=\"icon-question-sign\"></i>&nbsp;" + menu + "<span></span></a>";
    }
    helper.html(html);
}

// Table隐藏列（）
function TableHideColumn(table, col) {
    if (!table)
        return null;
    var jqTable;
    if (typeof table === "string")
        jqTable = $("#" + table);
    else
        jqTable = $(table);
    if (jqTable.size() == 0)
        return;

    jqTable.find("tr th:nth-child(" + col + ")").hide();
    jqTable.find("tr td:nth-child(" + col + ")").hide();
}

// Table隐藏列表示（）
function TableShowColumn(table, col) {
    if (!table)
        return null;
    var jqTable;
    if (typeof table === "string")
        jqTable = $("#" + table);
    else
        jqTable = $(table);
    if (jqTable.size() == 0)
        return;

    jqTable.find("tr th:nth-child(" + col + ")").show();
    jqTable.find("tr td:nth-child(" + col + ")").show();
}

// Table加行（row：从0开始，-1最后一行）
function TableAddRow(table, row, trHtml) {
    if (!table)
        return null;
    var tbody;
    if (typeof table === "string")
        tbody = $("#" + table + " tbody");
    else
        tbody = table.find("tbody");
    if (tbody.size() == 0) {
        //alert("指定的table id或行数不存在！");
        return null;
    }

    var tr;
    if (typeof trHtml === "string")
        tr = $(trHtml);
    else
        tr = trHtml;

    var trPos;
    if (row < 0)
        trPos = tbody.find("tr:last");
    else
        trPos = tbody.find("tr").eq(row);

    if (trPos.size() == 0) {
        tbody.append(tr);
    } else {
        trPos.after(tr);
    }
    return tr;
}

// Table加行（row：从0开始，-1最后一行）
function TableAddRowValues(table, row, values) {
    if (!values || !Array.isArray(values) || values.length == 0)
        return null;

    var trHtml = "<tr>";
    for (var i = 0; i < values.length; i++) {
        if (!values[i] || values[i] == "null")
            trHtml += "<td>-</td>";
        else
            trHtml += "<td>" + values[i] + "</td>";
    }
    trHtml += "</tr>";

    return TableAddRow(table, row, trHtml);
}

//是否存在指定函数 
function IsExitsFunction(funcName) {
    try {
        if (typeof (eval(funcName)) == "function") {
            return true;
        }
    } catch (e) { }
    return false;
}
//是否存在指定变量 
function IsExitsVariable(variableName) {
    try {
        if (typeof (variableName) == "undefined") {
            return false;
        } else {
            return true;
        }
    } catch (e) { }
    return false;
}

// 获取ViewURL
// 例：GetJsViewList({ "WorkflowName": XXX_XXX })
function GetJsViewList(p) {
    var url = WcfGetUrl("SYS/BisUtility.svc", "GetJsViewList") + "?token=" + LoginResult.Token;
    if (p && p.WorkflowName)
        url += "&workflowName=" + p.WorkflowName;
    var viewList;
    CallAjax(encodeURI(url), null, function (result) {
        viewList = eval(result.d);
        return result;
    }, null, null, null, true, true);
    return viewList;
}

// 从数据库读取图片(get)
// 例：GetImageFromDB({ "Table": "T_K3EmpPhotoSnap", "Key": "UserCode", "Value": LoginResult.UserCd, "Image": "EmpPhoto" })
function GetImageFromDB(p) {
    var url = WcfGetUrl("SYS/BisUtility.svc", "GetImageFromTable") + "?token=" + LoginResult.Token;
    if (!p)
        return "Images/ShowBinaryImg.jpg";

    if (p.Table)
        url += "&tableName=" + p.Table;
    if (p.Key)
        url += "&keyName=" + p.Key;
    if (p.Value)
        url += "&keyValue=" + p.Value;
    if (p.Image)
        url += "&imageField=" + p.Image;
    if (p.SubSystem)
        url += "&subSystem=" + p.SubSystem;
    return encodeURI(url);
}

// 从获取图片URL(get)
// 更新后需刷新URL（url += "&_RANDOM=" + (new Date().getTime() + "_" + Math.random());）
// 例1[从IIS获取]：GetImageUrl({ "SubSystem": "HRM", "Value": "20150520114831c139ce", "Ext": ".jpg" })
// 例2[从DB获取 ]：GetImageUrl({ "SubSystem": "HRM", "Value": "20150520114831c139ce" })
// 例3[从DB获取 ]：GetImageUrl({ "Table": "T_K3EmpPhotoSnap", "Key": "UserCode", "Value": LoginResult.UserCd, "Image": "EmpPhoto" })
// 例4[从IIS获取]：GetImageUrl({ "SubSystem": "HRM", "Table": "T_FileHRM", "File": "20150520114831c139ce.jpg" })
// 例5[从DB获取 ]：GetImageUrl({ "SubSystem": "HRM", "Value": "20150520114831c139ce", "Thumbnail": true })
function GetImageUrl(p) {
    if (!p)
        return "";

    if (!p.Value)
        return "";

    if (!p.Ext) {
        p.Ext = ".jpg";
    }

    if (p.SubSystem || p.Table) {
        // 文件
        if (LoginResult.FileServerContent) {
            for (var i = 0; i < LoginResult.FileServerContent.length; i++) {
                if (LoginResult.FileServerContent[i] == p.SubSystem || LoginResult.FileServerContent[i] == p.Table) {
                    // 文件URL
                    var url = "";
                    if (p.SubSystem) {
                        url += "/" + p.SubSystem;
                    }
                    if (p.Table) {
                        url += "/" + p.Table;
                    } else {
                        url += "/T_File";
                        if (p.SubSystem) {
                            url += p.SubSystem;
                        }
                    }

                    // 预览图
                    if (p.Thumbnail) {
                        url += "/Thumbnail";
                    }

                    if (p.File) {
                        url += "/" + p.File;
                    } else {
                        if (p.Value) {
                            url += "/" + p.Value;
                        }
                        if (p.Ext) {
                            url += p.Ext;
                        }
                    }

                    return GetFullUrl(url);
                }
            }
        }
    }

    if (p.SubSystem && !p.Table) {
        p.Table = "T_File" + p.SubSystem;
    }
    if (!p.Key) {
        p.Key = "Id";
    }
    if (!p.Image) {
        p.Image = "Content";
    }
    return GetImageFromDB(p);
}

// 从获取图片URL(get)
// 更新后需刷新URL（url += "&_RANDOM=" + (new Date().getTime() + "_" + Math.random());）
function GetFullUrl(url) {
    if (!url)
        return "";
    if (url.indexOf("http://") == 0 || url.indexOf("https://") == 0)
        return url;
    if (!LoginResult.FileServerRootUrl)
        return url;
    //if (LoginResult.FileServerRootUrl.substring(0, URL_WCF.length) != URL_WCF) {
    //    Logger.debug("FileServerRootUrl:" + LoginResult.FileServerRootUrl);
    //    Logger.debug("URL_WCF:" + URL_WCF);
    //}

    var fullUrl = LoginResult.FileServerRootUrl;
    if (fullUrl.substr(fullUrl.length - 1, 1) == "/") {
        if (url.substr(0, 1) == "/") {
            fullUrl += url.substr(1, url.length - 1);
        } else {
            fullUrl += url;
        }
    } else {
        if (url.substr(0, 1) == "/") {
            fullUrl += url;
        } else {
            fullUrl += "/" + url;
        }
    }
    return encodeURI(fullUrl);
}

// 下载文件(get)
function DownloadFile(p) {
    var url = WcfGetUrl("SYS/BisUtility.svc", "DownloadFile") + "?token=" + LoginResult.Token;
    if (!p) {
        alert("下载文件失败，缺少参数");
        return false;
    }
    if (p.SubSystem)
        url += "&subSystem=" + p.SubSystem;
    if (p.Id)
        url += "&id=" + p.Id;
    if (p.Extension)
        url += "&extension=" + p.Extension;
    if (p.Table)
        url += "&tableName=" + p.Table;
    url = encodeURI(url);
    //window.location.href = url;
    var dn = window.open(url, "_blank");
}

// 预览文件(get)
// <a href="javascript:GetPreviewHtmlUrl('Medias/文档.docx')">文档</a>
function GetPreviewHtmlUrl(p) {
    if (!p) {
        alert("预览文件失败，缺少参数");
        return false;
    }

    ShowLoading();

    var fileUrl = p;
    if (p.substring(0, window.location.protocol.length) != window.location.protocol) {
        fileUrl = window.location.href;
        if (fileUrl.lastIndexOf("/") >= 0) {
            fileUrl = fileUrl.substr(0, fileUrl.lastIndexOf("/") + 1);
        }
        fileUrl += p;
    }

    var url = WcfGetUrl("SYS/BisUtility.svc", "GetPreviewHtmlUrl") + "?token=" + LoginResult.Token;
    if (fileUrl)
        url += "&file=" + fileUrl;
    url = encodeURI(url);

    CallAjax(url, null, function (result) {
        //viewList = eval(result.d);
        HideLoading();

        if (!result.d)
            showLongTopMsg("预览失败", "red");
        else
            window.open(result.d, "_blank");
    }, null, null, null, false, true);
}

// 获取系统时间(get)
// fmt:格式化时间 月(M)、日(d)、12小时(h)、24小时(H)、分(m)、秒(s)、周(E)、季度(q) 可以用 1-2 个占位符 * 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
// 例：GetSysDateTime("yyyy-MM-dd HH:mm:ss");
function GetSysDateTime(fmt) {
    var sysDateTime = new Date()
    return sysDateTime.toLocaleString();
    CallWcf("SYS/Authentication.svc", "GetSysDateTime" + "?_RANDOM=" + (new Date().getTime() + "_" + Math.random()), null,
        function (result) {
            if (result && result.d) {
                sysDateTime = result.d;
            }
        }, null, null, true, true);
    if (sysDateTime) {
        sysDateTime.replace(/Date\([\d+]+\)/, function (a) { eval('d = new ' + a) });
        if (fmt)
            return DateTimeFormat(d, fmt);
        else
            return d;
    } else {
        return null;
    }
}

// 格式化时间 月(M)、日(d)、12小时(h)、24小时(H)、分(m)、秒(s)、周(E)、季度(q) 可以用 1-2 个占位符 * 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
// "yyyy-MM-dd hh:mm:ss.S"==> 2006-07-02 08:09:04.423
// "yyyy-MM-dd EEE hh:mm:ss" ==> 2009-03-10 星期二 08:09:04
function JsonDateTimeFormat(json, fmt) {
    if (json) {
        json.replace(/Date\([\d+]+\)/, function (a) { eval('d = new ' + a) });
        if (fmt)
            return DateTimeFormat(d, fmt);
        else
            return d;
    } else {
        return null;
    }
}

// DateTime序列化为JSON
function DateTimeToJson(value) {
    if (value) {
        //return "/Date(" + value.getTime() + "+" + value.getTimezoneOffset() + ")/";
        return "/Date(" + value.getTime() + "" + value.getTimezoneOffset() + ")/";
    } else {
        return null;
    }
}

// string序列化为JSON
function DateTimeStringToJson(value) {
    if (value) {
        var dt = ParseDate(value);
        return DateTimeToJson(dt);
    } else {
        return null;
    }
}

// 格式化时间 月(M)、日(d)、12小时(h)、24小时(H)、分(m)、秒(s)、周(E)、季度(q) 可以用 1-2 个占位符 * 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
// "yyyy-MM-dd hh:mm:ss.S"==> 2006-07-02 08:09:04.423
// "yyyy-MM-dd EEE hh:mm:ss" ==> 2009-03-10 星期二 08:09:04
function DateTimeFormat(dt, fmt) {
    if (!dt)
        return null;

    var o = {
        "M+": dt.getMonth() + 1, //月份         
        "d+": dt.getDate(), //日         
        "h+": dt.getHours() % 12 == 0 ? 12 : dt.getHours() % 12, //小时         
        "H+": dt.getHours(), //小时         
        "m+": dt.getMinutes(), //分         
        "s+": dt.getSeconds(), //秒         
        "q+": Math.floor((dt.getMonth() + 3) / 3), //季度         
        "S": dt.getMilliseconds() //毫秒         
    };
    var week = {
        "0": "/u65e5",
        "1": "/u4e00",
        "2": "/u4e8c",
        "3": "/u4e09",
        "4": "/u56db",
        "5": "/u4e94",
        "6": "/u516d"
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (dt.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    if (/(E+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, ((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468") : "") + week[dt.getDay() + ""]);
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    }
    return fmt;
}

//获取二维Url
function GetQRCode(p) {
    var url = WcfGetUrl("FAM/AssetsManage.svc", "GetQRCode") + "?token=" + LoginResult.Token;
    if (p.Text)
        url += "&text=" + p.Text;
    if (p.Size)
        url += "&size=" + p.Size;
    if (p.NoShowBYLogo)
        url += "&noshowbylogo=" + p.NoShowBYLogo;
    if (p.LogoPath)
        url += "&logoPath=" + p.LogoPath;
    url = encodeURI(url);
    return url;
}

//转换时间 /Date(1431399720000+0800)/转时间字符串
function Data2String(str) {
    if (!str || str.length < 8)
        return str;
    var d = eval('new ' + str.substr(1, str.length - 2));
    var ar_date = [d.getFullYear(), d.getMonth() + 1, d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds()];
    for (var i = 0; i < ar_date.length; i++) ar_date[i] = dFormat(ar_date[i]);
    return ar_date.slice(0, 3).join('-') + ' ' + ar_date.slice(3).join(':');
    function dFormat(i) { return i < 10 ? '0' + i.toString() : i; }
}

// 弹出显示提示消息
function ShowTooltip(title, x, y, contents) {
    $('<div id="tooltip" class="chart-tooltip"><div class="date">' + title + '<\/div><div class="label label-success">CTR: ' + x / 10 + '%<\/div><div class="label label-important">Imp: ' + x * 12 + '<\/div><\/div>').css({
        position: 'absolute',
        display: 'none',
        top: y - 100,
        width: 75,
        left: x - 40,
        border: '0px solid #ccc',
        padding: '2px 6px',
        'background-color': '#fff',
    }).appendTo("body").fadeIn(200);
}

//设置Select选中项 没有则添加 
function SetSelectValue(id, value) {
    var isExist = false;
    var obj = $('#' + id);
    var count = obj.find('option').length;
    for (var i = 0; i < count; i++) {
        if (obj.get(0).options[i].value == value) {
            isExist = true;
            obj.get(0).selectedIndex = i;
            break;
        }
    }
    if (!isExist) {
        obj.prepend("<option value='" + value + "'>" + value + "</option>");
        obj.get(0).selectedIndex = 0;
    }
}

//设置Select选中项 没有则添加 
function SetSelectTextVal(id, text, value) {
    var isExist = false;
    var obj = $('#' + id);
    var count = obj.find('option').length;
    for (var i = 0; i < count; i++) {
        if (obj.get(0).options[i].value == value) {
            isExist = true;
            obj.get(0).selectedIndex = i;
            break;
        }
    }
    if (!isExist) {
        obj.prepend("<option value='" + value + "'>" + text + "</option>");
        obj.get(0).selectedIndex = 0;
    }
}

// 模态窗(唯一)
function showDialogPage(title, pageUrl, width) {
    var divModal = $('#modal_dialog');
    if (divModal.is(":visible"))
        return;

    if (divModal.length == 0) {
        // 模态框
        divModal = $(' \
	                <div id="modal_dialog" class="modal container hide fade" tabindex="-1"> \
		                <div class="modal-header" style="background-color:#FFF;"> \
			                <div class="caption"> \
				                <i class="icon-reorder"></i> \
				                <span id="modal_title" style="padding-left:10px"></span> \
				                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&nbsp;</button> \
			                </div> \
		                </div> \
		                <div id="modal_content" class="modal-body"> \
		                </div> \
	                </div> \
                ');
        $('body').append(divModal);

        // 关闭对话框时（清空）
        divModal.on('hidden', function () {
            $("#modal_content").html("");
            //divModal.css('top', '0px');
            //divModal.css('margin-top', '0px');
            //divModal.width(0);
            //divModal.height(0);
        })
        // 显示对话框时（根据内容宽度[被包含物需设固定宽度]重设对话框宽度及位置）
        divModal.on('shown', ajustDialogPage);
    }

    // create the backdrop and wait for next modal to be triggered
    $('body').modalmanager('loading');
    $("#modal_title").html(title);

    // 显示对话框
    setTimeout(function () {
        var regHtml = /.*\.html\??.*/;
        if (!regHtml.exec(pageUrl)) {
            $("#modal_content").html(pageUrl);

            var maxWidth = $(window).width() - 100;
            var maxHeight = $(window).height() - 100;
            var option = {
                keyboard: false,
                backdrop: 'static',
                //width: maxWidth + 'px',
                maxHeight: maxHeight + 'px'
            }
            if (width)
                option.width = width + 'px';

            divModal.modal(option);
            return;
        }

        LoadHtml(pageUrl, "modal_content", null, function (responseTxt, statusTxt, xhr) {
            //HideLoading();

            if (statusTxt == "error") {
                //hideDialog();
                ShowAlert("加载失败：" + pageUrl);
            } else if (statusTxt == "success") {
                var maxWidth = $(window).width() - 100;
                var maxHeight = $(window).height() - 100;
                var option = {
                    keyboard: false,
                    backdrop: 'static',
                    //width: maxWidth + 'px',
                    maxHeight: maxHeight + 'px'
                }
                if (width)
                    option.width = width + 'px';

                divModal.modal(option);
            }
        });
    }, 100);
}
// 调整模态窗
/*
function ajustDialogPage() {
    var maxWidth = 0;
    var divModal = $('#modal_dialog');
    var divContent = $("#modal_content");
    divContent.children().each(
        function () {
            if ($(this).context.nodeName != 'SCRIPT' && $(this).context.nodeName != 'STYLE' && maxWidth < $(this).width()) {
                maxWidth = $(this).width();
            }
        });
    if (maxWidth==0 && divContent.outerWidth())
        maxWidth = divContent.outerWidth();
    if (maxWidth > 0) {
        maxWidth = maxWidth + 30;

        if (divContent[0].clientHeight < divContent[0].scrollHeight)
            maxWidth = maxWidth + 15;

        divModal.animate({
            width: maxWidth + 'px'
          , marginLeft: -maxWidth / 2 + 'px'
        });

        // 上下位置调整(?)
        setTimeout(function () {
            //var t = Math.floor(($(window).height() - divModal.height()) / 2);
            //if (t < 0)
            //    t = 0;
            //divModal.css('top', '0px');
            //divModal.css('margin-top', t + 'px');
            $(window).resize();
            if (maxWidth > 0) {
                setTimeout(function () {
                    divModal.width(maxWidth);
                }, 500);
            }
        }, 500);
    }
}
*/
function ajustDialogPage() {
    // 等待加载延时
    setTimeout(DoAjustDialogPage, 1500);
}
function DoAjustDialogPage() {
    // 利用bootstrap设大小及位置
    $(window).resize();

    // 宽度调整（bootstrap不对的修补）
    var maxWidth = 0;
    var divModal = $('#modal_dialog');
    var divContent = $("#modal_content");
    divContent.children().each(
        function () {
            if ($(this).context.nodeName != 'SCRIPT' && $(this).context.nodeName != 'STYLE' && maxWidth < $(this).width()) {
                maxWidth = $(this).width();
            }
        });
    if (maxWidth == 0 && divContent.outerWidth())
        maxWidth = divContent.outerWidth();
    if (maxWidth > 0) {
        maxWidth = maxWidth + 50;

        if (divContent[0].clientHeight < divContent[0].scrollHeight)
            maxWidth = maxWidth + 15;

        divModal.animate({
            width: maxWidth + 'px'
          , marginLeft: -maxWidth / 2 + 'px'
        });
    }
}

// 关闭模态窗
function hideDialog() {
    //$('#modal_dialog').modal().hide();
    $('#modal_dialog').modal('hide');
}

// 警告框
function ShowAlert(msg, element) {
    var alertDialog = $(' \
                <div class="alert myAlert" style="position:absolute;min-width:200px;min-height:60px;"> \
                  <button type="button" class="close" data-dismiss="alert">&nbsp;</button> \
                  <br/><span class="label label-success">[%]</span> \
                </div> \
            '.replace('[%]', msg));

    element = $('body'); // 屏幕居中
    element.append(alertDialog);
    alertDialog.show();
    element = $(window); // 屏幕居中
    var top = (element.height() - alertDialog.height()) / 2;
    if (top < 0)
        top = 0;
    var left = (element.width() - alertDialog.width()) / 2;
    if (left < 0)
        left = 0;
    alertDialog.animate({ left: left + "px", top: top + "px" });
    setTimeout('$(".myAlert").hide()', 3000);
}

// 模态窗(层叠)
function PopModal(index) {
    this.id = index;
    this.idModal = "_POP_ID_" + index;
    this.idHeader = "_POP_HEADER_ID_" + index;
    this.idBody = "_POP_BODY_ID_" + index;

    return this;
}
PopModal.prototype = {
    hide: function () {
        $('#' + this.idModal).modal('hide');
    },
    getId: function () {
        alert(this.id);
    }
}
// 显示模态窗(层叠)
function ShowPopModal(title, pageUrl, width) {
    var popModal = new PopModal(++PopModalCount);

    var idModal = popModal.idModal;
    var idHeader = popModal.idHeader;
    var idBody = popModal.idBody;
    // 模态框
    var divHtml = ' \
                <div id="[POP_ID]" class="modal hide fade" tabindex="-1" data-focus-on="input:first"> \
                    <div class="modal-header" style="background-color:#FFF;"> \
			            <div class="caption"> \
				            <i class="icon-reorder"></i> \
				            <span id="[POP_HEADER_ID]" style="padding-left:10px"></span> \
				            <button type="button" class="close" data-dismiss="modal" aria-hidden="true" style="opacity:0.5;">&nbsp;</button> \
			            </div> \
                    </div> \
                    <div id="[POP_BODY_ID]" class="modal-body"> \
                    </div> \
                </div> \
            ';
    divModal = $(divHtml.replace("[POP_ID]", idModal).replace("[POP_HEADER_ID]", idHeader).replace("[POP_BODY_ID]", idBody));
    $('body').append(divModal);

    $("#" + idHeader).html(title);

    // 关闭对话框时（清空）
    divModal.on('hidden.bs.modal', function (e) {
        $("#" + idBody).html("");
        $("body").remove("#" + idBody);
    })
    // 显示对话框时（根据内容宽度[被包含物需设固定宽度]重设对话框宽度及位置）
    divModal.on('shown', function () {
        var maxWidth = 0;
        var divContent = $("#" + idBody);
        divContent.children().each(
            function () {
                if ($(this).context.nodeName != 'SCRIPT' && maxWidth < $(this).width())
                    maxWidth = $(this).width();
            });
        if (maxWidth > 0) {
            maxWidth = maxWidth + 30;
            divModal.animate({
                width: maxWidth + 'px'
              , marginLeft: -maxWidth / 2 + 'px'
            });
        }
    })

    // 显示对话框
    setTimeout(function () {
        var regHtml = /.*\.html\??.*/;
        if (!regHtml.exec(pageUrl)) {
            $("#" + idBody).html(pageUrl);

            var maxWidth = $(window).width() - 100;
            var maxHeight = $(window).height() - 100;
            var option = {
                keyboard: false,
                backdrop: 'static',
                //width: maxWidth + 'px',
                maxHeight: maxHeight + 'px'
            }
            if (width)
                option.width = width + 'px';

            divModal.modal(option);
            return;
        }

        LoadHtml(pageUrl, idBody, null, function (responseTxt, statusTxt, xhr) {
            //HideLoading();
            if (statusTxt == "error") {
                ShowAlert("加载失败：" + pageUrl);
            } else if (statusTxt == "success") {
                var maxWidth = $(window).width() - 100;
                var maxHeight = $(window).height() - 100;
                var option = {
                    keyboard: false,
                    backdrop: 'static',
                    //width: maxWidth + 'px',
                    maxHeight: maxHeight + 'px'
                }
                if (width)
                    option.width = width + 'px';

                divModal.modal(option);
            }
        });
    }, 100);

    function hide() {
        $('#' + idModal).modal('hide');
    };
    return popModal;
}

// 屏蔽选择与复制
function NoCopyAction() {
    try {
        // IT用
        if (LoginResult && (LoginResult.IsDebug || LoginResult.ItLevel > 0 || LoginResult.Area == "财务部" || LoginResult.SalaryDepartment == "非营业部")) {
            if ($.browser.msie) {
                document.oncontextmenu = null;
                document.onselectstart = null;
            } else {
                document.oncontextmenu = null;
                document.onselectstart = null;
            }
        } else {
            if ($.browser.msie) {
                document.oncontextmenu = function () { window.event.returnValue = false };
                document.onselectstart = function () { window.event.returnValue = false };
            } else {
                document.oncontextmenu = function (evt) { evt.preventDefault(); };
                document.onselectstart = function (evt) { evt.preventDefault(); };
            }
        }
    } catch (e) {

    }
}

/*========== 初期化 ===========*/

// JS跨域
jQuery.support.cors = true;

// 页面加载后初期处理
$(document).ready(
    function () {
        NoCopyAction();

        // 浏览器兼容性
        if (CheckBrowser() < 0)
            return false;

        // 表单提交（显示屏蔽）
        //$("form").submit(function (e) {
        //    ShowOverlayMessage();
        //});
        // 页面迁移、提交（显示屏蔽）
        $(window).unload(function () {
            ShowOverlayMessage();
        });

        // 用户验证
        ShowLoading();
        Authenticate();
        //加载返回顶部按钮
        BackToTop();
    }
);

//验证值是否符合正则表达式
//参数[Type:正则表达式名称,或自定义表达式 , element:需要验证的元素名称 验证失败返回空值]
function checkInputValue(Type, element) {
    var regExp;
    var value = $(element).val();
    switch (Type) {
        case "Number":
            regExp = /^[0-9]+$/;
            break;
        case "Money":
            regExp = /^[\+-]?\d+(\.\d{1,2})?$/;
            break;
        case "NotNum":
            regExp = /^([A-Za-z\u4E00-\u9FA5]+,?)+$/;//中文：/^([\u4E00-\u9FA5]+,?)+$/；   英文：/^[A-Za-z]+$/;
            break;
        default:
            regExp = Type;
    }
    if (!value.match(regExp)) {
        $(element).val("");
    }
}
//上传控件js
this.ajaxFileUpload = function (url, id, datatypes, callback) {
    $.ajaxFileUpload
      (
           {
               type: "post",
               url: url, //用于文件上传的服务器端请求地址
               secureuri: false, //是否需要安全协议，一般设置为false
               fileElementId: id, //文件上传域的ID
               dataType: datatypes, //返回值类型 一般设置为json
               success: function (data, status)  //服务器成功响应处理函数
               {
                   callback(data, status);
               },
               error: function (data, status, e)//服务器响应失败处理函数
               {
                   alert("error::" + e);
               }
           }
      )
}

//设置 按钮启用 false/禁用 true状态  
function SetBtnDisabled(flag, element) {
    if (typeof element !== "string") {
        element = "";
    }
    $.each($("" + element + " button," + element + " input[type='button']"), function (i, e) {
        if ($(e).prop("disabled") != flag)
            $(e).prop("disabled", flag);
    });
}

//禁止用F5键
function EnableUserRefresh(enable) {
    if (enable) {
        document.onkeydown = null;
        document.oncontextmenu = null;
    } else {
        //禁止用F5键
        document.onkeydown = function () {
            if (event.keyCode == 116) {    // F5
                event.keyCode = 0;
                event.cancelBubble = true;
                return false;
            } else if (event.keyCode == 13) { // Enter
                event.keyCode = 0;
                event.cancelBubble = true;
                return false;
            }
        }
        //禁止右键弹出菜单 
        document.oncontextmenu = function () {
            return false;
        }
    }
}

// 获取最大z-index
function GetMaxZindex() {
    var maxZ = Math.max.apply(null, $.map($('body > *'), function (e, n) {
        //if ($(e).css('position') == 'absolute')
        if (!$(e).hasClass('spinner')) // 忽略spinner
            return parseInt($(e).css('z-index')) || 1;
    }));
    return maxZ;
}

// 获取Form的所有Input转为Json对象
function FormToJson(formId) {
    var form = $("#" + formId);
    var serializeObj = {};
    var array = form.serializeArray();
    var str = form.serialize();
    $(array).each(function () {
        if (serializeObj[this.name]) {
            if ($.isArray(serializeObj[this.name])) {
                serializeObj[this.name].push(this.value);
            } else {
                serializeObj[this.name] = [serializeObj[this.name], this.value];
            }
        } else {
            serializeObj[this.name] = this.value;
        }
    });
    return serializeObj;
};

// 清除Input值
function ClearInput(tagId) {
    var file = document.getElementById(tagId);
    if (file) {
        if (file.outerHTML) {
            file.outerHTML = file.outerHTML;
        } else { // FF(包括3.5)
            file.value = "";
        }
    }
    //$('#'+tagId).html($('#'+tagId).html());
}

// 锁定DIV
function LockDiv(select, time) {
    if (!time)
        time = 1000;
    var el = $("body");
    if (select) {
        if (select instanceof jQuery) {
            el = select;
        } else {
            if (typeof select == "string") {
                el = $("#" + select);
            } else {
                el = $(select);
            }
        }
    }

    el.block({
        //message: '<img src="/Images/System/ajax-loading.gif" align="">',
        message: '<img src="/Images/loading_mini.gif" align="">',
        centerY: true,
        css: {
            top: '10%',
            border: 'none',
            padding: '2px',
            backgroundColor: 'none'
        },
        overlayCSS: {
            backgroundColor: '#000',
            opacity: 0.05,
            cursor: 'wait'
        }
    });

    if (time > 0) {
        window.setTimeout(function () {
            UnLockDiv(el);
        }, time);
    }
}

// 解锁DIV
function UnLockDiv(select) {
    var el = $("body");
    if (select) {
        if (select instanceof jQuery) {
            el = select;
        } else {
            if (typeof select == "string") {
                el = $("#" + select);
            } else {
                el = $(select);
            }
        }
    }
    el.unblock({
        onUnblock: function () {
            el.removeAttr("style");
        }
    });
}

//js日期字符串转换成日期类型
function ParseDate(dateStr) {
    return new Date(dateStr.replace("-", "/"));
}
//增加月  
function AddMonths(date, value) {
    date.setMonth(date.getMonth() + value);
    return date;
}
//增加天  
function AddDays(date, value) {
    date.setDate(date.getDate() + value);
    return date;
}
//增加时
function AddHours(date, value) {
    date.setHours(date.getHours() + value);
    return date;
}
//增加分
function AddMinutes(date, value) {
    date.setMinutes(date.getMinutes() + value);
    return date;
}
//增加秒
function AddSeconds(date, value) {
    date.setSeconds(date.getSeconds() + value);
    return date;
}

/* 
    系统管理用函数
    javascript:SystemRefresh('GeneralCode');       // 刷新系统参数表数据(T_GeneralCode数据变化后调用可即时刷新)
    javascript:SystemRefresh('Session:');          // 清除当前用户SESSION
    javascript:SystemRefresh('Session:all');       // 清除所有用户SESSION
    javascript:SystemRefresh('Session:xxxxxx');    // 清除用户SESSION
    javascript:SystemRefresh('UserInfo:');         // 清除/刷新当前用户信息(K3等信息变化后调用可即时刷新)
    javascript:SystemRefresh('UserInfo:all');      // 清除/刷新用户信息(K3等信息变化后调用可即时刷新)
    javascript:SystemRefresh('UserInfo:xxxxxx');   // 清除/刷新所有用户信息(K3等信息变化后调用可即时刷新)
    javascript:SystemRefresh('HourseArea:');       // 清除/刷新当前用户房源行政区及片区
    javascript:SystemRefresh('HourseArea:all');    // 清除/刷新所有用户房源行政区及片区
    javascript:SystemRefresh('HourseArea:xxxxxx'); // 清除/刷新用户房源行政区及片区
    javascript:SystemRefresh('Cache');             // 清除全部缓存(MemoryCache)
    javascript:SystemRefresh('Couchbase');         // 清除全部Couchbase缓存(拼音)
    javascript:ShowChangeUser();                   // 调出切换用户页面(调试用)
*/
// 系统数据刷新
function SystemRefresh(mode) {
    var url = "SystemRefresh" + "?token=" + LoginResult.Token;
    if (mode)
        url += "&mode=" + mode;
    CallWcf("SYS/BisUtility.svc", url, null,
        function (result) {
            alert(result.d);
        }, null, null, true, true);
}

// 清除我的缓存
function ClearMyCache() {
    var u = 'UserInfo:' + LoginResult.UserId;
    var s = 'Session:' + LoginResult.UserId;

    var meCleared = false;
    if (LoginResult.IsDebug) {
        SystemRefresh(u);
        SystemRefresh(s);
        meCleared = true;
        return;
    }

    var host = ["10.55.253.23", "10.55.253.21", "10.55.5.37", "10.55.5.113", "10.55.5.114", "10.55.5.115", "10.55.5.116", "10.55.5.117"];
    for (var i = 0; i < host.length; i++) {
        if (!meCleared && window.location.href.indexOf(host[i]) > 0) {
            SystemRefresh(u);
            SystemRefresh(s);
            meCleared = true;
            continue;
        }

        var HostToken = LoginResult.Token;
        var url = "http://" + host[i] + ":8013/WCF/SYS/BisUtility.svc/SystemRefresh?token=" + HostToken + "&mode=";
        //url = "http://localhost:19990/SYS/BisUtility.svc/SystemRefresh?token=" + HostToken + "&mode=";
        CallAjax(url + u, null,
            function (result) {
                alert(result.d);
            },
            null,
            function (result) {
                alert("清除用户信息失败");
            },
            null, null, true);
        CallAjax(url + s, null,
            function (result) {
                alert(result.d);
            },
            null,
            function (result) {
                alert("清除用户SESSION失败");
            },
            null, null, true);
    }
}

// 切换用户(WCF)
function ShowChangeUser() {
    //if (!LoginResult.IsDebug) {
    //    return false;
    //}

    $.getScript("/JS/debug.js", function () {
        if (LoginResult && LoginResult.UserId)
            if (!AllowChangeUser())
                return false;

        DoShowChangeUser();
    });
}

//返回顶部按钮事件
function BackToTop() {
    var obj = $(".back-to-top");
    if (obj.length <= 0) {
        var html = "<div class=\"back-to-top\" title=\"返回顶部\"><span class=\"icon-arrow-up\"></span></div>";
        $('body').append(html);
        obj = $(".back-to-top");
    }
    obj.hide();
    //当滚动条的位置处于距顶部300像素以下时，跳转链接出现，否则消失
    $(function () {
        $(window).scroll(function () {
            if ($(window).scrollTop() > 300) {
                obj.fadeIn(1000);
            }
            else {
                obj.fadeOut(1000);
            }
        });
        //当点击跳转链接后，回到页面顶部位置
        obj.click(function () {
            $('body,html').animate({ scrollTop: 0 }, 1000);
            return false;
        });
    });
}

//返回顶部
function BackToTopPage() {
    $('body,html').animate({ scrollTop: 0 }, 1000);
}


//输出错误信息 
//[ele]:元素id
//[Type]:ok、error、attention、stop、tips、notice、question、help
//[Msg]:显示的内容
function ShowErrorMsg(ele, Type, Msg) {
    var obj = $("#" + ele);
    obj.children("p").attr("class", Type);
    obj.children("p").html(Msg);
    obj.fadeIn(500);
}

// 构造检索过滤条件（id:条件DIV的id，json:后台返回条件数据)
// 例：[{name:"分类",field:"SysName",values:["房源管理","客源管理"]},{name:"流程名",field:"WorkflowName",values:["房源管理_钥匙","房源管理_钥匙"]}]
function CreateFilter(id, json) {
    if (!id || !json)
        return;
    var cond = $("#" + id);
    if (cond.length != 1)
        return;

    var idSelectField = id + '_SelectField';
    var idSelectValue = id + '_SelectValue';
    cond.children("#" + idSelectField).remove();
    cond.children("#" + idSelectValue).remove();
    //cond.empty();

    var filters = eval(json);


    if (filters && filters.length > 0) {
        var select = $('<select id="' + idSelectField + '" style="background-color:lightgray;width:120px;" placeholder="选择过滤种类" title="选择过滤种类"></select>');
        for (var i = 0; i < filters.length; i++) {
            var option = $('<option value="' + filters[i].field + '">' + filters[i].name + '</option>');
            select.append(option);
        }
        cond.prepend(select);

        var values = $('<input id="' + idSelectValue + '"></input>');
        select.after(values);

        var SetInput = function (sel) {
            for (var i = 0; i < filters.length; i++) {
                if (sel == filters[i].field) {
                    var inp = $("#" + idSelectValue);
                    if (filters[i].values.length > 20) {  // 最多显示N件
                        if (inp.length > 0 && inp[0].nodeName != 'INPUT') {
                            inp.remove();
                            inp = $('<input id="' + idSelectValue + '" placeholder="输入过滤内容" title="输入过滤内容"></input>');
                            select.after(inp);
                        }

                        inp.empty();

                        // AutoComplete
                        inp.typeahead({
                            minLength: 0,
                            source: filters[i].values
                        });
                        //inp.on('click', function (e) {
                        //    //inp.typeahead().show();
                        //});
                    } else {
                        if (inp.length > 0 && inp[0].nodeName != 'SELECT') {
                            inp.remove();
                            inp = $('<select id="' + idSelectValue + '" placeholder="选择过滤内容" title="选择过滤内容"></select>');
                            select.after(inp);
                        }
                        inp.empty();
                        inp.append('<option value=""></option>');
                        for (var j = 0; j < filters[i].values.length; j++) {
                            inp.append('<option value="' + filters[i].values[j] + '">' + filters[i].values[j] + '</option>');
                        }
                    }

                    break;
                }
            }
        };

        select.on('change', function (e) {
            SetInput($(this).val());
        });

        if (select.val())
            SetInput(select.val());
        // $("#SearchCondition_SelectValue").val("工商铺交易流程");
    }
}

// 根据TEXT设置SELECT值
function SetSelectText(id, text) {
    var select = $("#" + id);
    if (select.length == 0)
        return;
    var opts = select.get(0).options;
    for (var i = 0; i < opts.length; i++) {
        if (opts[i].text == text) {
            opts.selected = true;
            return opts[i].value;
        }
    }
}

// 日期时间检查  
// 格式为：YYYY-MM-DD HH:MM:SS  
function CheckDateTime(str) {
    var reg = /^(\d{4})-([0][1-9]|1[012]|[1-9])-([012][1-9]|[3][01]|[1-9]) ([01][0-9]|2[0-3]|[0-9]):([0-5][0-9]|[0-9]):([0-5][0-9]|[0-9])$/;
    var r = str.match(reg);
    if (r == null) return false;
    r[2] = r[2] - 1;
    var d = new Date(r[1], r[2], r[3], r[4], r[5], r[6]);
    if (d.getFullYear() != r[1]) return false;
    if (d.getMonth() != r[2]) return false;
    if (d.getDate() != r[3]) return false;
    if (d.getHours() != r[4]) return false;
    if (d.getMinutes() != r[5]) return false;
    if (d.getSeconds() != r[6]) return false;
    return true;
}

// 上传文件检查  
// <INPUT TYPE="file" NAME="file" onchange="CheckUploadFile(this,function(){alert('OK')},function(){})"> 
function CheckUploadFile(file, fnOk, fnError) {
    if (!file)
        return false;

    var AllowImageExt = ".jpg|.jpeg|.gif|.png|";
    var AllowFileSize = { min: 40, max: 10000 }; // 文件大小 k Byte
    var AllowImageSize = { min: [200, 100], max: [2000, 1000] }; // 图片大小 pix

    // 文件类型
    var src = file.value;
    var ext = (src.substr(src.lastIndexOf("."))).toLowerCase();
    var isImage = false;
    if (AllowImageExt && AllowImageExt.length > 0) {
        if (AllowImageExt.indexOf(ext + "|") >= 0) {
            isImage = true;
        } else {
            alert("只能上传以下类型的图片：\n  " + AllowImageExt);
            file.outerHTML = file.outerHTML; // CLEAR
            fnError();
            return false;
        }
    }

    // 文件大小
    if (AllowFileSize) {
        var size = -1;
        if (typeof FileReader !== "undefined") {
            size = file.files[0].size;
        } else {
            // IE (ActiveXObject)
            if ($.browser.msie) {
                try {
                    var fso, f, fname, fsize;
                    fso = new ActiveXObject("Scripting.FileSystemObject");
                    f = fso.GetFile(src);//文件的物理路径
                    size = f.Size;  //文件大小（bit）
                }
                catch (e) {
                    alert("请设置浏览器activex控件：\n" +
                        "在浏览器菜单栏上依次选择\n" +
                        "工具->internet选项->\"安全\"选项卡->自定义级别,\n" +
                        "打开\"安全设置\"对话框，把\"对没有标记为安全的\n" +
                        "ActiveX控件进行初始化和脚本运行\"，改为\"启动\"");
                    file.outerHTML = file.outerHTML; // CLEAR
                    fnError();
                    return false;
                }
            }
        }

        if (AllowFileSize.min && size < AllowFileSize.min * 1024 || AllowFileSize.max && size > AllowFileSize.max * 1024) {
            alert("只能上传以下大小的文件：\n  " + AllowFileSize.min + "-" + AllowFileSize.max + "k");
            file.outerHTML = file.outerHTML; // CLEAR
            fnError();
            return false;
        }
    }

    // 图片大小（像素）
    if (isImage && AllowImageSize) {
        var imgWidth = 0;
        var imgHeight = 0;
        var img = $("#_upload_image_temp");
        if (img.length == 0) {
            img = $("<img id='_upload_image_temp'></img>");
            img.hide();
            $("body").append(img);
        }

        if (file.files) { //HTML5，兼容chrome、火狐7+等  
            if (window.FileReader) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    img.attr("src", e.target.result);
                }
                reader.readAsDataURL(file.files[0]);
            } else {
            }
        } else {
            // IE 未支持
            //img.attr("src", fileObj.value);
            return;
        }

        var times = 0;
        var stop = false;
        var checkImageSize = function (image) {
            if (image) {
                if (AllowImageSize) {
                    var imgWidth = image.width();
                    var imgHeight = image.height();
                    var ret = true;
                    if (AllowImageSize.min) {
                        if (imgWidth < AllowImageSize.min[0]) {
                            ret = false;
                        } else if (imgHeight < AllowImageSize.min[1]) {
                            ret = false;
                        }
                    }
                    if (AllowImageSize.max) {
                        if (imgWidth > AllowImageSize.max[0]) {
                            ret = false;
                        } else if (imgHeight > AllowImageSize.max[1]) {
                            ret = false;
                        }
                    }
                    if (ret)
                        return true;
                    else {
                        alert("只能上传以下大小的图片：\n  " + AllowImageSize.min + "-" + AllowImageSize.max);
                        file.outerHTML = file.outerHTML; // CLEAR
                        return false;
                    }
                } else {
                    return true;
                }
            }
        };

        if (img[0].complete) {
            if (checkImageSize(img)) {
                fnOk();
            } else {
                fnError();
            }
        } else {
            img[0].onload = function () {
                img[0].onload = null;
                if (checkImageSize(img)) {
                    fnOk();
                } else {
                    fnError();
                }
            }
        }
    }
}

// 是否合法URL
function IsURL(str_url) {
    var strRegex = '^((https|http|ftp|rtsp|mms)?://)'
        + '?(([0-9a-z_!~*\'().&=+$%-]+: )?[0-9a-z_!~*\'().&=+$%-]+@)?' //ftp的user@ 
        + '(([0-9]{1,3}.){3}[0-9]{1,3}' // IP形式的URL- 199.194.52.184 
        + '|' // 允许IP和DOMAIN（域名） 
        + '([0-9a-z_!~*\'()-]+.)*' // 域名- www. 
        + '([0-9a-z][0-9a-z-]{0,61})?[0-9a-z].' // 二级域名 
        + '[a-z]{2,6})' // first level domain- .com or .museum 
        + '(:[0-9]{1,4})?' // 端口- :80 
        + '((/?)|' // a slash isn't required if there is no file name 
        + '(/[0-9a-z_!~*\'().;?:@&=+$,%#-]+)+/?)$';
    var re = new RegExp(strRegex);
    //re.test() 
    if (re.test(str_url)) {
        return (true);
    } else {
        return (false);
    }
}

//function LoadImage(url, callback) {
//    var img = new Image();
//    img.src = url;
//    if (img.complete) {
//        callback(img.width, img.height);
//    } else {
//        img.onload = function () {
//            callback(img.width, img.height);
//            img.onload = null;
//        };
//    };
//}

/*
function PreviewImage(fileObj, imgPreviewId, divPreviewId) {
    var allowExtention = ".jpg,.bmp,.gif,.png";//允许上传文件的后缀名document.getElementById("hfAllowPicSuffix").value;  
    var extention = fileObj.value.substring(fileObj.value.lastIndexOf(".") + 1).toLowerCase();
    var browserVersion = window.navigator.userAgent.toUpperCase();
    if (allowExtention.indexOf(extention) > -1) {
        if (fileObj.files) {//HTML5实现预览，兼容chrome、火狐7+等  
            if (window.FileReader) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    document.getElementById(imgPreviewId).setAttribute("src", e.target.result);
                }
                reader.readAsDataURL(fileObj.files[0]);
            } else if (browserVersion.indexOf("SAFARI") > -1) {
                alert("不支持Safari6.0以下浏览器的图片预览!");
            }
        } else if (browserVersion.indexOf("MSIE") > -1) {
            if (browserVersion.indexOf("MSIE 6") > -1) {//ie6  
                document.getElementById(imgPreviewId).setAttribute("src", fileObj.value);
            } else {//ie[7-9]  
                fileObj.select();
                if (browserVersion.indexOf("MSIE 9") > -1)
                    fileObj.blur();//不加上document.selection.createRange().text在ie9会拒绝访问  
                var newPreview = document.getElementById(divPreviewId + "New");
                if (newPreview == null) {
                    newPreview = document.createElement("div");
                    newPreview.setAttribute("id", divPreviewId + "New");
                    newPreview.style.width = document.getElementById(imgPreviewId).width + "px";
                    newPreview.style.height = document.getElementById(imgPreviewId).height + "px";
                    newPreview.style.border = "solid 1px #d2e2e2";
                }
                newPreview.style.filter = "progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod='scale',src='" + document.selection.createRange().text + "')";
                var tempDivPreview = document.getElementById(divPreviewId);
                tempDivPreview.parentNode.insertBefore(newPreview, tempDivPreview);
                tempDivPreview.style.display = "none";
            }
        } else if (browserVersion.indexOf("FIREFOX") > -1) {//firefox  
            var firefoxVersion = parseFloat(browserVersion.toLowerCase().match(/firefox\/([\d.]+)/)[1]);
            if (firefoxVersion < 7) {//firefox7以下版本  
                document.getElementById(imgPreviewId).setAttribute("src", fileObj.files[0].getAsDataURL());
            } else {//firefox7.0+                      
                document.getElementById(imgPreviewId).setAttribute("src", window.URL.createObjectURL(fileObj.files[0]));
            }
        } else {
            document.getElementById(imgPreviewId).setAttribute("src", fileObj.value);
        }
    } else {
        alert("仅支持" + allowExtention + "为后缀名的文件!");
        fileObj.value = "";//清空选中文件  
        if (browserVersion.indexOf("MSIE") > -1) {
            fileObj.select();
            document.selection.clear();
        }
        fileObj.outerHTML = fileObj.outerHTML;
    }
}
*/


//设置日期控件初始值当月或上月月初和月末时间
//[txtStartDate] 开始日期控件ID 
//[txtEndDate]   结束日期控件ID
//[Type]   0 赋值上月日期 1赋值当月日期
function SetSearchDate(txtStartDate, txtEndDate, Type) {
    //获取服务器时间
    var sysdate = GetSysDateTime("yyyy-MM-dd HH:mm:ss");
    var date = new Date(sysdate);
    date.setDate(1);
    if (Type == 0) {//上月
        date.setMonth(date.getMonth() - 1);
    }
    $("#" + txtStartDate).val(DateTimeFormat(date, "yyyy-MM-dd"));

    //设置月末时间
    var currentMonth = date.getMonth();
    var nextMonth = ++currentMonth;
    var nextMonthFirstDay = new Date(date.getFullYear(), nextMonth, 1);
    var oneDay = 1000 * 60 * 60 * 24;
    var endMonth = new Date(nextMonthFirstDay - oneDay);
    if (Type == 0)
        $("#" + txtEndDate).val(DateTimeFormat(endMonth, "yyyy-MM-dd"));
    else
        $("#" + txtEndDate).val(GetSysDateTime("yyyy-MM-dd"));
}

//日期时间相加
function DateAdd(SysDate, strInterval, Number) {
    var dtTmp = SysDate;
    switch (strInterval) {
        case 's': return new Date(Date.parse(dtTmp) + (1000 * Number));
        case 'n': return new Date(Date.parse(dtTmp) + (60000 * Number));
        case 'h': return new Date(Date.parse(dtTmp) + (3600000 * Number));
        case 'd': return new Date(Date.parse(dtTmp) + (86400000 * Number));
        case 'w': return new Date(Date.parse(dtTmp) + ((86400000 * 7) * Number));
        case 'q': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number * 3, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        case 'm': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        case 'y': return new Date((dtTmp.getFullYear() + Number), dtTmp.getMonth(), dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
    }
}

//获得过期时间 系统日期 + 小时(int)
//return date.getTime()
function GetExpiredTime(Hour) {
    var SysTime = GetSysDateTime("yyyy-MM-dd HH:mm:ss");
    SysTime = DateAdd(SysTime, "h", Hour);
    return SysTime.getTime();
}


//获得本地缓存数据 并且未过期
function GetLocalStorage(KeyName) {
    var obj = localStorage.getItem(KeyName);
    if (obj == null) return null;
    try {
        if (typeof (obj) == "string") {
            obj = JSON.parse(obj);
        } if (typeof (obj) != "object") {
            return null;
        }
        if (obj.ExpiredTime != null) {
            var ExpiredTime;
            if (typeof (obj.ExpiredTime) == "string")
                ExpiredTime = new Date(obj.ExpiredTime).getTime();
            else if (typeof (obj.ExpiredTime) == "number")
                ExpiredTime = obj.ExpiredTime;
            var SysTime = new Date(GetSysDateTime("yyyy-MM-dd HH:mm:ss"));
            var etime = ExpiredTime;
            var stime = SysTime.getTime();
            if (stime >= etime) return null;
            else return obj.List;
        }
    } catch (e) {
        return null;
    }
}

//保存数据(Json字符串 其中必须包含ExpiredTime过期日期的属性)至本地存储 并设置过期时间
function SetLocalStorage(KeyName, JsonObj, ExpiredTime) {
    var jsonStr = "";
    if (typeof (JsonObj) == "object") {
        var Json = {};
        Json.ExpiredTime = ExpiredTime;
        Json.List = JsonObj;
        jsonStr = JSON.stringify(Json);
    }
    try {
        localStorage.setItem(KeyName, jsonStr);
    } catch (ex) {

    }
}

//更改URL参数的值并跳转
//para_name 参数名称 para_value 参数值
function setUrlParam(para_name, para_value) {
    var strNewUrl = new String();
    var strUrl = new String();
    var url = new String();
    url = window.location.href;
    strUrl = window.location.href;
    //alert(strUrl);
    if (strUrl.indexOf("?") != -1) {
        strUrl = strUrl.substr(strUrl.indexOf("?") + 1);
        //alert(strUrl);
        if (strUrl.toLowerCase().indexOf(para_name.toLowerCase()) == -1) {
            strNewUrl = url + "&" + para_name + "=" + para_value;
            window.location = strNewUrl;
            //return strNewUrl;
        } else {
            var aParam = strUrl.split("&");
            //alert(aParam.length);
            for (var i = 0; i < aParam.length; i++) {
                if (aParam[i].substr(0, aParam[i].indexOf("=")).toLowerCase() == para_name.toLowerCase()) {
                    aParam[i] = aParam[i].substr(0, aParam[i].indexOf("=")) + "=" + para_value;
                }
            }

            strNewUrl = url.substr(0, url.indexOf("?") + 1) + aParam.join("&");
            //alert(strNewUrl);
            window.location = strNewUrl;
            //return strNewUrl;
        }

    } else {
        strUrl += "?" + para_name + "=" + para_value;
        //alert(strUrl);
        window.location = strUrl;
    }
}

//重新加载页面
function ResetPage() {
    //console.log(Exception);
    window.location.href = window.location.href;
}

function SetNavbarTool(html) {
    var obj = $(".navbar-tool");
    obj.html(html);
}

function Twinkle(ele, seconds) {
    if (!ele)
        return;

    var element = null;
    if (ele instanceof jQuery) {
        element = ele;
    } else {
        if (ele.substring(0, 1) == "." || ele.substring(0, 1) == "#") {
            element = $(ele);
        } else {
            element = $("#" + ele);
        }
    }

    if (element.length == 0)
        return;

    if (isNaN(seconds))
        seconds = 30 * 1000;

    // IE8
    if ($.browser.msie && $.browser.version <= 8) {
        element.each(function (i, e) {
            var d = $(e);
            d.width(d.width() * 1.5);
            d.height(d.height() * 1.5);
            var fs = d.css("font-size");
            if (fs) {
                var s = (new RegExp(/^(\d+)(.*)/)).exec(fs);
                if (s.length == 3) {
                    d.css("font-size", s[1] * 1.5 + s[2]);
                }
            }
            setTimeout(function () {
                d.width(d.width() / 1.5);
                d.height(d.height() / 1.5);
                d.css("font-size", fs);
            }, seconds);
        });
        return;
    }

    element.css({ "-webkit-animation": "twinkling 3s infinite ease-in-out" });
    setTimeout(function () { element.css("-webkit-animation", ""); }, seconds);
}

String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
}

function FullScreen() {
    var n = document.documentElement;
    $("body").hasClass("full-screen") ? ($("body").removeClass("full-screen"), $("#fullscreen-toggler").removeClass("active"), document.exitFullscreen ? document.exitFullscreen() : document.mozCancelFullScreen ? document.mozCancelFullScreen() : document.webkitExitFullscreen && document.webkitExitFullscreen()) : ($("body").addClass("full-screen"), $("#fullscreen-toggler").addClass("active"), n.requestFullscreen ? n.requestFullscreen() : n.mozRequestFullScreen ? n.mozRequestFullScreen() : n.webkitRequestFullscreen ? n.webkitRequestFullscreen() : n.msRequestFullscreen && n.msRequestFullscreen());
}

// K2（跨域）
//function K2CallWcf(method, paramter, success, error, sync) {
//    debugger
//    var url = "http://shbsv-k2demo.centaline.com.cn:81/K2Services/WCF.svc";

//    url += "/" + method;

//    CallAjax(url, paramter, success, null, error, null, sync);
//}
//function K2GetAllUsers(success, error, sync) {
//    debugger
//    if (!success)
//        success = function (result) {
//            debugger
//        };

//    K2CallWcf("GetAllUsers", null, success, error, sync);
//}
//function K2StartNewProcessInstance(svc, method, paramter, success, element, error, sync, get) {
//    K2CallWcf("StartNewProcessInstance", paramter, success, error, sync);
//}

;// 补回jQuery.browser
(function (jQuery) {
    if (jQuery.browser) return;

    jQuery.browser = {};
    jQuery.browser.mozilla = false;
    jQuery.browser.webkit = false;
    jQuery.browser.opera = false;
    jQuery.browser.msie = false;

    var nAgt = navigator.userAgent;
    jQuery.browser.name = navigator.appName;
    jQuery.browser.fullVersion = '' + parseFloat(navigator.appVersion);
    jQuery.browser.majorVersion = parseInt(navigator.appVersion, 10);
    var nameOffset, verOffset, ix;

    // In Opera, the true version is after "Opera" or after "Version" 
    if ((verOffset = nAgt.indexOf("Opera")) != -1) {
        jQuery.browser.opera = true;
        jQuery.browser.name = "Opera";
        jQuery.browser.fullVersion = nAgt.substring(verOffset + 6);
        if ((verOffset = nAgt.indexOf("Version")) != -1)
            jQuery.browser.fullVersion = nAgt.substring(verOffset + 8);
    }
        // In MSIE, the true version is after "MSIE" in userAgent 
    else if ((verOffset = nAgt.indexOf("MSIE")) != -1) {
        jQuery.browser.msie = true;
        jQuery.browser.name = "Microsoft Internet Explorer";
        jQuery.browser.fullVersion = nAgt.substring(verOffset + 5);
    }
        // In Chrome, the true version is after "Chrome" 
    else if ((verOffset = nAgt.indexOf("Chrome")) != -1) {
        jQuery.browser.webkit = true;
        jQuery.browser.name = "Chrome";
        jQuery.browser.fullVersion = nAgt.substring(verOffset + 7);
    }
        // In Safari, the true version is after "Safari" or after "Version" 
    else if ((verOffset = nAgt.indexOf("Safari")) != -1) {
        jQuery.browser.webkit = true;
        jQuery.browser.name = "Safari";
        jQuery.browser.fullVersion = nAgt.substring(verOffset + 7);
        if ((verOffset = nAgt.indexOf("Version")) != -1)
            jQuery.browser.fullVersion = nAgt.substring(verOffset + 8);
    }
        // In Firefox, the true version is after "Firefox" 
    else if ((verOffset = nAgt.indexOf("Firefox")) != -1) {
        jQuery.browser.mozilla = true;
        jQuery.browser.name = "Firefox";
        jQuery.browser.fullVersion = nAgt.substring(verOffset + 8);
    }
        // In most other browsers, "name/version" is at the end of userAgent 
    else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) <
    (verOffset = nAgt.lastIndexOf('/'))) {
        jQuery.browser.name = nAgt.substring(nameOffset, verOffset);
        jQuery.browser.fullVersion = nAgt.substring(verOffset + 1);
        if (jQuery.browser.name.toLowerCase() == jQuery.browser.name.toUpperCase()) {
            jQuery.browser.name = navigator.appName;
        }
    }
    // trim the fullVersion string at semicolon/space if present 
    if ((ix = jQuery.browser.fullVersion.indexOf(";")) != -1)
        jQuery.browser.fullVersion = jQuery.browser.fullVersion.substring(0, ix);
    if ((ix = jQuery.browser.fullVersion.indexOf(" ")) != -1)
        jQuery.browser.fullVersion = jQuery.browser.fullVersion.substring(0, ix);

    jQuery.browser.majorVersion = parseInt('' + jQuery.browser.fullVersion, 10);
    if (isNaN(jQuery.browser.majorVersion)) {
        jQuery.browser.fullVersion = '' + parseFloat(navigator.appVersion);
        jQuery.browser.majorVersion = parseInt(navigator.appVersion, 10);
    }
    jQuery.browser.version = jQuery.browser.majorVersion;
})(jQuery);