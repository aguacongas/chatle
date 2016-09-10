define(["require", "exports", 'gulp', 'browser-sync', 'connect-history-api-fallback/lib', '../aurelia.json', './build', 'aurelia-cli'], function (require, exports, gulp, browserSync, historyApiFallback, project, build_1, aurelia_cli_1) {
    "use strict";
    function onChange(path) {
        console.log("File Changed: " + path);
    }
    function reload(done) {
        browserSync.reload();
        done();
    }
    var serve = gulp.series(build_1.default, function (done) {
        browserSync({
            online: false,
            open: false,
            port: 9000,
            logLevel: 'silent',
            server: {
                baseDir: ['.'],
                middleware: [historyApiFallback(), function (req, res, next) {
                        res.setHeader('Access-Control-Allow-Origin', '*');
                        next();
                    }]
            }
        }, function (err, bs) {
            var urls = bs.options.get('urls').toJS();
            console.log("Application Available At: " + urls.local);
            console.log("BrowserSync Available At: " + urls.ui);
            done();
        });
    });
    var refresh = gulp.series(build_1.default, reload);
    var watch = function () {
        gulp.watch(project.transpiler.source, refresh).on('change', onChange);
        gulp.watch(project.markupProcessor.source, refresh).on('change', onChange);
        gulp.watch(project.cssProcessor.source, refresh).on('change', onChange);
    };
    var run;
    if (aurelia_cli_1.CLIOptions.hasFlag('watch')) {
        run = gulp.series(serve, watch);
    }
    else {
        run = serve;
    }
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = run;
});
//# sourceMappingURL=run.js.map