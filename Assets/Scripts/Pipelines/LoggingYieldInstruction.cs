using UnityEngine;

namespace Pipelines
{
    public class LoggingYieldInstruction : CustomYieldInstruction
    {
        private IPipelineTaskEvents Events { get; }
        private CustomYieldInstruction Inner { get; }
            
        private int Step { get; set; }

        public LoggingYieldInstruction(IPipelineTaskEvents events, CustomYieldInstruction inner)
        {
            Inner = inner;
            Events = events;
            events?.TaskStarted();
        }
        public override bool keepWaiting {
            get
            {
                var w = Inner.keepWaiting;
                if (w)
                {
                    Events?.TaskProgress(++Step);
                }
                else
                {
                    Events?.TaskFinished();
                }
                return w;
            }
        }
    }
}