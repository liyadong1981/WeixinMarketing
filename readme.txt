此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。

首次运行需要执行的操作：
1、创建一个名为WeixinMarketing的空SQL Server数据库（或者对应到Senparc.Web项目下Web.config中修改）
2、使用书中的教程，将Senparc.Database项目中的数据库表同步到WeixinMarketing数据库中（也可以按照书上的提示直接使用SQL语句创建，可以在BookHelper中找到）
3、在Senparc.Web下，/Areas/SenparcWeixinMarketingAdmin/Content/Scripts/screen.js文件第99行需要修改实际线上部署的网址
4、初次运行项目先执行/Install，如：http://localhost:17561/Install ，系统会默认进行初始化
5、初始化之后即可登录后台：http://[域名]/

注意：本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。


购买正版：http://item.jd.com/12220004.html