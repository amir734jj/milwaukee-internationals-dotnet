using System.Collections.Generic;

namespace Models.ViewModels.Config
{
    public class YearContextViewModel
    {
        public IEnumerable<int> Years { get; set; }
        
        public int UpdatedYear { get; set; }
    }
}