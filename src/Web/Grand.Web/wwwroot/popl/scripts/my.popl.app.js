vm.myPoplUploadFile = function (e) {
    var formData = new FormData();
    var imagefile = e;
    var url = imagefile.getAttribute('data-url');
    formData.append("image", qqfile.files[0]);
    axios.post(url, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    }).then(function (response) {
        if (response.data.success) {
            var message = response.data.message;
            var downloadGuid = response.data.downloadGuid;
            var downloadUrl = response.data.downloadUrl;
            var downloadBtn = document.querySelectorAll('.download-file');
            var messageContainer = document.getElementById('download-message');

            e.setAttribute('qq-button-id', downloadGuid);
            document.querySelector('.hidden-upload-input').value = downloadGuid;

            messageContainer.style.display = "block";
            messageContainer.classList.remove('alert-danger');
            messageContainer.classList.add('alert-info');
            messageContainer.innerText = message;

            //downloadBtn[0].style.display = "block";
            downloadBtn[0].children[0].setAttribute('href', downloadUrl);

            //aha.com ------------------------------------------
            downloadBtn[1].setAttribute('src', downloadUrl);
            //aha.com ------------------------------------------

        } else {
            var message = response.data.message;
            var messageContainer = document.getElementById('download-message');
            messageContainer.style.display = "block";
            messageContainer.classList.remove('alert-info');
            messageContainer.classList.add('alert-danger');
            messageContainer.innerText = message;
        }
    })
};

vm.myPoplProductImage = function (event) {

    var Imagesrc = event.target.parentElement.getAttribute('data-href');
    if (!Imagesrc) Imagesrc = event.target.getAttribute('data-href');

    function collectionHas(a, b) {
        for (var i = 0, len = a.length; i < len; i++) {
            if (a[i] == b) return true;
        }
        return false;
    }
    function findParentBySelector(elm, selector) {
        var all = document.querySelectorAll(selector);
        var cur = elm.parentNode;
        while (cur && !collectionHas(all, cur)) {
            cur = cur.parentNode;
        }
        return cur;
    }

    var yourElm = event.target
    var selector = ".product-box";
    var parent = findParentBySelector(yourElm, selector);
    var Image = parent.querySelectorAll(".main-product-img")[0];
    Image.setAttribute('src', Imagesrc);

};

vm.myPoplProductColorSelected = function (event) {
    var element = event.target;
    var tagName = element.tagName;
    if (tagName != 'A' && tagName != 'a') {
        element = event.target.parentElement;
    }

    var parent = element.parentElement;
    for (var i = 0; i < parent.children.length; i++) {
        var child = parent.children[i];
        child.classList.remove('selected');
    }

    element.classList.add('selected');
}