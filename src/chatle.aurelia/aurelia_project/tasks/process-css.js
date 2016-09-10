define(["require", "exports", 'gulp', 'gulp-changed-in-place', '../aurelia.json', 'aurelia-cli'], function (require, exports, gulp, changedInPlace, project, aurelia_cli_1) {
    "use strict";
    function processCSS() {
        return gulp.src(project.cssProcessor.source)
            .pipe(changedInPlace({ firstPass: true }))
            .pipe(aurelia_cli_1.build.bundle());
    }
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = processCSS;
    ;
});
//# sourceMappingURL=process-css.js.map