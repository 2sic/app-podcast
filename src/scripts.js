$(document).ready(function(){
    /* Configure the jPlayers */
    $('.jp-jplayer').each(function() {
        var episodeNumber = $(this).data('episode-number');

        $(this).jPlayer({
            ready: function () {
                $(this).jPlayer('setMedia', {
                    mp3: $(this).data('episode-audio'),
                });
            },
            play: function () {
                // exchange play with pause icon
                $('.jp-play-' + episodeNumber + ' svg').attr('data-icon', 'pause');
                $('.jp-play-' + episodeNumber + ' svg').removeClass('fa-play').addClass('fa-pause');

                showAudioPlayerElements(episodeNumber);

                // pause other instances of player when current one plays
                $(this).jPlayer('pauseOthers');
            },
            timeupdate: function(event) {
                $('.pc-drag-handler-' + episodeNumber).css('left', event.jPlayer.status.currentPercentAbsolute + '%');
            },
            pause: function () {
                // exchange pause with play icon
                $('.jp-play-' + episodeNumber + ' svg').attr('data-icon', 'play');
                $('.jp-play-' + episodeNumber + ' svg').removeClass('fa-pause').addClass('fa-play');
            },
            ended: function () {
                // reset the drag handler
                $('.pc-drag-handler-' + episodeNumber).css('left', '0');

                hideAudioPlayerElements(episodeNumber);
            },
            cssSelectorAncestor: '#jp_container_' + episodeNumber,
            swfPath: '/assets',
            supplied: 'mp3',
            useStateClassSkin: true,
            remainingDuration: true,
            toggleDuration: true,
            preload:  'none',
        })
    });

    /* Format the episode duration */
    $('.pc-episode-duration').each(function() {
        var hr = ~~($(this).context.innerHTML / 60);
        var min = ~~($(this).context.innerHTML % 60);
        var formatedDuration = hr + ' hr ' + min + ' min';

        $(this).context.innerHTML = formatedDuration;
    });

    /* Show audio player elements */
    var showAudioPlayerElements = function(episodeNumber) {
        $('.jp-audio-' + episodeNumber).css('width', '100%');
        $('.jp-interface-' + episodeNumber).css('width', '30%');
        $('.jp-progress-' + episodeNumber).css('display', 'block');
        $('.jp-current-time-' + episodeNumber + ', .jp-duration-' + episodeNumber).css('display', 'inline');
        $('.pc-episode-duration-' + episodeNumber).css('display', 'none');
    }

    /* Hide audio player elements */
    var hideAudioPlayerElements = function(episodeNumber) {
        $('.jp-audio-' + episodeNumber).css('width', '46px');
        $('.jp-interface-' + episodeNumber).css('width', '100%');
        $('.jp-progress-' + episodeNumber).css('display', 'none');
        $('.jp-current-time-' + episodeNumber + ', .jp-duration-' + episodeNumber).css('display', 'none');
        $('.pc-episode-duration-' + episodeNumber).css('display', 'inline');
    }

    /* Drag handler */
    var timeDrag = false; // Drag status
    var episodeNumber = 0;
    $('.pc-drag-handler').draggable({
        axis: 'x',
        containment: 'parent',
        start: function (e) {
            episodeNumber = $(this).data('episode-number');
            $('#jquery_jplayer_' + episodeNumber).jPlayer('pause');
            timeDrag = true;
            $('.pc-drag-handler-' + episodeNumber).addClass('dragging');
            updateBar(e.pageX);
        },
        drag: function (e) {
            if (timeDrag) {
                updateBar(e.pageX);
            }
        },
        stop: function (e) {
            if (timeDrag) {
                timeDrag = false;
                $('.pc-drag-handler-' + episodeNumber).removeClass('dragging');
                updateBar(e.pageX);
                $('#jquery_jplayer_' + episodeNumber).jPlayer('play');
            }
        }
    });

    /* Update Progress Bar control */
    var updateBar = function (x) {
        var progress = $('.jp-progress-' + episodeNumber);
        var maxduration = $('#jquery_jplayer_' + episodeNumber).data('jPlayer').status.duration; // audio duration
        var position = x - progress.offset().left; //Click pos
        var percentage = 100 * position / progress.width();

        // Check within range
        if (percentage > 100) {
            percentage = 100;
        }
        if (percentage < 0) {
            percentage = 0;
        }

        $('#jquery_jplayer_' + episodeNumber).jPlayer('playHead', percentage);

        // Update progress bar and audio currenttime
        $('.pc-drag-handler-' + episodeNumber).css('left', percentage + '%');
        $('.jp-play-bar-' + episodeNumber).css('width', percentage + '%');
        $('#jquery_jplayer_' + episodeNumber).jPlayer.currentTime = maxduration * percentage / 100;
    };
});