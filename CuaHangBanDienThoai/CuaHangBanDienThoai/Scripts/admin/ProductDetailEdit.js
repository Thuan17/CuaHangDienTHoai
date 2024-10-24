function addImageProduct(url) {
    var temp = $('#tCurrentId').val();
    var currentId = parseInt(temp) + 1;
    var str = "";
    str += `<tr id="trow_${currentId}">

                                <td class="text-center"><img style="max-width:150px;  height:auto;overflow:hidden"  src="${url}" /> <input type='hidden' value="${url}" name="Images"/></td>
                                <td class="text-center"><input type="checkbox" name="rDefault" value="${currentId}"/></td>
                                <td class="text-center"><a href="#" data-id="${currentId}" class="btn btn-sm text-danger btnXoaAnh_NewUp">
                                                                <svg xmlns="http://www.w3.org/2000/svg" width=18" height="18" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
  <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
</svg>
                                </a></td>
                                </tr>`;
    $('#tbHtml').append(str);
    $('#tCurrentId').val(currentId);

}
function BrowseServerProDetailEdit(field) {
    var finder = new CKFinder();
    finder.selectActionFunction = function (fileUrl) {
        addImageProduct(fileUrl);
    };
    finder.popup();
}
function addImageProductUpdate(url, index) {
    var imgElement = document.getElementById('img_' + index);
    if (imgElement) {
        imgElement.src = url;


        $.ajax({
            type: "POST",
            url: '/Admin/ProductDetail/UpdateImage',
            data: {
                alias: $(imgElement).data('alias'),
                producdetailid: $(imgElement).data('producdetailid'),
            
                productImageId: $(imgElement).data('productimage'),
                url: url
            },
            success: function (response) {
                if (response.success) {
                    console.log("Image updated successfully.");
                } else {
                    console.log("Error: " + response.message);
                    Swal.fire({
                        icon: "error",
                        title: "Lỗi ...",
                        text: "Xảy ra trong quá trình đổi  ảnh ..."
                    });
                }
            },
            error: function () {
                console.log("An error occurred while updating the image.");
                Swal.fire({
                    icon: "error",
                    title: "Lỗi ...",
                    text: "Máy chủ gặp vấn đề load..."
                });
            }
        });
    }
}


function BrowseServerUpdate(element) {
    var index = element.getAttribute('data-index');
    var finder = new CKFinder();
    finder.selectActionFunction = function (fileUrl) {
        addImageProductUpdate(fileUrl, index);
    };
    finder.popup();
}
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


    /*   start xoa anh moi up */
    $('body').on('click', '.btnXoaAnh_NewUp', function () {
        Swal.fire({
            title: "Bạn có muốn xoá ảnh ?",

            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Xoá ảnh !"
        }).then((result) => {
            if (result.isConfirmed) {

                var _id = $(this).data('id');
                $('#trow_' + _id).fadeTo('fast', 0.5, function () {
                    $(this).slideUp('fast', function () { $(this).remove(); });
                });
                var temp = $('#tCurrentId').val();
                var currentId = parseInt(temp) - 1;
                $('#tCurrentId').val(currentId);
                const Toast = Swal.mixin({
                    toast: true,
                    position: "top-end",
                    showConfirmButton: false,
                    timer: 1500,
                    timerProgressBar: true,
                    didOpen: (toast) => {
                        toast.onmouseenter = Swal.stopTimer;
                        toast.onmouseleave = Swal.resumeTimer;
                    }
                });

                Toast.fire({
                    icon: "success",
                    title: "Xoá ảnh thành công"
                });

            }
        });
      
    });
    /*start update anh dai dien */

    $('input[type="checkbox"].is-default-checkbox').change(function () {
        var $checkbox = $(this);
        var isDefault = $checkbox.is(':checked');
        var productImageId = $(this).data('productimageid');
        var producdetailid = $(this).data('producdetailid');
        var alias = $(this).data('alias');
        var url = $checkbox.data('url');
/*        console.log("chekcboc " + productImageId + producdetailid + alias + url);*/
        console.log("productImageId " + productImageId   );
        console.log("producdetailid " + producdetailid) ;
        console.log("alias " + alias );
        console.log("url " + url);
        $.ajax({
            url: '/Admin/ProductDetail/UpdateImageisDefault',
            type: 'POST',
            data: {
                alias: alias,
                producdetailid: producdetailid,
                productImageId: productImageId,
                url: url,
                isDefault: isDefault
            },
            success: function (response) {
                if (response.success) {

                    if (isDefault) {
                       
                        $('input[type="checkbox"].is-default-checkbox').not($checkbox).prop('checked', false);
                    }
                } else {
                    alert('Lỗi cập nhập ảnh đại điện : ' + response.message);
                }
            },
            error: function () {
                alert('An error occurred while updating the image.');
            }
        });
    });
});



$(document).ready(function () {

    function getSelectedProductId() {
        // Lấy ID của checkbox được chọn
        const selectedCheckbox = $('.select-option-checkbox input[type="checkbox"]:checked');

        return selectedCheckbox.length > 0 ? selectedCheckbox.data('productid') : null;
    }
    function validateForm() {

        const ProductsId = getSelectedProductId();
        const quantity = parseFloat($('#Quantity').val().trim());
        const image = $('#tCurrentId').val().trim();
        const price = parseInt($('#demoPrice').val().replace(/\D/g, ''), 10);
        const priceSale = parseInt($('#demoPriceSale').val().replace(/\D/g, ''), 10);
        const originalPrice = parseInt($('#demoOriginalPrice').val().replace(/\D/g, ''), 10);
        const Ram = $('#Ram').val();
        const Capacity = $('#Capacity').val();
        const Color = $('#Color').val();


        let isValid = true;
        console.log('ProductsId'+ProductsId);
        console.log('quantity' + quantity);
        console.log('price' + price);
        console.log('priceSale' + priceSale);
        console.log('originalPrice' + originalPrice);
        console.log('Ram' + Ram);
        console.log('Capacity' + Capacity);
        console.log('Color' + Color);
      
        const allFields = [
            'Quantity',
            'demoOriginalPrice', 'demoPriceSale', 'demoPrice', 'Color'
        ];

        const isAllEmpty = allFields.every(id => $(`#${id}`).val().trim() === "");

        if (isAllEmpty) {
            showError(allFields, "Vui lòng điền đầy đủ thông tin .");
            return false;
        }


        if (!ProductsId) {
            Swal.fire({
                icon: "warning",
                title: "Chú ý",
                text: "Vui lòng chọn sản phẩm chính.",
            });
            /*  customSelect('customSelect', "Vui lòng chọn sản phẩm chính.");*/
            $('.custom-select').addClass('error-border');
            isValid = false;

        } else {
            $('.custom-select').removeClass('error-border');
        }

        if (!Color) {
            showError('Color', "Vui lòng nhập màu .");
            $('#Color').addClass('error-border');
            isValid = false;
        } else {
            $('#Color').removeClass('error-border');
        }


        if (!Ram || Ram === "-Chọn Ram sản phẩm-") {
            showError('Ram', "Vui lòng chọn mục sản phẩm.");
            $('#Ram').addClass('error-border');
            isValid = false;
        } else {
            $('#Ram').removeClass('error-border');
        }

        if (!Capacity || Capacity === "-Chọn dung lượng sản phẩm-") {
            showError('Capacity', "Vui lòng chọn mục sản phẩm.");
            $('#Capacity').addClass('error-border');
            isValid = false;
        } else {
            $('#Capacity').removeClass('error-border');
        }


        if (isNaN(quantity) || quantity <= 0) {
            showError('Quantity', "Vui lòng điền số lượng sản phẩm.");
            $('#Quantity').addClass('error-border');
            isValid = false;
        } else {
            removeError('Quantity');
        }

        if (isNaN(price) || isNaN(originalPrice) || price === 0 || originalPrice === 0) {
            showError(['demoPrice', 'demoPriceSale', 'demoOriginalPrice'], "Vui lòng điền đầy đủ giá sản phẩm.");
            $('.demoPrice').addClass('error-border');
            $('.demoPriceSale').addClass('error-border');
            $('.demoOriginalPrice').addClass('error-border');
            isValid = false;
        } else {
            removeError(['demoPrice', 'demoPriceSale', 'demoOriginalPrice']);
        }

        if (price < originalPrice) {
            showError(['demoPrice', 'demoOriginalPrice'], "Giá bán không được bé hơn giá nhập.");
            $('.demoPrice').addClass('error-border');
            $('.demoOriginalPrice').addClass('error-border');
            isValid = false;
        } else {
            removeError(['demoPrice', 'demoOriginalPrice']);
        }

        if (priceSale > 1 && priceSale < originalPrice) {
            showError(['demoPriceSale', 'demoOriginalPrice'], "Giá bán khuyến mãi không được bé hơn giá nhập.");
            $('.demoPriceSale').addClass('error-border');
            $('.demoOriginalPrice').addClass('error-border');
            isValid = false;
        } else {
            removeError(['demoPriceSale', 'demoOriginalPrice']);
        }

        //if (!image) {
        //    Swal.fire({
        //        icon: "warning",
        //        title: "Chú ý",
        //        text: "Vui lòng chọn ảnh sản phẩm.",
        //    });
        //    isValid = false;
        //}

        return isValid;
    }

    // Hàm hiển thị lỗi
    function showError(elements, message) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                document.getElementById(id).classList.add('error-border');
            });
        } else {
            document.getElementById(elements).classList.add('error-border');
        }

        Swal.fire({
            icon: "warning",
            title: "Chú ý",
            text: message,
        });
    }

    // Hàm xóa lỗi
    function removeError(elements) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                document.getElementById(id).classList.remove('error-border');
            });
        } else {
            document.getElementById(elements).classList.remove('error-border');
        }
    }

    // Hàm xử lý thành công
    function OnSuccessCO(res) {
        if (res.Success) {
            if (res.Code === 1) {
                Swal.fire({
                    icon: "success",
                    title: "Sửa sản phẩm thành công",
                    toast: true,
                    position: "top-end",
                    showConfirmButton: false,
                    timer: 1500,
                    timerProgressBar: true,
                });

                setTimeout(() => window.location.href = "/san-pham-con", 1500);
            } else {
                // Xử lý các mã phản hồi khác nếu cần
            }
        } else {
            let errorMessage = null;
            switch (res.Code) {
                case -2:
                    Swal.fire({
                        icon: "warning",
                        title: "Chú ý",
                        text: res.msg,
                    });

                    break;
                case -3:
                    Swal.fire({
                        icon: "warning",
                        title: "Chú ý",
                        text: res.msg,
                    });

                    break;

                case -100:
                    errorMessage = "Lỗi hệ thống...";
                    break;
            }
            if (errorMessage != null) {
                Swal.fire({
                    icon: "error",
                    title: "Lỗi",
                    text: errorMessage,
                });
            }

        }
    }

    // Hàm xử lý submit form
    $('#editProductDetail').submit(function (event) {
        event.preventDefault();


        let isValid = validateForm();

        const ProductsId = getSelectedProductId();
        console.log(ProductsId);
        if (!ProductsId) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: 'Vui lòng chọn một sản phẩm hợp lệ!',
            });
            return;
        }
        // Gửi dữ liệu form nếu hợp lệ
        if (isValid) {
            for (var instance in CKEDITOR.instances) {
                CKEDITOR.instances[instance].updateElement();
            }
            const formData = $('#editProductDetail').serialize();
            const token = $('input[name="__RequestVerificationToken"]').val();
            console.log('Form Data:', formData);

            $.ajax({
                url: '/Admin/ProductDetail/Edit',
                type: 'POST',
                data: formData,
                success: function (res) {
                    OnSuccessCO(res);
                },
                error: function (xhr) {
                    Swal.fire({
                        icon: "error",
                        title: "Lỗi",
                        text: "Hệ thống gặp lỗi thêm sản phẩm.",
                    });
                    console.error(xhr.responseText);
                }
            });
        } else {
            console.log("Form validation failed");
        }
    });
});

