using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using frznUpload.Web.Data;
using frznUpload.Web.Models;

namespace frznUpload.Web.Areas.Admin.Pages.Shares
{
    public class DeleteModel : PageModel
    {
        private readonly frznUpload.Web.Data.Database _context;

        public DeleteModel(frznUpload.Web.Data.Database context)
        {
            _context = context;
        }

        [BindProperty]
        public Share Share { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Share = await _context.Shares.FirstOrDefaultAsync(m => m.Id == id);

            if (Share == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Share = await _context.Shares.FindAsync(id);

            if (Share != null)
            {
                _context.Shares.Remove(Share);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
