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

    function SearchCtrl($scope, $http) {
        $scope.createIndex = function () {
            $http.post("api/index");
        };
        $scope.deleteIndex = function () {
            $http.delete("api/index");
        };
    }
})();