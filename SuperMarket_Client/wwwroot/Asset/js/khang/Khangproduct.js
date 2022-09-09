
    //filter price

    function Price(picker,modelData) {

        let chosenPrice = picker.getAttribute("data-price");
        //alert(chosenPrice);
        //get brand;
        let priceTicked = $('[data-price]');
        for (let i = 0; i < priceTicked.length; i++) { priceTicked[i].setAttribute("data-check", ""); priceTicked[i].setAttribute("class", ""); }
        //add ticked

        picker.setAttribute("data-check", "ticked");
        picker.setAttribute("class", "boder-chosen");
        let brandList = $('[data-brand]');

        let brandId;
        for (let i = 0; i < brandList.length; i++) {

            if (brandList[i].getAttribute('data-check') == "ticked") {
                brandId = brandList[i].getAttribute('data-brand');
            };


        }

        //console.log(brandId);

        let PData = [];
        SData = modelData;
        SData.forEach(item => PData.push(item.Product))

        switch (chosenPrice) {
            case "1":
                if (brandId != 0) { productData = Data.filter(p => p.Price >= 0 && p.Price < 20 && p.Brand_Category.BrandId == parseInt(brandId)); }
                else { productData = PData.filter(p => p.Price >= 0 && p.Price < 20); }
                break;
            case "2":
                if (brandId != 0) { productData = PData.filter(p => p.Price >= 20 && p.Price < 50 && p.Brand_Category.BrandId == parseInt(brandId)); }
                else { productData = PData.filter(p => p.Price >= 20 && p.Price < 50); }

                break;
            case "3":
                if (brandId != 0) { productData = PData.filter(p => p.Price >= 50 && p.Price < 100 && p.Brand_Category.BrandId == parseInt(brandId)); }
                else { productData = PData.filter(p => p.Price >= 50 && p.Price < 100) }

                break;
            case "4":
                if (brandId != 0) { productData = PData.filter(p => p.Price >= 100 && p.Brand_Category.BrandId == parseInt(brandId)); }
                else { productData = PData.filter(p => p.Price >= 100) }

                break;
            default:
                if (brandId != 0) { productData = PData.filter(p => p.Brand_Category.BrandId == parseInt(brandId)); }
                else { productData = PData; }

        }
        return productData;
/*        loaddata(productData);*/
    }


    //filter brand
function Brand(picker, modelData) {

        let chosenBrand = picker.getAttribute("data-brand");
        //reset ticked
        let tickedBrand = $('[data-brand]');
    for (let i = 0; i < tickedBrand.length; i++) { tickedBrand[i].setAttribute("data-check", ""); tickedBrand[i].setAttribute("class", ""); }

        picker.setAttribute("data-check", "ticked");
        picker.setAttribute("class", "boder-chosen");

        //get brand;
        let priceList = $('[data-price]');

        let price;
        for (let i = 0; i < priceList.length; i++) {

            if (priceList[i].getAttribute('data-check') == "ticked") {
                price = priceList[i].getAttribute('data-price');
            };


        }
        //console.log(price);

        let PData = [];
        SData = modelData;
        SData.forEach(item => PData.push(item.Product))
        switch (price) {
            case "1":
                if (chosenBrand != 0) { productData = PData.filter(p => p.Price >= 0 && p.Price < 20 && p.Brand_Category.BrandId == parseInt(chosenBrand)); }

                else { productData = PData.filter(p => p.Price >= 0 && p.Price < 20); }

                break;
            case "2":
                if (chosenBrand != 0) { productData = PData.filter(p => p.Price >= 20 && p.Price < 50 && p.Brand_Category.BrandId == parseInt(chosenBrand)); }

                else { productData = PData.filter(p => p.Price >= 20 && p.Price < 50); }

                break;
            case "3":
                if (chosenBrand != 0) { productData = PData.filter(p => p.Price >= 50 && p.Price < 100 && p.Brand_Category.BrandId == parseInt(chosenBrand)); }

                else { productData = PData.filter(p => p.Price >= 50 && p.Price < 100); }

                break;
            case "4":
                if (chosenBrand != 0) { productData = PData.filter(p => p.Price >= 100 && p.Brand_Category.BrandId == parseInt(chosenBrand)); }

                else { productData = PData.filter(p => p.Price >= 100); }

                break;

            default:
                if (chosenBrand != 0) { productData = PData.filter(p => p.Brand_Category.BrandId == parseInt(chosenBrand)); }

                else { productData = PData; }

    }
    return productData;
        //loaddata(productData);
    }
    //
    //load data js
    function loaddata(productData) {

        var product = productData;
        //console.log(product);

        var perPage = 9;
        var currentPage = 1;
        var start = 0;
        var end = perPage;
        var totalPage = Math.ceil(product.length / perPage);



        //nut nam duoi ham render
        const btnNext = document.querySelector('.next');
        const btnPrevious = document.querySelector('.previous');

        if (product == '') { renderDefault(); }
        else {
            renderProduct();
            renderListPages();
            changePage();
        }







        //render default
        function renderDefault() {
            html = `<div style="width:60rem;height:30rem" >

					<div id="carouselExampleCaptions" class="carousel slide" data-bs-ride="carousel">
		  <div class="carousel-indicators">
			<button type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1"></button>
			<button type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide-to="1" aria-label="Slide 2"></button>
			<button type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide-to="2" aria-label="Slide 3"></button>
		  </div>
		  <div class="carousel-inner">
			<div class="carousel-item active">
			  <img style="width:60rem; height:30rem;" src="/Images/Supermarket1.jpg" class="d-block w-100" alt="slide1">
			  <div class="carousel-caption d-none d-md-block">


			  </div>
			</div>
			<div class="carousel-item">
			  <img style="width:60rem; height:30rem;" src="/Images/Supermarket2.jpg" class="d-block w-100" alt="slide2">
			  <div class="carousel-caption d-none d-md-block">


			  </div>
			</div>
			<div class="carousel-item">
			  <img style="width:60rem; height:30rem;" src="/Images/Supermarket3.jpg" class="d-block w-100" alt="slide3">
			  <div class="carousel-caption d-none d-md-block">

			  </div>
			</div>
		  </div>
		  <button class="carousel-control-prev" type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide="prev">
			<span class="carousel-control-prev-icon" aria-hidden="true"></span>
			<span class="visually-hidden">Previous</span>
		  </button>
		  <button class="carousel-control-next" type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide="next">
			<span class="carousel-control-next-icon" aria-hidden="true"></span>
			<span class="visually-hidden">Next</span>
		  </button>
		</div>
				</div>
				<hr>
				<div><h4 style="text-align:center;">+++++++++++++++++No Product Found!+++++++++++++++++</h4></div>	`
                ;

            document.getElementById("productContainer").innerHTML = html;
            document.getElementById('default').style.display = "none";
        }



        function renderProduct() {



            html = ``;
            const content = product.map((item, index) => {
                if (index >= start && index < end) {
                    console.log(item);
                    let PUrl = "anh.jpg";
                    let img = item.ImageProduct.find(i => i != null);
                    let ratingTotal = 0;
                    let count = 0;
                    for (let j = 0; j < item.Feedback_Ratings.length; j++) { ratingTotal += item.Feedback_Ratings[j].RatingPoint; count++; }
                    let rating;
                    if (count == 0) { rating = 0; }
                    else { rating = Math.floor(ratingTotal / count) };
                    if (img != null) { PUrl = img.Url; }


                    html += `<div class="col-sm-6 col-lg-4">
																					<div class="shop_item">
																						<div class="thumb">
																							<img style="width:200px;height:150px" src="${PUrl}"></img>
																							<div class="thumb_info">
																								<ul class="mb0">

																									<li class="btn-color"><a href="/Customer/Product/CompareProduct?CategoryId=${item.Brand_Category.CategoryId}&productId=${item.ProductId}" ><i class="fa fa-balance-scale" aria-hidden="true"></i></a></li>
                                                                                                    <li class="btn-color"><a href="/Customer/Product/Details?id=${item.ProductId}" ><i class="fa fa-info-circle" aria-hidden="true"></i></a></li>
																								</ul>
																							</div>
																						</div>
																						<div class="details text-center">
																							<div class="title">${item.ProductName}</div>
																				<div class="review">
																								<ul class="small-ratings">`;
                    for (let i = 0; i < rating; i++) {
                        html += `	<i class="fa fa-star rating-color"></i>`
                    }
                    for (let i = 0; i < 5 - rating; i++) {
                        html += `<i class="fa fa-star"></i>`
                    }
                    html += `</ul>
																							</div>
																					<div class="sub_title">${item.Title}</div>
																							<div class="sub_title">${item.Brand_Category.Brand.BrandName}</div>
																							<div class="si_footer">
																								<div class="price">$ ${item.Price}</div>
																								<a class="cart_btn btn-thm addToCartHome" data-productid=${item.ProductId} ><span class="flaticon-shopping-cart mr10"></span>ADD TO CART</a>
																							</div>
																						</div>
																					</div>
																				</div>`




                    return html;
                }



            });
            document.getElementById("productContainer").innerHTML = html;
            //bat numlist
            document.getElementById('default').style.display = "block";
            callCart();
        }

        //renderListPages();
        //renderProduct();
        //changePage();

        ////nut nam duoi ham render
        //const btnNext = document.querySelector('.next');
        //const btnPrevious = document.querySelector('.previous');
        //btn Next
        btnNext.addEventListener('click', () => {

            const numberPages = document.querySelectorAll('#listPagesContainer li');

            if (currentPage < totalPage) {
                currentPage++;

                for (let j = 0; j < numberPages.length; j++) {
                    //if(numberPages[j])
                    numberPages[j].setAttribute("class", "page-item");
                }
                numberPages[currentPage - 1].setAttribute("class", "page-item active");

            }
            else { currentPage = totalPage; }

            getItem(currentPage);


            renderProduct();

        })
        //btnPev
        btnPrevious.addEventListener('click', () => {


            const numberPages = document.querySelectorAll('#listPagesContainer li');


            if (currentPage > 1) {
                currentPage--;
                //
                for (let j = 0; j < numberPages.length; j++) {
                    //if(numberPages[j])
                    numberPages[j].setAttribute("class", "page-item");
                }
                numberPages[currentPage - 1].setAttribute("class", "page-item active");
            }
            else { currentPage = 1; }
            getItem(currentPage);

            renderProduct();
        })

        //get item
        function getItem(currentPage) {
            start = (currentPage - 1) * perPage;
            end = currentPage * perPage;
            renderProduct();
        }

        //render number of page
        function renderListPages() {

            let listPages = '';
            listPages += `
										<li class="page-item active"><a class="page-link" >1</a></li>`;
            for (let i = 2; i <= totalPage; i++) {
                listPages += `<li class="page-item"><a class="page-link" >${i}</a></li>`;
            }




            document.getElementById('listPagesContainer').innerHTML = listPages;
        }
        //choose page
        function changePage() {
            const numberPages = document.querySelectorAll('#listPagesContainer li');

            for (let i = 0; i < numberPages.length; i++) {

                numberPages[i].addEventListener('click', () => {
                    for (let j = 0; j < numberPages.length; j++) { numberPages[j].setAttribute("class", "page-item"); }

                    currentPage = i + 1;
                    getItem(currentPage);
                    numberPages[i].setAttribute("class", "page-item active");
                    renderProduct();

                });

            }

        }

    }

    function callCart() {

        function reloadCart() {
            var container = $("#myComponentContainer");
            var refreshComponent = function () {
                $.ajax({
                    url: "/Customer/Home/CartListViewComponent",
                    type: "GET",
                    success: function (response) {
                        container.html(response);
                    }
                });
            };
            refreshComponent();
            //$(function () { window.setInterval(refreshComponent, 1000); });

        }
        //
        $(".addToCartHome").on("click", function (e) {
            var checkLoggedIn = $("#LoggedIn").val();
            if (checkLoggedIn != 1) {
                window.location.href = domain + "Identity/Account/Login";
                return false;
            }
            e.preventDefault();
            let id = $(this).data("productid");
            $.ajax({
                url: "/Customer/Product/Details?id=" + id,
                type: "GET",
                success: function (responseDetailGet) {
                    console.log(responseDetailGet);
                    $.ajax({
                        url: "/Customer/Product/Details",
                        type: "POST",
                        data: responseDetailGet,
                        success: function (response) {
                            if (response.statusCode == 200 || response.statusCode == 201) {
                                Swal.fire({
                                    icon: 'success',
                                    title: response.message,
                                    showConfirmButton: false,
                                    timer: 1500
                                })
                                reloadCart();
                                $("#Count").val(response.count);
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Oops...',
                                    text: response.message,
                                })
                            }

                        }
                    });
                }
            });
        })
    }

