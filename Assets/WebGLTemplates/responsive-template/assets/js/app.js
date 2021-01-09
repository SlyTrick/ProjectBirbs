var isFullscreen = false;
var ratio = window.devicePixelRatio || 1;

// update canvas size
var refreshCanvas = function(){
	var isTouch = ('ontouchstart' in document.documentElement);

	if ( !isTouch ) {
		var newWidth = $( ".webgl-content" ).width();
		var newHeight = (9/16) * newWidth;
		$("#unityContainer").width(newWidth);
		$("#unityContainer").height(newHeight);
		//unityInstance.SetFullscreen(0);
    }else{
		$("#unityContainer").width(window.screen.width);
		$("#unityContainer").height(window.screen.height);
	}
  /**/
};

$(function() {
  // scale canvas correctly once on start
  refreshCanvas();
});

// on window resize, apply width to game
$( window ).resize(function() {
  refreshCanvas();
});

$( window ).on( "load", function() {
   unityInstance.SetFullscreen(0);
});


// check for fullscreen toggle
/*$(document).keydown(function(event) {
  var keycode = (event.keyCode ? event.keyCode : event.which);
  if(keycode == '70'){
    if (!isFullscreen) {
      unityInstance.SetFullscreen(1);
      isFullscreen = true;
    }
    else {
      unityInstance.SetFullscreen(0);
      isFullscreen = false;
    }
  }
});*/
