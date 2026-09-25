using GiftOfTheGivers.Data;
using GiftOfTheGivers.Helpers;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace GiftOfTheGivers.Controllers
{
    public class DonationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAzureFunctionClient _azureFunctionClient;

        public DonationController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IAzureFunctionClient azureFunctionClient)
        {
            _db = db;
            _userManager = userManager;
            _azureFunctionClient = azureFunctionClient;
        }

        // GET: /Donation/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Projects = await _db.ReliefProjects.Where(p => p.Status == "Active").ToListAsync();
            return View(new Donation());
        }

        // POST: /Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Donation donation, string? guestName, string? guestEmail)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Projects = await _db.ReliefProjects.Where(p => p.Status == "Active").ToListAsync();
                return View(donation);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                donation.ApplicationUserId = user.Id;
            }
            else
            {
                // Anonymous guest donor - keep values symbolic at prototype stage
                donation.GuestName = string.IsNullOrWhiteSpace(guestName) ? "Anonymous Donor" : guestName;
                donation.GuestEmail = guestEmail;
            }

            donation.DonatedOn = DateTime.UtcNow;
            donation.CertificateGenerated = true;

            _db.Donations.Add(donation);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Certificate), new { id = donation.DonationId });
        }

        // GET: /Donation/Certificate/5 - displays confirmation with a download link
        public async Task<IActionResult> Certificate(int id)
        {
            var donation = await _db.Donations
                .Include(d => d.ReliefProject)
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.DonationId == id);

            if (donation is null) return NotFound();

            return View(donation);
        }

        // GET: /Donation/CertificatePdf/5 - generates a placeholder tax certificate PDF
        public async Task<IActionResult> CertificatePdf(int id)
        {
            var donation = await _db.Donations
                .Include(d => d.ReliefProject)
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.DonationId == id);

            if (donation is null) return NotFound();

            var donorName = donation.ApplicationUser?.FullName ?? donation.GuestName ?? "Anonymous Donor";
            var certificateReference = DonationHelper.FormatCertificateReference(donation.DonationId, donation.DonatedOn);
            var functionPdf = await _azureFunctionClient.GenerateDonationCertificateAsync(donation, donorName, certificateReference, HttpContext.RequestAborted);
            if (functionPdf is not null)
            {
                return File(functionPdf, "application/pdf", $"{certificateReference}.pdf");
            }

            var total = DonationHelper.CalculateDonationTotal(new[] { donation.Amount });
            var formattedAmount = DonationHelper.FormatDonationAmount(total, donation.Currency.ToString());

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Gift of the Givers Foundation")
                        .SemiBold().FontSize(20).FontColor(Colors.Red.Darken2);

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().PaddingTop(10).Text("PLACEHOLDER TAX CERTIFICATE (Section 18A)")
                            .FontSize(16).SemiBold();
                        col.Item().Text("This is a prototype-stage document. It is not a valid legal tax certificate.")
                            .Italic().FontColor(Colors.Grey.Darken1);

                        col.Item().PaddingTop(15).Text($"Certificate Reference: {certificateReference}");
                        col.Item().Text($"Donor: {donorName}");
                        col.Item().Text($"Date: {donation.DonatedOn:dd MMMM yyyy}");
                        col.Item().Text($"Amount: {formattedAmount}");
                        col.Item().Text($"Frequency: {donation.Frequency}");
                        col.Item().Text($"Supporting Project: {(donation.ReliefProject != null ? donation.ReliefProject.Title : "General Fund")}");

                        col.Item().PaddingTop(20).Text(
                            "Thank you for your generous support of the Gift of the Givers Foundation's " +
                            "disaster relief efforts. Your contribution helps us respond faster to communities in crisis.");
                    });

                    page.Footer().AlignCenter().Text("Gift of the Givers Foundation - Prototype System (Part 1)")
                        .FontSize(9).FontColor(Colors.Grey.Medium);
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"GOTG-Certificate-{donation.DonationId}.pdf");
        }
    }
}
