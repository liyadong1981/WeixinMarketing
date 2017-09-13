using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Senparc.Core.Enums;
using Senparc.Core.Models;

namespace Senparc.Core.Models.VD
{
    public class MessageBoard_BaseVD : BaseVD, IBaseVD
    {

    }

    public class MessageBoard_IndexVD : MessageBoard_BaseVD
    {
        public PagedList<MessageBoard> MessageBoardList { get; set; }
    }

    public class MessageBoard_EditVD : MessageBoard_BaseVD
    {
        public bool IsEdit { get; set; }
        public MessageBoard MessageBoard { get; set; }
    }
}
