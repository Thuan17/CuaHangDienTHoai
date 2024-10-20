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
function BrowseServer(field) {
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
            url: '/Admin/Product/UpdateImageS',
            data: {
                productid: $(imgElement).data('productid'),
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
                        text: "Xảy ra trong quá trình thêm ảnh 123..."
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




document.addEventListener('DOMContentLoaded', function () {
    var selectedPin = '@ViewBag.SelectedPin';
    var pinDropdown = document.getElementById('Pin');
    if (pinDropdown) {
        pinDropdown.value = selectedPin;
    }
});

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
        var conf = confirm('Bạn có muốn xóa ảnh này không?');
        if (conf === true) {
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
$(document).ready(function () {
    $('input[type="checkbox"].is-default-checkbox').change(function () {
        var $checkbox = $(this);
        var isDefault = $checkbox.is(':checked');
        var productId = $checkbox.data('productid');
        var productImageId = $checkbox.data('productimageid');
        var url = $checkbox.data('url'); // Assuming you have a way to get the URL

        $.ajax({
            url: '/Admin/Product/UpdateImageisDefault', // Replace with your controller name
            type: 'POST',
            data: {
                productId: productId,
                productImageId: productImageId,
                url: url,
                isDefault: isDefault
            },
            success: function (response) {
                if (response.success) {
                   
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
/*End update anh dai dien */


$(document).ready(function () {
    // Khởi tạo CKEditor
    CKEDITOR.replace('txtContent', {
        customConfig: '/content/ckeditor/config.js',
        extraAllowedContent: 'span',
        allowedContent: true,
    });

    // Cập nhật sự hiển thị của các phần tử dựa trên danh mục sản phẩm
    function updateVisibility() {
        const selectedOption = $('#ProductCategoryId option:selected');
        const isGift = selectedOption.data('isgift');

        $('#IsGiftCategory').val(isGift);
        if (isGift === 'True') {
            $('#ProductCategoryId_Group').removeClass('col-4').addClass('col-12');
            $('#PinContainer, #ChipContainer').hide();
        } else {
            $('#ProductCategoryId_Group').removeClass('col-12').addClass('col-4');
            $('#PinContainer, #ChipContainer').show();
        }
    }

    updateVisibility();
    $('#ProductCategoryId').change(updateVisibility);

    // Xử lý sự kiện submit form
    $('#editBill').submit(function (event)
    {
        event.preventDefault();

        let isValid = validateForm();
        const isGift = $('#ProductCategoryId option:selected').data('isgift');

        if (isGift === 'False') {
            isValid = validateProductDetails(isValid);
        }

        if (isValid) {
            // Cập nhật CKEditor
            Object.keys(CKEDITOR.instances).forEach(function (instance) {
                var editor = CKEDITOR.instances[instance];
                if (editor && typeof editor.updateElement === 'function') {
                    try {
                        editor.updateElement();
                    } catch (error) {
                        console.error('Error updating CKEditor instance:', instance, error);
                    }
                } else {
                    console.warn('Instance not valid or updateElement method missing:', instance);
                }
            });

            // Serialize form data
            const formData = new FormData($('#editBill')[0]);
            const token = $('input[name="__RequestVerificationToken"]').val();

            console.log('Form Data:', formData);
            console.log('editBill:');

            $.ajax({
                url: '/Admin/Product/Edit',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                headers: {
                    'X-CSRF-TOKEN': token
                },
                success: function (res) {
                    handleFormSubmissionSuccess(res);
                },
                error: function (xhr) {
                    handleFormSubmissionError(xhr);
                }
            });
        } else {
            console.log("Form validation failed");
        }
    });

    function validateProductDetails(isValid) {
        const chip = $('#Chip').val();
        const pin = $('#Pin').val();

        if (!pin || pin === "-Chọn giờ sản phẩm-") {
            displayError('Pin', "Vui lòng chọn giờ sản phẩm.");
            isValid = false;
        }

        if (!chip || chip === "-Chọn chip sản phẩm-") {
            displayError('Chip', "Vui lòng chọn chip sản phẩm.");
            isValid = false;
        }

        return isValid;
    }

    function validateForm() {
        const Title = $('#Title').val().trim();
        const ProductCategoryId = $('#ProductCategoryId').val().trim();
        const quantity = parseFloat($('#Quantity').val().trim());
        const image = $('#tCurrentId').val().trim();
        const price = parseInt($('#demoPrice').val().replace(/\D/g, ''), 10);
        const priceSale = parseInt($('#demoPriceSale').val().replace(/\D/g, ''), 10);
        const originalPrice = parseInt($('#demoOriginalPrice').val().replace(/\D/g, ''), 10);

        let isValid = true;

        if (!Title) {
            displayError('Title', "Vui lòng điền đầy đủ thông tin.");
            isValid = false;
        }

        if (!ProductCategoryId || ProductCategoryId === "-Chọn danh mục sản phẩm-") {
            displayError('ProductCategoryId', "Vui lòng chọn mục sản phẩm.");
            isValid = false;
        }

        if (isNaN(quantity) || quantity <= 0) {
            displayError('Quantity', "Vui lòng điền số lượng sản phẩm.");
            isValid = false;
        }

        if (isNaN(price) || isNaN(originalPrice) || price === 0 || originalPrice === 0) {
            displayError(['demoPrice', 'demoPriceSale', 'demoOriginalPrice'], "Vui lòng điền đầy đủ giá sản phẩm.");
            isValid = false;
        }

        if (price < originalPrice) {
            displayError(['demoPrice', 'demoOriginalPrice'], "Giá bán không được bé hơn giá nhập.");
            isValid = false;
        }

        if (priceSale > 1 && priceSale < originalPrice) {
            displayError(['demoPriceSale', 'demoOriginalPrice'], "Giá bán khuyến mãi không được bé hơn giá nhập.");
            isValid = false;
        }

        if (!image) {
            Swal.fire({
                icon: "error",
                title: "Lỗi",
                text: "Vui lòng chọn ảnh sản phẩm.",
            });
            isValid = false;
        }

        return isValid;
    }

    function displayError(elements, message) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                $('#' + id).addClass('error-border');
            });
        } else {
            $('#' + elements).addClass('error-border');
        }

        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: message,
        });
    }

    function handleFormSubmissionSuccess(res) {
        if (res.Success) {
            Swal.fire({
                icon: "success",
                title: "Sửa sản phẩm thành công",
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });

            setTimeout(() => window.location.href = "/admin/Product/index", 1500);
        } else {
            displaySubmissionError(res.Code);
        }
    }

    function handleFormSubmissionError(xhr) {
        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: "Hệ thống gặp lỗi thêm sản phẩm.",
        });
        console.error(xhr.responseText);
    }

    function displaySubmissionError(code) {
        let errorMessage = "Có lỗi trong quá trình";
        switch (code) {
            case -2:
                errorMessage = "Vui lòng điền tên tiêu đề";
                break;
            case -3:
                errorMessage = "Danh mục đã tồn tại...";
                break;
            case -5:
                errorMessage = "Chọn ảnh đại diện";
                break;
            case -100:
                errorMessage = "Lỗi hệ thống...";
                break;
        }

        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: errorMessage,
        });
    }
});

