using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using frznUpload.Web.Data;
using frznUpload.Web.Models;

namespace frznUpload.Web.Areas.Admin.Pages.Tokens
{
    public class DetailsModel : PageModel
    {
        private readonly frznUpload.Web.Data.Database _context;

        public DetailsModel(frznUpload.Web.Data.Database context)
        {
            _context = context;
        }

        public Token Token { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Token = await _context.Tokens.FirstOrDefaultAsync(m => m.Id == id);

            if (Token == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
