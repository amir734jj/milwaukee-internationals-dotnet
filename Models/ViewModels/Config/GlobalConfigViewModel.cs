using System.Collections.Generic;

namespace Models.ViewModels.Config
{
    public class GlobalConfigViewModel
    {
        public IEnumerable<int> Years { get; set; }
        
        public int UpdatedYear { get; set; }
    }
}