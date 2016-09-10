define(["require", "exports", 'gulp', 'gulp-changed-in-place', 'gulp-plumber', 'gulp-sourcemaps', 'gulp-notify', 'gulp-rename', 'gulp-typescript', '../aurelia.json', 'aurelia-cli', 'event-stream'], function (require, exports, gulp, changedInPlace, plumber, sourcemaps, notify, rename, ts, project, aurelia_cli_1, eventStream) {
    "use strict";
    function configureEnvironment() {
        var env = aurelia_cli_1.CLIOptions.getEnvironment();
        return gulp.src("aurelia_project/environments/" + env + ".ts")
            .pipe(changedInPlace({ firstPass: true }))
            .pipe(rename('environment.ts'))
            .pipe(gulp.dest(project.paths.root));
    }
    var typescriptCompiler = typescriptCompiler || null;
    function buildTypeScript() {
        if (!typescriptCompiler) {
            typescriptCompiler = ts.createProject('tsconfig.json', {
                "typescript": require('typescript')
            });
        }
        var dts = gulp.src(project.transpiler.dtsSource);
        var src = gulp.src(project.transpiler.source)
            .pipe(changedInPlace({ firstPass: true }));
        return eventStream.merge(dts, src)
            .pipe(plumber({ errorHandler: notify.onError('Error: <%= error.message %>') }))
            .pipe(sourcemaps.init())
            .pipe(ts(typescriptCompiler))
            .pipe(aurelia_cli_1.build.bundle());
    }
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = gulp.series(configureEnvironment, buildTypeScript);
});
//# sourceMappingURL=transpile.js.map