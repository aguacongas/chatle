/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp");

var paths = {
    webroot: "./wwwroot/",
    node_modules: "./node_modules/"
};

paths.lib = paths.webroot + "lib/";

paths.signalr = paths.node_modules + "signalr/**/*.js"

gulp.task("copy:signalr", function () {

    return gulp.src(paths.signalr)
			.pipe(gulp.dest(paths.lib));
});

gulp.task("default", gulp.series("copy:signalr"));
