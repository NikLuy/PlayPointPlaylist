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

window.stopYouTubePlayer = function () {
    if (ytPlayer && ytPlayer.stopVideo) {
        ytPlayer.stopVideo();
    }
};
