﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NesaBot.Core
{
    internal class Program
    {
        static async Task Main(string[] args)
            => await new Bot().MainAsync();
    }
}
