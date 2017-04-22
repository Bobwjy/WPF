using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easi365UI.Models
{
    public class DiskEntryModelBase : EntryModelBase
    {
        public DiskEntryModelBase()
        {

        }

        public DiskEntryModelBase(IProfile profile)
            : base(profile)
        { }
    }
}
