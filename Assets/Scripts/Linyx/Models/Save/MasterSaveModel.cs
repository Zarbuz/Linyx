using System.Collections.Generic;
using OPS.Serialization.Attributes;

namespace Linyx.Models.Save
{
    [SerializeAbleClass]
    public sealed class MasterSaveModel
    {
        [SerializeAbleField(0)]
        public List<SaveLineModel> AllLines;
    }
}
