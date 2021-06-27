/// <binding AfterBuild='cleanDestination, copyScripts, compileStyles' />

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var sass = require('gulp-sass')(require('sass'));
var gulp = require('gulp');
var del = require('del');

var scriptFiles = {
  myTypeScripts: ['./Scripts/**/*.js', './Scripts/**/*.ts', './Scripts/**/*.map'],
  destinations: ['./wwwroot/js']
};

var styleFiles = {
  mySass: ['./wwwroot/css/**/*.scss'],
  destinations: ['./wwwroot/css']
}

gulp.task('cleanDestination', function () {
  return del(['./wwwroot/js/**/*']);
});

gulp.task('copyScripts', function () {
  return gulp.src(scriptFiles.myTypeScripts)
    .pipe(gulp.dest(scriptFiles.destinations))
});

gulp.task('compileStyles', function (done) {
  gulp.src(styleFiles.mySass)
    .pipe(sass().on('error', sass.logError))
    .pipe(
      gulp.dest(styleFiles.destinations)
    );
  done();
});


