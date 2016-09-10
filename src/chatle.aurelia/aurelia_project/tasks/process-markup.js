define(["require", "exports", 'gulp', 'gulp-changed-in-place', '../aurelia.json', 'aurelia-cli'], function (require, exports, gulp, changedInPlace, project, aurelia_cli_1) {
    "use strict";
    function processMarkup() {
        return gulp.src(project.markupProcessor.source)
            .pipe(changedInPlace({ firstPass: true }))
            .pipe(aurelia_cli_1.build.bundle());
    }
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = processMarkup;
});
//# sourceMappingURL=process-markup.js.map