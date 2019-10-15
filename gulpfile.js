var gulp = require('gulp');
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');

gulp.task('watch-sass', function () {
	gulp.watch(['./src/scss/*.scss'], ['sass']);
});

gulp.task('watch-javascript', function () {
	gulp.watch(['./src/js/*.js'], ['javascript']);
});

gulp.task('sass', function () {
  return gulp.src('./src/scss/*.scss')
	.pipe(sourcemaps.init())
    .pipe(sass().on('error', sass.logError))
	.pipe(sourcemaps.write('.', { sourceRoot: '../src/' }))
    .pipe(gulp.dest('./dist/'));
});

gulp.task('javascript', function() {
	return gulp.src('./src/js/*.js')
		.pipe(gulp.dest('./dist/'));
});

gulp.task('default', ['sass', 'javascript', 'watch-sass', 'watch-javascript']);