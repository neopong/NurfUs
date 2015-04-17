using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Models.API
{
    public class BlockDto
    {
        public BlockItemDto[] Items { get; set; }
        public bool RecMath { get; set; }
        public string Type { get; set; }
    }
}