using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using frznUpload.Web.Data;
using frznUpload.Web.Models;

namespace frznUpload.Web.Areas.Admin.Pages.Files
{
    public class DeleteModel : PageModel
    {
        private readonly frznUpload.Web.Data.Database _context;

        public DeleteModel(frznUpload.Web.Data.Database context)
        {
            _context = context;
        }

        [BindProperty]
        public File File { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            File = await _context.Files.FirstOrDefaultAsync(m => m.Id == id);

            if (File == null)
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

            File = await _context.Files.FindAsync(id);

            if (File != null)
            {
                _context.Files.Remove(File);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
