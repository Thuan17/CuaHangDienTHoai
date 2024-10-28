
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


function BrowseServerUpdate(element) {
    var index = element.getAttribute('data-index');
    var finder = new CKFinder();
    finder.selectActionFunction = function (fileUrl) {
        addImageProduct(fileUrl);
    };
    finder.popup();
}

function checkImage(image) {
    const width = image.naturalWidth;
    const height = image.naturalHeight;
    const aspectRatio = width / height;
    const isCorrectAspectRatio = aspectRatio === (1920 / 1080);
    return isCorrectAspectRatio;
}


function addImageProduct(url) {
    var currentId = 1;

    var imgCheck = new Image();
    imgCheck.src = url;
    imgCheck.onload = function () {
        if (checkImage(imgCheck)) {
            $('#tbHtml').empty();


            var str = `
                <div class="row m-0 p-0 w-100" >
                    <img src="${url}" data-productid="${currentId}" data-productImage="${url}" class="text-center" data-index="${currentId}" style="width:100%" height="500" alt="Hình ảnh sản phẩm" onclick="BrowseServerUpdate(this);" />
                 
                    <input type='hidden' value="${url}"id="Images" name="Images"/>
                </div>`;

            $('#tbHtml').html(str);


            $('#tCurrentId').val(currentId);
        } else {
            createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', 'Tỷ lệ ảnh không đúng 1920x1080 px.');
        }
    };
}



$(document).ready(function () {
    $('#myFormBannerEdit').submit(function (e) {
        e.preventDefault();
        console.log("thuan");
        const formData = $('#myFormBannerEdit').serialize();
        $.ajax({
            url: '/admin/banner/edit',
            type: 'POST',
            data: formData,
            success: function (res) {
                if (res.success) {
                    if (res.Code = 1) {
                        createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg);
                        setTimeout(() => window.location.href = res.url, 1500);
                    }
                } else {
                    if (res.Code = -2) {
                        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', res.msg);
                        setTimeout(() => window.location.href = res.url, 1500);
                    }
                    if (res.Code = -3) {
                        createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', res.msg);

                    }
                }
            },
            error: function (xhr) {
                createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', 'Có lỗi trong hệ thống');
                console.error(xhr.responseText);
            }

        });

    });
});


