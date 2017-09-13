var activityVM = new Vue({
    el: '#wrapper',
    data: {
        registerMessage: [],   //个人登记信息
        currentStep: 0,    //当前步数
        currentType: "",     //当前处于哪一步骤
        previousType:""    //上一步处于什么步骤
        //points: {
        //    List: [],
        //    LastId: 0
        //}
    },
    methods: {
        init: function () {

            $.getJSON('/WX/Home/GetSystemConfig?Url=' + encodeURIComponent(location.href.split('#')[0]), {
                t: (+new Date())
            }, function (json) {
                wx.config({
                    debug: false,
                    appId: json.Result.appId,
                    timestamp: json.Result.timestamp,
                    nonceStr: json.Result.nonceStr,
                    signature: json.Result.signature,
                    jsApiList: [
                        'checkJsApi',
                        'onMenuShareTimeline',
                        'onMenuShareAppMessage',
                        'onMenuShareQQ',
                        'onMenuShareWeibo',
                        'onMenuShareQZone',
                        'hideMenuItems',
                        'showMenuItems',
                        'hideAllNonBaseMenuItem',
                        'showAllNonBaseMenuItem',
                        'translateVoice',
                        'startRecord',
                        'stopRecord',
                        'onVoiceRecordEnd',
                        'playVoice',
                        'onVoicePlayEnd',
                        'pauseVoice',
                        'stopVoice',
                        'uploadVoice',
                        'downloadVoice',
                        'chooseImage',
                        'previewImage',
                        'uploadImage',
                        'downloadImage',
                        'getNetworkType',
                        'openLocation',
                        'getLocation',
                        'hideOptionMenu',
                        'showOptionMenu',
                        'closeWindow',
                        'scanQRCode',
                        'chooseWXPay',
                        'getBrandWCPayRequest', //
                        'openProductSpecificView',
                        'addCard',
                        'chooseCard',
                        'openCard'
                    ]
                });

            });
        },

        // 显示摇到红包
        showRedPacket: function () {
            $('#tip_nomoney').hide();
            $('#tip_money').show();
            $('.reward_res').animate({ 'margin-top': '0px' });
            $('.activity_shake img').removeClass('shake_img');
        },

        // 隐藏摇到红包
        hideRedPacket: function () {
            $('.reward_res').animate({ 'margin-top': '-200%' });
            $('.activity_shake').removeClass('shake_tag');     //确定是否摇到红包
            setTimeout(function () {
                $('#tip_nomoney').hide();
                $('#tip_money').hide();
            }, 300);

            setTimeout(function () {  //如果20s内还没有摇到红包，直接给出红包
                if (!($('.activity_shake').hasClass('shake_tag'))) {
                    $('.activity_shake').addClass('top_bottom_img'); //“摇一摇图片：手”抖动
                    setTimeout(function () {
                        activityVM.showRedPacket();   //显示摇到红包
                        $('.activity_shake').addClass('shake_tag');     //确定是否摇到红包
                        $('.activity_shake').removeClass('top_bottom_img');

                    }, 2000);
                }
            }, 15000);

        },

        //显示没有中奖
        showEmptyPacket: function () {
            $('#tip_nomoney').show();
            $('.reward_res').animate({ 'margin-top': '0px' });
        },

        //隐藏没有中奖
        hideEmptyPacket: function () {
            $('.reward_res').animate({ 'margin-top': '-200%' });
            setTimeout(function () {
                $('#tip_nomoney').hide();
            }, 300)
        },

        //显示活动分享提示
        showActivityShare: function () {
            $('#activityShare').show();
        },

        //隐藏活动分享提示
        hideActivityShare: function () {
            $('#activityShare').hide();
        },

        //点击“领取红包”
        writeMessage: function (id) {
            page.post("/WX/Home/Shaked", {
                id: id
            }, function (result) {
                if (result.Type === 'OpenRegister') {
                    activityVM.registerMessage = eval(result.Value); //把字符串转换成对象
                    activityVM.currentStep = result.NextStep;
                    activityVM.receiveEmotion(id);     //获取emotion值(心情API)
                    $('.info').show();   //显示上传头像页面
                } else if (result.Type === 'ShowPic') {
                    activityVM.currentStep = result.NextStep;
                    $('#picContainer img').attr("src", result.Value);
                    $('.info_promt').text("（注：上传的图片与上张图片相似度越高，领取的红包越多哟！）");  //领红包提示信息
                    $('.showPic_info').show();   //显示 后台上传的“对比图片”
                }
                activityVM.currentType = result.Type;
                activityVM.previousType = result.Type;
                $('.redPackageActivity').hide();
            });
        },

        //获取emotion值
        receiveEmotion: function (id) {
            page.post("/WX/Home/GetUserInput", {
                id: id
            }, function (result) {

                var inputEmotions = JSON.parse(result.Emotions);
                if (inputEmotions.length > 0) {
                    console.log(inputEmotions);
                    var text = inputEmotions[0].split(' > ')[1];//取所有的心情api中的第一个
                    //$('.info_promt').text("（注：脸部表情越‘" + text + "’，领取的红包越多噢！）");  //领红包提示信息
                    $('.upload_promt').text("‘"+ text +"’");
                    $('.info_promt').text("（注：脸部表情越‘" + text + "’，分数越高噢！）");  //领红包提示信息
                    $('.info').show();
                    $('.redPackageActivity').hide();
                }
            });
        },

        //点击“下一步”，显示上传图片页面
        //nextStep: function (activityId) {
        //    activityVM.submitPerMsg(activityId);   //下一步
        //},

        //上传个人照片
        activity_choosePersonalImg: function () {
            wx.chooseImage({
                count: 1, // 默认9
                sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
                sourceType: ['camera'], // 可以指定来源是相册还是相机，默认二者都有   ['album', 'camera']
                success: function (res) {
                    var localIds = res.localIds;
                    var localId = localIds[0];
                    if (localId.indexOf("wxlocalresource") !== -1) {
                        localId = localId.replace("wxlocalresource", "wxLocalResource");
                    }
                    //上传图片接口
                    wx.uploadImage({
                        localId: localId, // 需要上传的图片的本地ID，由chooseImage接口获得
                        isShowProgressTips: 1, // 默认为1，显示进度提示
                        success: function (res) {
                            var serverId = res.serverId; // 返回图片的服务器端ID
                            try {
                                //后台下载图片
                                page.post('/WX/RedPackageActivity/SaveRedPackageRegisterImg', {  //更换路径
                                    serverId: serverId
                                }, function (result) {
                                    $('#img_personalImgUrl').attr('src', result.Url);
                                    //alert('上传成功，正在载入预览图片，请稍等。');
                                });
                            } catch (e) {
                                alert('网络繁忙，请稍后再试！(2)');
                            }
                        }
                    });
                }
            });
        },
        //手机号只能输入数字
        telNumber: function () {
            $("input").keyup(function () { //keyup事件处理 
                $(this).val($(this).val().replace(/\D|^0/g, ''));
            });
        },

        //下一步
        nextStep: function (id) {
            page.$loadingToast.show();
            
            if (activityVM.currentType == 'OpenRegister') {
                $.each(activityVM.registerMessage, function (i, item) {
                    if (item.Type == "Pic") {
                        item.Value = "/Areas/WX/Content/images/img/img.jpg";  
                    }

                });

                var image = "/Areas/WX/Content/images/img/img.jpg";
                var name = $('#info_name').val();
                var tel = $('#info_phone').val();
                var phonefilter = /^(1[3/5/7/8/9]\d{9})$/;
                if (image == "/Areas/WX/Content/images/per_img.png") {
                    page.show_confirm("提示", "请上传图片！");
                    page.$loadingToast.hide();
                    return;
                }
                if (name == "") {
                    page.show_confirm("提示", "请输入真实姓名！");
                    page.$loadingToast.hide();
                    return;
                }
                if (tel == "") {
                    page.show_confirm("提示", "请输入手机号！");
                    page.$loadingToast.hide();
                    return;
                }
                if (tel) { 
                    if (tel.length != 11) {
                        page.show_confirm("提示", "请输入正确的手机号！");
                        page.$loadingToast.hide();
                        return;
                    }
                    if (tel.match(phonefilter)) {

                    } else {
                        page.show_confirm("提示", "请输入正确的手机号！");
                        page.$loadingToast.hide();
                        return;
                    }
                }
                
            }
            if (activityVM.currentType == 'ShowPic') {
                $('.showPic_info').hide();
                
                activityVM.registerMessage = [];
            }

            var nextStep = activityVM.currentStep + 1;

            var data = {
                currentStep: activityVM.currentStep,
                nextStep: nextStep,
                inputJson: JSON.stringify(activityVM.registerMessage),
                activityId: id     //Id
            }

            page.post('/WX/Home/SubmitRegister', {
                currentStep: activityVM.currentStep,
                nextStep: nextStep,
                inputJson: JSON.stringify(activityVM.registerMessage),
                activityId: id
            }, function (result) {
                //console.log(result);
                if (result.Type === "OpenResult") {
                    location.href = '/WX/RedPackageActivity/GetRedPacket?id=' + id;
                } else if (result.Type === "OpenRegister") {
                    $('.info').show(); 
                    activityVM.registerMessage = eval(result.Value); //把字符串转换成对象
                    activityVM.currentStep = result.NextStep;
                    
                    setTimeout(function () {
                        //根据后端返回的指令进行显示页面
                        if (activityVM.previousType === 'OpenRegister') {
                            activityVM.receiveEmotion(id);     //获取emotion值(心情API)
                        } else if (activityVM.previousType === 'ShowPic') {
                            $('.info_promt').text("（注：上传的图片与上张图片相似度越高，领取的红包越多哟！）");  //领红包提示信息
                        }
                    },10)
                }
                activityVM.previousType = activityVM.currentType;
                activityVM.currentType = result.Type;
            });

        },
        aaa: function () {

        }
    }

});

$(function () {
    activityVM.init();
})