
function layout() {
    var rowCount = 2; //行数

    var screenHeight = $(window).height();
    var screenWidth = $(window).width();

    //if (screenWidth >= screenHeight) {
    var titleHeight = parseFloat(screenHeight / 5);
    //var contentHeight = screenHeight - titleHeight;
    var contentHeight = screenHeight - titleHeight -65;
    var blockHeight = contentHeight / rowCount / 5 * 4;
    var marginBottomHeight = contentHeight / rowCount / 5;

    var expression_scoreWidth = screenWidth * 0.6 * 0.58;
    var expression_imgeWidth = blockHeight / 5 * 4;
    var expression_textWidth = 0;
    if (screenWidth <= 1000) {
        expression_textWidth = 78;
    }else if (screenWidth > 1000 && screenWidth <= 2000) {
        expression_textWidth = 150;
    } else if (screenWidth > 2000 && screenWidth <= 3000) {
        expression_textWidth = 200;
    }
    else if (screenWidth > 3000) {
        expression_textWidth = 260;
    }

    var alignPadding = (expression_scoreWidth - expression_imgeWidth - expression_textWidth) / 2;

    $('.expression_text').css('height', '65px');
    $('.expression_title').css('height', titleHeight + 'px');
    $('.expression_title').css('line-height', titleHeight + 'px');
    $(".expression_con_block").css("height", blockHeight);
    $(".expression_con_block").css("margin-bottom", marginBottomHeight);

    //$('.expression_headImg').css("height", blockHeight / 2);
    //$('.expression_headImg').css("width", blockHeight / 2);
    //$('.expression_headImg').css("margin-top", blockHeight / 4);aa

    $('.expression_score').css('height', blockHeight);
    $('.expression_score').css('padding-left', alignPadding);
    $('.expression_score').css('padding-right', alignPadding);
    $('.expression_score_headImg').css("max-height", blockHeight / 5 * 4);
    $('.expression_score_headImg').css("max-width", blockHeight / 5 * 4);
    $('.expression_score_headImg').css("margin-top", blockHeight / 10 * 1);
    $('.expression_userMessage').css("line-height", blockHeight + 'px');
    $('.expression_ranking').css("line-height", blockHeight + 'px');
    $('.expression_name').css("line-height", blockHeight + 'px');

}
function _setInterval(activityId) {
    //轮循

    var _setInterval = self.setInterval("getActivityAwardLog(" + activityId + ")", 2000); //间隔1s执行一次事件   //TODO：需要活动Id
}
function getActivityAwardLog(activityId) {

    $.ajax({
        url: '/Azureadmin/RedPackageActivity/GetActivityAwardLog',
        type: 'POST',
        data: JSON.stringify({ id: activityId }),
        contentType: 'application/json;',
        success: function (json) {
            if (json.Success) {
                var html = "";
                var awardLogList = [];
                awardLogList = json.Result.ActivityAwardLogList;
                for (var i = 0; i < awardLogList.length; i++) {
                    if (awardLogList[i].NickName == null) {
                        awardLogList[i].NickName = "";
                    }
                    var ecb = '.ecb' + (i + 1);
                    $(ecb).show();
                    $(ecb).find('.expression_headImg img').attr('src', awardLogList[i].PicUrl);
                    $(ecb).find('.expression_name').text(awardLogList[i].NickName);
                    $(ecb).find('.expression_score_headImg img').attr('src', awardLogList[i].InputPic);
                    $(ecb).find('.expression_userMessage span').text(awardLogList[i].Money);
                    $(ecb).find('.expression_ranking span').text(i + 1);
                }

                //layout();

            } else {
                alert("提示", json.Result.Message);
            }
        },
        error: function (xhr, errorType, error) {
            alert('发生错误，请联系管理员');
        },
        complete: function () {
            //page.$loadingToast.hide();
        }
    });
}

function qrCode(activityId) {
    //TODO:这里的网址需要改成实际运行的网址，不然生成的二维码将指向错误的域名！
    $('#activityCode').qrcode('http://azuredemo.senparc.com/WX?id=' + activityId);
}

//显示活动二维码
function enlargeCode() {
    $('.packageActivityCode').show();
}
function hide_enlargeCode() {
    $('.packageActivityCode').hide();
}
//function imageEnlarge() {
//    var src_first=$('.ecb1').attr('src');  //获取第一名图片地址
//    var src_first=$('.ecb2').attr('src');  //获取第二名图片地址
//    $('#expression_first').attr('src', src_first);
//    $('#expression_second').attr('src', src_first);

//    $('#expression_first').show();
//    setTimeout(function () {
//        $('#expression_first').hide();
//    }, 3000);

//    setTimeout(function () {
//        $('#expression_second').show();
//    }, 4000);

//    setTimeout(function () {
//        $('#expression_second').hide();
//    }, 7000);
//}

