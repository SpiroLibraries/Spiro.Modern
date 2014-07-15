﻿/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="spiro.models.ts" />


// tested 
module Spiro.Angular.Modern {

    interface ISelectScope extends ng.IScope {
        select: any;
    }

    // based on code in AngularJs, Green and Seshadri, O'Reilly
    app.directive('nogDatepicker', function ($filter : ng.IFilterService) : ng.IDirective {
            return {
                // Enforce the angularJS default of restricting the directive to
                // attributes only
                restrict: 'A',
                // Always use along with an ng-model
                require: '?ngModel',
                // This method needs to be defined and passed in from the
                // passed in to the directive from the view controller
                scope: {
                    select: '&'        // Bind the select function we refer to the right scope
                },
                link: function (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                    if (!ngModel) return;

                    var optionsObj: { dateFormat?: string; onSelect?: Function } = {};

                    optionsObj.dateFormat = 'd M yy'; // datepicker format
                    var updateModel = function (dateTxt) {
                        scope.$apply(function () {
                            // Call the internal AngularJS helper to
                            // update the two way binding

                            ngModel.$parsers.push((val) => { return new Date(val).toISOString(); });
                            ngModel.$setViewValue(dateTxt);
                        });
                    };

                    optionsObj.onSelect = function (dateTxt, picker) {
                        updateModel(dateTxt);
                        if (scope.select) {
                            scope.$apply(function () {
                                scope.select({ date: dateTxt });
                            });
                        }
                    };


                    ngModel.$render = function () {
                        var formattedDate = $filter('date')(ngModel.$viewValue, 'd MMM yyyy'); // angularjs format

                        // Use the AngularJS internal 'binding-specific' variable
                        element.datepicker('setDate', formattedDate);
                    };
                    element.datepicker(optionsObj);
                }
            };
        });

    app.directive('nogAutocomplete', function ($filter: ng.IFilterService, $parse, context: IContext): ng.IDirective {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: 'A',
            // Always use along with an ng-model
            require: '?ngModel',
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: '&'        // Bind the select function we refer to the right scope
            },
            link: function (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                if (!ngModel) return;
        
                var optionsObj: { autoFocus?: boolean; minLength?: number; source?: Function; select?: Function; focus?: Function } = {};
                var parent = <any>scope.$parent;
                var viewModel = <ValueViewModel> (parent.parameter || parent.property);

                function render ( initialChoice? :ChoiceViewModel) {
                    var cvm = ngModel.$modelValue || initialChoice;

                    if (cvm) {
                        ngModel.$parsers.push((val) => { return cvm; });
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    }
                };

                ngModel.$render = render;
             
                var updateModel = function (cvm: ChoiceViewModel) {

                    //context.setSelectedChoice(cvm.id, cvm.search, cvm);

                    scope.$apply(function () {

                        ngModel.$parsers.push((val) => { return cvm; });
                        ngModel.$setViewValue(cvm.name);
                        element.val(cvm.name);
                    });
                };

                optionsObj.source = (request, response) => {

                    var prompts = scope.select({ request: request.term });

                    scope.$apply(function () {
                        prompts.then(function (cvms: ChoiceViewModel[]) {
                            response(_.map(cvms, (cvm) => {
                                return { "label": cvm.name, "value": cvm };
                            }));
                        }, function () {
                            response([]);
                        });
                    });
                };

                optionsObj.select = (event, ui) => {
                    updateModel(ui.item.value);
                    return false; 
                };

                optionsObj.focus = (event, ui) => {
                    return false;
                };

                optionsObj.autoFocus = true;
                optionsObj.minLength = viewModel.minLength;

                var clearHandler = function () {
                    var value = $(this).val();

                    if (value.length == 0) {
                        updateModel(ChoiceViewModel.create(new Value(""), ""));
                    }
                };

                element.keyup(clearHandler); 
                element.autocomplete(optionsObj);
                render(viewModel.choice);
            }
        };
    });

    app.directive('nogConditionalchoices', function ($filter: ng.IFilterService, $parse, context: IContext): ng.IDirective {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: 'A',
            // Always use along with an ng-model
            require: '?ngModel',
            // This method needs to be defined and passed in from the
            // passed in to the directive from the view controller
            scope: {
                select: '&'        // Bind the select function we refer to the right scope
            },
            link: function (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                if (!ngModel) return;

                var parent = <any>scope.$parent;
                var viewModel = <ValueViewModel> (parent.parameter || parent.property);
                var pArgs = viewModel.arguments;
                var currentOptions: ChoiceViewModel[] = [];

                function populateArguments() {
                    var nArgs = <ValueMap>{};

                    var dialog = <DialogViewModel> parent.dialog;
                    var object = <DomainObjectViewModel> parent.object;

                    if (dialog) {
                        _.forEach(<_.Dictionary<Value>>pArgs, (v, n) => {

                            var parm = <ParameterViewModel> _.find(dialog.parameters, (p: ParameterViewModel) => p.id == n);

                            var newValue = parm.getValue();
                            nArgs[n] = newValue;
                        });
                    }

                    if (object) {
                        _.forEach(<_.Dictionary<Value>>pArgs, (v, n) => {

                            var property = <PropertyViewModel> _.find(object.properties, (p: PropertyViewModel) => p.argId == n);

                            var newValue = property.getValue();
                            nArgs[n] = newValue;
                        });
                    }

                    return nArgs;
                }

                function populateDropdown() {
                    var nArgs = populateArguments();
      
                    var prompts = scope.select({ args: nArgs });

                    prompts.then(function (cvms: ChoiceViewModel[]) {
                        // if unchanged return 
                        if (cvms.length === currentOptions.length && _.all(cvms, (c : ChoiceViewModel, i) => { return c.equals(currentOptions[i]); })) {
                            return;
                        }

                        element.find("option").remove();
                        var emptyOpt = "<option></option>";
                        element.append(emptyOpt);

                        _.forEach(cvms, (cvm) => {
                           
                            var opt = $("<option></option>");
                            opt.val(cvm.value);
                            opt.text(cvm.name);

                            element.append(opt);
                        });

                        currentOptions = cvms;

                        if (viewModel.isMultipleChoices && viewModel.multiChoices) {
                            var vals = _.map(viewModel.multiChoices, (c: ChoiceViewModel) => {
                                return c.value;
                            });

                            $(element).val(vals);
                        } else if (viewModel.choice) {
                            $(element).val(viewModel.choice.value);
                        } 
                    },
                    function () {
                          // error clear everything 
                          element.find("option").remove();
                          viewModel.choice = null;
                          currentOptions = []; 
                    });
                }

                function optionChanged() {

                    if (viewModel.isMultipleChoices) {
                        var options = $(element).find("option:selected");
                        var kvps = [];

                        options.each((n, e) => {
                            kvps.push({ key: $(e).text(), value: $(e).val() });
                        });
                                  
                        var cvms =  _.map(kvps, (o) =>  ChoiceViewModel.create(new Value(o.value), viewModel.id, o.key));
                        viewModel.multiChoices = cvms;

                    } else {
                        var option = $(element).find("option:selected");

                        var val = option.val();
                        var key = option.text();
                        var cvm = ChoiceViewModel.create(new Value(val), viewModel.id, key);
                        viewModel.choice = cvm;
                        scope.$apply(function() {
                            ngModel.$parsers.push((val) => { return cvm; });
                            ngModel.$setViewValue(cvm.name);
                        });
                    }
                }


                function setListeners() {
                    _.forEach(<_.Dictionary<Value>>pArgs, (v, n) => {
                        $("#" + n + " :input").on("change", (e: JQueryEventObject) => populateDropdown() );
                    });
                    $(element).on("change", optionChanged); 
                }

                ngModel.$render = function () {
                    // initial populate
                    //populateDropdown();
                }; // do on the next event loop,

                setTimeout(() => {
                    setListeners();
                    // initial populate
                    populateDropdown();
                }, 1); 
            }
        };
    });


    app.directive('nogAttachment', function ($window : ng.IWindowService): ng.IDirective {
        return {
            // Enforce the angularJS default of restricting the directive to
            // attributes only
            restrict: 'A',
            // Always use along with an ng-model
            require: '?ngModel',
            link: function (scope: ISelectScope, element, attrs, ngModel: ng.INgModelController) {
                if (!ngModel) {
                    return;
                }

                function downloadFile(url : string, mt : string, success : (resp : Blob) => void ) {
                    var xhr = new XMLHttpRequest();
                    xhr.open('GET', url, true);
                    xhr.responseType = "blob";
                    xhr.setRequestHeader("Accept", mt); 
                    xhr.onreadystatechange = function () {
                        if (xhr.readyState == 4) {
                            success(<Blob>xhr.response);
                        }
                    };
                    xhr.send(null);
                }

                function displayInline(mt: string) {

                    if (mt === "image/jpeg" ||
                        mt === "image/gif" ||
                        mt === "application/octet-stream") {
                        return true;
                    }

                    return false;
                }

                var clickHandler = function () {
                    var attachment: AttachmentViewModel = ngModel.$modelValue;

                    var url = attachment.href;
                    var mt = attachment.mimeType;

                    downloadFile(url, mt, (resp : Blob) => {
                        var burl = URL.createObjectURL(resp); 
                        $window.location.href = burl;                    
                    });
                    return false; 
                };
                ngModel.$render = function () {
                    var attachment: AttachmentViewModel = ngModel.$modelValue;

                    var url = attachment.href;
                    var mt = attachment.mimeType;
                    var title = attachment.title;

                    var link = "<a href='" + url + "'><span></span></a>";
                    element.append(link);

                    var anchor = element.find("a");
                    if (displayInline(mt)) {

                        downloadFile(url, mt, (resp: Blob) => {
                            var reader = new FileReader();
                            reader.onloadend = function () {
                                anchor.html("<img src='" + reader.result + "' alt='" + title + "' />");
                            };
                            reader.readAsDataURL(resp);
                        });
                    }
                    else {
                        anchor.html(title); 
                    }
                  
                    anchor.on('click', clickHandler);                 
                };
            }
        };
    });
}