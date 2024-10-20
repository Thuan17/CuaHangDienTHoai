/* JS Document */

/******************************

[Table of Contents]

1. Vars and Inits
2. Set Header
3. Init Menu
4. Init Timer
5. Init Favorite
6. Init Fix Product Border
7. Init Isotope Filtering
8. Init Slider


******************************/

jQuery(document).ready(function ($) {
	"use strict";

	//// Vars and Inits
	//var hamburgerMenu = $('.hamburger_container_Menu');
	//var hamburgerCart = $('.hamburger_container_cart');
	//var hamburgerCartNavbarUser = $('.hamburger_container_cart_navbar_user');
	//var menu = $('.hamburger_menu');
	//var menuCart = $('.hamburger_menu_cart');
	//var fsOverlay = $('.fs_menu_overlay');
	//var fsOverlayCart = $('.fs_menu_overlay_cart');
	//var hamburgerClose = $('.hamburger_close');
	//var hamburgerCloseCart = $('.hamburger_close_cart');
	//var menuTopNav = $('.menu_top_nav');
	//var menuTopNavCart = $('.menu_top_nav_cart');



	initIsotopeFiltering();
	var swiper = new Swiper('.product-swiper', {
		slidesPerView: 4,
		spaceBetween: 30,
		pagination: false, // Vô hiệu hóa phần pagination
	
		loop: true
	});

	//var menuActive = false;
	//var menuActiveCart = false;

	//// Event Listeners for Toggle Buttons
	//hamburgerMenu.on('click', function (event) {
	//	event.stopPropagation();
	//	if (menuActiveCart) {
	//		closeMenu(menuCart, fsOverlayCart);
	//	}
	//	if (!menuActive) {
	//		openMenu(menu, fsOverlay);
	//	} else {
	//		closeMenu(menu, fsOverlay);
	//	}
	//});

	//hamburgerCart.on('click', function (event) {
	//	event.stopPropagation();
	//	if (menuActive) {
	//		closeMenu(menu, fsOverlay);
	//	}
	//	if (!menuActiveCart) {
	//		openMenu(menuCart, fsOverlayCart);
	//	} else {
	//		closeMenu(menuCart, fsOverlayCart);
	//	}
	//});

	//hamburgerCartNavbarUser.on('click', function (event) {
	//	event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ <a>
	//	event.stopPropagation();
	//	if (menuActive) {
	//		closeMenu(menu, fsOverlay);
	//	}
	//	if (!menuActiveCart) {
	//		openMenu(menuCart, fsOverlayCart);
	//	} else {
	//		closeMenu(menuCart, fsOverlayCart);
	//	}
	//});

	//// Close Menu on Overlay Click
	//fsOverlay.on('click', function () {
	//	if (menuActive) {
	//		closeMenu(menu, fsOverlay);
	//	}
	//});

	//fsOverlayCart.on('click', function () {
	//	if (menuActiveCart) {
	//		closeMenu(menuCart, fsOverlayCart);
	//	}
	//});

	//// Close Menu on Close Button Click
	//menu.on('click', function () {
	//	if (menuActive) {
	//		closeMenu(menu, fsOverlay);
	//	}
	//});

	//menuCart.on('click', function () {
	//	if (menuActiveCart) {
	//		closeMenu(menuCart, fsOverlayCart);
	//	}
	//});

	//// Close Menu on Close Button Click
	//hamburgerClose.on('click', function () {
	//	if (menuActive) {
	//		closeMenu(menu, fsOverlay);
	//	}
	//});

	//hamburgerCloseCart.on('click', function () {
	//	if (menuActiveCart) {
	//		closeMenu(menuCart, fsOverlayCart);
	//	}
	//});
	//// Prevent closing the menu when clicking inside the specific menu content
	//menuTopNav.on('click', function (event) {
	//	event.stopPropagation();
	//});

	//menuTopNavCart.on('click', function (event) {
	//	event.stopPropagation();
	//});

	//// Close menu if clicking outside of menu or cart content
	//$(document).on('click', function (event) {
	//	var target = $(event.target);
	//	if (!target.closest('.hamburger_menu').length && menuActive) {
	//		closeMenu(menu, fsOverlay);
	//	}
	//	if (!target.closest('.hamburger_menu_cart').length && menuActiveCart) {
	//		closeMenu(menuCart, fsOverlayCart);
	//	}
	//});

	//function openMenu(menu, fsOverlay) {
	//	menu.addClass('active');
	//	fsOverlay.css('pointer-events', 'auto');
	//	menuActive = menu.hasClass('hamburger_menu');
	//	menuActiveCart = menu.hasClass('hamburger_menu_cart');
	//}

	//function closeMenu(menu, fsOverlay) {
	//	menu.removeClass('active');
	//	fsOverlay.css('pointer-events', 'none');
	//	if (menu.hasClass('hamburger_menu')) {
	//		menuActive = false;
	//	} else if (menu.hasClass('hamburger_menu_cart')) {
	//		menuActiveCart = false;
	//	}
	//}
	/* 

	4. Init Timer

	*/

	function initTimer() {
		if ($('.timer').length) {
			// Uncomment line below and replace date
			// var target_date = new Date("Dec 7, 2017").getTime();

			// comment lines below
			var date = new Date();
			date.setDate(date.getDate() + 3);
			var target_date = date.getTime();
			//----------------------------------------

			// variables for time units
			var days, hours, minutes, seconds;

			var d = $('#day');
			var h = $('#hour');
			var m = $('#minute');
			var s = $('#second');

			setInterval(function () {
				// find the amount of "seconds" between now and target
				var current_date = new Date().getTime();
				var seconds_left = (target_date - current_date) / 1000;

				// do some time calculations
				days = parseInt(seconds_left / 86400);
				seconds_left = seconds_left % 86400;

				hours = parseInt(seconds_left / 3600);
				seconds_left = seconds_left % 3600;

				minutes = parseInt(seconds_left / 60);
				seconds = parseInt(seconds_left % 60);

				// display result
				d.text(days);
				h.text(hours);
				m.text(minutes);
				s.text(seconds);

			}, 1000);
		}
	}

	/* 

	5. Init Favorite

	*/

	function initFavorite() {
		if ($('.favorite').length) {
			var favs = $('.favorite');

			favs.each(function () {
				var fav = $(this);
				var active = false;
				if (fav.hasClass('active')) {
					active = true;
				}

				fav.on('click', function () {
					if (active) {
						fav.removeClass('active');
						active = false;
					}
					else {
						fav.addClass('active');
						active = true;
					}
				});
			});
		}
	}

	/* 

	6. Init Fix Product Border

	*/

	function initFixProductBorder() {
		if ($('.product_filter_item').length) {
			var products = $('.product_filter:visible');
			var wdth = window.innerWidth;

			// reset border
			products.each(function () {
				$(this).css('border-right', 'solid 1px #e9e9e9');
			});

			// if window width is 991px or less

			if (wdth < 480) {
				for (var i = 0; i < products.length; i+=2) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			else if (wdth < 576) {
				if (products.length < 5) {
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for (var i = 1; i < products.length; i += 2) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			else if (wdth < 768) {
				if (products.length < 5) {
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for (var i = 2; i < products.length; i += 3) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			else if (wdth < 992) {
				if (products.length < 5) {
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for (var i = 3; i < products.length; i += 4) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			//if window width is larger than 991px
			else {
				if (products.length < 5) {
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for (var i = 4; i < products.length; i += 5) {
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}
		}
	}

	/* 

	7. Init Isotope Filtering

	*/

	function initIsotopeFiltering() {
		if ($('.grid_sorting_button').length) {
			$('.grid_sorting_button').click(function () {
				setTimeout(function () {
					initFixProductBorder();
				}, 500);

				$('.grid_sorting_button.active').removeClass('active');
				$(this).addClass('active');

				var selector = $(this).attr('data-filter');
				$('.product__grid').isotope({
					filter: selector,
					animationOptions: {
						duration: 750,
						easing: 'linear',
						queue: false
					}
				});

				return false;
			});
		}
	}

	/* 

	8. Init Slider

	*/

	function initSlider() {
		if ($('.product_slider').length) {
			var slider1 = $('.product_slider');

			slider1.owlCarousel({
				loop: false,
				dots: false,
				nav: false,
				responsive:
				{
					0: { items: 1 },
					480: { items: 2 },
					768: { items: 3 },
					991: { items: 4 },
					1280: { items: 5 },
					1440: { items: 5 }
				}
			});

			if ($('.product_slider_nav_left').length) {
				$('.product_slider_nav_left').on('click', function () {
					slider1.trigger('prev.owl.carousel');
				});
			}

			if ($('.product_slider_nav_right').length) {
				$('.product_slider_nav_right').on('click', function () {
					slider1.trigger('next.owl.carousel');
				});
			}
		}
	}
});