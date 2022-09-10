var domain = "https://" + window.location.host + "/";
var rating_product = $('[data-rating = "ratingProduct"]');
var globalRating = 0;
var isApplied = false;

(function ($) {
  
    /* Formatting function for row details - modify as you need */
    function format(data) {
        // `d` is the original data object for the row
        var trs = '';

                for (var item of data.orderDetail) {
                    trs +=
                        `<tr>
                         <td>${item.product.title}</td>
                         <td>${item.count}</td> 
                         <td>${item.price}</td>
                         </tr>`;
                }
       
        return (
            '<table class="table table-border table-hover">' +
            '<thead>' +
            '<th>Product</th>' +
            '<th>Quantity</th>' +
            '<th>Price</th>' +
                '</thead>'+
               '<tbody>'
                 + trs +
                '</tbody>' +
                '</table>');
        
    }

    $(document).ready(function () {

        var selectCoupon = $("#selectCoupon");
        selectCoupon.on("change", function () {
            $("#couponField").val(selectCoupon.val());
        });
        $('#selectBranch_popup').modal({ backdrop: 'static', keyboard: false })  
        var checkSessionBranchId = $("#checkSessionBranchId").val();
        if (checkSessionBranchId == 1) {
            $('#selectBranch_popup').modal('show');
        }
        $("#showModal").on("click", function () {
            $('#selectBranch_popup').modal('show');
        })

        var table = $('#OrderDataTable').DataTable({
            ajax: '/Customer/Customer/GetAllOrder',
            columns: [
                {
                    className: 'dt-control',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                },
                { data: 'orderId' },
                { data: 'orderDate' },
                { data: 'orderStatus' },
                { data: 'paymentStatus' },
                { data: 'orderTotal', render: $.fn.dataTable.render.number('.', ',', 2, '$') },
                {
                    data: "orderId", render: function (dataField,type,row)
                    {
                        if (row.orderStatus == "Approved") {
                            return '<a style="color:red;" href="/Customer/Customer/CancelRequest?orderId=' + dataField + '"> Order Cancellation </a>';
                        } else if (row.orderStatus == "CancelRequest") {
                            return '<span href="#"> Waiting For Acceptance </span>';
                        } else {
                            return '<span href="#"> N/A </span>';

                        }
                    }
                },
            ],
            
            order: [[1, 'asc']],
        });



        // Add event listener for opening and closing details
        $('#OrderDataTable tbody').on('click', 'td.dt-control', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);

            if (row.child.isShown()) {
                // This row is already open - close it
                row.child.hide();
                tr.removeClass('shown');
            } else {
                // Open this row
                row.child(format(row.data())).show();
                tr.addClass('shown');
            }
        });
    });


    $('.ratingProduct').on("click", function () {
        let id = $(this).data("id");
        globalRating = id;
        for (var i = 0; i < rating_product.length; i++) {
            rating_product[i].checked = false;
        }
        $("#" + id).prop("checked", true);
    });
    $("#submitEditReviewBtn").on("click", function () {
     
        let newRatingPoint = $("#newRatingPoint").val();
        let ProductId = $("#ProductId").val();
        let newFeedback = $("#newFeedback").val();
        $.ajax({
            url: "/Customer/FeedbackAndRating/EditReview",
            type: "POST",
            data: { newRatingPoint: newRatingPoint, newFeedback: newFeedback, ProductId: ProductId },
            success: function (response) {
                window.location.href = "https://localhost:7166/Customer/Product/Details?id=" + ProductId;
            }
        });
    })

    $("#submitReview").on("click", function (e) {
        e.preventDefault();
        let isChecked = false;
        var content = $.trim($('#feedbackContent').val());
        var productId = $("#ProductId").val();
        for (var i = 0; i < rating_product.length; i++) {
            if (rating_product[i].checked == true) {
                isChecked = true;
            }
        }
        if (!isChecked) {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Please rate product before submit!',
            });
        } else if (content == "") {
            Swal.fire({
                icon: 'error',
                title: 'Oops...',
                text: 'Feedback cannot be blank!',
            });

        } else {
            let id = globalRating;
            $.ajax({
                url: "/Customer/FeedbackAndRating/SubmitReview",
                type: "POST",
                data: { content: content, ratingPoint: id, productId: productId },
                success: function (response) {
                    console.log(response)
                    if (response.statusCode == 401 || response.statusCode == 400) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: response.message,
                        });
                        return false;
                    } else {
                        let data = `
                                 <div class="mbp_first d-flex align-items-center">
                                <div class="flex-shrink-0">
                                  <img src="${response.content.customer.customerAvatar}" class="mr-3" alt="reviewer1.png">
                                </div>
                                <div class="flex-grow-1 ms-4">
                                  <h4 class="sub_title mt20">${response.content.customer.fullName}</h4>
                                <div class="sspd_postdate mb15">${response.content.postedDate}
                                    <div class="sspd_review pull-right">
                                      <div class="small-ratings">
                                  `
                        for (var i = 0; i < response.content.ratingPoint; i++) {
                            data += `<i class="fa fa-star rating-color"></i>`
                        }
                        for (var i = 0; i < 5 - response.content.ratingPoint; i++) {
                            data += `<i class="fa fa-star"></i>`
                        }
                        data +=
                            `
                                </div>
                                    </div>
                                  </div>
                                </div>
                              </div>
                              <p class="mt15 mb30">${response.content.content}</p>
                              <hr>
                            `;

                        $("#listUserFeedback").prepend(data);

                        $("#comments_formDetail").html(`
                                <h3>You have already reviewed this product ! Do you want to edit your review ? </h3>
                                        <div>
                                            <form>
                                                <input type="hidden" name="ProductId" id="ProductId" value="${productId}" />
                                                <div class="mb-3">
                                                <label for="newRatingPoint" class="form-label">Rating Point</label>
                                                <input name="newRatingPoint" required  class="form-control" type='text' id="newRatingPoint" value="${response.content.ratingPoint}" placeholder="Rating Point must be between 1-5">

                                                </div>
                                                <div class="mb-3">
                                                <label for="newFeedback" class="form-label">Your Feedback</label>
                                                <input type="text" name="newFeedback" required class="form-control" id="newFeedback" value="${content}">
                                                </div>
                                                <button type="button" id="submitEditReviewBtn" class="btn btn-primary">Submit</button>
                                            </form>
                                        </div>
                        `);

                        //$("#submitEditReviewBtn").on("click", function () {
                        //    let newRatingPoint = $("#newRatingPoint").val();
                        //    let ProductId = $("#ProductId").val();
                        //    let newFeedback = $("#newFeedback").val();
                        //    $.ajax({
                        //        url: "/Customer/FeedbackAndRating/EditReview",
                        //        type: "POST",
                        //        data: { newRatingPoint: newRatingPoint, newFeedback: newFeedback, ProductId: ProductId },
                        //        success: function (response) {
                        //            window.location.href = "https://localhost:7166/Customer/Product/Details?id=" + ProductId;
                        //        }
                        //    });
                        //})

                        let numOfReview = $("#numberOfReview").text();
                        $("#numberOfReview").text(parseInt(numOfReview) + 1);

                        for (var i = 0; i < rating_product.length; i++) {
                            $("#" + i).prop("checked", false);
                        }

                        Swal.fire({
                            icon: 'success',
                            title: 'Your feedback has has been sent',
                            showConfirmButton: false,
                            timer: 1500
                        })
                    }
                }
            });
            globalRating = 0;
        }
    });


    "use strict";

    /* ----- Preloader ----- */
    function preloaderLoad() {
        if ($('.preloader').length) {
            $('.preloader').delay(200).fadeOut(300);
        }
        $(".preloader_disabler").on('click', function () {
            $("#preloader").hide();
        });
    }

    /** Main Menu Custom Script Start **/
    $(document).on('ready', function () {
        $("#respMenu").aceResponsiveMenu({
            resizeWidth: '768', // Set the same in Media query
            animationSpeed: 'fast', //slow, medium, fast
            accoridonExpAll: false //Expands all the accordion menu on click
        });

        
    });

   

   

    function mobileNavToggle() {
        if ($('#main-nav-bar .navbar-nav .sub-menu').length) {
            var subMenu = $('#main-nav-bar .navbar-nav .sub-menu');
            subMenu.parent('li').children('a').append(function () {
                return '<button class="sub-nav-toggler"> <span class="sr-only">Toggle navigation</span> <span class="icon-bar"></span> <span class="icon-bar"></span> <span class="icon-bar"></span> </button>';
            });
            var subNavToggler = $('#main-nav-bar .navbar-nav .sub-nav-toggler');
            subNavToggler.on('click', function () {
                var Self = $(this);
                Self.parent().parent().children('.sub-menu').slideToggle();
                return false;
            });

        };
    }

    /* ----- Tags Bar Code for job list 1 page ----- */
    $('.tags-bar > span i').on('click', function () {
        $(this).parent().fadeOut();
    });

    /* ----- This code for menu ----- */
    $(window).on('scroll', function () {
        if ($('.scroll-to-top').length) {
            var strickyScrollPos = 100;
            if ($(window).scrollTop() > strickyScrollPos) {
                $('.scroll-to-top').fadeIn(500);
            } else if ($(this).scrollTop() <= strickyScrollPos) {
                $('.scroll-to-top').fadeOut(500);
            }
        };
        if ($('.stricky').length) {
            var headerScrollPos = $('.header-navigation').next().offset().top;
            var stricky = $('.stricky');
            if ($(window).scrollTop() > headerScrollPos) {
                stricky.removeClass('slideIn animated');
                stricky.addClass('stricky-fixed slideInDown animated');
            } else if ($(this).scrollTop() <= headerScrollPos) {
                stricky.removeClass('stricky-fixed slideInDown animated');
                stricky.addClass('slideIn animated');
            }
        };
    });

    $(".mouse_scroll").on('click', function () {
        $('html, body').animate({
            scrollTop: $("#feature-property, #property-city").offset().top
        }, 1000);
    });
    /** Main Menu Custom Script End **/


    function makeTimer() {
        //  var endTime = new Date("20 Dec 2021 9:56:00 GMT+01:00");  
        var endTime = new Date("20 Dec 2021 9:56:00 GMT+01:00");
        endTime = (Date.parse(endTime) / 1000);
        var now = new Date();
        now = (Date.parse(now) / 1000);
        var timeLeft = endTime - now;
        var days = Math.floor(timeLeft / 86400);
        var hours = Math.floor((timeLeft - (days * 86400)) / 3600);
        var minutes = Math.floor((timeLeft - (days * 86400) - (hours * 3600)) / 60);
        var seconds = Math.floor((timeLeft - (days * 86400) - (hours * 3600) - (minutes * 60)));
        if (hours < "10") { hours = "0" + hours; }
        if (minutes < "10") { minutes = "0" + minutes; }
        if (seconds < "10") { seconds = "0" + seconds; }
        $(".days").html(days + "<span>Days</span>");
        $(".hours").html(hours + "<span>Hours</span>");
        $(".minutes").html(minutes + "<span>Minutes</span>");
        $(".seconds").html(seconds + "<span>Seconds</span>");
    }
    setInterval(function () { makeTimer(); }, 1000);

    /* ----- Blog innerpage sidebar according ----- */
    $(document).on('ready', function () {
        $('.collapse').on('show.bs.collapse', function () {
            $(this).siblings('.card-header').addClass('active');
        });
        $('.collapse').on('hide.bs.collapse', function () {
            $(this).siblings('.card-header').removeClass('active');
        });

    });
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })

    /* ----- Menu Cart Button Dropdown ----- */
    $(document).on('ready', function () {
        // Loop through each nav item
        $('.cart_btn').children('ul.cart').children('li').each(function (indexCount) {
            // loop through each dropdown, count children and apply a animation delay based on their index value
            $(this).children('ul.dropdown_content').children('li').each(function (index) {
                // Turn the index value into a reasonable animation delay
                var delay = 0.1 + index * 0.03;
                // Apply the animation delay
                $(this).css("animation-delay", delay + "s")
            });
        });
    });

    /* Menu Search Popup */
    // jQuery(document).on('ready', function($) {
    //     var wHeight = window.innerHeight;
    //     /* search bar middle alignment */
    //     $('#mk-fullscreen-searchform').css('top', wHeight / 2);
    //     /* reform search bar */
    //     jQuery(window).resize(function() {
    //         wHeight = window.innerHeight;
    //         $('#mk-fullscreen-searchform').css('top', wHeight / 2);
    //     });

    //     /* Search */
    //     $('#search-button, #search-button2').on('click', function() {
    //         console.log("Open Search, Search Centered");
    //         $("div.mk-fullscreen-search-overlay").addClass("mk-fullscreen-search-overlay-show");
    //     });
    //     $("button.mk-fullscreen-close").on('click', function() {
    //         console.log("Closed Search");
    //         $("div.mk-fullscreen-search-overlay").removeClass("mk-fullscreen-search-overlay-show");
    //     });
    // });


    //$('.circlechart').circlechart(); // Initialization

    /* ----- Mobile Nav ----- */
    $(function () {
        $('nav#menu').mmenu();
    });

    $(function () {
        $("#switcher-id").change(function () {
            if ($(this).is(":checked")) {
                $(".second-switch-content").show();
                $(".first-switch-content").hide();
            } else {
                $(".second-switch-content").hide();
                $(".first-switch-content").show();
            }
        });
    });

    /* ----- Candidate SIngle Page Price Slider ----- */
    

    /* ----- Employee List v1 page range slider widget ----- */
    $(document).on('ready', function () {
        $(".slider-range").slider({
            range: true,
            min: 11200,
            max: 30000,
            values: [11200, 15200],
            slide: function (event, ui) {
                $(".amount").val(ui.values[0]);
                $(".amount2").val(ui.values[1]);
            }
        });
        $(".amount").change(function () {
            $(".slider-range").slider('values', 0, $(this).val());
        });
        $(".amount2").change(function () {
            $(".slider-range").slider('values', 1, $(this).val());
        });
    });

    /* ----- Pricing Range Slider ----- */
    $(document).on("ready", function () {
        $(".range-example-km").asRange({
            limit: false,
            min: 0,
            max: 150,
            range: false,
            step: 1,
            value: 50,
            format: function (value) {
                return value + ' km';
            }
        });
        $(".range-uilayouts").asRange({
            limit: false,
            max: 1000,
            min: 0,
            range: true,
            step: 1,
            format: function (value) {
                return '$' + value;
            }
        });
    });

    /* ----- Progress Bar ----- */
    if ($('.progress-levels .progress-box .bar-fill').length) {
        $(".progress-box .bar-fill").each(function () {
            var progressWidth = $(this).attr('data-percent');
            $(this).css('width', progressWidth + '%');
            $(this).children('.percent').html(progressWidth + '%');
        });
    }

    // Display the progress bar calling progressbar.js
    //$('.progressbar1').progressBar({
    //    shadow: false,
    //    percentage: false,
    //    animation: true,
    //    barColor: "#ff5a5f",
    //});
    //$('.progressbar2').progressBar({
    //    shadow: false,
    //    percentage: false,
    //    animation: true,
    //    barColor: "#ff5a5f",
    //});
    //$('.progressbar3').progressBar({
    //    shadow: false,
    //    percentage: false,
    //    animation: true,
    //    animateTarget: true,
    //    barColor: "#ff5a5f",
    //});
    //$('.progressbar4').progressBar({
    //    shadow: false,
    //    percentage: false,
    //    animation: true,
    //    animateTarget: true,
    //    barColor: "#ff5a5f",
    //});
    //$('.progressbar5').progressBar({
    //    shadow: false,
    //    percentage: false,
    //    animation: true,
    //    animateTarget: true,
    //    barColor: "#ff5a5f",
    //});

    /* ----- Background Parallax ----- */
    var isMobile = {
        Android: function () {
            return navigator.userAgent.match(/Android/i);
        },
        BlackBerry: function () {
            return navigator.userAgent.match(/BlackBerry/i);
        },
        iOS: function () {
            return navigator.userAgent.match(/iPhone|iPad|iPod/i);
        },
        Opera: function () {
            return navigator.userAgent.match(/Opera Mini/i);
        },
        Windows: function () {
            return navigator.userAgent.match(/IEMobile/i);
        },
        any: function () {
            return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
        }
    };

    

    /* ----- MagnificPopup ----- */
    if (($(".popup-img").length > 0) || ($(".popup-iframe").length > 0) || ($(".popup-img-single").length > 0)) {
        $(".popup-img").magnificPopup({
            type: "image",
            gallery: {
                enabled: true,
            }
        });
        $(".popup-img-single").magnificPopup({
            type: "image",
            gallery: {
                enabled: false,
            }
        });
        $('.popup-iframe').magnificPopup({
            disableOn: 700,
            type: 'iframe',
            preloader: false,
            fixedContentPos: false
        });
        $('.popup-youtube, .popup-vimeo, .popup-gmaps').magnificPopup({
            disableOn: 700,
            type: 'iframe',
            mainClass: 'mfp-fade',
            removalDelay: 160,
            preloader: false,
            fixedContentPos: false
        });
    };

    $('#myTab a,#myTab2 a').on('click', function (e) {
        e.preventDefault()
        $(this).tab('show')
    })

    /* ----- Wow animation ----- */
    function wowAnimation() {
        var wow = new WOW({
            animateClass: 'animated',
            mobile: true, // trigger animations on mobile devices (default is true)
            offset: 0
        });
        wow.init();

        new WOW().init();
        $('#show_advbtn, #show_advbtn2').on('click', function () {
            new WOW().init();
        })
    }

    /* ----- Date & time Picker ----- */
    if ($('.datepicker').length) {
        $('.datepicker').datetimepicker();
    }


    // Custom Search Dropdown Script Start
    var showSuggestions = function () {
        $(".top-search form.form-search .box-search").each(function () {
            $("form.form-search .box-search input").on('focus', (function () {
                $(this).closest('.boxed').children('.overlay').css({
                    opacity: '1',
                    display: 'block'
                });
                $(this).parent('.box-search').children('.search-suggestions').css({
                    opacity: '1',
                    visibility: 'visible',
                    top: '77px'
                });
            }));
            $("form.form-search .box-search input").on('blur', (function () {
                $(this).closest('.boxed').children('.overlay').css({
                    opacity: '0',
                    display: 'none'
                });
                $(this).parent('.box-search').children('.search-suggestions').css({
                    opacity: '0',
                    visibility: 'hidden',
                    top: '100px'
                });
            }));
        });

        $(".top-search.style1 form.form-search .box-search").each(function () {
            $("form.form-search .box-search input").on('focus', (function () {
                $(this).closest('.boxed').children('.overlay').css({
                    opacity: '1',
                    display: 'block'
                });
                $(this).parent('.box-search').children('.search-suggestions').css({
                    opacity: '1',
                    visibility: 'visible',
                    top: '50px'
                });
            }));
        });
    }; // Toggle Location
    $(function () {
        showSuggestions();
    });
    // Custom Search Dropdown Script Start
    // Custom DropDown Js Code Start
    $('.custom_select_dd').each(function () {
        // Cache the number of options
        var $this = $(this),
            numberOfOptions = $(this).children('option').length;

        // Hides the select element
        $this.addClass('s-hidden');

        // Wrap the select element in a div
        $this.wrap('<div class="select"></div>');

        // Insert a styled div to sit over the top of the hidden select element
        $this.after('<div class="styledSelect"></div>');

        // Cache the styled div
        var $styledSelect = $this.next('div.styledSelect');

        // Show the first select option in the styled div
        $styledSelect.text($this.children('option').eq(0).text());

        // Insert an unordered list after the styled div and also cache the list
        var $list = $('<ul />', {
            'class': 'options'
        }).insertAfter($styledSelect);

        // Insert a list item into the unordered list for each select option
        for (var i = 0; i < numberOfOptions; i++) {
            $('<li />', {
                text: $this.children('option').eq(i).text(),
                rel: $this.children('option').eq(i).val()
            }).appendTo($list);
        }

        // Cache the list items
        var $listItems = $list.children('li');

        // Show the unordered list when the styled div is clicked (also hides it if the div is clicked again)
        $styledSelect.on('click', function (e) {
            e.stopPropagation();
            $('div.styledSelect.active').each(function () {
                $(this).removeClass('active').next('ul.options').hide();
            });
            $(this).toggleClass('active').next('ul.options').toggle();
        });

        // Hides the unordered list when a list item is clicked and updates the styled div to show the selected list item
        // Updates the select element to have the value of the equivalent option
        $listItems.on('click', function (e) {
            e.stopPropagation();
            $styledSelect.text($(this).text()).removeClass('active');
            $this.val($(this).attr('rel'));
            $list.hide();
            /* alert($this.val()); Uncomment this for demonstration! */
        });

        // Hides the unordered list when clicking outside of it
        $(document).on('click', function () {
            $styledSelect.removeClass('active');
            $list.hide();
        });
    });
    // Custom DropDown Js Code End

    function showDivWhenEmptyCart() {
        if ($('.table_body tr').length == 0) {
            var markup_empty_cart = `
                    <tr>
                        <td colspan="4">
                            <img width="100" src="/Asset/images/cart/empty_cart.png"/> <br />
                                Your Cart Is Empty
                        </td>
                    </tr>
                `;
            $('.table_body').append(markup_empty_cart);
        }
    }

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


    //Index Cart Page
    $(function () {

        showDivWhenEmptyCart();

        $(".PlusItem_Cart").on("click", function () {
            let _cartId = $(this).data("cartid");
            let cartCount = $(this).closest("div.quantity-block").find("input[name='cartCount']");
            let subTotalItem = $("#subTotalItem_" + _cartId);
            $.ajax({
                url: "/Customer/Cart/Plus",
                type: "POST",
                data: { cartId: _cartId, itemCount: cartCount.val() },
                success: function (response) {
                    if (response.statusCode == 200) {
                        cartCount.val(response.count);
                        subTotalItem.html("$" + response.subTotalItem + ".00");
                        $("#subTotalOrder").html("$" + response.subTotalOrder + ".00");
                        reloadCart();
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: response.message,
                        })
                        cartCount.val(response.count);
                        subTotalItem.html("$" + response.subTotalItem + ".00");
                        $("#subTotalOrder").html("$" + response.subTotalOrder + ".00");
                    }
                }
            });
        });

        $("#ProceedCheckout").on("click", function (e) {
            $.ajax({
                url: "/Customer/Cart/CheckCartBeforeCheckout",
                type: "GET",
                success: function (response) {
                    if (response.statusCode == 200 && response.count != 0) {
                        window.location.href = domain + "Customer/Cart/Checkout";
                        return true;
                    } else {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Oops...',
                            text: 'Your cart is empty!',
                        });
                    }
                }
            });
        });

        $(".RemoveItem_Cart").on("click", function () {
            let _cartId = $(this).data("cartid");
            let tr = $(this).closest("tr");
            //var subTotalItem = $("#subTotalItem_" + _cartId);
            $.ajax({
                url: "/Customer/Cart/RemoveItem",
                type: "POST",
                data: { cartId: _cartId },
                success: function (response) {
                    if (response.statusCode == 200 && response.actionClient == "removed") {
                        tr.remove();
                        reloadCart();
                        if (response.subTotalOrder != undefined) {
                            $("#subTotalOrder").html("$" + response.subTotalOrder + ".00");
                        } else {
                            $("#subTotalOrder").html("$0.00");
                        }
                        showDivWhenEmptyCart();

                    }
                }
            });
        });

        $(".RemoveItem_CartList").on("click", function () {
            let _cartId = $(this).data("cartid");

            let tr = $(".trForRemove_" + _cartId);
            $.ajax({
                url: "/Customer/Cart/RemoveItem",
                type: "POST",
                data: { cartId: _cartId },
                success: function (response) {
                    if (response.statusCode == 200 && response.actionClient == "removed") {
                        tr.remove();
                        reloadCart();
                        if (response.subTotalOrder != undefined) {
                            $("#subTotalOrder").html("$" + response.subTotalOrder + ".00");
                        } else {
                            $("#subTotalOrder").html("$0.00");
                        }
                        showDivWhenEmptyCart();
                    }
                }
            });
        });

        $(".MinusItem_Cart").on("click", function () {
            var _cartId = $(this).data("cartid");
            var cartCount = $(this).closest("div.quantity-block").find("input[name='cartCount']");
            var subTotalItem = $("#subTotalItem_" + _cartId);
            var tr = $(this).closest("tr");
            $.ajax({
                url: "/Customer/Cart/Minus",
                type: "POST",
                data: { cartId: _cartId },
                success: function (response) {
                    if (response.statusCode == 200 && response.actionClient == "ask") {
                        Swal.fire({
                            title: 'Are you sure you want to remove this product?',
                            text: "You won't be able to revert this!",
                            icon: 'warning',
                            showCancelButton: true,
                            confirmButtonColor: '#3085d6',
                            cancelButtonColor: '#d33',
                            confirmButtonText: 'Yes, delete it!'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                $.ajax({
                                    url: "/Customer/Cart/Minus",
                                    type: "POST",
                                    data: { cartId: _cartId, actionClient: "confirmed" },
                                    success: function (response) {
                                        if (response.statusCode == 200 && response.actionClient == "removed") {
                                            tr.remove();
                                            if (response.subTotalOrder != undefined) {
                                                $("#subTotalOrder").html("$" + response.subTotalOrder + ".00");
                                            } else {
                                                $("#subTotalOrder").html("$0.00");
                                            }
                                            showDivWhenEmptyCart();
                                            reloadCart();
                                            Swal.fire(
                                                'Deleted!',
                                                'Your item has been removed.',
                                                'success'
                                            )
                                        }
                                    }
                                });
                            }
                        })
                    } else if (response.statusCode == 200) {
                        cartCount.val(response.count);
                        subTotalItem.html("$" + response.subTotalItem + ".00");
                        if (response.subTotalOrder != undefined) {
                            $("#subTotalOrder").html("$" + response.subTotalOrder + ".00");
                        } else {
                            $("#subTotalOrder").html("$0.00");
                        }
                        showDivWhenEmptyCart();
                        reloadCart();

                    }
                }

            });
        });
        //Add To Cart Home Page
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
                type:"GET",
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

        //Add To Cart
        $("#formAddCart").on('submit', function (e) {
            let userLogged = $("#manage").val();
            if (userLogged != undefined) {
                e.preventDefault();
                var shoppingCart = {
                    CartId: null,
                    CustomerId: null,
                    Count: $('#Count').val(),
                    ProductId: $('#ProductId').val(),
                    branchId: $('#branchId').val(),
                };
                $.ajax({
                    url: "/Customer/Product/Details",
                    type: "POST",
                    data: shoppingCart,
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

        })

    });

    //Check out Page
    $(function () {
        $("#applyCouponBtn").on("click", function (e) {
            if (isApplied == true) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Oops...',
                    text: 'Coupon Code Was Applied!',
                });
                return false;
            }
            e.preventDefault();

            let couponCode = $("#couponField").val();
            $.ajax({
                url: "/Customer/Coupon/AddCoupon",
                type: "POST",
                data: { couponCode: couponCode },
                success: function (response) {
                    if (response.cpCode == "Expired") {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Oops...',
                            text: 'This Coupon Code Has Expired',
                        });
                    } else if (response.cpCode == "NotExists") {
                        Swal.fire({
                            icon: 'warning',
                            title: 'Oops...',
                            text: 'Coupon Code Not Exists!',
                        });
                    } else {
                        $("#discountValue").text("$" + response.discountAmount)
                        $("#orderTotalValue").text("$" + response.orderTotalAfterCoupon)
                        $("#cpId").val(response.couponId);
                        Swal.fire({
                            icon: 'success',
                            title: response.message,
                            showConfirmButton: false,
                            timer: 1500
                        })
                        isApplied = true;
                    }
                }

            });
        });
    });



    // Custom Shop item add Option increase decrease home 3
    $(function () {
        (function quantityProducts() {
            var $quantityArrowMinus = $(".quantity-arrow-minus");
            var $quantityArrowPlus = $(".quantity-arrow-plus");
            var $quantityNum = $(".quantity-num");
            $quantityArrowMinus.click(quantityMinus);
            $quantityArrowPlus.click(quantityPlus);

            function quantityMinus() {
                
                let stockCount = parseInt($("#stockCount").text())
                if ($quantityNum.val() > 1) {
                    $quantityNum.val(+$quantityNum.val() - 1);
                    $("#messageLimitedQuantity").text("");
                }
                if ($quantityNum.val() == stockCount) {
                    $("#messageLimitedQuantity").text("The product you have selected has reached a limited quantity");
                } else {
                    $("#messageLimitedQuantity").text("");
                }
            }
            function quantityPlus() {
                let stockCount = parseInt($("#stockCount").text());
                let productId = $("#ProductId").val();
                let countNumber = parseInt($("#Count").val());
                $.ajax({
                    url: "/Customer/Cart/Plus",
                    type: "POST",
                    data: { productId: productId, itemCount: countNumber },
                    success: function (response) {
                        console.log(response)
                        if (response.statusCode == 400 || $quantityNum.val() == stockCount) {
                            $("#messageLimitedQuantity").text("The product you have selected has reached a limited quantity");
                        } else {
                            $quantityNum.val(+$quantityNum.val() + 1);
                            $("#messageLimitedQuantity").text("");
                        }
                    }
                });
               
            }
        })();

        $("#Count").keyup(function () {
            let countNumber = parseInt($("#Count").val());
            let stockCount = parseInt($("#stockCount").text());

            if (countNumber > stockCount) {
                $("#Count").val(stockCount);
            } else {
                $("#messageLimitedQuantity").text("");
            }

        });

        if (parseInt($("#stockCount").text()) == 0) {
            $(".disabledBtn").prop('disabled', true);
        }
    });


    /* ----- Home Maximage Slider ----- */
    if ($('#maximage').length) {
        $('#maximage').maximage({
            cycleOptions: {
                fx: 'fade',
                speed: 500,
                timeout: 20000,
                prev: '#arrow_left',
                next: '#arrow_right'
            },
            onFirstImageLoaded: function () {
                jQuery('#cycle-loader').hide();
                jQuery('#maximage').fadeIn('fast');
            }
        });
        // Helper function to Fill and Center the HTML5 Video
        jQuery('#html5video').maximage('maxcover');

        // To show it is dynamic html text
        jQuery('.in-slide-content').delay(2000).fadeIn();
    }

    /* ----- Slick Slider For Testimonial ----- */
    if ($('.tes-for').length) {
        $('.tes-for').slick({
            slidesToShow: 1,
            slidesToScroll: 1,
            arrows: false,
            fade: false,
            autoplay: true,
            autoplaySpeed: 2000,
            asNavFor: '.tes-nav'
        });
        $('.tes-nav').slick({
            slidesToShow: 3,
            slidesToScroll: 1,
            asNavFor: '.tes-for',
            dots: false,
            arrows: false,
            centerPadding: '1px',
            centerMode: true,
            focusOnSelect: true
        });
    }

    /*  Popular-Listing-Slider  */
    if ($('.popular_listing_slider1').length) {
        $('.popular_listing_slider1').owlCarousel({
            loop: true,
            margin: 0,
            dots: true,
            nav: false,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="fa fa-arrow-left"></i>',
                '<i class="fa fa-arrow-right"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                767: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 2
                },
                1200: {
                    items: 3
                },
                1280: {
                    items: 4
                }
            }
        })
    }

    /*  Popular-Listing-Slider  */
    if ($('.feature_product_slider').length) {
        $('.feature_product_slider').owlCarousel({
            loop: true,
            margin: 10,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1
                },
                767: {
                    items: 2
                },
                768: {
                    items: 3
                },
                992: {
                    items: 3
                },
                1200: {
                    items: 4
                },
                1280: {
                    items: 5
                }
            }
        })
    }

    /*  Team-Slider-Owl-carousel  */
    if ($('.testimonial_slider_home1').length) {
        $('.testimonial_slider_home1').owlCarousel({
            loop: true,
            margin: 0,
            dots: true,
            nav: false,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 2,
                    center: false
                },
                600: {
                    items: 2,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 3
                },
                1200: {
                    items: 4
                }
            }
        })
    }

    /*  Team-Slider-Owl-carousel  */
    if ($('.instagram_slider').length) {
        $('.instagram_slider').owlCarousel({
            loop: true,
            margin: 30,
            dots: false,
            nav: false,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                320: {
                    items: 1,
                    center: false
                },
                375: {
                    items: 2,
                    center: false
                },
                520: {
                    items: 2,
                    center: false
                },
                600: {
                    items: 2,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 3
                },
                1200: {
                    items: 4
                },
                1366: {
                    items: 5
                },
                1400: {
                    items: 5
                }
            }
        })
    }

    /*  Team-Slider-Owl-carousel  */
    if ($('.bsp_grid3_slider').length) {
        $('.bsp_grid3_slider').owlCarousel({
            loop: true,
            margin: 15,
            dots: true,
            nav: false,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                600: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 2
                },
                1200: {
                    items: 3
                }
            }
        })
    }

    /*  Shop-Item-3-Grid-Slider-Owl-carousel  */
    if ($('.shop_item_1grid_slider').length) {
        $('.shop_item_1grid_slider').owlCarousel({
            loop: true,
            margin: 30,
            center: false,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 1
                },
                992: {
                    items: 1
                },
                1200: {
                    items: 1
                }
            }
        })
    }

    /*  Shop-Item-2-Grid-Slider-Owl-carousel  */
    if ($('.shop_item_2grid_slider').length) {
        $('.shop_item_2grid_slider').owlCarousel({
            loop: true,
            margin: 30,
            center: false,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                600: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 2
                },
                1200: {
                    items: 2
                }
            }
        })
    }

    /*  Shop-Item-3-Grid-Slider-Owl-carousel  */
    if ($('.shop_item_3grid_slider').length) {
        $('.shop_item_3grid_slider').owlCarousel({
            loop: true,
            margin: 30,
            center: true,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                600: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 2
                },
                1200: {
                    items: 3
                }
            }
        })
    }

    /*  Shop-Item-4-Grid-Slider-Owl-carousel  */
    if ($('.shop_item_4grid_slider').length) {
        $('.shop_item_4grid_slider').owlCarousel({
            loop: true,
            margin: 30,
            center: false,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                600: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 2
                },
                1024: {
                    items: 3
                },
                1200: {
                    items: 4
                }
            }
        })
    }

    /*  Shop-Item-5-Grid-Slider-Owl-carousel  */
    if ($('.shop_item_5grid_slider').length) {
        $('.shop_item_5grid_slider').owlCarousel({
            loop: true,
            margin: 30,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                767: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 3
                },
                1200: {
                    items: 3
                },
                1366: {
                    items: 4
                },
                1400: {
                    items: 5
                }
            }
        })
    }

    /*  Shop-Item-6-Grid-Slider-Owl-carousel  */
    if ($('.shop_item_6grid_slider').length) {
        $('.shop_item_6grid_slider').owlCarousel({
            loop: true,
            margin: 30,
            center: false,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                630: {
                    items: 2,
                    center: false
                },
                768: {
                    items: 3
                },
                992: {
                    items: 4
                },
                1024: {
                    items: 5
                },
                1200: {
                    items: 6
                }
            }
        })
    }

    /*  Shop-Item-7-Grid-Slider-Owl-carousel  */
    if ($('.shop_item_7grid_slider').length) {
        $('.shop_item_7grid_slider').owlCarousel({
            loop: true,
            margin: 30,
            center: false,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                767: {
                    items: 2,
                    center: false
                },
                768: {
                    items: 3
                },
                992: {
                    items: 4
                },
                1024: {
                    items: 4
                },
                1200: {
                    items: 5
                },
                1400: {
                    items: 5
                },
                1500: {
                    items: 7
                }
            }
        })
    }

    /*  Shop-Item-7-Grid-Slider-Owl-carousel  */
    if ($('.shop_item_8grid_slider').length) {
        $('.shop_item_8grid_slider').owlCarousel({
            loop: true,
            margin: 30,
            center: false,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                767: {
                    items: 2,
                    center: false
                },
                768: {
                    items: 3
                },
                992: {
                    items: 4
                },
                1024: {
                    items: 4
                },
                1200: {
                    items: 5
                },
                1400: {
                    items: 7
                },
                1500: {
                    items: 8
                }
            }
        })
    }

    /*  Recent-Property-slider-home1-Slider-Owl-carousel  */
    if ($('.recent_property_slider_home1').length) {
        $('.recent_property_slider_home1').owlCarousel({
            loop: true,
            margin: 30,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                767: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 2
                },
                1200: {
                    items: 3
                },
                1366: {
                    items: 4
                },
                1400: {
                    items: 4
                }
            }
        })
    }

    /*  Recent-Property-slider-home1-Slider-Owl-carousel  */
    if ($('.recent_property_slider_home5').length) {
        $('.recent_property_slider_home5').owlCarousel({
            loop: true,
            margin: 30,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                767: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 2
                },
                992: {
                    items: 2
                },
                1200: {
                    items: 3
                },
                1366: {
                    items: 4
                }
            }
        })
    }

    /*  Recent-Property-slider-home1-Slider-Owl-carousel  */
    if ($('.shop_slider_col6').length) {
        $('.shop_slider_col6').owlCarousel({
            loop: true,
            margin: 30,
            dots: true,
            nav: false,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                767: {
                    items: 2,
                    center: false
                },
                768: {
                    items: 3
                },
                992: {
                    items: 3
                },
                1200: {
                    items: 4
                },
                1280: {
                    items: 4
                },
                1366: {
                    items: 5
                },
                1440: {
                    items: 6
                }
            }
        })
    }

    /*  Team-Slider-Owl-carousel  */
    if ($('.single_product_slider').length) {
        $('.single_product_slider').owlCarousel({
            loop: true,
            margin: 30,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                600: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 1
                },
                992: {
                    items: 1
                },
                1200: {
                    items: 1
                },
                1366: {
                    items: 1
                },
                1400: {
                    items: 1
                }
            }
        })
    }

    /*  Team-Slider-Owl-carousel  */
    if ($('.bestseller_sidebar_slider').length) {
        $('.bestseller_sidebar_slider').owlCarousel({
            loop: true,
            margin: 30,
            dots: true,
            nav: true,
            rtl: false,
            autoplayHoverPause: false,
            autoplay: false,
            singleItem: true,
            smartSpeed: 1200,
            navText: [
                '<i class="flaticon-left-arrow"></i>',
                '<i class="flaticon-chevron"></i>'
            ],
            responsive: {
                0: {
                    items: 1,
                    center: false
                },
                480: {
                    items: 1,
                    center: false
                },
                520: {
                    items: 1,
                    center: false
                },
                768: {
                    items: 1
                },
                992: {
                    items: 1
                },
                1200: {
                    items: 1
                }
            }
        })
    }

    /*  Expert-Freelancer-Owl-carousel  */
    if ($('.banner-style-one').length) {
        $('.banner-style-one').owlCarousel({
            loop: true,
            items: 1,
            margin: 0,
            dots: true,
            nav: true,
            animateOut: 'slideOutDown',
            animateIn: 'fadeIn',
            active: true,
            smartSpeed: 1000,
            autoplay: false
        });
        $('.banner-carousel-btn .left-btn').on('click', function () {
            $('.banner-style-one').trigger('next.owl.carousel');
            return false;
        });
        $('.banner-carousel-btn .right-btn').on('click', function () {
            $('.banner-style-one').trigger('prev.owl.carousel');
            return false;
        });
    }

    /* ----- Scroll To top ----- */
    function scrollToTop() {
        $(window).scroll(function () {
            if ($(this).scrollTop() > 600) {
                $('.scrollToHome').fadeIn();
            } else {
                $('.scrollToHome').fadeOut();
            }
        });

        //Click event to scroll to top
        $('.scrollToHome').on('click', function () {
            $('html, body').animate({ scrollTop: 0 }, 800);
            return false;
        });
    }

    /* ----- Mega Dropdown Content ----- */
    $(document).on('ready', function () {
        $("#show_advbtn, #show_advbtn2").on('click', function () {
            $("body").addClass("mobile_ovyh");
        });
        $("#show_advbtn, #show_advbtn2").on('click', function () {
            $("body").removeClass("mobile_ovyh");
        });
        $("#prncgs").on('click', function () {
            $(".dd_content2").toggle();
        });
        $("#prncgs2, #prncgs3, #prncgs4").on('click', function () {
            $(".dd_content2").toggle();
        });
        $(".drop_btn").on('click', function () {
            $(".drop_content").toggle();
        });
        $(".drop_btn2").on('click', function () {
            $(".drop_content2").toggle();
        });
        $(".drop_btn3").on('click', function () {
            $(".drop_content3").toggle();
        });
        $(".drop_btn4").on('click', function () {
            $(".drop_content4").toggle();
        });
        $(".drop_btn5 ").on('click', function () {
            $(".drop_content5 ").toggle();
        });
        $(".drop_btn6").on('click', function () {
            $(".drop_content6").toggle();
        });

        $(".filter_open_btn").on('click', function () {
            $(".sidebar_content_details.style3").addClass("sidebar_ml0");
        });

        $(".filter_closed_btn").on('click', function () {
            $(".sidebar_content_details.style3").removeClass("sidebar_ml0");
        });

        $(".filter_open_btn").on('click', function () {
            $("body").addClass("body_overlay");
        });

        $(".filter_closed_btn").on('click', function () {
            $("body").removeClass("body_overlay");
        });

        $(".overlay_close").on('click', function () {
            $(".white_goverlay").toggle(500);
        });

        $(".mega_tags_dropdown").on('click', function () {
            $(".tag_dropdown_content").toggle(500);
        });

        $('.sticky-nav-tabs-container li').on('click', function () {
            $('.sticky-nav-tabs-container li').removeClass('active');
            $(this).addClass('active');
        });

    });


    /* ======
       When document is ready, do
       ====== */
    $(document).on('ready', function () {
        // add your functions
        //navbarScrollfixed();
        scrollToTop();
        //wowAnimation();
        mobileNavToggle();


        // extending for text toggle
        $.fn.extend({
            toggleText: function (a, b) {
                return this.text(this.text() == b ? a : b);
            }
        });
        if ($('.showFilter').length) {
            $('.showFilter').on('click', function () {
                $(this).toggleText('Show Filter', 'Hide Filter');
                $(this).toggleClass('flaticon-close flaticon-web-page sidebarOpended sidebarClosed');
                $('.listing_toogle_sidebar.sidenav').toggleClass('opened');
                $('.body_content').toggleClass('translated');
            });
        }

        $.fn.extend({
            toggleText2: function (a, b) {
                return this.text(this.text() == b ? a : b);
            }
        });

        if ($('.showBtns').length) {
            $('.showBtns').on('click', function () {
                $(this).toggleText2('Show Filter', 'Hide Filter');
                $(this).toggleClass('flaticon-close flaticon-web-page sidebarOpended2 sidebarClosed2');
                $('.sidebar_content_details').toggleClass('is-full-width');
            });
        }

    });

    /* ======
       When document is loading, do
       ====== */
    // window on Load function
    $(window).on('load', function () {
        preloaderLoad();

    });
  

})(window.jQuery);



$(function () {
    $('#thumbnail li').click(function () {
        var thisIndex = $(this).index()

        if (thisIndex < $('#thumbnail li.active').index()) {
            prevImage(thisIndex, $(this).parents("#thumbnail").prev("#image-slider"));
        } else if (thisIndex > $('#thumbnail li.active').index()) {
            nextImage(thisIndex, $(this).parents("#thumbnail").prev("#image-slider"));
        }

        $('#thumbnail li.active').removeClass('active').css('border', '').css('opacity','');
        $(this).addClass('active').css('border', '1px solid #86BC42').css('opacity', 1);
    });
});

var width = $('#image-slider').width();

function nextImage(newIndex, parent) {
    parent.find('li').eq(newIndex).addClass('next-img').css('left', width).animate({ left: 0 }, 600);
    parent.find('li.active-img').removeClass('active-img').css('left', '0').animate({ left: -width }, 600);
    parent.find('li.next-img').attr('class', 'active-img');
}
function prevImage(newIndex, parent) {
    parent.find('li').eq(newIndex).addClass('next-img').css('left', -width).animate({ left: 0 }, 600);
    parent.find('li.active-img').removeClass('active-img').css('left', '0').animate({ left: width }, 600);
    parent.find('li.next-img').attr('class', 'active-img');
}

/* Thumbails */
//var ThumbailsWidth = ($('#image-slider').width() - 18.5) / 7;
//$('#thumbnail li').find('img').css('width', ThumbailsWidth);
