using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GeneralDTOs
{
    public class LabelClassDto
    {
        public int LabelClassId { get; set; }
        public required string Name { get; set; }
        public required string Color { get; set; }
    }
}
