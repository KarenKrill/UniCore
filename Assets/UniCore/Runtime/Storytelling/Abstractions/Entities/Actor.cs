using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KarenKrill.Storytelling.Abstractions
{
    public class Actor
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public uint Age { get; set; }
        public Sprite Icon { get; set; }
        public string Description { get; set; }
        public bool IsKnown { get; set; }
    }
}
