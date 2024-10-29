




            
$(document).ready(function () {
 
    ShowCount();
    registerEvents();
    var isDeletingItem = false;
    var isSyncingQuantity = false;




    //var menuCart = $('.hamburger_menu_cart');
    //var menuActiveCart = false;
    //var hamburgerCloseCart = $('.hamburger_close_cart');
    //var fsOverlayCart = $('.fs_menu_overlay_cart');
    var bg = $('.bg-sg');

    // Vars and Inits
    var hamburgerMenu = $('.hamburger_container_Menu');
    var hamburgerCart = $('.hamburger_container_cart');
    var hamburgerCartNavbarUser = $('.hamburger_container_cart_navbar_user');
    var menu = $('.hamburger_menu');
    var menuCart = $('.hamburger_menu_cart');
    var fsOverlay = $('.fs_menu_overlay');
    var fsOverlayCart = $('.fs_menu_overlay_cart');
    var hamburgerClose = $('.hamburger_close');
    var hamburgerCloseCart = $('.hamburger_close_cart');
    var menuTopNav = $('.menu_top_nav');
    var menuTopNavCart = $('.menu_top_nav_cart');



    var menuActive = false;
    var menuActiveCart = false;

    // Event Listeners for Toggle Buttons
    hamburgerMenu.on('click', function (event) {
        event.stopPropagation();
        if (menuActiveCart) {
            closeMenu(menuCart, fsOverlayCart);
        }
        if (!menuActive) {
            openMenu(menu, fsOverlay);
        } else {
            closeMenu(menu, fsOverlay);
        }
    });

    hamburgerCart.on('click', function (event) {
        event.stopPropagation();
        if (menuActive) {
            closeMenu(menu, fsOverlay);
        }
        if (!menuActiveCart) {
            openMenu(menuCart, fsOverlayCart);
        } else {
            closeMenu(menuCart, fsOverlayCart);
        }
    });

    hamburgerCartNavbarUser.on('click', function (event) {
        event.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ <a>
        event.stopPropagation();
        if (menuActive) {
            closeMenu(menu, fsOverlay);
        }
        if (!menuActiveCart) {
            openMenu(menuCart, fsOverlayCart);
        } else {
            closeMenu(menuCart, fsOverlayCart);
        }
    });

    // Close Menu on Overlay Click
    fsOverlay.on('click', function () {
        if (menuActive) {
            closeMenu(menu, fsOverlay);
        }
    });

    fsOverlayCart.on('click', function () {
        if (menuActiveCart) {
            closeMenu(menuCart, fsOverlayCart);
        }
    });

    // Close Menu on Close Button Click
    menu.on('click', function () {
        if (menuActive) {
            closeMenu(menu, fsOverlay);
        }
    });

    menuCart.on('click', function () {
        if (menuActiveCart) {
            closeMenu(menuCart, fsOverlayCart);
        }
    });

    // Close Menu on Close Button Click
    hamburgerClose.on('click', function () {
        if (menuActive) {
            closeMenu(menu, fsOverlay);
        }
    });

    hamburgerCloseCart.on('click', function () {
        if (menuActiveCart) {
            closeMenu(menuCart, fsOverlayCart);
        }
    });
    // Prevent closing the menu when clicking inside the specific menu content
    menuTopNav.on('click', function (event) {
        event.stopPropagation();
    });

    menuTopNavCart.on('click', function (event) {
        event.stopPropagation();
    });

    // Close menu if clicking outside of menu or cart content
    $(document).on('click', function (event) {
        var target = $(event.target);
        if (!target.closest('.hamburger_menu').length && menuActive) {
            closeMenu(menu, fsOverlay);
        }
        if (!target.closest('.hamburger_menu_cart').length && menuActiveCart) {
            closeMenu(menuCart, fsOverlayCart);
        }
    });

    function openMenu(menu, fsOverlay) {
        menu.addClass('active');
        fsOverlay.css('pointer-events', 'auto');
        menuActive = menu.hasClass('hamburger_menu');
        menuActiveCart = menu.hasClass('hamburger_menu_cart');
        menuActiveCart = true;
        LoadCartSmall();
    }

    function closeMenu(menu, fsOverlay) {
        menu.removeClass('active');
        fsOverlay.css('pointer-events', 'none');
        if (menu.hasClass('hamburger_menu')) {
            menuActive = false;
        } else if (menu.hasClass('hamburger_menu_cart')) {
            menuActiveCart = false;
        }
    }
  
    function LoadCartSmall() {
        $.ajax({
            url: '/shoppingcart/Partial_Item_Cart_Small',
            type: 'GET',
            success: function (data) {
                $('.hamburger_menu_cart').html(data);

                registerEvents();
            },
            error: function () {
                console.log('Error loading content');
            }
        });
    }
 
    function registerEvents() {
        document.querySelectorAll('.btnquantityCartS').forEach(button => {
            button.removeEventListener('click', handleClick);
            button.addEventListener('click', handleClick);
        });

        document.querySelectorAll('.btnMinus').forEach(function (btn) {
            btn.removeEventListener('click', handleMinusClick);
            btn.addEventListener('click', handleMinusClick);
        });

        document.querySelectorAll('.btnFlus').forEach(function (btn) {
            btn.removeEventListener('click', handlePlusClick);
            btn.addEventListener('click', handlePlusClick);
        });
        $(document).off('click', '.btnDeleteCart').on('click', '.btnDeleteCart', function (e) {
            e.preventDefault();
           
            if (isDeletingItem || isSyncingQuantity) return;
            isDeletingItem = true;

            const productid = $(this).data('productid');
            if (productid > 0) {
                $.ajax({
                    url: '/ShoppingCart/DeleteFromCartSession',
                    type: 'POST',
                    data: { productid: productid },
                    success: function (rs) {
                        if (rs.Success && rs.Code === 1) {
                            if (!menuActiveCart) {
                                openMenu(menuCart, fsOverlayCart);
                            } else {
                                closeMenu(menuCart, fsOverlayCart);
                            }
                            LoadCartSmall();
                            LoadItemThanhToanCheckOut();
                            ShowCount();

                            registerEvents();
                           
                            createToast('success', 'fa-solid fa-circle-check', 'Thành công', rs.msg);
                        } else {
                            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Lỗi', rs.msg);
                        }
                    },
                    error: function () {
                        createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Không thể xóa sản phẩm');
                    },
                    complete: function () {
                        isDeletingItem = false;
                    }
                });
            }
        
            isDeletingItem = false;
        });
    }

    $('body').on('click', '.btnDeleteItemCheckOut', function (e) {
        e.preventDefault();
        if (isDeletingItem || isSyncingQuantity) return;
        isDeletingItem = true;

        const productid = $(this).data('productid');
        Swal.fire({
            title: "Bạn muốn xoá sản phẩm",
            confirmButtonText: "Đồng ý",
        }).then((result) => {
            if (result.isConfirmed) {
                deleteItemCart(productid);
                registerEvents();
            }
            isDeletingItem = false;
        });
    });

    $('body').on('click', '.btnbuynow', function (e) {
      
        e.preventDefault();
        var id = $(this).data('productid');
        var selectedOption = $('.custom-select .select-option:has(input:checked)');
        var productCaseId = 0;
        if (selectedOption.length != 0) {
            productCaseId = selectedOption.data('productcaseid');

        }
        var quantity = $('#quantity_value').val();
        if (quantity == null) {
            quantity = 1;
        }
        $.ajax({
            url: '/ShoppingCart/BuyNow',
            type: 'POST',
            data: { id: id, quantity: quantity, productCaseid: productCaseId },
            success: function (rs) {
                if (rs.Success) {
                    if (rs.Code == 1) {

                        ShowCount();
                        loadTotalQuantitySession();
     
                        LoadCartSmall();
                        registerEvents();
                        //setTimeout(() => {
                        //    window.location.href = "/thanh-toan";
                        //}, 500);
                        window.location.href = "/thanh-toan";
                      
                    }
                } else {
                    if (rs.Code == -99) {
                        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', rs.msg);
                    } else {
                        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', rs.msg);
                    }
                }
            },
            error: function (xhr, status, error) {
                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', '');
            }
        });
    });


    $('body').on('click', '.btnAddtoCart', function (e)
    {
        e.preventDefault();
        var id = $(this).data('productid');
      
        var quantity = $('#quantity_value').val();
        if (quantity == null) {
            quantity = 1;
        }
        if (!window.sessionStorage.getItem('CustomerId')) {
         
            $.ajax({
                url: '/Account/SetRedirectUrl', 
                type: 'POST',
                data: { redirectUrl: window.location.href },
                success: function () {
                   
                    window.location.href = '/dang-nhap';
                }
            });
            return;
        }

        $.ajax({
            url: '/Cart/AddtoCart',
            type: 'POST',
            data: { id: id, quantity: quantity },
            success: function (rs) {
                if (rs.Success) {

                    ShowCount();
                    /*  loadTotalQuantitySession();*/
                    createToast('success', 'fa-solid fa-circle-check', 'Thành công', rs.msg);
                    //LoadCartSmall();
                    //registerEvents();
                } else {
                    if (rs.code == -99) {
                        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', rs.msg);
                    } else {
                        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', rs.msg);
                    }
                }
            },
            error: function (xhr, status, error) {
                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', '');
            }
        });
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

    function handleClick(event) {
        event.stopPropagation();
        event.preventDefault();
        stepper(this, this.dataset.productid);
    }

    function stepper(btn, productid) {
        let input = $(`.QuantityCartSmall[data-productid="${productid}"]`);
        if (input.length === 0) return;

        let min = parseInt(input.attr("min"), 10);
        let max = parseInt(input.attr("max"), 10);
        let step = parseInt(input.attr("step"), 10);
        let currentVal = parseInt(input.val(), 10);
        let calcStep = $(btn).hasClass("btn-increment") ? step : -step;
        let newValue = currentVal + calcStep;

        newValue = Math.max(min, Math.min(newValue, max));

        if (newValue !== currentVal) {
            input.val(newValue).trigger('change');
        }
    }

    function handleMinusClick(event) {
        event.preventDefault();
        let input = this.nextElementSibling;
        let min = parseInt(input.getAttribute('min'), 10);
        let step = parseInt(input.getAttribute('step'), 10);
        let currentVal = parseInt(input.value, 10);
        if (currentVal > min) {
            input.value = currentVal - step;
            $(input).trigger('change');
        }
    }

    function handlePlusClick(event) {
        event.preventDefault();
        let input = this.previousElementSibling;
        let max = parseInt(input.getAttribute('max'), 10);
        let step = parseInt(input.getAttribute('step'), 10);
        let currentVal = parseInt(input.value, 10);
        if (currentVal < max) {
            input.value = currentVal + step;
            $(input).trigger('change');
        }
    }

    function LoadItemThanhToanCheckOut() {
        $.ajax({
            url: '/shoppingcart/Partial_Item_ThanhToan',
            type: 'GET',
            success: function (data) {
                $('.ItemCheckOut').html(data);
                registerEvents();

            }
        });
    }

   






    $('body').on('click', '.hamburger_close_cart', function (e) {
        if (menuActiveCart) {
            closeMenu(menuCart, fsOverlayCart);
        }
    });
    function deleteItemCart(productid) {
        if (productid > 0) {
            $.ajax({
                url: '/ShoppingCart/DeleteFromCartSession',
                type: 'POST',
                data: { productid: productid },
                success: function (rs) {
                    if (rs.Success && rs.Code === 1) {
                        LoadCartSmall();
                        LoadItemThanhToanCheckOut();
                        ShowCount();

                        registerEvents(); 
                        createToast('success', 'fa-solid fa-circle-check', 'Thành công', rs.msg);
                    } else {
                        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Lỗi', rs.msg);
                    }
                },
                error: function () {
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Không thể xóa sản phẩm');
                },
                complete: function () {
                    isDeletingItem = false;
                }
            });
        }
    }

    function UpdateQuantity(productid, quantity) {
        $.ajax({
            type: 'POST',
            url: '/ShoppingCart/UpdateQuantitySession',
            data: {
                productid: productid,
                quantity: quantity
            },
            success: function (rs) {
                if (rs.Success) {
                    if (rs.Code === 1) {
                        loadTotalPriceSession();
                        loadTotalQuantitySession();
                    } else {
                        createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', rs.msg);
                    }
                } else {
                    createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Cập nhập không thành công');
                }
            },
            error: function () {
                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Lỗi máy chủ');
            }
        });
    }


  
   

    $('body').on('change', '.QuantityCartSmall', function () {
        if (isSyncingQuantity || isDeletingItem) return;
        isSyncingQuantity = true;

        var productid = $(this).data('productid');
        var quantity = $(this).val();

        $('.QuantityCheckOut[data-productid="' + productid + '"]').val(quantity);

        UpdateQuantity(productid, quantity);

        isSyncingQuantity = false;
    });

    $('body').on('change', '.QuantityCheckOut', function () {
        if (isSyncingQuantity || isDeletingItem) return;
        isSyncingQuantity = true;

        var productid = $(this).data('productid');
        var quantity = $(this).val();

        $('.QuantityCartSmall[data-productid="' + productid + '"]').val(quantity);

        UpdateQuantity(productid, quantity);

        isSyncingQuantity = false;
    });





    
    registerEvents();
});           




function loadTotalPriceSession() {
    $.ajax({
        url: '/shoppingcart/GetPrice',
        type: 'GET',
        success: function (data) {
            $('.right__price').html(data.TotalPrice + ' đ');
            $('.priceCheckOut').html(data.TotalPrice + ' đ');
        },
        error: function (xhr, status, error) {
            console.error('Có lỗi xảy ra khi lấy dữ liệu giá:', error);
        }
    });
}

function loadTotalQuantitySession() {
    $.ajax({
        url: '/shoppingcart/GetTotalQuantity',
        type: 'GET',
        success: function (data) {
            $('.Quantiy__Pro').html('(' + data.TotalQuantity + ')');
        },
        error: function (xhr, status, error) {
            console.error('Có lỗi xảy ra khi lấy dữ liệu giá:', error);
        }
    });
}

function ShowCount() {
    var checkout_count = $(".checkout_count");
    $.ajax({
        url: '/Cart/ShowCount',
        type: 'GET',
        success: function (rs) {
            if (rs.Count > 0) {
                checkout_count.show();
                $('.checkout_count').html(rs.Count);
            }
            else {
                checkout_count.hide();
            }
        }
    });
}

$(document).ready(function(){
    const scrollLeftButton = document.querySelector('.scroll-left');
    const scrollRightButton = document.querySelector('.scroll-right');
    const menu = document.querySelector('.navbar_menu');

    const itemWidth = 220;
    const maxScrollLeft = menu.scrollWidth - menu.clientWidth;

  
    scrollLeftButton.addEventListener('click', function () {
        menu.scrollLeft -= itemWidth;
        if (menu.scrollLeft < 0) {
            menu.scrollLeft = 0;
        }
    });


    scrollRightButton.addEventListener('click', function () {
        menu.scrollLeft += itemWidth;
        if (menu.scrollLeft > maxScrollLeft) {
            menu.scrollLeft = maxScrollLeft;
        }
    });


    let isDown = false;
    let startX;
    let scrollLeft;

    menu.addEventListener('mousedown', (e) => {
        isDown = true;
        menu.classList.add('active');
        startX = e.pageX - menu.offsetLeft;
        scrollLeft = menu.scrollLeft;
    });

    menu.addEventListener('mouseleave', () => {
        isDown = false;
        menu.classList.remove('active');
    });

    menu.addEventListener('mouseup', () => {
        isDown = false;
        menu.classList.remove('active');
    });

    menu.addEventListener('mousemove', (e) => {
        if (!isDown) return;
        e.preventDefault();
        const x = e.pageX - menu.offsetLeft;
        const walk = (x - startX) * 2; // Adjust scroll speed here
        menu.scrollLeft = scrollLeft - walk;
    });
});

$(document).ready(function () {
    var searchInput = document.getElementById("search_input");
    var bg = document.querySelector(".bg-sg");
    var searchSuggestions = document.querySelector(".search-suggestions");
    var iconSearch = document.querySelector(".search");
    var iconSearchgif = document.querySelector(".searchgif");
    var searchGr = document.querySelector(".searchGr");
    searchInput.addEventListener("focus", function (event) {
        event.preventDefault();
      
        iconSearch.style.display = "none";
        iconSearchgif.style.display = "block";
        iconSearchgif.style.width = "25px";
        searchGr.classList.add("active");
        bg.style.display = "block";
      
    });
    searchInput.addEventListener("input", function () {
        var inputValue = searchInput.value.trim();
        if (inputValue.length >= 3) {
            searchSuggestions.style.display = "block";
        }
        else {
            searchSuggestions.style.display = "none";
        }
    });
    $("#search_input").on("input", function () {
        var inputValue = searchInput.value.trim();
        if (inputValue.length >= 3) {
            $.ajax({
                url: "/Product/SuggestionsSearch",
                type: "GET",
                data: { search: inputValue },
                success: function (response) {
                    $(".search-suggestions").html(response);
                },
                 error: function (xhr, status, error) {

                    console.error(xhr.responseText);
                }
            });

        }


    });
    bg.addEventListener("click", function (event) {
        event.preventDefault();
        searchSuggestions.style.display = "none";
        bg.style.display = "none";
        iconSearch.style.display = "block";
        iconSearchgif.style.display = "none";
        search_input.value = "";
        searchGr.classList.remove("active");
    }); 
});



