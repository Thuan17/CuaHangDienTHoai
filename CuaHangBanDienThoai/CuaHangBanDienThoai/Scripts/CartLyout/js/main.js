(function($) {

	"use strict";

	$(window).on('load', function () {
	    $(".loader").fadeOut();
	    $("#preloder").delay(200).fadeOut("slow");

	    /*------------------
	        Gallery filter
	    --------------------*/
	    $('.featured__controls li').on('click', function () {
	        $('.featured__controls li').removeClass('active');
	        $(this).addClass('active');
	    });
	    if ($('.featured__filter').length > 0) {
	        var containerEl = document.querySelector('.featured__filter');
	        var mixer = mixitup(containerEl);
	    }
	});
	var fullHeight = function() {

		$('.js-fullheight').css('height', $(window).height());
		$(window).resize(function(){
			$('.js-fullheight').css('height', $(window).height());
		});

	};
	fullHeight();

	$(".toggle-password").click(function() {

	  $(this).toggleClass("fa-eye fa-eye-slash");
	  var input = $($(this).attr("toggle"));
	  if (input.attr("type") == "password") {
	    input.attr("type", "text");
	  } else {
	    input.attr("type", "password");
	  }
	});
	$(document).on('scroll', function () {
		var scrollDistance = $(this).scrollTop();
		if (scrollDistance > 100) {
			$('.scroll-to-top').fadeIn();
		} else {
			$('.scroll-to-top').fadeOut();
		}
	});

	
	$(document).on('click', 'a.scroll-to-top', function (e) {
		var $anchor = $(this);
		
		$('html, body').stop().animate({
			scrollTop: 0
		}, 1000, 'easeInOutExpo');
		e.preventDefault();
	});
})(jQuery);
