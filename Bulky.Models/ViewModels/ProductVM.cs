using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    internal class ProductVM
    {
        public int Product { get; set; }
        // Dropdown
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }


    }
}
