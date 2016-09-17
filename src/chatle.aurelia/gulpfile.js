/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp");

var paths = {
    webroot: "./wwwroot/node_modules",
    node_modules: "./node_modules/**/*.js"
};

gulp.task("copy:lib", function () {

    return gulp.src(paths.node_modules)
			.pipe(gulp.dest(paths.webroot));
});

gulp.task("default", gulp.series("copy:lib"));
