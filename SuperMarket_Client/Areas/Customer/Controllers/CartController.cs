
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Models.ViewModels;
using SuperMarket_Utility;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles ="Customer")]
    [BranchActionFilter]

    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
    

        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
         
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                //check branchId is exists in Cookie
                var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int result);
                //redirect to Home if user not login and not set branchId
                if (User.Identity != null && result != 0)
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
                    {
                        ListCart = (List<ShoppingCart>)await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value, includeProperties: "Product.ImageProduct"),
                        Order = new()
                    };


                    //return View if cart is empty
                    if (shoppingCartVM.ListCart.Count() == 0)
                    {
                        return View(shoppingCartVM);
                    }
                    unitOfWork.ClearTracking();

                    //create a list to add which item out of stock
                    List<string> ListItemCartOutStock = new List<string>();

                    //clone listcart to another list
                    List<ShoppingCart>? indexListCart = new List<ShoppingCart>(shoppingCartVM.ListCart);
                    foreach (var item in indexListCart)
                    {
                        var IsOutStock = await unitOfWork.Stock.GetFirstOrDefault(x => x.BranchId == result && x.ProductId == item.ProductId && x.Count < item.Count);
                        if (IsOutStock != null)
                        {
                            //add item to list then remove out from cart.
                            ListItemCartOutStock.Add(item.Product.Title);
                            unitOfWork.ShoppingCart.Remove(item);
                            shoppingCartVM.ListCart.Remove(item);
                            await unitOfWork.Save();
                        }
                    }


                    if (shoppingCartVM.ListCart.Count() > 0)
                    {
                        foreach (var item in shoppingCartVM.ListCart)
                        {
                            shoppingCartVM.Order.OrderTotal += (item.Product.Price * item.Count);
                        }
                    }
                    else
                    {
                        shoppingCartVM.Order.OrderTotal = 0;
                    }


                    //send list of item out stock to View, then show alert with sweetalert.
                    if (ListItemCartOutStock.Count() != 0)
                    {
                        ViewBag.ListItemCartOutStock = ListItemCartOutStock;
                    }

                    indexListCart = null;
                    return View(shoppingCartVM);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Plus(int cartId, int itemCount,int? productId)
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                //check branchId is exists in Cookie
                var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int result);
                var cartFromDb = new ShoppingCart();
                if (productId != null)
                {
                    cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ProductId == productId && x.CustomerId == claim.Value, includeProperties: "Product");
                    if (cartFromDb != null && branchId == true)
                    {
                        var stock = await unitOfWork.Stock.GetFirstOrDefault(x => x.BranchId == result && x.ProductId == cartFromDb.ProductId);

                        if (itemCount == stock.Count || itemCount + cartFromDb.Count >= stock.Count)
                        {
                            return Json(new
                            {
                                statusCode = 400,
                                message = "The product you have selected has reached a limited quantity",
                            });
                        }

                        return Json(new
                        {
                            statusCode = 200,
                            message = "Increment Count Successfully",
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 404,
                            message = "Cannot Found Item"
                        });
                    }
                }
                else
                {
                    cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.CartId == cartId, includeProperties: "Product");
                    if (cartFromDb != null && branchId == true)
                    {
                        var stock = await unitOfWork.Stock.GetFirstOrDefault(x => x.BranchId == result && x.ProductId == cartFromDb.ProductId);

                        if (itemCount == stock.Count)
                        {
                            return Json(new
                            {
                                statusCode = 400,
                                message = "The product you have selected has reached a limited quantity",
                                count = cartFromDb.Count,
                                subTotalItem = cartFromDb.Product.Price * cartFromDb.Count,
                                subTotalOrder = await GetOrderTotal()
                            });
                        }

                        unitOfWork.ShoppingCart.IncrementCount(cartFromDb, 1);
                        await unitOfWork.Save();
                        return Json(new
                        {
                            statusCode = 200,
                            message = "Increment Count Successfully",
                            count = cartFromDb.Count,
                            subTotalItem = cartFromDb.Product.Price * cartFromDb.Count,
                            subTotalOrder = await GetOrderTotal()
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            statusCode = 404,
                            message = "Cannot Found Item"
                        });
                    }
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500,
                    message = "Something went wrong..."
                });
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Minus(int cartId, string? actionClient)
        {
            try
            {
                var cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.CartId == cartId, includeProperties: "Product");
                if (cartFromDb != null)
                {
                    if (cartFromDb.Count <= 1 && string.IsNullOrEmpty(actionClient))
                    {
                        return Json(new
                        {
                            statusCode = 200,
                            message = "Do you want to remove item from cart?",
                            actionClient = "ask"
                        });

                    }
                    else if (cartFromDb.Count <= 1 && actionClient == "confirmed")
                    {
                        unitOfWork.ShoppingCart.Remove(cartFromDb);
                        await unitOfWork.Save();

                        return Json(new
                        {
                            statusCode = 200,
                            message = "Remove Success!",
                            actionClient = "removed",
                            subTotalOrder = await GetOrderTotal()
                        });
                    }
                    else
                    {
                        unitOfWork.ShoppingCart.DecrementCount(cartFromDb, 1);
                        await unitOfWork.Save();
                        return Json(new
                        {
                            statusCode = 200,
                            message = "Decrement Count Successfully",
                            count = cartFromDb.Count,
                            subTotalItem = cartFromDb.Product.Price * cartFromDb.Count,
                            subTotalOrder = await GetOrderTotal()
                        });

                    }
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 404,
                        message = "Cannot Found Item"
                    });
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500,
                    message = "Something went wrong..."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartId)
        {
            try
            {
                var cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.CartId == cartId, includeProperties: "Product");
                if (cartFromDb != null)
                {
                    unitOfWork.ShoppingCart.Remove(cartFromDb);
                    await unitOfWork.Save();
                    return Json(new
                    {
                        statusCode = 200,
                        message = "Remove Success!",
                        actionClient = "removed",
                        subTotalOrder = await GetOrderTotal()
                    });
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 404,
                        message = "Cannot Found Item"
                    });
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500,
                    message = "Something went wrong..."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var paymentIntentSession = HttpContext.Session.GetString("paymentIntent");
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var listCoupon = await unitOfWork.Coupon.GetAll(x => x.Count > 0 && x.ExpiredDate > DateTime.Now);
                var selectListCoupon = new SelectList(listCoupon, nameof(Coupon.CouponCode), nameof(Coupon.Description));
                ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = (List<ShoppingCart>)await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId.Equals(claim.Value), includeProperties: "Product"),
                    ListCoupon = selectListCoupon
                };
                if (shoppingCartVM.ListCart.Count() == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (paymentIntentSession == null)
                {
                    shoppingCartVM.Order = new();
                    shoppingCartVM.Order.Customer = await unitOfWork.Customer.GetFirstOrDefault(x => x.Id.Equals(claim.Value));
                    if (shoppingCartVM.Order.Customer != null)
                    {
                        shoppingCartVM.Order.Name = shoppingCartVM.Order.Customer.FullName;
                        shoppingCartVM.Order.Phone = shoppingCartVM.Order.Customer.PhoneNumber;
                        shoppingCartVM.Order.Address = shoppingCartVM.Order.Customer.Address;
                        shoppingCartVM.Order.City = shoppingCartVM.Order.Customer.City;
                        shoppingCartVM.Order.Country = shoppingCartVM.Order.Customer.Country;
                        //khang
                        foreach (var item in shoppingCartVM.ListCart)
                        {
                            shoppingCartVM.Order.OrderTotal += item.Product.Price * item.Count;
                        }
                    }
                }
                else
                {
                    var orderPending = await unitOfWork.Order.GetFirstOrDefault(x => x.CustomerId == claim.Value && x.SessionId == paymentIntentSession && x.PaymentStatus == SD.PaymentStatusPending);
                    var coupon = await unitOfWork.Coupon.GetFirstOrDefault(x => x.CouponId == orderPending.CouponId);
                    if (coupon != null)
                    {
                        coupon.Count += 1;
                        unitOfWork.Coupon.Update(coupon);
                        await unitOfWork.Save();
                    }

                    if (orderPending != null)
                    {
                        unitOfWork.Order.Remove(orderPending);
                        await unitOfWork.Save();
                    }
                    HttpContext.Session.Remove("paymentIntent");
                    return RedirectToAction("Index", "Cart");

                }


                return View(shoppingCartVM);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new { area = "Customer" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(ShoppingCartVM shoppingCartVM,int? cpId)
        {
            try
            {
                var branchId = int.TryParse(HttpContext.Request.Cookies["branchId"], out int result);

                var paymentIntentSession = HttpContext.Session.GetString("paymentIntent");
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCartVM.ListCart = (List<ShoppingCart>)await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId.Equals(claim.Value), includeProperties: "Product");

                int dcPercent = 0;

                if (paymentIntentSession == null)
                {

                    shoppingCartVM.Order.OrderDate = System.DateTime.Now;
                    shoppingCartVM.Order.CustomerId = claim.Value;
                    //khang
                    var usedCoupon = await unitOfWork.Coupon.GetFirstOrDefault(c => c.CouponId.Equals(cpId));
                    if (usedCoupon != null)
                    {
                        shoppingCartVM.Order.Coupon = usedCoupon;
                        shoppingCartVM.Order.CouponId = usedCoupon.CouponId;
                        dcPercent = usedCoupon.DiscountPercent;
                    }

                    decimal total = 0;
                    //khang
                    foreach (var item in shoppingCartVM.ListCart)
                    {
                        //shoppingCartVM.Order.OrderTotal += item.Product.Price * item.Count;
                        total += item.Product.Price * item.Count;
                    }

                    shoppingCartVM.Order.OrderTotal = total - total * dcPercent / 100;
                    //
                    shoppingCartVM.Order.OrderStatus = SD.StatusPending;
                    shoppingCartVM.Order.PaymentStatus = SD.StatusPending;
                    shoppingCartVM.Order.BranchId = result;
                    await unitOfWork.Order.Add(shoppingCartVM.Order);
                    await unitOfWork.Save();

                    foreach (var item in shoppingCartVM.ListCart)
                    {
                        OrderDetail orderDetail = new OrderDetail()
                        {
                            ProductId = item.ProductId,
                            Count = item.Count,
                            OrderId = shoppingCartVM.Order.OrderId,
                            Price = item.Product.Price * item.Count,

                        };
                        await unitOfWork.OrderDetail.Add(orderDetail);
                        await unitOfWork.Save();
                    }
                }
                else
                {
                    shoppingCartVM.Order = await unitOfWork.Order.GetFirstOrDefault(x => x.SessionId == paymentIntentSession, includeProperties: "Customer,OrderDetail");
                }

                //Start Redirect To Stripe Page
                var domain = "https://localhost:7166/";
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/CompleteOrder?id={shoppingCartVM.Order.OrderId}",
                    CancelUrl = domain + $"customer/cart/index",
                };

                foreach (var item in shoppingCartVM.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            //khang

                            UnitAmount = (long)(item.Product.Price * 100 * (100 - dcPercent) / 100), //Product Price, 
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title,
                            },

                        },
                        Quantity = item.Count,
                        //stripe will auto calculate amount based on unitAmount & quantity
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                //create a session for options based on SessionService
                Session session = service.Create(options);
                HttpContext.Session.SetString("paymentIntent", session.Id);
                unitOfWork.Order.UpdateStripePaymentId(shoppingCartVM.Order.OrderId, session.Id, session.PaymentIntentId);
                await unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error", new { area = "Customer" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckCartBeforeCheckout()
        {
            try
            {
                if (User.Identity != null)
                {
                    var claimsIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    var listCart = await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value);
                    if (listCart != null)
                    {
                        return Json(new
                        {
                            statusCode = 200,
                            count = listCart.Count(),
                        });
                    }
                }
                return Json(new
                {
                    statusCode = 401,
                    message = "User login required!"
                });

            }
            catch (Exception)
            {

                return Json(new
                {
                    statusCode = 500,
                    message = "Something went wrong..."
                });
            }

        }
      
        [HttpGet]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                var order = await unitOfWork.Order.GetFirstOrDefault(x => x.OrderId == id, includeProperties: "Coupon");
                var orderDetails = await unitOfWork.OrderDetail.GetAll(x => x.OrderId == id, includeProperties: "Product");
                //
                //var useCoupon = await unitOfWork.Coupon.GetFirstOrDefault(c => c.CouponId.Equals(order.CouponId));
                //
                OrderVM orderVM = new OrderVM()
                {
                    Order = order,
                    OrderDetails = orderDetails
                };
                //check the stripe status to make sure payment is actually successful
                if (orderVM.Order != null && orderVM.OrderDetails != null)
                {
                    var service = new SessionService();
                    //get a session for options based on SessionService
                    Session session = service.Get(orderVM.Order.SessionId);
                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        unitOfWork.Order.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                        IEnumerable<ShoppingCart> shoppingCarts = await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == orderVM.Order.CustomerId);
                        //khang
                        if (order.Coupon != null && order.Coupon.Count > 0) { order.Coupon.Count -= 1; }


                        //
                        unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
                        HttpContext.Session.Remove("paymentIntent");
                        await unitOfWork.Save();
                    }
                    else
                    {
                        return RedirectToAction("Index", "Error");
                    }
                }
                return View(orderVM);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error", new
                {
                    area = "Customer"
                });
            }
           
        }

        public async Task<decimal> GetOrderTotal()
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = (List<ShoppingCart>)await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value, includeProperties: "Product"),
                    Order = new()
                    //khang

                };

                foreach (var item in shoppingCartVM.ListCart)
                {
                    shoppingCartVM.Order.OrderTotal += (item.Product.Price * item.Count);
                }
                return shoppingCartVM.Order.OrderTotal;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
