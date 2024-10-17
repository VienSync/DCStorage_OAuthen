document.addEventListener("DOMContentLoaded", function () {
    var labels = document.querySelectorAll('.label-length-fix');
    let registor_Type = document.getElementById("RegistorType");
    labels.forEach(label => {
        if (registor_Type.value === "発注" || registor_Type.value === "検収") {
            label.style.width = "165px";
        } else if (registor_Type.value === "完成" || registor_Type.value === "受注") {
            label.style.width = "140px";
        } else {
            label.style.width = "140px";
        }
    });
});
//check Order No.
function checkOrderNo(input) {
    if (input.value == '') {
        input.setCustomValidity('注番を入力してください！');
    } else {
        input.setCustomValidity('');
    }
}
//check valid Order No.
document.addEventListener("DOMContentLoaded", function () {
    let orderNo = document.getElementById("Order_No");
    checkOrderNo(orderNo);
});

//check 工番
function checkWorkNo(input) {
    if (input.value == '') {
        input.setCustomValidity('工番を入力してください！');
    } else {
        input.setCustomValidity('');
    }
}
//check valid 工番
document.addEventListener("DOMContentLoaded", function () {
    let workNo = document.getElementById("Work_No");
    checkWorkNo(workNo);
});


//check 取引先コード
function checkSupplier_Cd(input) {
    if (input.value == '') {
        input.setCustomValidity('取引先コードを入力してください！');
    } else {
        input.setCustomValidity('');
    }
}
//check valid 取引先コード
document.addEventListener("DOMContentLoaded", function () {
    let Supplier_Cd = document.getElementById("Supplier_Cd");
    checkSupplier_Cd(Supplier_Cd);
});


//check 取引先名称
function checkSupplier_Name(input) {
    if (input.value == '') {
        input.setCustomValidity('取引先名称を入力してください！');
    } else {
        input.setCustomValidity('');
    }
}

//check valid 取引先名称
document.addEventListener("DOMContentLoaded", function () {
    let Supplier_Name = document.getElementById("Supplier_Name");
    checkSupplier_Name(Supplier_Name);
});


//check Currency_Ut
function checkCurrency_Ut(input) {
    if (input.value == '') {
        input.setCustomValidity('通貨を入力してください！');
    } else {
        input.setCustomValidity('');
    }
}

//check valid Currency_Ut
document.addEventListener("DOMContentLoaded", function () {
    let Currency_Ut = document.getElementById("Currency_Ut");
    checkCurrency_Ut(Currency_Ut);
});

// Check 取引金額
function formatNumber(input) {
    input.style.borderColor = "#dee2e6";
    let order_amount = input.value.replace(/[^\d.]/g, ''); // Loại bỏ mọi ký tự ngoại trừ chữ số và dấu chấm
    let dotCount = order_amount.split('.').length - 1;

    if (dotCount > 1 || (dotCount === 2 && input.value.slice(-1) === '.')) {
        order_amount = order_amount.slice(0, -1);
    }

    if (isNaN(order_amount) || input.value === '') {
        input.setCustomValidity('取引金額を数字で入力してください!');
        return false;
    } else if (Number(parseFloat(order_amount)) > 9999999999) {
        input.setCustomValidity('取引金額の最大値は9,999,999,999です!');
        input.value = order_amount.trim() === '' ? '' : addCommasToNumber(order_amount);
        return false;
    } else {
        // Clear error
        input.setCustomValidity('');

        // Xử lý để chỉ cho phép một dấu chấm
        const dotIndex = order_amount.indexOf('.');
        if (dotIndex !== -1) {
            const secondDotIndex = order_amount.indexOf('.', dotIndex + 1);
            if (secondDotIndex !== -1) {
                order_amount = order_amount.slice(0, secondDotIndex);
            }
        }

        input.value = order_amount.trim() === '' ? '' : addCommasToNumber(order_amount);
        return true;
    }
}

function addCommasToNumber(number) {
    const parts = number.toString().split('.');
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    return parts.join('.');
}

function checkNumber(input) {
    let value = input.value.replace(/,/g, '');
    input.style.borderColor = "#dee2e6";

    if (isNaN(value) || input.value == '') {
        input.setCustomValidity('「YYYYMMDD」形式で入力してください！');
        input.value = '';
        return false;
    } else if (input.value.length != 8) {
        input.setCustomValidity('「YYYYMMDD」形式で入力してください！');
        return false;
    } else {
        let year = parseInt(value.substring(0, 4), 10);
        let month = parseInt(value.substring(4, 6), 10);
        let day = parseInt(value.substring(6, 8), 10);

        if (year < 1000 || year > 9999 || month < 1 || month > 12 || day < 1 || day > 31) {
            input.setCustomValidity('「YYYYMMDD」形式で入力してください！');
            return false;
        } else if ((month == 2 && (day < 1 || (isLeapYear(year) ? day > 29 : day > 28))) || ((month == 4 || month == 6 || month == 9 || month == 11) && day > 30)) {
            input.setCustomValidity('「YYYYMMDD」形式で入力してください！');
            return false;
        } else {
            input.setCustomValidity('');
            return true;
        }
    }
}

function isLeapYear(year) {
    return (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
}

//check 証憑フラグ
function checkClass(input) {
    input.style.borderColor = "#dee2e6";
    if (input.value == '') {
        input.setCustomValidity('証憑区分を選択してください！');
    } else {
        input.setCustomValidity('');
    }

    //No 12 . 4
    var selectedValue = input.value;
    var inputElement = document.getElementById("Order_Date_View");
    inputElement.style.borderColor = "#dee2e6";
    if (selectedValue === "検収証憑" || selectedValue === "検収 証憑" || selectedValue.includes("検収")) {
        // Lấy thẻ input
        inputElement.removeAttribute("readonly");
        inputElement.style.backgroundColor = "white";
        document.getElementById("orderDateError").innerHTML = "";
    } else {
        inputElement.setAttribute("readonly", "true");
        inputElement.style.backgroundColor = "#e9ecef";
    }

}

//check valid Order Date
document.addEventListener("DOMContentLoaded", function () {
    let classAuthen = document.getElementById("Report_Typ");
    checkClass(classAuthen);
});

//check valid Order Date
document.addEventListener("DOMContentLoaded", function () {
    let order_date = document.getElementById("Order_Date_View");
    checkNumber(order_date);
});

// 画面を表示する際に"000,000"フォマットを変更する
document.addEventListener("DOMContentLoaded", function () {
    let order_amount = document.getElementById("Order_Amount_View");
    formatNumber(order_amount);
});

function onclickSearch(UserId) {
    var baseUrl = window.location.origin;

    // Tạo URL tới action Index trong controller Home
    var url = baseUrl + '/Search/Index?UserId=' + UserId;

    // Chuyển hướng đến action Index
    window.location.href = url;
}
var alertModalVisible = false;
function showAlertModal(title, content, color) {
    // Đặt title và content
    alertModalVisible = false;
    $("#modalHeader").css("background-color", color);
    $("#exampleModalLabel").text(title);
    $("#errorContent").text(content);
    var closeButton = $('<button type="button" class="btn-close"aria-label="Close"></button>');
    $("#modalHeader").append(closeButton);
    closeButton.on('click', function () {
        alertModalVisible = true;
        $("#alertModal").modal("hide");
        closeButton.remove();
    });

    $('#alertModal').on('hide.bs.modal', function (e) {
        if (!alertModalVisible) {
            e.preventDefault();
        }
    });

    // Display Modal
    $("#alertModal").modal("show");
   
}


window.addEventListener("message", function (event) {
    if (event.data === "success") {
        var form = document.getElementById("saveForm");
        // Form Valid を確認
        if (form.checkValidity()) {
            var formData = new FormData($("#saveForm")[0]);
            performSaveAction(formData);
        } else {
            form.reportValidity();
        }
    }
}, false);

function saveForm() {
    var form = document.getElementById("saveForm");
    // Form Valid を確認
    if (form.checkValidity()) {
        var formData = new FormData($("#saveForm")[0]);
        var loadingIndicator = document.getElementById("spinLoading");
        loadingIndicator.style.display = "block";
        $.ajax({
            url: "/Registor/CheckTokenServices",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                loadingIndicator.style.display = "none";
                if (response.error) {
                    // Token Expried, Login Authen                    
                    var width = 600;
                    var height = 600;
                    var left = (window.innerWidth - width) / 2 + window.screenX;
                    var top = (window.innerHeight - height) / 2 + window.screenY;
                    window.open("/Registor/AuthenticateWithBox", "_blank", "width=" + width + ",height=" + height + ",left=" + left + ',top=' + top);

                } else {
                    performSaveAction(formData);
                }
            },
            error: function (xhr, status, error) {
                loadingIndicator.style.display = "none";
            }
        });

    } else {
        form.reportValidity();
    }
}

function performSaveAction(formData) {
    var loadingIndicator = document.getElementById("spinLoading");
    loadingIndicator.style.display = "block";
    var form = document.getElementById("saveForm");
    var dataForm = new FormData(form);

    // Xóa tất cả các file từ FileList trong formData
    dataForm.delete('FileList');

    // Thêm toàn bộ file từ allSelectedFiles vào formData
    for (var i = 0; i < allSelectedFiles.length; i++) {
        dataForm.append('FileList', allSelectedFiles[i]);
    }
    // Form Valid を確認
    if (form.checkValidity()) {
        $.ajax({
            url: "/Registor/Save",
            type: "POST",
            data: dataForm,
            contentType: false,
            processData: false,
            success: function (response) {
                loadingIndicator.style.display = "none";
                if (response.error) {
                    showAlertModal("エラー", "証憑の登録が失敗しました。\nネットワークの混雑等による一時的なエラーです。\n再度、登録の実施をお願いします。\n再登録で証憑の登録に成功すれば問題ございません。\nDX推進Gの連絡先：g-irm-irm_system_administrator@ihi-g.com", "red");
                } else {
                    // Success, display alert
                    showAlertModal("登録", response.message, "#00CC99");
                    allSelectedFiles = [];
                    const selectedFilesContainer = document.getElementById('selectedFiles');
                    selectedFilesContainer.innerHTML = '';
                    const fileNameDiv = document.createElement('div');
                    fileNameDiv.textContent = '証憑を添付してください。';
                    selectedFilesContainer.appendChild(fileNameDiv);
                }
            },
            error: function (response) {
                loadingIndicator.style.display = "none";
                showAlertModal("エラー", response.responseText, "red");
            }
        });
    }
}
var modalVisible = false;
document.getElementById('openModalButton').addEventListener('click', function () {
    modalVisible = false;
    var form = document.getElementById("saveForm");
    var formData = new FormData(form);

    var orderNo = formData.get('Order_No');
    var workNo = formData.get('Work_No');
    var supplierCode = formData.get('Supplier_Cd');
    var supplierName = formData.get('Supplier_Name');
    var orderDate = formData.get('Order_Date_View');
    var orderAmount = formData.get('Order_Amount_View');
    var currentUt = formData.get('Currency_Ut');
    var reportTyp = formData.get('Report_Typ');
    var checkResult = true;

    if (orderNo == '' || orderNo == "") {
        checkResult = false;
        var orderNoDiv = document.getElementById("Order_No");
        orderNoDiv.style.borderColor = "red";

    }
    if (workNo == '' || workNo == "") {
        checkResult = false;
        var workNoDiv = document.getElementById("Work_No");
        workNoDiv.style.borderColor = "red";
    }
    if (supplierCode == '' || supplierCode == "") {
        checkResult = false;
        var supplierCodeDiv = document.getElementById("Supplier_Cd");
        supplierCodeDiv.style.borderColor = "red";
    }
    if (supplierName == '' || supplierName == "") {
        checkResult = false;
        var supplierNameDiv = document.getElementById("Supplier_Name");
        supplierNameDiv.style.borderColor = "red";
    }
    if (orderDate == '' || orderDate == "") {
        checkResult = false;
        var orderDateDiv = document.getElementById("Order_Date_View");
        orderDateDiv.style.borderColor = "red";
    }
    if (orderAmount == '' || orderAmount == "") {
        checkResult = false;
        var orderAmountDiv = document.getElementById("Order_Amount_View");
        orderAmountDiv.style.borderColor = "red";
    }
    if (currentUt == '' || currentUt == "") {
        checkResult = false;
        var currencyDiv = document.getElementById("Currency_Ut");
        currencyDiv.style.borderColor = "red";
    }

    var reportTypDiv = document.getElementById("Report_Typ");
    if (reportTyp == null || reportTyp == '' || reportTyp == "") {
        checkResult = false;        
        reportTypDiv.style.borderColor = "red";
    }

    var orderDateInput = document.getElementById("Order_Date_View");
    var checkDateTime = checkNumber(orderDateInput);
    if (!checkDateTime) {
        orderDateInput.style.borderColor = "red";
    } else {
        orderDateInput.style.borderColor = "#dee2e6";
    }

    var orderAmount = document.getElementById("Order_Amount_View");
    var checkOrderAmount = formatNumber(orderAmount);
    if (!checkOrderAmount) {
        orderAmount.style.borderColor = "red";
    } else {
        orderAmount.style.borderColor = "#dee2e6";
    }

    //check FileList
    if (!checkResult || !checkDateTime || !checkOrderAmount) {
        form.reportValidity();
        if (orderDateInput.value == "0" && reportTypDiv.value.includes("完成")) {
            showAlertModal("エラー", "完成していない為、登録できません。", "orange");
        }
        return;
    }
    if (allSelectedFiles.length === 0) {
        var selectedFilesDiv = document.getElementById("selectedFiles");
        selectedFilesDiv.style.color = "red";
        return;
    }

    if (form.checkValidity()) {
        document.getElementById('dataConfirm1').innerText = formData.get('Order_No');
        document.getElementById('dataConfirm2').innerText = formData.get('Work_No');
        document.getElementById('dataConfirm3').innerText = formData.get('Supplier_Cd');
        document.getElementById('dataConfirm4').innerText = formData.get('Supplier_Name');
        document.getElementById('dataConfirm5').innerText = formData.get('Order_Date_View');
        document.getElementById('dataConfirm6').innerText = formData.get('Order_Amount_View');
        document.getElementById('dataConfirm7').innerText = formData.get('Report_Typ');
        document.getElementById('dataConfirm8').innerText = formData.get('Report_Fmt');
        document.getElementById('dataConfirm9').innerText = formData.get('Detail_No');
        document.getElementById('dataConfirm10').innerText = formData.get('Currency_Ut');
        document.getElementById('dataConfirm11').innerText = formData.get('Memo');
        $('#confirmModal').modal('show');

    } else {
        form.reportValidity();
    }

});

$('#confirmModal').on('hide.bs.modal', function (e) {
    if (!modalVisible) {
        e.preventDefault();
    }
});

$('#closeConfirmModal').on('click', function () {
    modalVisible = true;
    $('#confirmModal').modal('hide');
});

$('#confirmSave').on('click', function () {
    // OAuthen call saveForm, login truoc khi call Function Save
    saveForm();
    // Khi JWT call truc tiep fucntion Save
    //performSaveAction(); 
    modalVisible = true;
    $('#confirmModal').modal('hide');
});

// 選択ファイル処理
document.getElementById('FileList').addEventListener('change', handleFileSelect);

var allSelectedFiles = [];  // Mảng để duy trì danh sách toàn bộ file đã chọn

function allowDrop(event) {
    event.preventDefault();
}

function drop(event) {
    event.preventDefault();

    var files = event.dataTransfer.files;

    if (files.length > 0) {
        // Lọc ra các file chưa tồn tại trong mảng
        var newFiles = [...files].filter(file => !fileExists(file) && isValidExtension(file));
        // Thêm các file mới vào mảng
        allSelectedFiles = allSelectedFiles.concat(newFiles);
        // Hiển thị danh sách
        displayFiles(allSelectedFiles);
    }
}

function fileExists(file) {
    // Kiểm tra xem file đã tồn tại trong mảng hay chưa
    return allSelectedFiles.some(existingFile => existingFile.name === file.name && existingFile.size === file.size);
}

function isValidExtension(file) {
    var allowedExtensions = ["pdf", "xlsx", "xls", "csv", "txt", "ppt", "rtf", "msg", "pptx"];
    var fileExtension = getFileExtension(file.name).toLowerCase();
    return allowedExtensions.includes(fileExtension);
}

function getFileExtension(filename) {
    return filename.slice((filename.lastIndexOf(".") - 1 >>> 0) + 2);
}

function displayFiles(files) {
    var selectedFilesDiv = document.getElementById("selectedFiles");
    // Kiểm tra xem có file nào hay không
    // Xóa toàn bộ nội dung hiện tại
    selectedFilesDiv.innerHTML = "";
    var hasFiles = files.length > 0;
    selectedFilesDiv.style.color = "black";

    for (var i = 0; i < files.length; i++) {
        let fileName = files[i].name;

        // Tạo một div chứa thông tin file
        var fileDiv = document.createElement("div");

        // Hiển thị tên file
        fileDiv.innerHTML = '・' + fileName + ' ';

        // Tạo nút "X" để xóa file
        var deleteButton = document.createElement("button");
        deleteButton.innerHTML = "x";
        deleteButton.style.border = "none";
        deleteButton.style.color = "red";
        deleteButton.style.backgroundColor = "none";
        deleteButton.onclick = function () {
            removeFile(fileName);
        };

        // Thêm nút "X" vào div
        fileDiv.appendChild(deleteButton);

        // Thêm div vào khu vực chứa file
        selectedFilesDiv.appendChild(fileDiv);
    }

    // Nếu không có file nào và chưa có dòng chữ, thêm dòng chữ vào
    if (!hasFiles && selectedFilesDiv.children.length === 0) {
        var fileNameDiv = document.createElement('div');
        fileNameDiv.textContent = '証憑を添付してください。';
        selectedFilesDiv.style.color = "red";
        selectedFilesDiv.appendChild(fileNameDiv);
    }
}

function handleFileSelect(event) {
    var files = event.target.files;
    if (files.length > 0) {
        // Lọc ra các file chưa tồn tại trong mảng
        var newFiles = [...files].filter(file => !fileExists(file));

        // Thêm các file mới vào mảng
        allSelectedFiles = allSelectedFiles.concat(newFiles);

        // Hiển thị danh sách
        displayFiles(allSelectedFiles);
        document.getElementById('FileList').value = '';
    }
}

function removeFile(fileName) {
    // Lọc ra tất cả các file trừ file cần xóa
    allSelectedFiles = allSelectedFiles.filter(file => file.name !== fileName);
    // Cập nhật hiển thị
    displayFiles(allSelectedFiles);
}

function changeReportType(reportType) {
    if (reportType.includes("発注") || reportType.includes("受注")) {
        $("#Order_Date_View").val($("#idOrder_DateHide").text());
        $("#Order_Amount_View").val($("#idOrder_AmountHide").text());
    } else {
        $("#Order_Date_View").val($("#idCompletion_Date").text());
        $("#Order_Amount_View").val($("#idCompletion_Amount").text());
    }
    let order_amount = document.getElementById("Order_Amount_View");
    formatNumber(order_amount);
}


