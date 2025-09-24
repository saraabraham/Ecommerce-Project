using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ElectronicsStoreMVC.Models;
using ElectronicsStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;

namespace ElectronicsStoreMVC.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IConfiguration configuration;

        private string PayPalClientId { get; set; } = "";
        private string PayPalSecret { get; set; } = "";
        private string PayPalUrl { get; set; } = "";

        private readonly decimal shippingFee;
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public CheckoutController(IConfiguration configuration, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            PayPalClientId = configuration["PaypalSettings:ClientId"]!;
            PayPalSecret = configuration["PaypalSettings:Secret"]!;
            PayPalUrl = configuration["PaypalSettings:Url"]!;
            shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
            this.context = context;
            this.userManager = userManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal total = CartHelper.GetSubTotal(cartItems) + shippingFee;
            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            TempData.Keep();
            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.Total = total;
            ViewBag.PayPalClientId = PayPalClientId;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrder()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal totalAmount = CartHelper.GetSubTotal(cartItems) + shippingFee;
            //Create the request body
            JsonObject createOrderRequest = new JsonObject();
            createOrderRequest.Add("intent", "CAPTURE");

            JsonObject amount = new JsonObject();
            amount.Add("currency_code", "USD");
            amount.Add("value", totalAmount);

            JsonObject purchaseUnit1 = new JsonObject();
            purchaseUnit1.Add("amount", amount);

            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchaseUnit1);

            createOrderRequest.Add("purchase_units", purchaseUnits);

            //get access token
            string accessToken = await GetPaypalAccessToken();

            //send request
            string url = PayPalUrl + "/v2/checkout/orders";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(createOrderRequest.ToString(), null, "application/json");
                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        string paypalOrderId = jsonResponse["id"]?.ToString() ?? "";
                        return new JsonResult(new { Id = paypalOrderId });

                    }
                }
            }

            return new JsonResult(new { Id = "" });
        }

        [HttpPost]
        public async Task<JsonResult> CompleteOrder([FromBody] JsonObject data)
        {
            var orderId = data?["orderID"]?.ToString();
            var deliveryAddress = data?["deliveryAddress"]?.ToString();

            if (orderId == null || deliveryAddress == null)
            {
                return new JsonResult("error");
            }
            //get access token
            string accessToken = await GetPaypalAccessToken();

            //send request
            string url = PayPalUrl + "/v2/checkout/orders/" + orderId + "/capture";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", null, "application/json");
                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
                        if (paypalOrderStatus == "COMPLETED")
                        {
                            //Save the order in the db
                            await SaveOrder(jsonResponse.ToString(), deliveryAddress);
                            return new JsonResult("success");
                        }

                    }
                }
            }

            return new JsonResult("error");

        }

        private async Task SaveOrder(string paypalResponse, string deliveryAddress)
        {

            //get cart items
            var cartItems = CartHelper.GetCartItems(Request, Response, context);
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return;
            }

            //Save the order

            var order = new Order
            {
                ClientId = appUser.Id,
                Items = cartItems,
                ShippingFee = shippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = "paypal",
                PaymentStatus = "accepted",
                PaymentDetails = paypalResponse,
                OrderStatus = "pending",
                CreatedAt = DateTime.Now
            };
            context.Orders.Add(order);
            context.SaveChanges();

            //delete the shopping cart cookie
            Response.Cookies.Delete("shopping_cart");

        }

        public async Task<string> Token()
        {
            return await GetPaypalAccessToken();
        }

        private async Task<string> GetPaypalAccessToken()
        {
            string accessToken = "";
            string url = PayPalUrl + "/v1/oauth2/token";
            using (var client = new HttpClient())
            {
                string credentials64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(PayPalClientId + ":" + PayPalSecret));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials64);

                var requestMesssage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMesssage.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");
                var httpResponse = await client.SendAsync(requestMesssage);
                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        accessToken = jsonResponse["access_token"]?.ToString() ?? "";
                    }
                }
            }
            return accessToken;
        }
    }
}
