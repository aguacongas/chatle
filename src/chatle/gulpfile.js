/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
        rimraf = require("rimraf"),
        concat = require("gulp-concat"),
        cssmin = require("gulp-cssmin"),
        uglify = require("gulp-uglify"),
		copy= require("gulp-copy"),
		rename = require("gulp-rename");

var paths = {
	webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/chat.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";
paths.angular = "node_modules/@angular/**/bundles/*.js"
paths.lib = paths.webroot + "lib/";

gulp.task("clean:js", function (cb) {
	rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
	rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
	return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
			.pipe(concat(paths.concatJsDest))
			.pipe(uglify())
			.pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
	return gulp.src([paths.css, "!" + paths.minCss])
			.pipe(concat(paths.concatCssDest))
			.pipe(cssmin())
			.pipe(gulp.dest("."));
});

gulp.task("angular", function () {
	return gulp.src(paths.angular)
			.pipe(rename({ dirname: '' }))
			.pipe(gulp.dest(paths.lib + "angular"))
});

gulp.task("min", ["min:js", "min:css"]);

gulp.task("default", ["clean", "min"]);
