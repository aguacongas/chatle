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

paths.lib = paths.webroot + "lib/";

paths.angular = paths.node_modules + "@angular/**/bundles/*.js"
paths.angularWebApi = paths.node_modules + "angular2-in-memory-web-api/*.js"
paths.corejs = paths.node_modules + "core-js/client/shim*.js";
paths.zonejs = paths.node_modules + "zone.js/dist/zone*.js";
paths.reflectjs = paths.node_modules + "reflect-metadata/Reflect*.js";
paths.systemjs = paths.node_modules + "systemjs/dist/system*.js";
paths.rxjs = paths.node_modules + "rxjs/**/*.js";

paths.test = "app.test/**/*.js";
paths.testDest = paths.webroot + "js/test";

paths.app = "../../src/chatle/app/**/*.ts";
paths.appDest = "app";

gulp.task("copy:angular", function () {
	return gulp.src(paths.angular,
				{ base: paths.node_modules + "@angular/" })
			.pipe(gulp.dest(paths.lib + "angular/"));
});

gulp.task("copy:angularWebApi", function () {
	return gulp.src(paths.angularWebApi,
				{ base: paths.node_modules })
			.pipe(gulp.dest(paths.lib));
});

gulp.task("copy:corejs", function () {
	return gulp.src(paths.corejs,
				{ base: paths.node_modules })
			.pipe(gulp.dest(paths.lib));
});

gulp.task("copy:zonejs", function () {
	return gulp.src(paths.zonejs,
				{ base: paths.node_modules })
			.pipe(gulp.dest(paths.lib));
});

gulp.task("copy:reflectjs", function () {
	return gulp.src(paths.reflectjs,
				{ base: paths.node_modules })
			.pipe(gulp.dest(paths.lib));
});

gulp.task("copy:systemjs", function () {
	return gulp.src(paths.systemjs,
				{ base: paths.node_modules })
			.pipe(gulp.dest(paths.lib));
});

gulp.task("copy:rxjs", function () {
	return gulp.src(paths.rxjs,
				{ base: paths.node_modules })
			.pipe(gulp.dest(paths.lib));
});

gulp.task("copy:test", function () {
	return gulp.src(paths.test + "*")
			.pipe(gulp.dest(paths.testDest));
});

gulp.task("copy:app", function () {
	return gulp.src(paths.app + "*")
			.pipe(gulp.dest(paths.appDest));
});

gulp.task("dependencies", [ "copy:angular", 
					"copy:angularWebApi", 
					"copy:corejs", 
					"copy:zonejs",
					"copy:reflectjs",
					"copy:systemjs",
					"copy:rxjs",
					"copy:test" ]);

gulp.task("watch", function() {
	return watch(paths.test + "*")
			.pipe(gulp.dest(paths.testDest))
});

gulp.task("default", ["copy:app", "dependencies"]);
