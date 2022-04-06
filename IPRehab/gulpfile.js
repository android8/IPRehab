/// <binding AfterBuild='cleanDestination, compileStyles, minifyAppModelsJs, minifyAppJs, copyMaps' />
//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var minify = require('gulp-minify');
var sass = require('gulp-sass')(require('sass'));
var gulp = require('gulp');
var del = require('del');

var scriptSources = {
  map: ['./Scripts/**/*.map'],
  destinations: ['./wwwroot/js']
};

var styleSources = {
  mySass: ['./wwwroot/css/**/*.scss'],
  destinations: ['./wwwroot/css']
}

gulp.task('cleanDestination', function () {
  return del(['./wwwroot/js/**/*']);
});

gulp.task('minifyAppJs', function () {
  return gulp.src(['./Scripts/app/*.js'], { allowEmpty: true })
    .pipe(minify({
      noSource: false,
      ext: {
        min: '.min.js'
      } }))
    .pipe(gulp.dest('./wwwroot/js/app'));
});

gulp.task('minifyAppModelsJs', function () {
  return gulp.src(['./Scripts/appModels/*.js'], { allowEmpty: true })
    .pipe(minify({
      noSource: false,
      ext: {
        min: '.min.js'
      }
    }))
    .pipe(gulp.dest('./wwwroot/js/appModels'));
});

gulp.task('copyMaps', function () {
  return gulp.src(scriptSources.map)
    .pipe(gulp.dest(scriptSources.destinations))
});

gulp.task('compileStyles', function (done) {
  gulp.src(styleSources.mySass)
    .pipe(
      sass() /*compile sass */
      .on('error', sass.logError)
    ) 
    .pipe(
      gulp.dest(styleSources.destinations)
    );
  done();
});


