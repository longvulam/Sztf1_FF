using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3EIM6_FF
{
    class Station
    {
        private string name;
        private bool isTransfer;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsTransfer
        {
            get { return isTransfer; }
            set { isTransfer = value; }
        }
    }
}
