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
/// <reference path="typings/underscore/underscore.d.ts" />
/// <reference path="spiro.models.ts" />
var Spiro;
(function (Spiro) {
    var Angular;
    (function (Angular) {
        var Modern;
        (function (Modern) {
            Angular.app.service('navigation', function ($location) {
                var nav = this;
                var history = [];
                var index = -1;
                var navigating = false;
                nav.back = function () {
                    if ((index - 1) >= 0) {
                        index--;
                        navigating = true;
                        $location.url(history[index]);
                    }
                };
                nav.forward = function () {
                    if ((index + 1) <= (history.length - 1)) {
                        index++;
                        navigating = true;
                        $location.url(history[index]);
                    }
                };
                nav.push = function () {
                    if (!navigating) {
                        var newUrl = $location.url();
                        var curUrl = history[history.length - 1];
                        var isActionUrl = newUrl.indexOf("?action") > 0;
                        if (!isActionUrl && newUrl != curUrl) {
                            history.push($location.url());
                        }
                        index = history.length - 1;
                    }
                    navigating = false;
                };
            });
        })(Modern = Angular.Modern || (Angular.Modern = {}));
    })(Angular = Spiro.Angular || (Spiro.Angular = {}));
})(Spiro || (Spiro = {}));
//# sourceMappingURL=spiro.modern.services.navigation.simple.js.map