(function() {

    'use strict';
    var app = angular.module('app', ['ngRoute']);

    app.config(['$routeProvider' ,function($routeProvider) {
        $routeProvider.
            when("/", { controller: 'SearchCtrl', templateUrl: "search.html" }).
            //when("/second", { controller: "DetailsCtrl", templateUrl: "details.html" }).
            otherwise({ redirectTo: "/" });
    }]);

    app.value('toastr', toastr);
    app.directive('searchFilter', searchFilter);
    app.controller('SearchCtrl', SearchCtrl);

    function searchFilter() {
        return {
            restrict: 'E',
            replace: false,
            templateUrl: 'searchFilter.html',
            link: function (scope, element) {
                scope.selectItem = function(item) {
                    alert(item + " directive");
                    scope.onSelect({ item: item });
                };
            },
            scope: {
                title: '@',
                filter: '=',
                onSelect: '&'
            }
        };
    }

    function SearchCtrl($scope, $http, toastr) {
        $scope.search = {
            Query: "",
            NumberToTake: 10
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

        $scope.productSelected = function(item) {
            alert(item);
        };

        $scope.$watch('search', function () {
            $http.post("api/search", $scope.search).success(function (data) {
                $scope.searchResult = data;
            });
        }, true);
    }
})();