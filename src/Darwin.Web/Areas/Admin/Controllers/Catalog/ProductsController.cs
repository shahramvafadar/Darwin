using Darwin.Application.Catalog.Commands;
using Darwin.Application.Catalog.DTOs;
using Darwin.Application.Catalog.Queries.GetProductForEdit;
using Darwin.Application.Catalog.Queries.GetProductsPage;
using Darwin.Web.Areas.Admin.ViewModels.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Darwin.Web.Areas.Admin.Controllers.Catalog
{
    [Area("Admin")]
    public sealed class ProductsController : Controller
    {
        private readonly CreateProductHandler _createProduct;
        private readonly UpdateProductHandler _updateProduct;
        private readonly GetProductsPageHandler _getProductsPage;
        private readonly GetProductForEditHandler _getProductForEdit;

        public ProductsController(
            CreateProductHandler createProduct,
            UpdateProductHandler updateProduct,
            GetProductsPageHandler getProductsPage,
            GetProductForEditHandler getProductForEdit)
        {
            _createProduct = createProduct;
            _updateProduct = updateProduct;
            _getProductsPage = getProductsPage;
            _getProductForEdit = getProductForEdit;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, CancellationToken ct = default)
        {
            var (items, total) = await _getProductsPage.HandleAsync(page, pageSize, "de-DE", ct);
            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new ProductCreateVm
            {
                Translations = { new ProductTranslationVm { Culture = "de-DE" } },
                Variants = { new ProductVariantCreateVm() }
            };
            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = new ProductCreateDto
            {
                BrandId = vm.BrandId,
                PrimaryCategoryId = vm.PrimaryCategoryId,
                Kind = vm.Kind,
                Translations = vm.Translations.Select(t => new ProductTranslationDto
                {
                    Culture = t.Culture,
                    Name = t.Name,
                    Slug = t.Slug,
                    ShortDescription = t.ShortDescription,
                    FullDescriptionHtml = t.FullDescriptionHtml,
                    MetaTitle = t.MetaTitle,
                    MetaDescription = t.MetaDescription,
                    SearchKeywords = t.SearchKeywords
                }).ToList(),
                Variants = vm.Variants.Select(v => new ProductVariantCreateDto
                {
                    Sku = v.Sku,
                    Gtin = v.Gtin,
                    ManufacturerPartNumber = v.ManufacturerPartNumber,
                    BasePriceNetMinor = v.BasePriceNetMinor,
                    CompareAtPriceNetMinor = v.CompareAtPriceNetMinor,
                    Currency = v.Currency,
                    TaxCategoryId = v.TaxCategoryId,
                    StockOnHand = v.StockOnHand,
                    StockReserved = v.StockReserved,
                    ReorderPoint = v.ReorderPoint,
                    BackorderAllowed = v.BackorderAllowed,
                    MinOrderQty = v.MinOrderQty,
                    MaxOrderQty = v.MaxOrderQty,
                    StepOrderQty = v.StepOrderQty,
                    PackageWeight = v.PackageWeight,
                    PackageLength = v.PackageLength,
                    PackageWidth = v.PackageWidth,
                    PackageHeight = v.PackageHeight,
                    IsDigital = v.IsDigital
                }).ToList()
            };

            try
            {
                var id = await _createProduct.HandleAsync(dto, ct);
                TempData["Success"] = "Product created successfully.";
                return RedirectToAction(nameof(Edit), new { id });
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var e in ex.Errors)
                    ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var dto = await _getProductForEdit.HandleAsync(id, ct);
            if (dto == null) return NotFound();

            // Map DTO -> VM
            var vm = new ProductEditVm
            {
                Id = dto.Id,
                BrandId = dto.BrandId,
                PrimaryCategoryId = dto.PrimaryCategoryId,
                Kind = dto.Kind,
                RowVersion = dto.RowVersion,
                Translations = dto.Translations.Select(t => new ProductTranslationVm
                {
                    Culture = t.Culture,
                    Name = t.Name,
                    Slug = t.Slug,
                    ShortDescription = t.ShortDescription,
                    FullDescriptionHtml = t.FullDescriptionHtml,
                    MetaTitle = t.MetaTitle,
                    MetaDescription = t.MetaDescription,
                    SearchKeywords = t.SearchKeywords
                }).ToList(),
                Variants = dto.Variants.Select(v => new ProductVariantCreateVm
                {
                    Sku = v.Sku,
                    Gtin = v.Gtin,
                    ManufacturerPartNumber = v.ManufacturerPartNumber,
                    BasePriceNetMinor = v.BasePriceNetMinor,
                    CompareAtPriceNetMinor = v.CompareAtPriceNetMinor,
                    Currency = v.Currency,
                    TaxCategoryId = v.TaxCategoryId,
                    StockOnHand = v.StockOnHand,
                    StockReserved = v.StockReserved,
                    ReorderPoint = v.ReorderPoint,
                    BackorderAllowed = v.BackorderAllowed,
                    MinOrderQty = v.MinOrderQty,
                    MaxOrderQty = v.MaxOrderQty,
                    StepOrderQty = v.StepOrderQty,
                    PackageWeight = v.PackageWeight,
                    PackageLength = v.PackageLength,
                    PackageWidth = v.PackageWidth,
                    PackageHeight = v.PackageHeight,
                    IsDigital = v.IsDigital
                }).ToList()
            };

            return View(vm);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = new ProductEditDto
            {
                Id = vm.Id,
                BrandId = vm.BrandId,
                PrimaryCategoryId = vm.PrimaryCategoryId,
                Kind = vm.Kind,
                RowVersion = vm.RowVersion ?? Array.Empty<byte>(),
                Translations = vm.Translations.Select(t => new ProductTranslationDto
                {
                    Culture = t.Culture,
                    Name = t.Name,
                    Slug = t.Slug,
                    ShortDescription = t.ShortDescription,
                    FullDescriptionHtml = t.FullDescriptionHtml,
                    MetaTitle = t.MetaTitle,
                    MetaDescription = t.MetaDescription,
                    SearchKeywords = t.SearchKeywords
                }).ToList(),
                Variants = vm.Variants.Select(v => new ProductVariantCreateDto
                {
                    Sku = v.Sku,
                    Gtin = v.Gtin,
                    ManufacturerPartNumber = v.ManufacturerPartNumber,
                    BasePriceNetMinor = v.BasePriceNetMinor,
                    CompareAtPriceNetMinor = v.CompareAtPriceNetMinor,
                    Currency = v.Currency,
                    TaxCategoryId = v.TaxCategoryId,
                    StockOnHand = v.StockOnHand,
                    StockReserved = v.StockReserved,
                    ReorderPoint = v.ReorderPoint,
                    BackorderAllowed = v.BackorderAllowed,
                    MinOrderQty = v.MinOrderQty,
                    MaxOrderQty = v.MaxOrderQty,
                    StepOrderQty = v.StepOrderQty,
                    PackageWeight = v.PackageWeight,
                    PackageLength = v.PackageLength,
                    PackageWidth = v.PackageWidth,
                    PackageHeight = v.PackageHeight,
                    IsDigital = v.IsDigital
                }).ToList()
            };

            try
            {
                await _updateProduct.HandleAsync(dto, ct);
                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Edit), new { id = vm.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError(string.Empty, "Concurrency conflict: the record was modified by another user. Please reload and try again.");
                return View(vm);
            }
            catch (FluentValidation.ValidationException ex)
            {
                foreach (var e in ex.Errors)
                    ModelState.AddModelError(e.PropertyName, e.ErrorMessage);
                return View(vm);
            }
        }
    }
}
