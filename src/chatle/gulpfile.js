/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
        rimraf = require("gulp-rimraf"),
        concat = require("gulp-concat"),
        cssmin = require("gulp-cssmin"),
        uglify = require("gulp-uglify"),
		copy= require("gulp-copy"),
		rename = require("gulp-rename"),
		watch = require("gulp-watch"),
		tsc = require("gulp-tsc");

var paths = {
	webroot: "./wwwroot/",
	node_modules:"./node_modules/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/chat.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

paths.lib = paths.webroot + "lib/";

paths.angular = paths.node_modules + "@angular/**/bundles/*.js"
paths.angularWebApi = paths.node_modules + "angular2-in-memory-web-api/*.js"
paths.corejs = paths.node_modules + "core-js/client/shim*.js";
paths.zonejs = paths.node_modules + "zone.js/dist/zone*.js";
paths.reflectjs = paths.node_modules + "reflect-metadata/Reflect*.js";
paths.systemjs = paths.node_modules + "systemjs/dist/system.*.js";
paths.rxjs = paths.node_modules + "rxjs/bundles/*.js";

paths.app = "app/**/*.js";
paths.appDest = paths.webroot + "js/app/app.min.js";

gulp.task("clean:js", function () {
	return gulp.src([ paths.concatJsDest, paths.appDest ], { read: false })
			.pipe(rimraf());
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
	return gulp.src([
				paths.angular,
				paths.angularWebApi,
				paths.corejs, 
				paths.zonejs, 
				paths.reflectjs,
				paths.rxjs ],
				{ base: "." })
			.pipe(rename({ dirname: '' }))
			.pipe(gulp.dest(paths.lib + "angular"));
});


gulp.task("watch", function() {
	var source = "app/**/*.js*";
	var dest = paths.webroot + "js/app";
	return gulp.src(source)
			.pipe(gulp.dest(dest))
			.pipe(watch(source))
			.pipe(gulp.dest(dest))
});

gulp.task("min:app", function() {
	return gulp.src(paths.app)
			.pipe(concat(paths.appDest))
			.pipe(uglify())
			.pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css", "min:app"]);

gulp.task("default", ["clean", "min", "angular"]);
