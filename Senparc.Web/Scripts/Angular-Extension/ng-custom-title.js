(function () {
    var module = angular.module('custom-title', []);
    module.provider('customTitleProvider', function () {
        this.$get = [function () {
            return {
                title: ''
            }
        }]
    })
    module.directive('customTitle', ['customTitleProvider', function (customTitleProvider) {
        return {
            restrict: 'A',
            scope: {
                defaultTitle: '=defaultTitle'
            },
            link: function ($scope, $ele, $attrs) {
                $scope.$watch(function(){
                    return customTitleProvider.title;
                }, function (newVal, oldVal) {
                    $ele.html(customTitleProvider.title || $scope.defaultTitle);
                });
            }
        }
    }]);
}());