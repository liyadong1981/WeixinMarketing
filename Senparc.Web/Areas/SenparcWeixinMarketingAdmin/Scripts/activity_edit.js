window.dataIndex = null;
window.feelAppend = true;  //拖放“某个心情”后锁定style
//window.dataType = null;

//点击加号显示内容
$(function () {
    //点击加号显示“登记信息”
    $('.leftCart_icon').on('click', function () {
        $(this).nextAll().find('.leftCart').toggle();
    });

    $('.redPackage_edit .activity_fa').hide();  //隐藏左侧“素材”的删除icon

    cancelLock(); //解除锁定

    //查看编辑页面有哪些“素材”
    $.each($("#activity_content .leftCart"), function (i, item) {
        if ($(item).attr('data-index') != 2) {
            var hideClass = '.' + $(item).attr('id');
            var hideId = '#' + $(item).attr('id');
            $('.redPackage_edit').find(hideId).remove();
            $('.redPackage_edit').find(hideClass).addClass('leftCart').show();
        } else {
            //查看编辑页面有心情哪些“素材” 
            if ($("#activity_content #feelEmotion_title").length > 0) {
                var afterElement = $('.redPackage_edit').find('#feelEmotion_title').next();
                $('.redPackage_edit').find('#feelEmotion_title').remove();
                $('.redPackage_edit').find('.feelEmotion_title').insertBefore(afterElement).show();
            }
            if ($("#activity_content .feeling_tag").length > 0) {
                var hideClass = '.' + $("#activity_content .feeling_tag").attr('id');
                var hideId = '#' + $("#activity_content .feeling_tag").attr('id');
                var afterElement = $('.redPackage_edit').find(hideId).next();
                $('.redPackage_edit').find(hideId).remove();
                $('.redPackage_edit').find(hideClass).insertBefore(afterElement);
                $('.redPackage_edit').find(hideClass).show();
            }
        }

    });



    //删除已拖拽的活动（登记信息、心情API、发红包）
    $('.activity_fa').on('click', function () {
        var hide_class = "." + $(this).parents('.leftCart').attr('id');
        var dispaly_id = "#" + $(this).parents('.leftCart').attr('id');
        var index = $(this).parents('.leftCart').attr('data-index');
        var element_append = "";
        $(hide_class).removeClass('leftCart');
        $(hide_class).hide();
        element_append = $(this).parents('.col-xs-12').prev().find('.activity_cart_' + index + '');
        element_append.append($(dispaly_id));
        $('#activity_content').find(dispaly_id).remove();

        //删除拖拽的“心情”
        var feel_hide_class_title = "." + $(this).parents('#feelEmotion_title').attr('id'); //标题class
        var feel_dispaly_id_title = "#" + $(this).parents('#feelEmotion_title').attr('id'); //标题id

        var feel_hide_class = "." + $(this).parents('.feeling_type').attr('id'); //单个心情class
        var feel_dispaly_id = "#" + $(this).parents('.feeling_type').attr('id'); //单个心情id

        var feel_element_append_title = "";
        var feel_element_append = "";

        $(feel_hide_class_title).hide();
        $(feel_hide_class).hide();
        feel_element_append_title = $(this).parents('.col-xs-12').prev().find('#fellingApi_detail');
        feel_element_append_title.append($(feel_dispaly_id_title));
        $(feel_dispaly_id_title).insertBefore($(feel_hide_class_title));
        $('#activity_content').find(feel_dispaly_id_title).remove();

        feel_element_append = $(this).parents('.col-xs-12').prev().find('#fellingApi_detail');
        feel_element_append.append($(feel_dispaly_id));
        $(feel_dispaly_id).insertBefore($(feel_hide_class));
        $('#activity_content').find(feel_dispaly_id).remove();

        $('.redPackage_edit .activity_fa').hide();  //隐藏左侧“素材”的删除icon

        //删除心情API添加的style
        var feel_title_length = feel_element_append_title.find('#feelEmotion_title').length;
        var feel_tag_length = feel_element_append_title.find('.feeling_tag').length;

        if (feel_title_length == 1 && feel_tag_length == 8) {
            $('.activity_felling').remove();
            window.feelAppend = true;

            //隐藏所有“替代心情API”
            $.each(feel_element_append_title.find('.feel_replace'), function (i, item) {
                $(item).hide();
            });
        }
    });

});

function allowDrop(ev) {
    ev.preventDefault();

}

function drag(ev, ths) {
    ev.dataTransfer.setData("Text", ev.target.id);
    window.dataIndex = $(ths).attr('data-index');
    window.ele_brother = $('.activity_cart_2').find('#feelEmotion_title').next();
    window.ele_brother_0 = $('.activity_cart_2').find('#feelEmotion_0').next();
    window.ele_brother_1 = $('.activity_cart_2').find('#feelEmotion_1').next();
    window.ele_brother_2 = $('.activity_cart_2').find('#feelEmotion_2').next();
    window.ele_brother_3 = $('.activity_cart_2').find('#feelEmotion_3').next();
    window.ele_brother_4 = $('.activity_cart_2').find('#feelEmotion_4').next();
    window.ele_brother_5 = $('.activity_cart_2').find('#feelEmotion_5').next();
    window.ele_brother_6 = $('.activity_cart_2').find('#feelEmotion_6').next();
    window.ele_brother_7 = $('.activity_cart_2').find('#feelEmotion_7').next();
}

function drop(ev, feelListCount) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("Text");
    ev.target.appendChild(document.getElementById(data));
    //$('.checkInMessage_detail').show();
    displayCart(feelListCount);
}

function displayCart(feelListCount) {
    $('#activity_content .activity_fa').show();

    $('.activity_cart_' + window.dataIndex + '').find('.replaceCart').show();
    $('.activity_cart_' + window.dataIndex + '').find('.replaceCart').addClass('leftCart');

    if ($('.activity_cart_2').find('#feelEmotion_title').length <= 0) {
        $('.feelEmotion_title').insertBefore(window.ele_brother);
        $('.feelEmotion_title').show();
    }

    window.feelListCount = feelListCount;

    if ($('.activity_cart_2').find('#feelEmotion_0').length <= 0) {
        $('.feelEmotion_0').insertBefore(window.ele_brother_0);
        $('.feelEmotion_0').show();
    }
    if ($('.activity_cart_2').find('#feelEmotion_1').length <= 0) {
        $('.feelEmotion_1').insertBefore(window.ele_brother_1);
        $('.feelEmotion_1').show();
    }
    if ($('.activity_cart_2').find('#feelEmotion_2').length <= 0) {
        $('.feelEmotion_2').insertBefore(window.ele_brother_2);
        $('.feelEmotion_2').show();
    }
    if ($('.activity_cart_2').find('#feelEmotion_3').length <= 0) {
        $('.feelEmotion_3').insertBefore(window.ele_brother_3);
        $('.feelEmotion_3').show();
    }
    if ($('.activity_cart_2').find('#feelEmotion_4').length <= 0) {
        $('.feelEmotion_4').insertBefore(window.ele_brother_4);
        $('.feelEmotion_4').show();
    }
    if ($('.activity_cart_2').find('#feelEmotion_5').length <= 0) {
        $('.feelEmotion_5').insertBefore(window.ele_brother_5);
        $('.feelEmotion_5').show();
    }
    if ($('.activity_cart_2').find('#feelEmotion_6').length <= 0) {
        $('.feelEmotion_6').insertBefore(window.ele_brother_6);
        $('.feelEmotion_6').show();
    }
    if ($('.activity_cart_2').find('#feelEmotion_7').length <= 0) {
        $('.feelEmotion_7').insertBefore(window.ele_brother_7);
        $('.feelEmotion_7').show();
    }


    //心情API锁定
    if ($('#activity_content').find('.feeling_tag').length > 0) {
        var feeling_tag = $('.activity_feeling').find('.feeling_tag');
        $.each(feeling_tag, function (i, item) {
            $(item).attr('draggable', 'false');
        });
    }

    if ($('#activity_content').find('#feelEmotion_title').length > 0) {
        if (window.feelAppend) {
            $('#activity_content .activity_edit').append('<div class=activity_felling></div>');
            $('#activity_content .activity_felling').append($('#feelEmotion_title'));
            window.feelAppend = false;
        }
    }
}

function cancelLock() {
    //解除心情API锁定
    var feeling_lenth = $('#activity_content').find('.feeling_tag').length;
    //var feeling_type = $(this).parents('.col-xs-12').prev().find('.feeling_type');
    var feeling_tag = $('.activity_cart_2').find('.feeling_tag');
    var feeling_title = $(this).parents('.col-xs-12').prev().find('#feelEmotion_title');
    if (feeling_lenth == 0) {
        $.each(feeling_tag, function (i, item) {
            $(item).attr('draggable', 'true');
        });
    } else {
        $.each(feeling_tag, function (i, item) {
            $(item).attr('draggable', 'false');
        });
    }
}

//保存活动编辑及校验
function Check() {
    //获取心情API内容
    var feel_title = $('#activity_content #feelEmotion_title').text();
    var feel_emotionType = $('#activity_content .feeling_tag').attr("data-type");

    //获取奖项设置内容
    var prizes_name = $('#activity_content .prizes_name input').val();
    var prizes_count = parseInt($('#activity_content .prizes_count input').val());

    //获取登记信息内容
    var account_img = $('#activity_content .account_img').text();
    var account_name = $('#activity_content .account_name').text();
    var account_tel = $('#activity_content .account_tel').text();
    var checkMessage_text_type = null;
    var checkMessage_img_type = null;
    if (account_img == "" || account_img == null) {
        checkMessage_text = "";
        checkMessage_type = null;
        var checkMessage_img_type = null;
    } else {
        checkMessage_text_type = 0;
        checkMessage_img_type = 2;
    }

    var data = {
        "Info": [
            { "Name": account_img, "Type": checkMessage_img_type },
            { "Name": account_name, "Type": checkMessage_text_type },
            { "Name": account_tel, "Type": checkMessage_text_type }
        ],
        "API": [
            { "Type": 0, "Value": feel_emotionType }
        ],
        "Awards": [
            { "Name": prizes_name, "Count": prizes_count }
        ]
    }

    $('#AppRedPackageActivity_Rule').attr("value", JSON.stringify(data));

    if (feel_title != "" && feel_emotionType == undefined) {
        alert("请拖拽具体的心情元素！");
        return false;
    }
    if (prizes_name == "") {
        alert("请填写奖项名称！");
        return false;
    }
    if (prizes_count == 0) {
        alert("请填写中奖人数！");
        return false;
    }
}
