﻿//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
//Licensed under the Apache License, Version 2.0(the
//"License"); you may not use this file except in compliance
//with the License.You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//Unless required by applicable law or agreed to in writing,
//software distributed under the License is distributed on an
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//KIND, either express or implied.See the License for the
//specific language governing permissions and limitations
//under the License.

/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />

// tested 
module Spiro.Angular.Modern {

	// tested
    app.controller('BackgroundController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleBackground($scope); 
    });

    // tested
    app.controller('ServicesController', ($scope : ng.IScope, handlers: IHandlers) => {
	    handlers.handleServices($scope);
    });

    // tested
    app.controller('ServiceController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleService($scope);
    });

    // tested
    app.controller('DialogController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.action) {
		    handlers.handleActionDialog($scope);
	    }
    });

    // tested
    app.controller('NestedObjectController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {

	    // action takes priority 
	    if ($routeParams.action) {
		    handlers.handleActionResult($scope);
	    }
	    // action + one of  
	    if ($routeParams.property) {
		    handlers.handleProperty($scope);
	    }
	    else if ($routeParams.collectionItem) {
		    handlers.handleCollectionItem($scope);
	    }
	    else if ($routeParams.resultObject) {
		    handlers.handleResult($scope);
	    }
    });

    // tested
    app.controller('CollectionController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.resultCollection) {
		    handlers.handleCollectionResult($scope);
	    }
	    else if ($routeParams.collection) {
		    handlers.handleCollection($scope);
	    }
    });

    // tested
    app.controller('ObjectController', ($scope: ng.IScope, $routeParams: ISpiroRouteParams, handlers: IHandlers) => {
	    if ($routeParams.editMode) {
		    handlers.handleEditObject($scope);
	    }
	    else {
		    handlers.handleObject($scope);
	    }
    });

    // tested
    app.controller('TransientObjectController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleTransientObject($scope);
    });

    // tested
    app.controller('ErrorController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleError($scope);
    });

    // tested
    app.controller('AppBarController', ($scope: ng.IScope, handlers: IHandlers) => {
	    handlers.handleAppBar($scope);    
    });
}