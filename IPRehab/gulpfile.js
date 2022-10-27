/// <binding AfterBuild='cleanMyCss, cleanMyJs, minifyAppJs, copyAppJsMap, copyAppTs, compileMySassStyles, minifyMyCss' />
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
    myMap: ['./Scripts/app/*.map'],
    myJs: ['./Scripts/app/*.js'],
    myTs: ['./Scripts/app/*.ts'],
    output: './wwwroot/js/app'
};

var style = {
    mySass: ['./wwwroot/css/*.scss'],
    myCssOnly: ['./wwwroot/css/**/*.css', '!./wwwroot/css/**/site.css'],
    output: './wwwroot/css'
}

gulp.task('cleanMyCss', function () {
    return del(style.myCssOnly); /* don't clean .scss */
});


gulp.task('cleanMyJs', function () {
    return del(script.output);
});


gulp.task('minifyAppJs', function () {
    return gulp.src(script.myJs, { allowEmpty: true })
        .pipe(minify({
            noSource: false,
            ext: {
                min: '.min.js'
            }
        }))
        .pipe(gulp.dest(script.output));
});

gulp.task('copyAppJsMap', function () {
    return gulp.src(script.myMap)
        .pipe(gulp.dest(script.output));
});

gulp.task('copyAppTs', function () {
    return gulp.src(script.myTs)
        .pipe(gulp.dest(script.output));
});

//gulp.task('minifyAppModelsJs', function () {
//  return gulp.src(script.myJs[1], { allowEmpty: true })
//    .pipe(minify({
//      noSource: false,
//      ext: {
//        min: '.min.js'
//      }
//    }))
//    .pipe(gulp.dest(script.output[0]));
//});

//gulp.task('copyAppModelsJsMap', function () {
//    return gulp.src(script.myMap[1])
//    .pipe(gulp.dest(script.output[0]));
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


