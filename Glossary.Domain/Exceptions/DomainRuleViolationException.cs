using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glossary.Domain.Exceptions
{
    public sealed class DomainRuleViolationException : Exception
    {
        public DomainRuleViolationException(string message) : base(message)
        {
        }
    }
}
