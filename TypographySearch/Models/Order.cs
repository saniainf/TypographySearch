using System;
using System.Collections.Generic;
using System.Text;

namespace TypographySearch.Models
{
    struct Order
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Client { get; set; }
        public string Manager { get; set; }
    }
}
