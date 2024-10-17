// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var alertModalVisible = false;
function Delete() {
    alertModalVisible = false;

    $("#modalDelCause").val($("#idDelCause").val());

    $('#DeleteModal').modal('show');

    $('#DeleteModal').on('hide.bs.modal', function (e) {
        if (!alertModalVisible) {
            e.preventDefault();
        }
    });
    $('#DeleteModal .btn-secondary ,.btn-close').click(function () {
        alertModalVisible = true;
        $('#DeleteModal').modal('hide');
    });

    $('#DeleteModal .btn-primary').click(function () {
        $('#DeleteModal').modal('hide');
        $("#idDelCause").val($("#modalDelCause").val());
        $("form").submit();
    });
}

//function onclickViewBox() {
//    window.open('https://app.box.com/folder/0', '_blank');
//}

function onclickSearch(userId, Value_Search) {
    var valuesArray = JSON.parse(Value_Search);

    var form = $('<form action="/Search/Index" method="post">' +
        '<input type="hidden" name="UserId" value="' + userId + '" />' +
        '<input type="hidden" name="Order_No" value="' + valuesArray[0] + '" />' +
        '<input type="hidden" name="Work_No" value="' + valuesArray[1] + '" />' +
        '<input type="hidden" name="Supplier_Cd" value="' + valuesArray[2] + '" />' +
        '<input type="hidden" name="Supplier_Name" value="' + valuesArray[3] + '" />' +
        '<input type="hidden" name="Order_Date1" value="' + valuesArray[4] + '" />' +
        '<input type="hidden" name="Order_Date2" value="' + valuesArray[5] + '" />' +
        '<input type="hidden" name="Order_Amount1" value="' + valuesArray[6] + '" />' +
        '<input type="hidden" name="Order_Amount2" value="' + valuesArray[7] + '" />' +
        '<input type="hidden" name="Currency_Ut" value="' + valuesArray[8] + '" />' +
        '<input type="hidden" name="Report_Typ" value="' + valuesArray[9] + '" />' +
        '<input type="hidden" name="Reg_Date_Start" value="' + valuesArray[10] + '" />' +
        '<input type="hidden" name="Reg_Date_End" value="' + valuesArray[11] + '" />' +
        '<input type="hidden" name="Report_Fmt" value="' + valuesArray[12] + '" />' +
        '<input type="hidden" name="Personal_Cd" value="' + valuesArray[13] + '" />' +
        '<input type="hidden" name="statusToggle" value="' + valuesArray[14] + '" />' +
        '<input type="hidden" name="Del_Flag" value=\'' + valuesArray[15] + '\' />' +
        '</form>');
    $('body').append(form);
    form.submit();
}

function updateFileContent(idfile) {
    var loadingIndicator = document.getElementById("spinLoadingDetail");
    loadingIndicator.style.display = "block";
    var iframe = document.getElementById('fileIframe');
    $.ajax({
        url: "/Detail/LoadFile",
        type: "POST",
        data: { idfile: idfile },
        success: function (result) {
            loadingIndicator.style.display = "none";
            if (result.success == true) {
                iframe.src = result.srcfile;
            }
            else {
                showAlertModal("エラー", result.message, "orange");
            }
        },
        error: function (error) {
            loadingIndicator.style.display = "none";
            reject(error);
        }
    });
}

// 画面を表示する際に"000,000"フォマットを変更する
// Check 取引金額
function formatNumber(input) {
    let order_amount = input.value.replace(/,/g, '');
    if (isNaN(order_amount) || input.value === '') {
        input.value = '';
    } else {
        let decimalIndex = order_amount.indexOf('.');
        let integerPart = decimalIndex !== -1 ? order_amount.slice(0, decimalIndex) : order_amount;
        let decimalPart = decimalIndex !== -1 ? order_amount.slice(decimalIndex + 1) : '';
        let formattedInteger = integerPart.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        input.value = decimalPart.length > 0 ? formattedInteger + '.' + decimalPart : formattedInteger;
    }
}

document.addEventListener("DOMContentLoaded", function () {
    let order_amount = document.getElementById("Order_Amount");
    formatNumber(order_amount);
});


var authWindowOpened = false;
function onclickViewBox(dataSeq) {
    authWindowOpened = false;
    checkToken(dataSeq)
        .then(function (result) {
            if (result.success) {
                return downloadService(dataSeq);
            } else {
                // Mở cửa sổ chọn thư mục để xác thực lại
                openAuthWindow();
                authWindowOpened = true; // Đặt biến này thành true khi cửa sổ đã được mở
                return new Promise(resolve => { }); // Trả về một Promise trống để tạm dừng chuỗi then
            }
        })
        .then(function (downloadResult) {
            if (!authWindowOpened) { // Kiểm tra xem cửa sổ đã được mở chưa
                if (downloadResult.success) {
                    showAlertModal("このデータを検索から除外しますか？", downloadResult.message, "#00CC99");
                } else {
                    showAlertModal("参照画面", downloadResult.message, "orange");
                }
            }
        })
        .catch(function (error) {
            alert('Error occurred: ' + error);
        });
}

function onclickViewBoxJWT(dataSeq) {
    var loadingIndicator = document.getElementById("spinLoadingDetail");
    loadingIndicator.style.display = "block";

    var dropdown = document.getElementById("fileDropdown");
    var fileID = dropdown.value;
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: "/Detail/DownloadSingleFile",
            type: "POST",
            data: { fileID: fileID },
            success: function (result) {
                loadingIndicator.style.display = "none";
                if (result && result.fileUri) {
                    window.open(result.fileUri, '_blank');
                }
            },
            error: function (error) {
                loadingIndicator.style.display = "none";
                reject(error);
            }
        });
    });
}

// Hàm chuyển đổi base64 thành ArrayBuffer
function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);

    for (var i = 0; i < binaryLen; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }

    return bytes.buffer;
}

function checkToken(datasequen) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: "/Search/CheckAndRefreshToken",
            type: "POST",
            data: { dataseq: datasequen },
            success: function (result) {
                resolve(result);
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function downloadService(datasequen) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: "/Search/DownloadMultipleFiles",
            type: "POST",
            data: { dataseq: datasequen },
            success: function (result) {
                if (result && result.fileUris && result.fileUris.length > 0) {
                    result.fileUris.forEach(function (fileUri) {
                        window.open(fileUri, '_blank');
                    });
                }
            },
            error: function (error) {
                reject(error);
            }
        });
    });
}

function openAuthWindow() {
    var width = 600;
    var height = 600;
    var left = (window.innerWidth - width) / 2 + window.screenX;
    var top = (window.innerHeight - height) / 2 + window.screenY;

    var authWindow = window.open("/Registor/AuthenticateWithBox", "_blank", "width=" + width + ",height=" + height + ",left=" + left + ',top=' + top);
}

function showAlertModal(title, content, color) {
    // Đặt title và content
    alertModalVisible = false;
    $("#modalHeader").css("background-color", color);
    $("#exampleModalLabel").text(title);
    $("#errorContent").text(content);
    // Display Modal
    $("#DetailModal").modal("show");
}
