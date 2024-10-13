﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Chat
{
    public class CreateorUpdateChatDto
    {
        public int ChatId { get; set; }
        public int ChatBoxId { get; set; }
        public DateTime ChatTime { get; set; }
        public string Content { get; set; } = null!;
        public int ChatBy { get; set; }
    }
}