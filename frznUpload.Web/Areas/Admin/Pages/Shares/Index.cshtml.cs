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
    public class IndexModel : PageModel
    {
        private readonly frznUpload.Web.Data.Database _context;

        public IndexModel(frznUpload.Web.Data.Database context)
        {
            _context = context;
        }

        public IList<Share> Share { get;set; }

        public async Task OnGetAsync()
        {
            Share = await _context.Shares.ToListAsync();
        }
    }
}
