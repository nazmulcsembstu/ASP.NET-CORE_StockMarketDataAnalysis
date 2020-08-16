using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreWeb.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Nancy.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace CoreWeb.Controllers
{
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StockController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(webRootPath, "data/stock_market_data.json");

            StreamReader streamReader = new StreamReader(path);
            string Json = streamReader.ReadToEnd();

            var stockdatalist = JsonConvert.DeserializeObject<StockDataClass>(Json);

            return View(stockdatalist);
        }

        public IActionResult UploadJson()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadJson(IFormFile jsonFile)
        {
            string JsonName = Guid.NewGuid().ToString() + Path.GetExtension(jsonFile.FileName);
            string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data", JsonName);

            using (var stream = new FileStream(SavePath, FileMode.Create))
            {
                jsonFile.CopyTo(stream);
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string contentRootPath = _webHostEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(webRootPath, "data/" + JsonName);

            StreamReader streamReader = new StreamReader(path);
            string Json = streamReader.ReadToEnd();

            List<StockMarketClass> stocks = JsonConvert.DeserializeObject<List<StockMarketClass>>(Json);

            stocks.ForEach(s =>
            {
                StockMarketClass stock = new StockMarketClass()
                {
                    date = s.date,
                    trade_code = s.trade_code,
                    high = s.high,
                    low = s.low,
                    open = s.open,
                    close = s.close,
                    volume = s.volume
                };

                _context.StockMarket.Add(stock);
                _context.SaveChanges();
            });

            TempData["UploadMessage"] = "<script>alert('File Uploaded Successfully')</script>";

            return RedirectToAction("UpdateData", "Stock");
        }

        // GET: Stock
        public async Task<IActionResult> UpdateData()
        {
            return View(await _context.StockMarket.ToListAsync());
        }

        // GET: Stock/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockMarketClass = await _context.StockMarket
                .FirstOrDefaultAsync(m => m.id == id);
            if (stockMarketClass == null)
            {
                return NotFound();
            }

            return View(stockMarketClass);
        }

        // GET: Stock/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stock/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,date,trade_code,high,low,open,close,volume")] StockMarketClass stockMarketClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockMarketClass);
                await _context.SaveChangesAsync();

                TempData["CreateMessage"] = "<script>alert('Data Added Successfully')</script>";
   
                return RedirectToAction(nameof(UpdateData));
            }
            return View(stockMarketClass);
        }

        // GET: Stock/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockMarketClass = await _context.StockMarket.FindAsync(id);
            if (stockMarketClass == null)
            {
                return NotFound();
            }
            return View(stockMarketClass);
        }

        // POST: Stock/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,date,trade_code,high,low,open,close,volume")] StockMarketClass stockMarketClass)
        {
            if (id != stockMarketClass.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockMarketClass);
                    await _context.SaveChangesAsync();

                    TempData["EditMessage"] = "<script>alert('Data Edited Successfully')</script>";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockMarketClassExists(stockMarketClass.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UpdateData));
            }
            return View(stockMarketClass);
        }

        // GET: Stock/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockMarketClass = await _context.StockMarket
                .FirstOrDefaultAsync(m => m.id == id);
            
            if (stockMarketClass == null)
            {
                return NotFound();
            }

            return View(stockMarketClass);
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockMarketClass = await _context.StockMarket.FindAsync(id);
            _context.StockMarket.Remove(stockMarketClass);
            await _context.SaveChangesAsync();

            TempData["DeleteMessage"] = "<script>alert('Data Deleted Successfully')</script>";

            return RedirectToAction(nameof(UpdateData));
        }

        private bool StockMarketClassExists(int id)
        {
            return _context.StockMarket.Any(e => e.id == id);
        }
    }
}
