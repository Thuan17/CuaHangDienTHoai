//start add products

$(document).ready(function () {
    $('.auto').autoNumeric('init');
    $('#demoPrice').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoPrice').autoNumeric('get');
        $('#Price').val(demoGet);
        $('#Price').autoNumeric('set', demoGet);
    });
    $('#demoPriceSale').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoPriceSale').autoNumeric('get');
        $('#PriceSale').val(demoGet);
        $('#PriceSale').autoNumeric('set', demoGet);
    });
    $('#demoOriginalPrice').bind('blur focusout keypress keyup', function () {
        var demoGet = $('#demoOriginalPrice').autoNumeric('get');
        $('#OriginalPrice').val(demoGet);
        $('#OriginalPrice').autoNumeric('set', demoGet);
    });

    $('body').on('click', '.btnXoaAnh', function () {
        var conf = confirm('Bạn có muốn xóa ảnh này không?');
        if (conf === true) {
            var _id = $(this).data('id');
            $('#trow_' + _id).fadeTo('fast', 0.5, function () {
                $(this).slideUp('fast', function () { $(this).remove(); });
            });
            var temp = $('#tCurrentId').val();
            var currentId = parseInt(temp) - 1;
            $('#tCurrentId').val(currentId);
        }
    });
});
$(document).ready(function () {
   
    var searchInput = document.getElementById("searchinput");
    var searchbtn = document.getElementById("searchbtn");
    var searchSuggestions = document.getElementById("searchSuggestions");

    searchInput.addEventListener("input", function () {
        var inputValue = searchInput.value.trim();
        if (inputValue.length >= 3) {
            searchSuggestions.style.display = "block";
        } else {
            searchSuggestions.style.display = "none";
        }
    });


    searchInput.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            var inputValue = searchInput.value.trim();
            if (inputValue.length >= 3) {

                window.location.href = "/Admin/Product/SearchProduct?key=" + encodeURIComponent(inputValue) + "&page=1";
            }
        }
    });

    $("#searchbtn").on("click", function () {
        var inputValue = searchInput.value.trim();
        if (inputValue.length >= 3) {

            window.location.href = "/Admin/Product/SearchProduct?key=" + encodeURIComponent(inputValue) + "&page=1";
        }
    });


    // Hàm xử lý submit form
    $('#myFormProduct').submit(function (event) {
        event.preventDefault();

        console.log('Form Data myFormProduct submit:', formData);
       /* let isValid = validateForm();*/
       

        // Gửi dữ liệu form nếu hợp lệ
        //for (var instance in CKEDITOR.instances) {
        //    CKEDITOR.instances[instance].updateElement();
        //}
        //const formData = $('#myFormProduct').serialize();
        //const token = $('input[name="__RequestVerificationToken"]').val();
        //console.log('Form Data:', formData);

        //$.ajax({
        //    url: '/Admin/Product/Add',
        //    type: 'POST',
        //    data: formData,
        //    success: function (res) {
        //        OnSuccessCO(res);
        //    },
        //    error: function (xhr) {
        //        Swal.fire({
        //            icon: "error",
        //            title: "Lỗi",
        //            text: "Hệ thống gặp lỗi thêm sản phẩm.",
        //        });
        //        console.error(xhr.responseText);
        //    }
        //});
    });
});


//End add products
//Start details products

//$(document).ready(function () {
//    var bg = $(".bg-sg");
//    var popup = $("#popupBill");
//    var btnCapNhatBill = $('.btnCapNhatBill');
//    var btnDetailsProducts = $('.btnDetailsProducts');
//    bg.on('click', function () {
//        bg.hide();
//        popup.hide();
//    });

//    $('.btnCloseEditBill').on('click', function () {
//        bg.hide();
//        popup.hide();
//    });



//    btnDetailsProducts.each(function () {
//        $(this).on('click', function (e) {

//            e.preventDefault();

//            bg.show();
//            popup.show();

//            var id = $(this).data('id');

//            if (id != null) {
//                $.ajax({
//                    url: '/Admin/Bill/Partial_DetailBill',
//                    type: 'GET',
//                    data: { id: id },
//                    success: function (response) {
//                        $("#loadDataBillEdit").html(response);
//                    },
//                    error: function (xhr, status, error) {
//                        console.error(xhr.responseText);
//                    }
//                });
//            }
//        });
//    });

//})