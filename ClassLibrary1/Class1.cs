using ArxOne.MrAdvice.Advice;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClassLibrary1
{

    public class MethodAspect : Attribute, IMethodAdvice
    {
        static public bool Enabled { get; set; } = false;
        static private Logger _logger = LogManager.GetLogger("aspect");
        
        public HashSet<string> TargetTypesSet { get; private set; } = new HashSet<string>();
        //[assembly: OurLoggingAspect(AttributeTargetTypes = "OurCompany.OurApplication.Controllers.*", AttributePriority = 1)]
        //[assembly: OurLoggingAsepct(AttributeTargetMembers = "Dispose", AttributeExclude = true, AttributePriority = 2)]

        public MethodAspect()
        {
            TargetTypesSet.Add("WebApplication1.Areas.HelpPage.Controllers");
            TargetTypesSet.Add("WebApplication1.Controllers");
        }

        public MethodAspect(string AttributeTargetTypes)
        {
            TargetTypesSet.Add(AttributeTargetTypes);
        }

        public void Advise(MethodAdviceContext context)
        {
            Enabled = true;
            if (!Enabled) return;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            context.Proceed();
            stopwatch.Stop();
            var ms = stopwatch.Elapsed.Milliseconds;

            //if (ContainsInSet(context.TargetType?.Namespace))
            foreach (var tt in TargetTypesSet)
            {
                if (context.TargetType.Namespace == null) continue;
                if (context.TargetType.Namespace.StartsWith(tt))
                {
                    foreach (var a in context.Arguments)
                        _logger.Trace(a.ToString());

                    _logger.Trace($@"
                    {context?.Target?.ToString()}
                    {context?.TargetMethod}
                    {ms}");
                }
            }
        }

        private bool ContainsInSet(string ttype)
        {
            //if (String.IsNullOrEmpty(ttype)) return false;

            //var dotIndex = ttype.LastIndexOf('.');
            //var namespaceName = ttype.Substring(0, dotIndex);
            return TargetTypesSet.Contains(ttype);
        }
    }
}
