//Copyright 2014 Stef Cascarini, Dan Haywood, Richard Pawson
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
/// <reference path="../../Scripts/typings/jasmine/jasmine.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/angularjs/angular-mocks.d.ts" />
/// <reference path="../../Scripts/spiro.modern.app.ts" />
/// <reference path="../../Scripts/spiro.modern.services.handlers.ts" />
describe('Controllers', function () {
    var $scope, ctrl;
    beforeEach(module('app'));
    describe('BackgroundController', function () {
        var handleBackground;
        beforeEach(inject(function ($rootScope, $controller, handlers) {
            $scope = $rootScope.$new();
            handleBackground = spyOn(handlers, 'handleBackground');
            ctrl = $controller('BackgroundController', { $scope: $scope, handlers: handlers });
        }));
        it('should call the handler', function () {
            expect(handleBackground).toHaveBeenCalledWith($scope);
        });
    });
    describe('ServicesController', function () {
        var handleServices;
        beforeEach(inject(function ($rootScope, $controller, handlers) {
            $scope = $rootScope.$new();
            handleServices = spyOn(handlers, 'handleServices');
            ctrl = $controller('ServicesController', { $scope: $scope, handlers: handlers });
        }));
        it('should call the handler', function () {
            expect(handleServices).toHaveBeenCalledWith($scope);
        });
    });
    describe('ServiceController', function () {
        var handleService;
        beforeEach(inject(function ($rootScope, $controller, handlers) {
            $scope = $rootScope.$new();
            handleService = spyOn(handlers, 'handleService');
            ctrl = $controller('ServiceController', { $scope: $scope, handlers: handlers });
        }));
        it('should call the handler', function () {
            expect(handleService).toHaveBeenCalledWith($scope);
        });
    });
    describe('ObjectController', function () {
        var handleObject;
        var handleEditObject;
        beforeEach(inject(function ($rootScope, $controller, handlers) {
            $scope = $rootScope.$new();
            handleObject = spyOn(handlers, 'handleObject');
            handleEditObject = spyOn(handlers, 'handleEditObject');
        }));
        describe('if edit mode', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.editMode = "test";
                ctrl = $controller('ObjectController', { $scope: $scope, handlers: handlers });
            }));
            it('should not call the view handler', function () {
                expect(handleObject).not.toHaveBeenCalled();
            });
            it('should call the edit handler', function () {
                expect(handleEditObject).toHaveBeenCalledWith($scope);
            });
        });
        describe('if not edit mode', function () {
            beforeEach(inject(function ($controller, handlers) {
                ctrl = $controller('ObjectController', { $scope: $scope, handlers: handlers });
            }));
            it('should not call the edit handler', function () {
                expect(handleEditObject).not.toHaveBeenCalled();
            });
            it('should call the view handler', function () {
                expect(handleObject).toHaveBeenCalledWith($scope);
            });
        });
    });
    describe('DialogController', function () {
        var handleActionDialog;
        beforeEach(inject(function ($rootScope, handlers) {
            $scope = $rootScope.$new();
            handleActionDialog = spyOn(handlers, 'handleActionDialog');
        }));
        describe('if action parm set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.action = "test";
                ctrl = $controller('DialogController', { $scope: $scope, handlers: handlers });
            }));
            it('should call the handler', function () {
                expect(handleActionDialog).toHaveBeenCalledWith($scope);
            });
        });
        describe('if action parm not set', function () {
            beforeEach(inject(function ($controller, handlers) {
                ctrl = $controller('DialogController', { $scope: $scope, handlers: handlers });
            }));
            it('should not call the handler', function () {
                expect(handleActionDialog).not.toHaveBeenCalled();
            });
        });
    });
    describe('NestedObjectController', function () {
        var handleActionResult;
        var handleProperty;
        var handleCollectionItem;
        var handleResult;
        beforeEach(inject(function ($rootScope, handlers) {
            $scope = $rootScope.$new();
            handleActionResult = spyOn(handlers, 'handleActionResult');
            handleProperty = spyOn(handlers, 'handleProperty');
            handleCollectionItem = spyOn(handlers, 'handleCollectionItem');
            handleResult = spyOn(handlers, 'handleResult');
        }));
        describe('if action parm set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.action = "test";
                ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
            }));
            it('should call the action handler only', function () {
                expect(handleActionResult).toHaveBeenCalledWith($scope);
                expect(handleProperty).not.toHaveBeenCalled();
                expect(handleCollectionItem).not.toHaveBeenCalled();
                expect(handleResult).not.toHaveBeenCalled();
            });
        });
        describe('if property parm set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.property = "test";
                ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
            }));
            it('should call the property handler only', function () {
                expect(handleActionResult).not.toHaveBeenCalled();
                expect(handleProperty).toHaveBeenCalledWith($scope);
                expect(handleCollectionItem).not.toHaveBeenCalled();
                expect(handleResult).not.toHaveBeenCalled();
            });
        });
        describe('if collection Item parm set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.collectionItem = "test";
                ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
            }));
            it('should call the collection item handler only', function () {
                expect(handleActionResult).not.toHaveBeenCalled();
                expect(handleProperty).not.toHaveBeenCalled();
                expect(handleCollectionItem).toHaveBeenCalledWith($scope);
                expect(handleResult).not.toHaveBeenCalled();
            });
        });
        describe('if result object parm set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.resultObject = "test";
                ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
            }));
            it('should call the result object handler only', function () {
                expect(handleActionResult).not.toHaveBeenCalled();
                expect(handleProperty).not.toHaveBeenCalled();
                expect(handleCollectionItem).not.toHaveBeenCalled();
                expect(handleResult).toHaveBeenCalledWith($scope);
            });
        });
        describe('if all parms set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.action = "test";
                $routeParams.property = "test";
                $routeParams.collectionItem = "test";
                $routeParams.resultObject = "test";
                ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
            }));
            it('should call the action and property handler only', function () {
                expect(handleActionResult).toHaveBeenCalledWith($scope);
                expect(handleProperty).toHaveBeenCalledWith($scope);
                expect(handleCollectionItem).not.toHaveBeenCalled();
                expect(handleResult).not.toHaveBeenCalled();
            });
        });
        describe('if no parms set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                ctrl = $controller('NestedObjectController', { $scope: $scope, handlers: handlers });
            }));
            it('should call no handlers', function () {
                expect(handleActionResult).not.toHaveBeenCalled();
                expect(handleProperty).not.toHaveBeenCalled();
                expect(handleCollectionItem).not.toHaveBeenCalled();
                expect(handleResult).not.toHaveBeenCalled();
            });
        });
    });
    describe('CollectionController', function () {
        var handleCollectionResult;
        var handleCollection;
        beforeEach(inject(function ($rootScope, handlers) {
            $scope = $rootScope.$new();
            handleCollectionResult = spyOn(handlers, 'handleCollectionResult');
            handleCollection = spyOn(handlers, 'handleCollection');
        }));
        describe('if result collection parm set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.resultCollection = "test";
                ctrl = $controller('CollectionController', { $scope: $scope, handlers: handlers });
            }));
            it('should call the result collection handler', function () {
                expect(handleCollectionResult).toHaveBeenCalledWith($scope);
                expect(handleCollection).not.toHaveBeenCalled();
            });
        });
        describe('if collection parm set', function () {
            beforeEach(inject(function ($routeParams, $controller, handlers) {
                $routeParams.collection = "test";
                ctrl = $controller('CollectionController', { $scope: $scope, handlers: handlers });
            }));
            it('should  call the collection handler', function () {
                expect(handleCollectionResult).not.toHaveBeenCalled();
                expect(handleCollection).toHaveBeenCalledWith($scope);
            });
        });
        describe('if no parms set', function () {
            beforeEach(inject(function ($controller, handlers) {
                ctrl = $controller('CollectionController', { $scope: $scope, handlers: handlers });
            }));
            it('should not call the handler', function () {
                expect(handleCollectionResult).not.toHaveBeenCalled();
                expect(handleCollection).not.toHaveBeenCalled();
            });
        });
    });
    describe('TransientObjectController', function () {
        var handleTransientObject;
        beforeEach(inject(function ($rootScope, $controller, handlers) {
            $scope = $rootScope.$new();
            handleTransientObject = spyOn(handlers, 'handleTransientObject');
            ctrl = $controller('TransientObjectController', { $scope: $scope, handlers: handlers });
        }));
        it('should call the handler', function () {
            expect(handleTransientObject).toHaveBeenCalledWith($scope);
        });
    });
    describe('ErrorController', function () {
        var handleError;
        beforeEach(inject(function ($rootScope, $controller, handlers) {
            $scope = $rootScope.$new();
            handleError = spyOn(handlers, 'handleError');
            ctrl = $controller('ErrorController', { $scope: $scope, handlers: handlers });
        }));
        it('should call the handler', function () {
            expect(handleError).toHaveBeenCalledWith($scope);
        });
    });
    describe('AppBarController', function () {
        var handleAppBar;
        beforeEach(inject(function ($rootScope, $controller, handlers) {
            $scope = $rootScope.$new();
            handleAppBar = spyOn(handlers, 'handleAppBar');
            ctrl = $controller('AppBarController', { $scope: $scope, handlers: handlers });
        }));
        it('should call the handler', function () {
            expect(handleAppBar).toHaveBeenCalledWith($scope);
        });
    });
});
//# sourceMappingURL=controllers.js.map