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
    public class IndexModel : PageModel
    {
        private readonly frznUpload.Web.Data.Database _context;

        public IndexModel(frznUpload.Web.Data.Database context)
        {
            _context = context;
        }

        public IList<Token> Token { get;set; }

        public async Task OnGetAsync()
        {
            Token = await _context.Tokens.ToListAsync();
        }
    }
}
