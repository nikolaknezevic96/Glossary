using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glossary.Application.Dtos;

    public sealed record AuthRequest(string Username, string Password);
   

