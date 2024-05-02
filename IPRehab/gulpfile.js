/// <binding AfterBuild='cleanMyCss, cleanMyJs, minifyAppJs, copyAppJsMap, compileMySassStyles, minifyMyCss, cleanMyAppModelJs, copyAppModelJsMap, copyMyAppModel, minifyAppModelJs' />
//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var minify = require('gulp-minify');
var sass = require('gulp-sass')(require('sass'));
var gulp = require('gulp');
var del = require('del');

var script = {
    myMap: ['./Scripts/app/*.map','./Scripts/appModels/*.map'],
    myJs: ['./Scripts/app/*.js', './Scripts/appModels/*.js'],
    //myTs: ['./Scripts/app/*.ts'],
    output: ['./wwwroot/js/app']
};

var style = {
    mySass: ['./wwwroot/css/*.scss'],
    myCssOnly: ['./wwwroot/css/*.css', '!./wwwroot/css/site.css','!./wwwroot/cube annimation'],
    output: './wwwroot/css'
}

gulp.task('cleanMyCss', function () {
    return del(style.myCssOnly); /* don't clean .scss */
});

gulp.task('cleanMyJs', function () {
    return del(script.output[0]);
});

gulp.task('minifyAppJs', function () {
    return gulp.src(script.myJs[0], { allowEmpty: true })
        .pipe(minify({
            noSource: false,
            ext: {
                min: '.min.js'
            }
        }))
        .pipe(gulp.dest(script.output[0]));
});

gulp.task('copyAppJsMap', function () {
    return gulp.src(script.myMap[0])
        .pipe(gulp.dest(script.output[0]));
});

gulp.task('minifyAppModelJs', function () {
  return gulp.src(script.myJs[1], { allowEmpty: true })
    .pipe(minify({
      noSource: false,
      ext: {
        min: '.min.js'
      }
    }))
    .pipe(gulp.dest(script.output[0]));

});

gulp.task('copyMyAppModel', function () {
    return gulp.src(script.myJs[1])
        .pipe(gulp.dest(script.output[0]));
});

gulp.task('copyAppModelJsMap', function () {
    return gulp.src(script.myMap[1])
    .pipe(gulp.dest(script.output[0]));
});

//gulp.task('copyAppTs', function () {
//    return gulp.src(script.myTs)
//        .pipe(gulp.dest(script.output[0]));
//});

gulp.task('compileMySassStyles', function (done) {
    gulp.src(style.mySass)
        .pipe(
            sass() /*compile sass */
                .on('error', sass.logError)
        )
        .pipe(
            gulp.dest(style.output)
        );
    done();
});

gulp.task('minifyMyCss', function () {
    return gulp.src(style.myCssOnly, { allowEmpty: true })
        .pipe(minify({
            noSource: false,
            ext: {
                min: '.min.css'
            }
        }))
        .pipe(gulp.dest(style.output));
});


