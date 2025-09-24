using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronicsStoreMVC.Models;
using ElectronicsStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;


namespace ElectronicsStoreMVC.Controllers
{

    public class StoreController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly int pageSize = 8;

        public StoreController(ApplicationDbContext context)
        {
            this.context = context;
        }
        // GET: /<controller>/
        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {
            IQueryable<Product> query = context.Products;

            //Search functionality
            if (search != null && search.Length > 0)
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            //filter functionalty
            if (brand != null && brand.Length > 0)
            {
                query = query.Where(p => p.Brand.Contains(brand));
            }
            if (category != null && category.Length > 0)
            {
                query = query.Where(p => p.Category.Contains(category));
            }

            //Sort functionality
            if (sort == "price_asc")
            {
                query = query.OrderBy(p => (double)p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => (double)p.Price);
            }
            else
            {
                query = query.OrderByDescending(p => p.Id);
            }

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var products = query.ToList();
            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            var storeSearchModel = new StoreSearchModel()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort

            };

            return View(storeSearchModel);
        }

        public IActionResult Details(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Store");
            }
            return View(product);
        }
    }
}
