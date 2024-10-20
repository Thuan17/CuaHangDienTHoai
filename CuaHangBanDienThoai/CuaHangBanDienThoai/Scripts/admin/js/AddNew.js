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



$(document).ready(function () {
    let imageIndex = 0;


    $('#btnAddImage').click(function () {
        imageIndex++;
        $('#imageContainer').append(`
                    <div class="form-group image-group" data-index="${imageIndex}">
                        <label for="imageUrl_${imageIndex}">Image  ${imageIndex}</label>
                        <input type="text" class="form-control" id="imageUrl_${imageIndex}" name="ImageNewDetail[${imageIndex}].ImageUrl" />
                        <input type="hidden" name="ImageNewDetail[${imageIndex}].IsDefault" value="false" />
                    </div>
                `);
    });

    let instagramIndex = 0;

    $('#btnAddInstagram').click(function () {
        instagramIndex++;
        $('#instagramContainer').append(`
        <div class="form-group instagram-group" data-index="${instagramIndex}">
            <label for="instagramUrl_${instagramIndex}">Instagram URL ${instagramIndex}</label>
            <input type="text" class="form-control" id="instagramUrl_${instagramIndex}" name="NewDetail[${instagramIndex}].LinkIg" />
        </div>
    `);
    });





    $('.expressitem').on('click', function () {
        var checkbox = $(this).find('input[type="checkbox"]');
        var isChecked = checkbox.prop('checked');
        checkbox.prop('checked', !isChecked);
        const groupName = checkbox.attr('name');
        $(`input[name='${groupName}']`).not(checkbox).prop('checked', false);
        $(`input[name='${groupName}']`).closest('.expressitem').removeClass('highlight');
        if (!isChecked) {
            $(this).addClass('highlight');
        } else {
            $(this).removeClass('highlight');
        }
    });

    function showErrorAddress(elements, message) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    const parent = element.closest('.custom__select');
                    if (parent) {
                        parent.classList.add('error');
                    }

                    const label = document.querySelector(`label[for='${id}']`);
                    if (label) {
                        label.classList.add('error');
                    }
                }
            });
        } else {
            const element = document.getElementById(elements);
            if (element) {
                const parent = element.closest('.custom__select');
                if (parent) {
                    parent.classList.add('error');
                }

                const label = document.querySelector(`label[for='${elements}']`);
                if (label) {
                    label.classList.add('error');
                }
            }
        }

        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: message,
        });
    }

    function removeErrorAddress(elements) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    const parent = element.closest('.custom__select');
                    if (parent) {
                        parent.classList.remove('error');
                    }

                    const label = document.querySelector(`label[for='${id}']`);
                    if (label) {
                        label.classList.remove('error');
                    }
                }
            });
        } else {
            const element = document.getElementById(elements);
            if (element) {
                const parent = element.closest('.custom__select');
                if (parent) {
                    parent.classList.remove('error');
                }

                const label = document.querySelector(`label[for='${elements}']`);
                if (label) {
                    label.classList.remove('error');
                }
            }
        }
    }

    function showError(elements, message) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    element.classList.add('error-border');
                    const parent = element.closest('.input-data');
                    if (parent) {
                        parent.classList.add('error');
                    }
                }
            });
        } else {
            const element = document.getElementById(elements);
            if (element) {
                element.classList.add('error-border');
                const parent = element.closest('.input-data');
                if (parent) {
                    parent.classList.add('error');
                }
            }
        }

        Swal.fire({
            icon: "error",
            title: "Lỗi",
            text: message,
        });
    }

    function removeError(elements) {
        if (Array.isArray(elements)) {
            elements.forEach(id => {
                const element = document.getElementById(id);
                if (element) {
                    element.classList.remove('error-border');
                    const parent = element.closest('.input-data');
                    if (parent) {
                        parent.classList.remove('error');
                    }
                }
            });
        } else {
            const element = document.getElementById(elements);
            if (element) {
                element.classList.remove('error-border');
                const parent = element.closest('.input-data');
                if (parent) {
                    parent.classList.remove('error');
                }
            }
        }
    }

    function validateForm() {
        const Title = $('#Title').val().trim();
        const LinkIg = $('#LinkIg').val().trim();
        const InstagramTitle = $('#InstagramTitle').val().trim();
      
        const content = CKEDITOR.instances.txtContent.getData().trim(); 

        let isValid = true;
      

        if (!Title) {
            showError('Title', "Vui lòng điền tiều đề .");
            isValid = false;
        } else {
            removeError('Title');
        }
        if (!content) {
            

            Swal.fire({
                icon: "error",
                title: "Lỗi",
                text: "Vui lòng nhập nội dung bài viết.",
            });
            isValid = false;
        } else {
            removeError('txtContent'); 
        }

        if (LinkIg) {
            if (!InstagramTitle) {
                showError('InstagramTitle', "Vui lòng điền tiêu đề liên kết.");
                isValid = false;
            } else {
                removeError('InstagramTitle');
            }
        } else {
            removeError('InstagramTitle');
        }

        return isValid;
    }
    function OnSuccessCO(res) {
        if (res.Success) {
            if (res.Code == 1) {
                location.href = "/mua-hang-thanh-cong";

            }
            else {
                location.href = res.Url;
            }
        }
        else {
            if (res.Code == -1) {
                Swal.fire({
                    icon: "error",
                    title: "Không thể bỏ trống...",
                    text: "Vui lòng điền đầy đủ thông tin",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }
            if (res.Code == -2) {
                // Chuỗi thông báo mặc định
                var message = "Số lượng không đủ...\n";

                // Hiển thị danh sách sản phẩm không đủ số lượng (nếu có)
                var insufficientItems = res.InsufficientItems;
                if (insufficientItems && insufficientItems.length > 0) {
                    message += "Các sản phẩm sau không đủ số lượng:\n";
                    insufficientItems.forEach(function (item) {
                        message += item.ProductName + "\n"; // Thay .ProductName bằng thuộc tính tên sản phẩm trong đối tượng ShoppingCartItem
                    });
                }

                // Hiển thị cảnh báo về số lượng không đủ hoặc hết hàng
                Swal.fire({
                    icon: "warning",
                    title: "Số lượng không đủ",
                    text: message
                });

                // Thực hiện các hành động khác sau khi nhận phản hồi
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");
            }

            if (res.Code == -3) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi hệ thống..",
                    text: "Tài khoản tạm ngừng",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }
            if (res.Code == -4) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi hệ thống..",
                    text: "Dữ liệu không hợp lệ",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }
            if (res.Code == -5) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi hệ thống..",
                    text: "Hệ thống đang bảo trì",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }
            if (res.Code == -7) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi giảm giá ..",
                    text: "Áp dụng voucher thất bại",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");

            }
            if (res.Code == -100) {
                Swal.fire({
                    icon: "warning",
                    title: "Lỗi hệ thống..",
                    text: "Mua hàng thất bại",

                });
                $('#btnCheckOut').prop("disabled", false);
                $('#load_send').css("display", "none");


            }

        }
    }

    $('#themMoiBaiViet').submit( function (event) {
        event.preventDefault();
      
        let isValid = validateForm();
      
     
        if (isValid) {
            for (var instance in CKEDITOR.instances) {
                CKEDITOR.instances[instance].updateElement();
            }
            const formData = $('#themMoiBaiViet').serialize();
            const token = $('input[name="__RequestVerificationToken"]').val();
            console.log('Form Data:', formData); 
            
            $.ajax({
               
                url: '/Admin/News/Add',
                type: 'POST',
                data: formData,
                success: function (res) {
                  
                    if (res.Success) {
                        if (res.Code === 1) {
                           
                            createToast('success', 'fa-solid fa-circle-exclamation', 'Thành công ',  res.msg);
                            setTimeout(() => {
                                window.location.href = res.Url;
                            }, 1500);
                        }
                    } else {
                        console.log('Error Code:', res.Code); createToast('warning', 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
                        //if (res.Code === -1) {
                        //    createToast('warring', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Vui lòng điền đủ thông tin ');
                        //} else if (res.Code === -2) {
                        //    $('.loading-overlay').hide();
                        //    $('.sent').hide();
                        //    $('.btnCheckOutS').prop('disabled', false);
                        //    if (res.InsufficientItems && res.InsufficientItems.length > 0) {
                        //        message = "\n";
                        //        res.InsufficientItems.forEach(function (item) {
                        //            message += "\n" + item.ProductName + "\n";
                        //        });

                        //        Swal.fire({
                        //            icon: "warning",
                        //            title: "Số lượng không đủ",
                        //            text: message
                        //        });
                        //    } else {
                        //        createToast('warring', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Số lượng kho không đủ');
                        //    }

                        //    /*  createToast('warring', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Số lượng kho không đủ');*/

                        //} else if (res.Code === -99) {
                        //    createToast('warring', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Lỗi máy chủ');
                        //}


                    }
                },
                error: function (xhr) {
                    $('.loading-overlay').hide();
                    $('.sent').hide();
                    $('.btnCheckOutS').prop('disabled', false);

                    Swal.fire({
                        icon: "error",
                        title: "Lỗi",
                        text: "Hệ thống gặp lỗi thêm sản phẩm.",
                    });
                    console.error('Error:', xhr.responseText);
                }
            });
        } else {
            console.log("Form validation failed");
        }
    });

});

function BrowseServer(field) {
    var finder = new CKFinder();
    finder.selectActionFunction = function (fileUrl) {
        addImageProduct(fileUrl);
    };
    finder.popup();
}


function addImageProduct(url) {
    var temp = $('#tCurrentId').val();
    var currentId = parseInt(temp) + 1;
    var str = "";
    if (currentId == 1) {
        str += `<tr id="trow_${currentId}">
                                <td class="text-center">${currentId}</td>
                                <td class="text-center"><img width="80" src="${url}" /> <input type='hidden' value="${url}" name="ImageNewDetail"/></td>
                                <td class="text-center"><input type="radio" name="rDefault" value="${currentId}" checked="checked"/></td>
                                <td class="text-center"><a href="#" data-id="${currentId}" class="btn btn-sm btn-danger btnXoaAnh">
                                <svg xmlns="http://www.w3.org/2000/svg" width=18" height="18" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
  <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
</svg>
                                
                                </a></td>
                                </tr>`;
    }
    else {
        str += `<tr id="trow_${currentId}">
                                <td class="text-center">${currentId}</td>
                                <td class="text-center"><img width="80" src="${url}" /> <input type='hidden' value="${url}" name="ImageNewDetail"/></td>
                                <td class="text-center"><input type="radio" name="rDefault" value="${currentId}"/></td>
                                <td class="text-center"><a href="#" data-id="${currentId}" class="btn btn-sm btn-danger btnXoaAnh">
                                                                <svg xmlns="http://www.w3.org/2000/svg" width=18" height="18" fill="currentColor" class="bi bi-trash3-fill" viewBox="0 0 16 16">
  <path d="M11 1.5v1h3.5a.5.5 0 0 1 0 1h-.538l-.853 10.66A2 2 0 0 1 11.115 16h-6.23a2 2 0 0 1-1.994-1.84L2.038 3.5H1.5a.5.5 0 0 1 0-1H5v-1A1.5 1.5 0 0 1 6.5 0h3A1.5 1.5 0 0 1 11 1.5m-5 0v1h4v-1a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5M4.5 5.029l.5 8.5a.5.5 0 1 0 .998-.06l-.5-8.5a.5.5 0 1 0-.998.06m6.53-.528a.5.5 0 0 0-.528.47l-.5 8.5a.5.5 0 0 0 .998.058l.5-8.5a.5.5 0 0 0-.47-.528M8 4.5a.5.5 0 0 0-.5.5v8.5a.5.5 0 0 0 1 0V5a.5.5 0 0 0-.5-.5"/>
</svg>
                                </a></td>
                                </tr>`;
    }
    $('#tbHtml').append(str);
    $('#tCurrentId').val(currentId);

}