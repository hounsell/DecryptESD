/// <binding BeforeBuild='sass-compile' ProjectOpened='watch-sass' />
var gulp = require('gulp');
var sass = require('gulp-sass');

gulp.task('sass-compile',
    function() {
        gulp.src('./res/scss/default.scss')
            .pipe(sass())
            .pipe(gulp.dest('./res/scss/'));
    });

gulp.task('watch-sass',
    function() {
        gulp.watch('./res/scss/*.scss', ['sass-compile']);
    });