using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stripe.Checkout;
using System.Security.Claims;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    public class ShoppingCartController : Controller
    {
        private IShoppingCartRepository repo;
        private IProductRepository productRepository;
        private IApplicationUserRepository applicationUserRepository;
        [BindProperty]
        public ShoppingCartVM scVM { get; set; }
        private IOrderHeaderRepository orderHeaderRepository { get; set; }
        private IOrderDetailRepository orderDetailRepository { get; set; }
        
        public ShoppingCartController(IShoppingCartRepository sc, IProductRepository productRepository, IApplicationUserRepository userRepository,
            IOrderHeaderRepository orderHeader, IOrderDetailRepository orderDetail)
        {
            this.orderHeaderRepository = orderHeader;
            this.orderDetailRepository = orderDetail;
            this.applicationUserRepository = userRepository;
            this.productRepository = productRepository;
            repo = sc;
        }

        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var uId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            scVM = new()
            {
                ShoppingCartList = repo.GetAll(u => u.UserId == uId, includeProperties: "Product"),
                OrderHeader = new()
                {
                    OrderTotal = CalculateOrderTotal(repo.GetAll(u => u.UserId == uId, includeProperties: "Product"))
                }
            };

            return View(scVM);
        }


        public IActionResult Increase(int cartId) {

            var shoppingCart = repo.Get(u => u.Id == cartId);
            shoppingCart.Count += 1;
            repo.Update(shoppingCart);
            repo.Save();
            return RedirectToAction(nameof(Index));   

        }
        public IActionResult Decrease(int cartId) {

            var shoppingCart = repo.Get(u => u.Id == cartId);
            if (shoppingCart.Count <= 1)
            {
                repo.Remove(shoppingCart);
            }
            else
            {
                shoppingCart.Count -= 1;
                repo.Update(shoppingCart);
            }
            repo.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Remove(int cartId) {
        
            var shoppingCart = repo.Get(u =>u.Id == cartId);
            repo.Remove(shoppingCart);
            repo.Save();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Summary() {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var uId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser applicationUser = applicationUserRepository.Get(u => u.Id == uId);

            scVM = new()
            {
                ShoppingCartList = repo.GetAll(u => u.UserId == uId, includeProperties: "Product"),
                OrderHeader = new()
                {
                    OrderTotal = CalculateOrderTotal(repo.GetAll(u => u.UserId == uId, includeProperties: "Product")),
                    ApplicationUser = applicationUser,
                    Name = applicationUser.Name,
                    PhoneNumber = applicationUser.PhoneNumber,
                    StreetAddress = applicationUser.StreetAddress,
                    City = applicationUser.City,
                    PostalCode = applicationUser.PostalCode,
                    State = applicationUser.State
                }
            };

            return View(scVM); 
        }


        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var uId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            scVM.ShoppingCartList = repo.GetAll(u => u.UserId == uId, includeProperties: "Product");

            scVM.OrderHeader.ApplicationUserId = uId;

            ApplicationUser user = applicationUserRepository.Get(u => u.Id == uId);

            scVM.OrderHeader.OrderDate = DateTime.Now;

            scVM.OrderHeader.OrderTotal = CalculateOrderTotal(scVM.ShoppingCartList);

            if(user.CompanyId.GetValueOrDefault() == 0)
            {
                scVM.OrderHeader.Name = "hello";
                scVM.OrderHeader.OrderStatus = SD.Status_Pending;
                scVM.OrderHeader.PaymentStatus = SD.Payment_Status_Pending;

            }else
            {
				scVM.OrderHeader.Name = "hello";
				scVM.OrderHeader.OrderStatus = SD.Status_Approved;
				scVM.OrderHeader.PaymentStatus = SD.Payment_Status_Delayed_Payment;
			}
            orderHeaderRepository.Add(scVM.OrderHeader);
            orderHeaderRepository.Save();

            foreach(var item in scVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = scVM.OrderHeader.Id,
                    Price = item.Product.Price,
                    Count = item.Count
                };
                orderDetailRepository.Add(orderDetail);
                orderDetailRepository.Save();
            }
            if (user.CompanyId.GetValueOrDefault() == 0)
            {
                // it is a regular customer account and we need to capture payment
                // add stripe logic here
                string domain = "http://localhost:5021/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain+$"customer/ShoppingCart/OrderConfirmation?id={scVM.OrderHeader.Id}",
                    CancelUrl = domain+"customer/ShoppingCart/index",
					LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
					Mode = "payment",
				};

                foreach(var item in scVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Product.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

				var service = new Stripe.Checkout.SessionService();
				Session session = service.Create(options);
                orderHeaderRepository.UpdateStripePaymentID(scVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                orderHeaderRepository.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);


			}
			return RedirectToAction(nameof(OrderConfirmation), new {id = scVM.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {

            OrderHeader orderHeader = orderHeaderRepository.Get(u => u.Id == id , includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus != SD.Payment_Status_Delayed_Payment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid" )
                {
                    orderHeaderRepository.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    orderHeaderRepository.UpdateStatus(id, SD.Status_Approved, SD.Payment_Status_Approved);
                    orderHeaderRepository.Save();
                }
            }

            List<ShoppingCart> shoppingCarts = repo.GetAll(u => u.ApplicationUser.Id == orderHeader.ApplicationUser.Id).ToList();
            repo.RemoveRange(shoppingCarts);
            repo.Save();
            return View(id);
        }


		private double CalculateOrderTotal(IEnumerable<ShoppingCart> shoppingCart)
        {
            double total = 0;
            if(shoppingCart != null)
            {
                foreach (var sc in shoppingCart)
                {
                    var product = productRepository.Get(u => u.Id == sc.ProductId);
                    var price = product.Price;
                    total += price * sc.Count;
                }
            }
            
            return total;
        }
    }
}
