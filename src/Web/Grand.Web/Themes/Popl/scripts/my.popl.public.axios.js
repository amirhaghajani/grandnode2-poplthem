
AxiosCart.myAddproducttocart_catalog = function (btn, urladd, showqty, productid, quickviewUrl, colorSectionContainerDataUniqueId) {

    var selectedColor = document.querySelector(`[data-uniqueid="${colorSectionContainerDataUniqueId}"] .selected`);

    var fd = new FormData();

    if (selectedColor) {
        valueId = selectedColor.getAttribute('data-valueid');
        attributeId = selectedColor.parentElement.getAttribute('data-attributeid');

        fd.append('product_attribute_' + attributeId, valueId);
    }

    if (showqty.toLowerCase() == 'true') {
        var qty = document.querySelector('#addtocart_' + productid + '_EnteredQuantity').value;
        if (urladd.indexOf("forceredirection") != -1) {
            urladd += '&quantity=' + qty;
        }
        else {
            urladd += '?quantity=' + qty;
        }
    }

    if (this.loadWaiting != false) {
        return;
    }

    this.setLoadWaiting(true);


    //befor send -----------------------------------
    btn.classList.add('disabled');
    btn.disabled = true;

    var spanAddToCard1 = btn.children[0];//انگلیسی
    spanAddToCard1.classList.remove('fadeInDown');
    spanAddToCard1.classList.add('zoomOut');

    spanAddToCard2 = btn.children[1];//فارسی
    spanAddToCard2.classList.remove('fadeInDown');
    spanAddToCard2.classList.add('zoomOut');

    var svg = btn.children[2];


    //-------------------------------------------------------


    axios({
        url: urladd,
        method: 'post',
        data: fd,
    }).then(function (response) {
        this.AxiosCart.mySuccess_process(response, quickviewUrl, svg);


    }).catch(function (error) {
        error.axiosFailure;
    }).then(function () {
        if (typeof vmwishlist !== 'undefined') {
            vmwishlist.getModel();
        }
        if (typeof vmorder !== 'undefined') {
            vmorder.getModel();
        }
        this.AxiosCart.resetLoadWaiting();

        setTimeout(function () {
            spanAddToCard1.classList.remove('zoomOut');
            spanAddToCard1.classList.add('fadeInDown');
            spanAddToCard2.classList.remove('zoomOut');
            spanAddToCard2.classList.add('fadeInDown');

            svg.classList.remove('checkmark-active');

            btn.classList.remove('disabled');
            btn.disabled = false;

        }, 1000);
    });
};

AxiosCart.mySuccess_process = function (response, quickViewUrl, svgTik) {
    if (response.data.updatetopwishlistsectionhtml) {
        if (document.querySelector('.wishlist-qty'))
            document.querySelector('.wishlist-qty').innerHTML = response.data.updatetopwishlistsectionhtml;
    }
    if (response.data.sidebarshoppingcartmodel) {
        var newfly = response.data.sidebarshoppingcartmodel;
        this.flycart = newfly;
        this.flycartitems = newfly.Items;
        this.flycartindicator = newfly.TotalProducts;
        vm.flycart = newfly;
        vm.flycartitems = newfly.Items;
        vm.flycartindicator = newfly.TotalProducts;
    }
    if (response.data.updatetopcartsectionhtml !== undefined) {
        vm.flycartindicator = response.data.updatetopcartsectionhtml;
    }
    if (response.data.product) {
        if (response.data.success == true) {

            vm.PopupQuickViewVueModal = response.data.model;

            Object.assign(vm.PopupQuickViewVueModal, { RelatedProducts: [] });

            vm.$refs['ModalQuickView'].show();

            if (response.data.model.ProductType == 20) {

                var fullDate = new Date(response.data.model.StartDate).toLocaleDateString('en-US');
                var year = new Date(response.data.model.StartDate).getFullYear();
                var month = new Date(response.data.model.StartDate).getUTCMonth() + 1;

                Object.assign(vm.PopupQuickViewVueModal, { ReservationFullDate: fullDate });
                Object.assign(vm.PopupQuickViewVueModal, { ReservationYear: year });
                Object.assign(vm.PopupQuickViewVueModal, { ReservationMonth: month });

            }

        }
    }
    if (response.data.message) {
        if (response.data.success == true) {
            //success
            vm.PopupAddToCartVueModal = response.data.model;
            vm.$refs['ModalQuickView'].hide();
            //vm.$refs['ModalAddToCart'].show();
            if (response.data.refreshreservation == true) {
                var param = "";
                if ($("#parameterDropdown").val() != null) {
                    param = $("#parameterDropdown").val();
                }
                Reservation.fillAvailableDates(Reservation.currentYear, Reservation.currentMonth, param, true);
            }

            svgTik.classList.add('checkmark-active');

        }
        else {
            //error
            vm.displayBarNotification(response.data.message, '', 'error', 3500);
        }
        return false;
    }
    if (response.data.redirect) {
        //location.href = response.data.redirect;
        AxiosCart.quickview_product(quickViewUrl)
        return true;
    }
    return false;
};


AxiosCart.success_process = AxiosCart.mySuccess_process;