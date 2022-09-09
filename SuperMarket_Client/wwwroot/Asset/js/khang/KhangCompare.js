function column() {

    for (var i = 0; i < comparecolumn.length; i++) {
        //

        if (comparecolumn[i].getAttribute("data-field") === "ProductName") {
            comparecolumn[i].innerHTML = product.ProductName;
            // thay lai du lieu
        } else if (comparecolumn[i].getAttribute("data-field") === "BrandName") {
            comparecolumn[i].innerHTML = product.Brand_Category.Brand.BrandName;
        } else if (comparecolumn[i].getAttribute("data-field") === "Origin") {
            comparecolumn[i].innerHTML = product.Brand_Category.Brand.Origin;
        } else if (comparecolumn[i].getAttribute("data-field") === "Price") {
            comparecolumn[i].innerHTML = product.Price;
        } else if (comparecolumn[i].getAttribute("data-field") === "Weight") {
            comparecolumn[i].innerHTML = product.Weight;
        } else if (comparecolumn[i].getAttribute("data-field") === "Description") {
            comparecolumn[i].innerHTML = product.Description;
        } else if (comparecolumn[i].getAttribute("data-field") === "ProductId") {
            comparecolumn[i].innerHTML = product.ProductId;
        }
    }

  

};
function picker(picker) {
    index = picker.getAttribute("data-picker");

}
function chooseproductjs(productId, jsObject) {

    var productChoosed = productId.getAttribute("data-product");

    product = jsObject.find(p => p.ProductId == productChoosed)

    comparecolumn = $('[data-index=' + index + ']');

    //check same product!
    var checkProduct = $('[data-field= ProductId]');
    var flag = 0;
    for (var i = 0; i < checkProduct.length; i++) {
        if (checkProduct[i].innerHTML == productChoosed) { flag += 1; }
    }
    if (flag > 0) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'This product was chosen!',

        })
    }
    else {
        //console.log($('[data-field= ProductId]'));
        column();

        for (var i = 0; i < compareLink.length; i++) {

            if (compareLink[i].getAttribute("data-index") == index) {
                compareLink[i].style.display = "none";
                compareProduct[i].style.display = "block";

                var test = addCompareProduct();



                compareProduct[i].innerHTML = test;

            }
        }

        //console.log(compareProduct);
    }

}
function searchProduct(e) {
    var sproduct = [];

    $("#main").html(sproduct.join(""));
    if (e.value != "") {
        var search = jsObject.filter(p => p.ProductName.toLowerCase().includes(e.value.toLowerCase()));
    }
    //console.log(search);
    search.forEach(info => {
        let PUrl;
        let img = info.ImageProduct.find(i => i != null);
        if (img != null) { PUrl = img.Url; }
        else { PUrl = "/Images/anh.jpg"; }

        sproduct.push(
            // html code for product

            ` <a id="myLink" title="" href="" data-product="${info.ProductId}" onclick="chooseproduct(this);return false;" class="list-group-item">
                                                                                            <div class="card">
                                                                                                <div class="d-flex align-content-start flex-wrap">
                                                                                                    <div style="height:100px;width:100px;" class="thumb img-thumbnail"> <img src="${PUrl}" alt="" style="width:100%"></div>
                                                                                                   
                                                                                                   <div class="info-product" style="margin:10px;">
                                                                                                    <div>
                                                                                                        <h3 style="color:blue" >${info.ProductName}</h3>
                                                                                                    </div>

                                                                                                    <div>
                                                                                                        <p class="comment">About: ${info.Title} </p>
                                                                                                    </div>
                                                                                                   </div>
                                                                                                </div>
                                                                                            </div>
                                                                                            </a>`

            // end
        )
    });
    $("#main").html(sproduct.join(""));
    //console.log($("#main"))
}
     //close
function closeCompareProduct(closeElement) {
    var closeIndex = closeElement.getAttribute("data-index");

    comparecolumn = $('[data-index=' + closeIndex + ']');
    for (var i = 0; i < comparecolumn.length; i++) {
        if (comparecolumn[i].getAttribute("data-field") != null) { comparecolumn[i].innerHTML = ""; }

    }

    //console.log(comparecolumn);
    // console.log(compareLink);
    for (var i = 0; i < compareLink.length; i++) {

        if (compareLink[i].getAttribute("data-index") == closeIndex) {
            compareLink[i].style.display = "block";
            compareProduct[i].style.display = "none";
            compareProduct[i].innerHTML = "";

        }

    }

}