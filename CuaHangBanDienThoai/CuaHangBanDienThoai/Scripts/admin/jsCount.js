//$(document).ready(function () {
//    CountBillNew();
//    setInterval(function () {
//        CountBillNew();
//    }, 5000);

//});
//function CountBillNew() {
//    $.ajax({
//        url: '/Admin/Bill/CountBillNew',
//        type: 'GET',
//        success: function (rs) {
//            if (rs && typeof rs.Count !== 'undefined') {
               
//                $('.CountBillNew').html(rs.Count);
                
//            } else {
//                $('.CountBillNew').html("0");
//                console.error("Phản hồi không có thuộc tính Count.");
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error("Lỗi khi gọi AJAX: ", error);
//        }
//    });
//}



$(document).ready(function () {
    CKEDITOR.replace('txtContent', {
        customConfig: '/content/ckeditor/config.js',
        extraAllowedContent: 'span',
        allowedContent: true,
    });
   

});

$(document).on('click', '.btnXoaAnh', function (e) {
    e.preventDefault();
    var id = $(this).data('id');
    $('#trow_' + id).remove();
    $('#tCurrentId_Product').val(0);
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
                                        <td class="text-center"><img width="80" src="${url}" /> <input type='hidden' value="${url}" name="Images"/></td>
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
                                        <td class="text-center"><img width="80" src="${url}" /> <input type='hidden' value="${url}" name="Images"/></td>
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

function updateHiddenImages() {
    var images = [];
    $('#tbHtml tr').each(function () {
        var imgSrc = $(this).find('img').attr('src');
        var isDefault = $(this).find('input[name="DefaultImage"]').is(':checked');
        if (imgSrc) {
            images.push({
                Image: imgSrc,
                IsDefault: isDefault
            });
        }
    });
    $('#ImagesJson').val(JSON.stringify(images));
}






