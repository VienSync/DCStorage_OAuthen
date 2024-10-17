var authWindowOpened = false;
function onclickDownload_File(datasequen) {
    var loadingIndicator = document.getElementById("spinLoadingSearch");
    loadingIndicator.style.display = "block";
    $("#slData_seq").text(datasequen);
    $("#slSates").text("clkdowload");
    authWindowOpened = false;
    checkToken(datasequen)
        .then(function (result) {
            loadingIndicator.style.display = "none";
            if (result.success) {
                return downloadService(datasequen);
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
                    showAlertModal("検索画面(試用環境_DV)", downloadResult.message, "#00CC99");
                } else {
                    showAlertModal("検索画面(試用環境_DV)", downloadResult.message, "orange");
                }
            }
        })
        .catch(function (error) {
            alert('Error occurred: ' + error);
        });
}

// JWT Authen
function onclickDownload_FileJWT(datasequen) {
    var loadingIndicator = document.getElementById("spinLoadingSearch");
    loadingIndicator.style.display = "block";
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: "/Search/DownloadMultipleFiles",
            type: "POST",
            data: { dataseq: datasequen },
            success: function (result) {
                loadingIndicator.style.display = "none";
                if (result && result.fileUris && result.fileUris.length > 0) {
                    result.fileUris.forEach(function (fileUri) {
                        window.open(fileUri, '_blank');
                    });
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

function downloadFile(fileUrl) {
    var link = document.createElement("a");
    link.href = fileUrl;
    link.download = fileUrl.substring(fileUrl.lastIndexOf("/") + 1);
    link.target = "_blank";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

//document.addEventListener("DOMContentLoaded", function () {
//    // Format Order_Amount when the page is loaded
//    var orderAmountElement = document.getElementById("formattedOrderAmount");
//    if (orderAmountElement) {
//        var orderAmount = parseFloat(orderAmountElement.innerText.replace(/,/g, ''));
//        if (!isNaN(orderAmount)) {
//            orderAmountElement.innerText = orderAmount.toLocaleString('ja-JP');
//        }
//    }
//});


$(function () {
    $("#start_Date, #end_Date, #start_Toroku, #end_Toroku").datepicker({
        dateFormat: "yy年mm月dd日"
    });
});

function clearDate(inputId) {
    $('#' + inputId).val('');
}


function onclickDetail(data_seq, userId) {
    var loadingIndicator = document.getElementById("spinLoadingSearch");
    loadingIndicator.style.display = "block";
    $("#slData_seq").text(data_seq);
    $("#slSates").text("clkdetail");
    var baseUrl = window.location.origin;
    var url = baseUrl + '/Search/CheckAndRefreshToken';
    //チェックボックスの検証申請を送るのためにAjax を使用
    $.ajax({
        type: 'POST',
        url: url,
        data: { data_seq: data_seq },
        success: function (result) {
            //Severから結果をチェックする。
            if (result.success === true) {
                // チェックボックスがチェックされている場合（OK）、アクション「Index」にリダイレクトします。
                window.location.href = baseUrl + '/Detail/Index?data_seq=' + data_seq + "&userId=" + userId;
                loadingIndicator.style.display = "none";
            } else {
                loadingIndicator.style.display = "none";
                // チェックボックスがチェックされていない場合（NG）、新しいウィンドウで認証を開きます。
                var width = 600;
                var height = 600;
                var left = (window.innerWidth - width) / 2 + window.screenX;
                var top = (window.innerHeight - height) / 2 + window.screenY;
                window.open("/Registor/AuthenticateWithBox", "_blank", "width=" + width + ",height=" + height + ",left=" + left + ',top=' + top);
            }
        },
        error: function () {
            loadingIndicator.style.display = "none";
            alert('Error occurred.');
        }
    });
}

function onclickDetailJWT(data_seq, userId, getValueFrom) {
    var baseUrl = window.location.origin;
    var url = baseUrl + '/Detail/Index?data_seq=' + data_seq + "&userId=" + userId + "&getValueFrom=" + JSON.stringify(getValueFrom);
    window.location.href = url;
}

function onclickRegistor(userId) {
    var baseUrl = window.location.origin;

    // コントローラー「Home」のアクション「Index」へのURLを作成します。
    var url = baseUrl + '/Registor/Index?UserId=' + userId;

    // アクション「Index」にリダイレクトします。
    window.location.href = url;
}

function getValueFrom() {
    if ($("#toggleButton").is(":checked")) {
        $("#statusToggle").val(true);
    } else {
        $("#statusToggle").val(false);
    }
    
    var orderNo = document.getElementById('Order_No').value;
    var workNo = document.getElementById('Work_No').value;
    var supplierCd = document.getElementById('Supplier_Cd').value;
    var Supplier_Name = document.getElementById('Supplier_Name').value;
    var Order_Date1 = document.getElementById('start_Date').value;
    var Order_Date2 = document.getElementById('end_Date').value;
    var Order_Amount1 = document.getElementById('myInput1').value;
    var Order_Amount2 = document.getElementById('myInput2').value;
    var Currency_Ut = document.getElementById('Currency_Ut').value;
    var Report_Typ = document.getElementById('Report_Typ').value;
    var Reg_Date_Start = document.getElementById('start_Toroku').value;
    var Reg_Date_End = document.getElementById('end_Toroku').value;
    var Report_Fmt = document.getElementById('Report_Fmt').value;
    var Personal_Cd = document.getElementById('Personal_Cd').value;
    var statusToggle = document.getElementById('statusToggle').value;
    var Del_Flag = document.getElementById('Del_Flag').value;

    return [orderNo, workNo, supplierCd, Supplier_Name, Order_Date1, Order_Date2, Order_Amount1, Order_Amount2, Currency_Ut,
        Report_Typ, Reg_Date_Start, Reg_Date_End, Report_Fmt, Personal_Cd, statusToggle, Del_Flag];
}

$(function () {
    $('#Report_Fmt').val($('input[name="Deta"]:checked').val());
    $('#Del_Flag').val($('input[name="Option"]:checked').val());

    $('input[name="Deta"]').change(function () {
        var selectedValue = $('input[name="Deta"]:checked').val();

        $('#Report_Fmt').val(selectedValue);
    });

    $('input[name="Option"]').change(function () {
        var flagDel = $('input[name="Option"]:checked').val();

        $('#Del_Flag').val(flagDel);
    });

    $("#Kensaku").on("click", function (e) {
        clearTable();
        if ($("#toggleButton").is(":checked")) {
            $("#statusToggle").val(true);
        } else {
            $("#statusToggle").val(false);
        }

        //取引日付の要件をチェック
        var startDate_hizuke = new Date(document.getElementById('start_Date').value.replace("年", "/").replace("月", "/").replace("日", "/"));
        var endDate_hizuke = new Date(document.getElementById('end_Date').value.replace("年", "/").replace("月", "/").replace("日", "/"));
        if (startDate_hizuke > endDate_hizuke) {
            $("#FailMsg").text("「項目取引日付では、後の日付の値は前の日付の値よりも大きくなければなりません。」");
            return false;
        }
        //証憑登録日付の要件をチェック
        var startDate_toroku = new Date(document.getElementById('start_Toroku').value.replace("年", "/").replace("月", "/").replace("日", "/"));
        var endDate_toroku = new Date(document.getElementById('end_Toroku').value.replace("年", "/").replace("月", "/").replace("日", "/"));
        if (startDate_toroku > endDate_toroku) {
            $("#FailMsg").text("「項目証憑登録日付では、後の日付の値は前の日付の値よりも大きくなければなりません。」");
            return false;
        }

        //取引金額の要件をチェック
        let inputValue1 = document.getElementById("myInput1").value;
        let inputValue2 = document.getElementById("myInput2").value;
        if (inputValue1 >= 0 && inputValue2 >= 0) {
            if (parseFloat(inputValue2) < parseFloat(inputValue1)) {
                $("#FailMsg").text("「項目取引金額では、後の値は前の値よりも大きくなければなりません。」");
                return false;
            }
        }
        $("form").submit();
    });

    function clearTable() {
        var table = document.getElementById('myTableBody');
        var rows = table.getElementsByTagName('tr');
        while (rows.length > 0) {
            table.deleteRow(0);
        }
    }


    


    $('input[name="Deta"]').change(function () {
        // 選択されたラジオボタンの値を取得する
        var selectedValue = $('input[name="Deta"]:checked').val();

        // Report_Fmt フィールドの値を更新する
        $('#Report_Fmt').val(selectedValue);
    });
});

function showAlertModal(title, content, color) {
    // Đặt title và content
    $("#modalHeader").css("background-color", color);
    $("#exampleModalLabel").text(title);
    $("#errorContent").text(content);
    // Display Modal
    $("#searchModal").modal("show");
}

window.addEventListener("message", function (event) {
    if (event.data === "success") {
        var baseUrl = window.location.origin;
        var userId = $("#UserId").val();
        var data_seq = $("#slData_seq").text();
        debugger;
        if ($("#slSates").text() == "clkdetail") {
            window.location.href = baseUrl + '/Detail/Index?data_seq=' + data_seq + "&userId=" + userId;
        }
        else {
            downloadService(data_seq);
        }
    }
}, false);

$(document).ready(function () {
    $("#toggleButton").change(function () {
        if ($(this).is(":checked")) {
            $("#status").text("On");
            $("#toggleView").slideDown();
            $("#statusToggle").val(true);
        } else {
            $("#status").text("Off");
            $("#toggleView").slideUp();
            $("#radio_Kata1").prop("checked", true);
            $('#Del_Flag').val("0");
            $("#statusToggle").val(false);
        }
    });
});