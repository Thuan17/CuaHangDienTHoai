$(document).ready(function () {
    var btnSuceess = $(".btnSuceess");
    var bg2 = $(".bg-sg2");
    var popup2 = $("#popupBill2");
    var btnCancel = $('.btnCancel');
    var btnReturn = $('.btnReturn');
    bg2.on('click', function () {
        bg2.hide();
        popup2.hide();
    });

    $('.btnCloseEditBill').on('click', function () {
        bg2.hide();
        popup2.hide();
    });
   

    btnSuceess.each(function () {
        $(this).on('click', function (e) {
         
            e.preventDefault();
            var orderid = $(this).data('id');
          
            if (orderid) {
                $.ajax({
                    url: '/Cart/SuccessOrder',
                    type: 'POST',
                    data: { orderid: orderid },
                    success: function (res) {
                        if (res.Success) {
                            if (res.Code == 1) {
                              
                                $.ajax({
                                    url: '/Cart/GetUpdatedOrderRow',
                                    type: 'POST',
                                    data: { orderid: orderid },
                                    success: function (rs) {
                                        createToast('success', 'fa-solid fa-circle-check', 'Thành công', res.msg);
                                        $('.card_IndexOrder_' + orderid).replaceWith(rs);
                                    }, error: function (xhr, status, error) {
                                      
                                        createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Load dữ liệu mới');
                                        window.location.reload();
                                    }

                                });
                            }
                            
                        } else {
                            if (res.Code = -2) {
                                createToast('warning', 'fa-solid fa-triangle-exclamation', 'Thất bại', res.msg);
                            } else if (res.Code = -99) {
                                createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', res.msg);
                            }
                        }
                    }, error: function (res) {
                        createToast('error', 'fa-solid fa-circle-exclamation', 'Thất bại', 'Load dữ liệu mới');
                    }
                });
            }

        });
        

    });


    btnCancel.each(function () {
       
        $(this).on('click', function (e) {
            bg2.show();
            popup2.show();
            console.log("btnCancel");

            var orderid = $(this).data('id')
            if (orderid) {
                $.ajax({
                    url: '/Cart/Partial_CancelOrder',
                    data: { orderid: orderid },
                    type: 'GET',
                    success: function (response) {
                        $("#loadDataBillEdit").html(response);
                    }, error: function (xhr, status, error) {
                        console.error(xhr.responseText);
                    }
                } );
            }
           
        });

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