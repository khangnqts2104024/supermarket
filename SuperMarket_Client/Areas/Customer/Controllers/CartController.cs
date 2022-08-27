
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_Models.Models;
using SuperMarket_Models.ViewModels;
using SuperMarket_Utility;
using System.Security.Claims;

namespace SuperMarket_Client.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
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
            if (User.Identity != null)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value, includeProperties: "Product"),
                    Order = new()
                };

                foreach (var item in shoppingCartVM.ListCart)
                {
                    shoppingCartVM.Order.OrderTotal += (item.Product.Price * item.Count);
                }
                return View(shoppingCartVM);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> CheckCount()
        {
            if (User.Identity != null)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var listCart = await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value);
                if(listCart != null)
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

        [HttpPost]
        public async Task<IActionResult> Plus(int cartId)
        {
            var cartFromDb = await unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.CartId == cartId, includeProperties: "Product");
            if (cartFromDb != null)
            {
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

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartId)
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

        [HttpPost]
        public async Task<IActionResult> Minus(int cartId, string? actionClient)
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

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ListCart = await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId.Equals(claim.Value), includeProperties: "Product"),
                Order = new()
            };
            if (shoppingCartVM.ListCart.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            shoppingCartVM.Order.Customer = await unitOfWork.Customer.GetFirstOrDefault(x => x.Id.Equals(claim.Value));
            if (shoppingCartVM.Order.Customer != null)
            {
                shoppingCartVM.Order.Name = shoppingCartVM.Order.Customer.FullName;
                shoppingCartVM.Order.Phone = shoppingCartVM.Order.Customer.PhoneNumber;
                shoppingCartVM.Order.Address = shoppingCartVM.Order.Customer.Address;
                shoppingCartVM.Order.City = shoppingCartVM.Order.Customer.City;
                shoppingCartVM.Order.Country = shoppingCartVM.Order.Customer.Country;
                //khang check coupon


                //khang
                foreach (var item in shoppingCartVM.ListCart)
                {
                    shoppingCartVM.Order.OrderTotal += item.Product.Price * item.Count;
                }
            }
         
            return View(shoppingCartVM);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(ShoppingCartVM shoppingCartVM,int? cpId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartVM.ListCart = await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId.Equals(claim.Value), includeProperties: "Product");
            shoppingCartVM.Order.OrderDate = System.DateTime.Now;
            shoppingCartVM.Order.CustomerId = claim.Value;
            //khang
            int dcPercent = 0;
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

            shoppingCartVM.Order.OrderTotal = total - total * dcPercent/ 100;

            //
            shoppingCartVM.Order.OrderStatus = SD.StatusPending;
            shoppingCartVM.Order.PaymentStatus = SD.StatusPending;

            await unitOfWork.Order.Add(shoppingCartVM.Order);
            await unitOfWork.Save();

            foreach (var item in shoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    OrderId = shoppingCartVM.Order.OrderId,
                    Price = item.Product.Price * item.Count
                };
                await unitOfWork.OrderDetail.Add(orderDetail);
                await unitOfWork.Save();
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

                        UnitAmount = (long)(item.Product.Price*100*(100-dcPercent)/100), //Product Price, 
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
                unitOfWork.Order.UpdateStripePaymentId(shoppingCartVM.Order.OrderId, session.Id, session.PaymentIntentId);


                await unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
        }


        [HttpGet]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var order = await unitOfWork.Order.GetFirstOrDefault(x => x.OrderId == id,includeProperties:"Coupon");
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
                    await unitOfWork.Save();
                }
            }
            return View(orderVM);
        }

        public async Task<decimal> GetOrderTotal()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ListCart = await unitOfWork.ShoppingCart.GetAll(x => x.CustomerId == claim.Value, includeProperties: "Product"),
                Order = new()
                //khang
                
            };

            foreach (var item in shoppingCartVM.ListCart)
            {
                shoppingCartVM.Order.OrderTotal += (item.Product.Price * item.Count);
            }
            return shoppingCartVM.Order.OrderTotal;
        }


    }
}
