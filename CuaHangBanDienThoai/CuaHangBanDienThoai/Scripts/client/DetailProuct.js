
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
    var bg = $(".bg-sgCommet");
    var popup = $("#popupCommmet");
    $(document).on('click', '.btnRating', function (event) {
        event.preventDefault();
        var productid = $(this).data('productid');
        var name = $('#namepro').val().trim();
        var image = $('#imagepro').val().trim();

        if (productid && name) {
            bg.show();
            popup.show();
            console.log("productid", productid);
            console.log("namepro", name);

            $.ajax({
                url: '/Review/Partial_AddReview',
                data: {
                    productid: productid,
                    name: name,
                    image: image,
                },
                type: 'GET',
                success: function (res) {
                    $("#popupCommmet").html(res); 

                   
                    initRatingEvents(); 
                },
                error: function () {
                    alert("Có lỗi khi load đánh giá sản phẩm");
                }
            });
        }
    });

 

    $(document).on('click', '.close-rate', function (evnet) {
        evnet.preventDefault();
        bg.hide();
        popup.hide();
    });
    $(bg).on('click', function () {
        bg.hide();
        popup.hide();
    });




   


});

// Khởi tạo các sự kiện cho rating
function initRatingEvents() {
    const stars = $(".rating-topzonecr-star li");
    const rateInput = $("#rate")[0];
    const submitrt = $("#submitrt")[0];
    var textarea = $(".fRContent")[0];
    const charCountSpan = $(".ct")[0];
    let selectedRating = 0;

    $(textarea).on("input", function () {
        const charCount = textarea.value.length;
        $(charCountSpan).text(`${charCount} chữ`);
        if (charCount > 0) {
            $(submitrt).removeClass("disabled").prop("disabled", false);
            $(charCountSpan).show();
        } else {
            $(charCountSpan).hide();
        }
    });

    $(".rating-topzonecr-star li").off("mouseover").on("mouseover", function () {
        updateStars($(this).index() + 1);
    });

    $(".rating-topzonecr-star li").off("mouseout").on("mouseout", function () {
        updateStars(selectedRating);
    });

    $(".rating-topzonecr-star li").off("click").on("click", function () {
        selectedRating = $(this).index() + 1;
        rateInput.value = selectedRating;
        console.log(`Bạn đã chọn ${selectedRating} sao`);
        if (selectedRating > 0) {
            $(submitrt).removeClass("disabled").prop("disabled", false);
        }
    });

   
    function updateStars(rating) {
        $(stars).each(function (i) {
            const svg = $(this).find("svg")[0];
            const text = $(this).find("p")[0];

            if (i < rating) {
                $(svg).css("color", "#FF8C00");
                $(text).show();
            } else {
                $(svg).css("color", "gray");
                $(text).hide();
            }
        });
    }
   

    $(document).on('click', '#submitrt', function (event) {
        event.preventDefault();
        var isValid = true;
        var $button = $(this); 
        $button.prop('disabled', true);
        $button.find('.loading-image').show();
        $button.find('.button-text').hide();
        var productid = $(this).data('productid');

    
        var content = $('#Content').val().trim();
        if (!content) {
            showError('Content', 'Vui lòng hãy điền nội dung.');
            isValid = false;
        } else {
            removeError('CustomerName');
        }
        if (selectedRating <=0) {
            showError('rate', 'Vui lòng hãy điền nội dung.');
            isValid = false;
        } else {
            removeError('rate');
        }


        if (isValid && productid && selectedRating >= 1) {
            var form = $('#fromrate')[0];
            var formData = new FormData(form);
            $.ajax({
                url: '/Review/Comment',
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (res) {
                    if (res.Success&&res.Code==1) {
                        
                    }else {

                    }
                }, error: function () {
                    alert("Lỗi Hệ thống đánh giá ");
                }
            });
        }
    });

}

function removeError(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
       /* element.classList.remove('error-border');*/
    }
}

function showError(elementId, errorMessage) {
    const element = document.getElementById(elementId);
    if (element) {
      /*  element.classList.add('error-border');*/
    }
    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', errorMessage);
}



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


function createToast(type, icon, title, text) {

    const notifications = document.querySelector('.notifications');
    if (!notifications) {
        console.warn('Không tìm thấy phần tử notifications trong DOM');
        return;
    }

    let newToast = document.createElement('div');
    newToast.innerHTML = `
        <div class="toastNotifi ${type}">
            <i class="${icon}"></i>
            <div class="content">
                <div class="title">${title}</div>
                <span>${text}</span>
            </div>
            <i class="close fa-solid fa-xmark" onclick="this.parentElement.remove()"></i>
        </div>`;
    notifications.appendChild(newToast);


    newToast.timeOut = setTimeout(() => newToast.remove(), 5000);
}
