// JavaScript helper functions for PlayPointPlaylist

// Download file from base64 content
window.downloadFile = function (fileName, base64Content) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = 'data:text/plain;base64,' + base64Content;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// YouTube Player Management
let ytPlayer = null;
let ytPlayerReady = false;
let dotNetHelper = null;
let checkTimeInterval = null;
const SKIP_BEFORE_END_SECONDS = 5; // Skip 5 seconds before video ends

// Load YouTube IFrame API
function loadYouTubeAPI() {
    if (window.YT) {
        return;
    }
    
    const tag = document.createElement('script');
    tag.src = 'https://www.youtube.com/iframe_api';
    const firstScriptTag = document.getElementsByTagName('script')[0];
    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
}

// Called automatically by YouTube API when ready
window.onYouTubeIframeAPIReady = function () {
    ytPlayerReady = true;
    console.log('YouTube IFrame API Ready');
};

window.setDotNetHelper = function (helper) {
    dotNetHelper = helper;
};

function startTimeCheck() {
    // Clear any existing interval
    if (checkTimeInterval) {
        clearInterval(checkTimeInterval);
    }
    
    // Check every second if we're near the end
    checkTimeInterval = setInterval(() => {
        if (ytPlayer && ytPlayer.getCurrentTime && ytPlayer.getDuration && ytPlayer.getPlayerState) {
            const playerState = ytPlayer.getPlayerState();
            
            // YT.PlayerState.PLAYING = 1
            // Only check time if video is actually playing
            if (playerState === 1) {
                const currentTime = ytPlayer.getCurrentTime();
                const duration = ytPlayer.getDuration();
                
                // If we're within SKIP_BEFORE_END_SECONDS of the end, auto-skip
                if (duration > 0 && currentTime > 0 && (duration - currentTime) <= SKIP_BEFORE_END_SECONDS) {
                    console.log(`Near end (${(duration - currentTime).toFixed(1)}s remaining), auto-skipping...`);
                    clearInterval(checkTimeInterval);
                    if (dotNetHelper) {
                        dotNetHelper.invokeMethodAsync('OnVideoEnded');
                    }
                }
            }
            // If paused, we just wait - don't skip
        }
    }, 1000); // Check every second
}

window.initializeYouTubePlayer = function (videoId, hideControls) {
    loadYouTubeAPI();
    
    // Wait for API to be ready
    const checkReady = setInterval(() => {
        if (window.YT && window.YT.Player) {
            clearInterval(checkReady);
            
            // Destroy existing player if exists
            if (ytPlayer) {
                ytPlayer.destroy();
            }
            
            // Clear any existing time check
            if (checkTimeInterval) {
                clearInterval(checkTimeInterval);
            }
            
            // Create new player
            ytPlayer = new YT.Player('youtube-player', {
                videoId: videoId,
                playerVars: {
                    autoplay: 1,
                    controls: hideControls ? 0 : 1,
                    modestbranding: hideControls ? 1 : 0,
                    rel: 0
                },
                events: {
                    onReady: function(event) {
                        event.target.playVideo();
                        // Start checking time once video is ready
                        startTimeCheck();
                    },
                    onStateChange: function(event) {
                        // YT.PlayerState.PLAYING = 1
                        if (event.data === 1) {
                            // Video started/resumed playing, ensure time check is running
                            console.log('Video playing, starting time check');
                            startTimeCheck();
                        }
                        // YT.PlayerState.PAUSED = 2
                        else if (event.data === 2) {
                            // Video paused, no need to check time (but keep interval running)
                            console.log('Video paused, time check suspended');
                        }
                        // YT.PlayerState.ENDED = 0 (backup in case time check fails)
                        else if (event.data === 0) {
                            console.log('Video ended (fallback)');
                            clearInterval(checkTimeInterval);
                            if (dotNetHelper) {
                                dotNetHelper.invokeMethodAsync('OnVideoEnded');
                            }
                        }
                    }
                }
            });
        }
    }, 100);
};

window.updateYouTubePlayerControls = function (hideControls) {
    // Player already loaded, we can't change controls dynamically
    // But we can note this for next video load
    console.log('Controls visibility updated:', !hideControls);
};

window.loadNewVideo = function (videoId, hideControls) {
    if (ytPlayer && ytPlayer.loadVideoById) {
        ytPlayer.loadVideoById(videoId);
    } else {
        window.initializeYouTubePlayer(videoId, hideControls);
    }
};

window.getPlayerTime = function () {
    if (ytPlayer && ytPlayer.getCurrentTime && ytPlayer.getDuration) {
        try {
            const currentTime = ytPlayer.getCurrentTime();
            const duration = ytPlayer.getDuration();
            return [currentTime, duration];
        } catch (e) {
            return [0, 0];
        }
    }
    return [0, 0];
};

window.stopYouTubePlayer = function () {
    // Clear time check interval
    if (checkTimeInterval) {
        clearInterval(checkTimeInterval);
        checkTimeInterval = null;
    }
    
    // Stop the player
    if (ytPlayer && ytPlayer.stopVideo) {
        ytPlayer.stopVideo();
    }
};
