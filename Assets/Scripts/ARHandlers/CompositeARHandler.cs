namespace ARHandlers
{
    public class CompositeARHandler : IARHandler
    {
        public CompositeARHandler(params IARHandler[] handlers)
        {
            this.Handlers = handlers;
        }

        private IARHandler[] Handlers { get; set; }

        public void Activate()
        {
            foreach (var handler in Handlers)
            {
                handler.Activate();
            }
        }

        public void Deactivate()
        {
            foreach (var handler in Handlers)
            {
                handler.Deactivate();
            }
        }
    }
}