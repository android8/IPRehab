/// <binding AfterBuild='cleanDestination, compileStyles, minifyCss, minifyAppJs, copyAppJsMap' />
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
  myMap: ['./Scripts/app/**/*.map'],
  myJs: ['./Scripts/app/*.js'],
  destinations: ['./wwwroot/js/app']
};

var styleSources = {
  mySass: ['./wwwroot/css/**/*.scss'],
  myCss: ['./wwwroot/css/**/*.css'],
  destinations: ['./wwwroot/css']
}

gulp.task('cleanDestination', function () {
  return del(scriptSources.destinations);
});

gulp.task('minifyAppJs', function () {
  return gulp.src(scriptSources.myJs[0], { allowEmpty: true })
    .pipe(minify({
      noSource: false,
      ext: {
        min: '.min.js'
      }
    }))
    .pipe(gulp.dest(scriptSources.destinations));
});

gulp.task('copyAppJsMap', function () {
  return gulp.src(scriptSources.myMap)
    .pipe(gulp.dest(scriptSources.destinations));
});

//gulp.task('minifyAppModelsJs', function () {
//  return gulp.src(scriptSources.myJs[0], { allowEmpty: true })
//    .pipe(minify({
//      noSource: false,
//      ext: {
//        min: '.min.js'
//      }
//    }))
//    .pipe(gulp.dest(scriptSources.destinations[0]));
//});

//gulp.task('copyAppModelsJsMap', function () {
//    return gulp.src(scriptSources.myMap[0])
//    .pipe(gulp.dest(scriptSources.destinations[0]));
//});


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

gulp.task('minifyCss', function () {
  return gulp.src(styleSources.myCss, { allowEmpty: true })
    .pipe(minify({
      noSource: false,
      ext: {
        min: '.min.css'
      }
    }))
    .pipe(gulp.dest(styleSources.destinations));
});


