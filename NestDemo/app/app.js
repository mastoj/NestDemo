(function() {

    'use strict';
    var app = angular.module('app', ['ngRoute']);

    app.config(['$routeProvider' ,function($routeProvider) {
        $routeProvider.
            when("/", { controller: 'SearchCtrl', templateUrl: "search.html" }).
            //when("/second", { controller: "DetailsCtrl", templateUrl: "details.html" }).
            otherwise({ redirectTo: "/" });
    }]);

    app.controller('SearchCtrl', SearchCtrl);

    app.value('toastr', toastr);

    app.factory("toastService", ['toastr', function (toastr) {
        return {
            toastNow: function (message) {
                switch (message.type) {
                    case 'success':
                        toastr.success(message.body, message.title);
                        break;
                    case 'info':
                        toastr.info(message.body, message.title);
                        break;
                    case 'warning':
                        toastr.warning(message.body, message.title);
                        break;
                    case 'error':
                        toastr.error(message.body, message.title);
                        break;
                }
            }
        };
    }]);

    function SearchCtrl($scope, $http, toastr) {
        $scope.search = {
            Query: ""
        };
        $scope.createIndex = function () {
            $scope.updatingIndex = true;
            $http.post("api/index").success(function () {
                toastr.success("Index created", "Yeah!");
                $scope.updatingIndex = false;
            }).error(function () {toastr.error("Index NOT created", "Oh no!"); });
        };
        $scope.deleteIndex = function () {
            $scope.updatingIndex = true;
            $http.delete("api/index").success(function() {
                toastr.success("Index deleted", "Yeah!");
                $scope.updatingIndex = false;
            }).error(function () { toastr.error("Index NOT deleted", "Oh no!"); });
        };
        
        $scope.$watch('search', function () {
            $http.post("api/search", $scope.search).success(function (data) {
                $scope.searchResult = data;
            });
        }, true);
    }
})();