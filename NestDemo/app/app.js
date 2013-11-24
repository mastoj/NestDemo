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
                scope.selectedItems = [];
                scope.selectItem = function (item) {
                    scope.selectedItems.push(item);
                    scope.onSelect({ item: item });
                };
                scope.deselectItem = function(item) {
                    var index = scope.selectedItems.indexOf(item);
                    scope.selectedItems.splice(index, 1);
                };
                scope.filterFunction = function(item) {
                    for (var selectedIndex in scope.selectedItems) {
                        var selectedItem = scope.selectedItems[selectedIndex];
                        if (selectedItem.term === item.term) {
                            return null;
                        }
                    }
                    return item;
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
        };

        $scope.$watch('search', function () {
            $http.post("api/search", $scope.search).success(function (data) {
                $scope.searchResult = data;
            });
        }, true);
    }
})();