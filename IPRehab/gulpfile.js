/// <binding AfterBuild='cleanDestination, copySelect2Styles, copyJqueryUiStyles, copyDataTableStyles, copyLoadAwesomeStyles, copyBootswatchStyles, copyLibs, copyMyScripts' />

//https://www.typescriptlang.org/docs/handbook/asp-net-core.html

/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var del = require('del');

var sources = {
   myScripts: ['./scripts/**/*.js', './scripts/**/*.ts', './scripts/**/*.map']
 };

gulp.task('cleanDestination', function () {
  return del([
    './wwwroot/css/**/*',
    './wwwroot/scripts/**/*',
    './wwwroot/lib/**/*']);
});


gulp.task('copyMyScripts', function () {
  return gulp.src(sources.myScripts)
    .pipe(gulp.dest('./wwwroot/scripts'))
});



