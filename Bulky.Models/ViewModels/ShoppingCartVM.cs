using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// NOTE: a view model is where the required info for a view is defined.
namespace Bulky.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
       
        public OrderHeader OrderHeader { get; set; }


    }
}
