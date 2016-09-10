define(["require", "exports", 'gulp', './transpile', './process-markup', './process-css', 'aurelia-cli', '../aurelia.json'], function (require, exports, gulp, transpile_1, process_markup_1, process_css_1, aurelia_cli_1, project) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = gulp.series(readProjectConfiguration, gulp.parallel(transpile_1.default, process_markup_1.default, process_css_1.default), writeBundles);
    function readProjectConfiguration() {
        return aurelia_cli_1.build.src(project);
    }
    function writeBundles() {
        return aurelia_cli_1.build.dest();
    }
});
//# sourceMappingURL=build.js.map