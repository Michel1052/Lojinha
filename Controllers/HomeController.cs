using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Collections;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using Lojinha.Models;

namespace Lojinha.Controllers
{
    public class HomeController : Controller
    {
        /*public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext() : base("name=DefaultConnection")
            {
            }

            //public DbSet<CartItem> CartItems { get; set; }
            //public DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        }*/

        /*[HttpPost]
        public JsonResult AddToCart(CartItem newItem)
        {
            // Step 1: Retrieve existing cart cookie (if it exists)
            var cartCookie = Request.Cookies["ShoppingCart"];

            string cartData;
            if (cartCookie != null)
            {
                // Step 2: Append the new item to existing cookie data
                cartData = cartCookie.Value + $"|{newItem.ProductId},{newItem.ProductName},{newItem.Quantity},{newItem.Price};";
            }
            else
            {
                // Step 2: Create a new cart string
                cartData = $"{newItem.ProductId},{newItem.ProductName},{newItem.Quantity},{newItem.Price};";
            }

            // Step 3: Save the updated cart back to the cookie
            cartCookie = new HttpCookie("ShoppingCart")
            {
                Value = cartData,
                Expires = DateTime.Now.AddDays(7) // Cookie expiration time
            };
            Response.Cookies.Add(cartCookie);

            if (newItem != null)
            {
                // Example: Add the new item to a session list
                bool existe = false;
                var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
                foreach (var item in cart)
                {
                    if (newItem.ProductId == item.ProductId)
                    {
                        existe = true;
                        item.Quantity++;
                        Session["Cart"] = cart;
                    }
                }

                if (!existe)
                {
                    cart.Add(newItem);
                    Session["Cart"] = cart;
                }

                //bool isNullOrEmpty = CartItem?.Any() != true;               
                if (cart.Count == 0)
                {
                    cart.Add(newItem);
                    Session["Cart"] = cart;
                }

                return Json(new { success = true, message = "Item added successfully!" });
            }

            return Json(new { success = false, message = "Invalid item data." });
        }

        [HttpPost]
        public JsonResult RemoveFromCart(CartItem tobeRemoved)
        {
            if (tobeRemoved != null)
            {
                var cart = Request.Cookies["ShoppingCart"];

                var items = cart.Value.Split('|');
                bool igual = false;

                if (tobeRemoved.ProductId2 != null)
                {

                    string[] final = { tobeRemoved.ProductId2.ToString() };
                    var updatedItems = items.Where(item =>
                    {
                        var parts = item.Split(',');
                        var fin = final[0].Split(',');

                        foreach (string i in fin)

                        {

                            if (parts[0] == i.ToString())
                            {
                                igual = true;
                                break;

                            }

                        }

                        if (!igual)
                        {

                            return parts[0] != null;
                        }
                        igual = false;
                        return parts[0] == null;
                    }).ToArray();


                    cart.Value = string.Join("|", updatedItems);

                    Response.Cookies.Set(cart);
                }

                return Json(new { success = true, message = "Item removido com sucesso!" });
            }

            return Json(new { success = false, message = "Item inválido." });
        }

        /*[HttpPost]
        public JsonResult RemoveFromCart(CartItem tobeRemoved)
        {
            if (tobeRemoved != null)
            {
                var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();                
                foreach (var item in cart)
                {
                    if (tobeRemoved.ProductId == item.ProductId)
                    {
                        cart.Remove(item);
                        Session["Cart"] = cart;
                        break;
                    }
                }

                return Json(new { success = true, message = "Item removed successfully!" });
            }

            return Json(new { success = false, message = "Invalid item data." });
        }*/

        /*public ActionResult GetCart()
        {
            var cartCookie = Request.Cookies["ShoppingCart"];
            var cartItems = new List<CartItem>();

            if (cartCookie != null)
            {
                // Step 1: Split the cookie data
                var items = cartCookie.Value.Split('|');
                foreach (var item in items)
                {
                    if (string.IsNullOrWhiteSpace(item)) continue;

                    var itemDetails = item.Split(',');

                    if (itemDetails[0].ToString() != "")
                    {

                        // Step 2: Map the cookie data to CartItem objects
                        var cartItem = new CartItem
                        {
                            ProductId = int.Parse(itemDetails[0]),
                            ProductName = itemDetails[1],
                            Quantity = int.Parse(itemDetails[2]),
                            Price = float.Parse(itemDetails[3])
                        };
                        cartItems.Add(cartItem);
                    }                    
                                                       
                }
            }

            return View(cartItems);
        }*/

        /*public ActionResult ClearCart()
        {            
            
            var cartCookie = new HttpCookie("ShoppingCart")
            {
                Expires = DateTime.Now.AddDays(-1) 
            };
            Response.Cookies.Add(cartCookie);

            return Json(new { success = true });
        }*/

        public ActionResult LoadCart()
        {

            string userId = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();
            var cartItems = db.CartItems.Where(c => c.UserId == userId)
            .Select(c => new { c.ProductId, c.ProductName, c.Price, c.Quantity }).ToList();

            if (cartItems != null)
            {
                return Json(new { cartItems }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult AddToCartDb(CartItem newItem)
        {
            string userId = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();
            newItem.UserId = userId;

            //string teste = newItem.Price.ToString();

            //float.TryParse(price, NumberStyles.Any, CultureInfo.InvariantCulture, out float parsedPrice);

            var id = newItem.ProductId;
            if (newItem.UserId != null)
            {
                var cartItem = db.CartItems.Where(c => c.UserId == userId && c.ProductId.ToString() == id.ToString()).FirstOrDefault();
                if (cartItem == null)
                {
                    //newItem.Price = 999.99;
                    db.CartItems.Add(newItem);

                    db.Configuration.ProxyCreationEnabled = false;
                    db.Configuration.LazyLoadingEnabled = false;

                    db.SaveChanges();
                    var crt = db.CartItems.ToList();
                }

                else if (cartItem != null)
                {
                    cartItem.Quantity += 1;

                    db.SaveChanges();

                }
            }

            return Json(new { success = true, message = "" });

        }

        [HttpPost]
        public JsonResult DeleteFromCartDb(CartItem newItem, bool removerTodos)
        {
            string userId = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();
            var item = newItem.ProductId2.Split(',');
            List<CartItem> cartItems = new List<CartItem>();


            if (removerTodos)
            {

                foreach (var Item in item)
                {

                    cartItems.AddRange(db.CartItems.Where(c => c.UserId == userId && c.ProductId.ToString() == Item.ToString()).ToList());

                }

                foreach (var cartItem in cartItems)
                {
                    db.CartItems.Remove(cartItem);
                }
                db.SaveChanges();
            }

            else
            {
                foreach (var Item in item)
                {

                    var cartItemToBeRemoved = db.CartItems.Where(c => c.UserId == userId && c.ProductId.ToString() == Item.ToString()).FirstOrDefault();

                    if (cartItemToBeRemoved.Quantity > 1)
                    {
                        cartItemToBeRemoved.Quantity -= 1;
                        db.SaveChanges();
                    }
                }
            }
            return Json(new { success = true, message = "" });
        }

        [HttpPost]
        public JsonResult ClearCartDb()
        {
            string userId = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();
            var cartItems = db.CartItems.Where(c => c.UserId == userId).ToList();
            foreach (var cartItem in cartItems)
            {
                db.CartItems.Remove(cartItem);
            }
            db.SaveChanges();
            return Json(new { success = true, message = "" });
        }


        //[Authorize(Roles = "User")]
        public ActionResult Index()
        {

            LoadCart();

            List<CartItem> crt = new List<CartItem>();

            return View(crt);

        }

        [HttpGet]
        public JsonResult GetCartItemCount()
        {
            string userId = User.Identity.GetUserId();
            ApplicationDbContext db = new ApplicationDbContext();



            var count = db.CartItems
                         .Where(c => c.UserId == userId)
                         .Select(c => (int?)c.Quantity) // cast to nullable int
             .Sum() ?? 0; // use ?? to handle possible null result

            return Json(new { count }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult IMDb()
        {
            return View();
        }


        //API MERCADO PAGO
        /*// Action para iniciar o pagamento
        public ActionResult Pagar()
        {
            MercadoPago.SDK.AccessToken = "SEU_ACCESS_TOKEN_DE_TESTE";

            var preference = new Preference();

            var item = new Item()
            {
                Title = "Produto de Teste",
                Quantity = 1,
                UnitPrice = 100
            };

            preference.Items = new List<Item> { item };
            preference.BackUrls = new BackUrls
            {
                Success = "https://www.seusite.com/sucesso",
                Failure = "https://www.seusite.com/falha",
                Pending = "https://www.seusite.com/pendente"
            };

            preference.AutoReturn = AutoReturnType.All;
            preference.Save();

            var redirectUrl = preference.InitPoint;
            return Redirect(redirectUrl);
        }*/
    }
}

