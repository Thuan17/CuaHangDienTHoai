document.addEventListener('DOMContentLoaded', function () {
    function initializeSelectBox() {
        var selectedDiv = document.getElementById('select-selected');
        var items = document.querySelector('.select-items');
        var checkboxes = document.querySelectorAll('.select-option-checkbox input[type="checkbox"]');
        selectedDiv.addEventListener('click', function () {
            items.style.display = items.style.display === 'block' ? 'none' : 'block';
        });
        checkboxes.forEach(function (checkbox) {
            checkbox.addEventListener('change', function () {
                if (this.checked) {
                    var caseTitle = this.closest('.select-option').querySelector('.select-option-title').textContent;
                    selectedDiv.textContent = caseTitle;
                    checkboxes.forEach(function (box) {
                        if (box !== checkbox) {
                            box.checked = false;
                        }
                    });

                    items.style.display = 'none'; 
                }
            });
        });

        items.addEventListener('click', function (event) {
            var target = event.target;

            if (target.classList.contains('select-option-title') || target.tagName === 'img') {
                var selectOption = target.closest('.select-option');
                var checkbox = selectOption.querySelector('.select-option-checkbox input[type="checkbox"]');

                if (checkbox) {
                    checkbox.checked = true;
                    checkboxes.forEach(function (box) {
                        if (box !== checkbox) {
                            box.checked = false;
                        }
                    });
                    var caseTitle = selectOption.querySelector('.select-option-title').textContent;
                    selectedDiv.textContent = caseTitle;
                    items.style.display = 'none';
                }
            }
        });

        document.addEventListener('click', function (event) {
            if (!event.target.closest('.custom-select')) {
                items.style.display = 'none';
            }
        });
    }
    initializeSelectBox();
});


//$(document).ready(function () {
//    // Xử lý sự kiện nhấn vào tiêu đề
//    $('.titleDescription').click(function () {
//        const container = $('.containerDescription');
//        const isExpanded = container.hasClass('expanded');

//        if (isExpanded) {
//            container.removeClass('expanded').addClass('collapsed');
//            $('.titleDescription i').removeClass('bi-chevron-double-down').addClass('bi-chevron-double-right').removeClass('rotate');
//            $('.XemThem').show();
//        } else {
//            container.removeClass('collapsed').addClass('expanded');
//            $('.titleDescription i').removeClass('bi-chevron-double-right').addClass('bi-chevron-double-down').addClass('rotate');
//            $('.XemThem').hide();
//        }
//    });

//    $('.XemThem').click(function () {
//        const container = $('.containerDescription');
//        container.removeClass('collapsed').addClass('expanded');
//        $('.titleDescription i').removeClass('bi-chevron-double-right').addClass('bi-chevron-double-down').addClass('rotate');
//        $(this).hide();
//    });
//});
$(document).ready(function () {
    // Xử lý sự kiện nhấn vào tiêu đề
    $('.titleDescription').click(function () {
        const container = $('.containerDescription');
        const isExpanded = container.hasClass('expanded');

        if (isExpanded) {
            container.removeClass('expanded').addClass('collapsed');
            $('.titleDescription i').removeClass('bi-chevron-double-down').addClass('bi-chevron-double-right');
            $('.XemThem').show();
            container.css('max-height', '250px'); 
        } else {
            container.removeClass('collapsed').addClass('expanded');
            $('.titleDescription i').removeClass('bi-chevron-double-right').addClass('bi-chevron-double-down');
            $('.XemThem').show();
            container.css('max-height', container[0].scrollHeight + 'px'); 
        }
    });

    $('.XemThem').click(function () {
        const container = $('.containerDescription');
        const isExpanded = container.hasClass('expanded');

        if (isExpanded) {
            container.removeClass('expanded').addClass('collapsed');
            $('.titleDescription i').removeClass('bi-chevron-double-down').addClass('bi-chevron-double-right');
            $('.XemThem').show();
            container.css('max-height', '250px');
        } else {
            container.removeClass('collapsed').addClass('expanded');
            $('.titleDescription i').removeClass('bi-chevron-double-right').addClass('bi-chevron-double-down');
            $('.XemThem').show();
            container.css('max-height', container[0].scrollHeight + 'px');
        }
    });
});



//$(document).ready(function () {
//    // Xử lý sự kiện nhấn vào tiêu đề
//    $('.titleDescription').click(function () {
//        const container = $('.containerDescription');
//        const isExpanded = container.hasClass('expanded');

//        if (isExpanded) {
//            container.removeClass('expanded').addClass('collapsed');
//            $('.titleDescription i').removeClass('bi-chevron-double-down').addClass('bi-chevron-double-right');
//            $('.XemThem').show(); 
//        } else {
//            container.removeClass('collapsed').addClass('expanded');
//            $('.titleDescription i').removeClass('bi-chevron-double-right').addClass('bi-chevron-double-down');
//            $('.XemThem').hide();
//        }
//    });


//    $('.XemThem').click(function () {
//        const container = $('.containerDescription');
//        container.removeClass('collapsed').addClass('expanded');
//        $('.titleDescription i').removeClass('bi-chevron-double-right').addClass('bi-chevron-double-down');
//        $(this).hide();
//    });
//});





var swiperSmall = new Swiper('.swiper-container-small', {
    slidesPerView: 4,
    spaceBetween: 10,
    freeMode: true,
    watchSlidesVisibility: true,
    watchSlidesProgress: true,
});

var swiper = new Swiper('.swiper-container-item', {
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
    pagination: {
        el: '.swiper-pagination-custom',
        type: 'fraction',
    },
    loop: true,
    on: {
        init: function () {
            this.pagination.render();
            this.pagination.update();
        },
        slideChange: function () {
            var currentIndex = this.realIndex;
            var slides = document.querySelectorAll('.swiper-container-small .swiper-slide');

            // Loop through all slides to update classes
            slides.forEach(function (slide, index) {
                slide.classList.remove('active-slide', 'next-slide');

                if (index === currentIndex) {
                    slide.classList.add('active-slide');

                } else if (index > currentIndex) {
                    slide.classList.add('next-slide');
                }
            });

            // Update the small swiper to sync with the main swiper
            swiperSmall.slideTo(currentIndex, 500, false);

            // Update pagination
            document.querySelector('.swiper-pagination-current').textContent = currentIndex + 1;
            document.querySelector('.swiper-pagination-total').textContent = this.slides.length;
        },
    },
    thumbs: {
        swiper: swiperSmall
    },
});