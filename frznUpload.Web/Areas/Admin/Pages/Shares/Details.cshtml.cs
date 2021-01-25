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
    public class DetailsModel : PageModel
    {
        private readonly frznUpload.Web.Data.Database _context;

        public DetailsModel(frznUpload.Web.Data.Database context)
        {
            _context = context;
        }

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
    }
}
