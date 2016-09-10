var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
define(["require", "exports", 'aurelia-dependency-injection', 'aurelia-cli'], function (require, exports, aurelia_dependency_injection_1, aurelia_cli_1) {
    "use strict";
    var TaskGenerator = (function () {
        function TaskGenerator(project, options, ui) {
            this.project = project;
            this.options = options;
            this.ui = ui;
        }
        TaskGenerator.prototype.execute = function () {
            var _this = this;
            return this.ui
                .ensureAnswer(this.options.args[0], 'What would you like to call the task?')
                .then(function (name) {
                var fileName = _this.project.makeFileName(name);
                var functionName = _this.project.makeFunctionName(name);
                _this.project.tasks.add(aurelia_cli_1.ProjectItem.text(fileName + ".ts", _this.generateSource(functionName)));
                return _this.project.commitChanges()
                    .then(function () { return _this.ui.log("Created " + fileName + "."); });
            });
        };
        TaskGenerator.prototype.generateSource = function (functionName) {
            return "import * as gulp from 'gulp';\nimport * as changed from 'gulp-changed';\nimport * as project from '../aurelia.json';\n\nexport default function " + functionName + "() {\n  return gulp.src(project.paths.???)\n    .pipe(changed(project.paths.output, {extension: '.???'}))\n    .pipe(gulp.dest(project.paths.output));\n}\n\n";
        };
        TaskGenerator = __decorate([
            aurelia_dependency_injection_1.inject(aurelia_cli_1.Project, aurelia_cli_1.CLIOptions, aurelia_cli_1.UI), 
            __metadata('design:paramtypes', [(typeof (_a = typeof aurelia_cli_1.Project !== 'undefined' && aurelia_cli_1.Project) === 'function' && _a) || Object, (typeof (_b = typeof aurelia_cli_1.CLIOptions !== 'undefined' && aurelia_cli_1.CLIOptions) === 'function' && _b) || Object, (typeof (_c = typeof aurelia_cli_1.UI !== 'undefined' && aurelia_cli_1.UI) === 'function' && _c) || Object])
        ], TaskGenerator);
        return TaskGenerator;
        var _a, _b, _c;
    }());
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = TaskGenerator;
});
//# sourceMappingURL=task.js.map