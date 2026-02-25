using MediatR;

namespace SwiftScale.BuildingBlocks
{
    public abstract class Entity
    {


        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        public IReadOnlyList<IDomainEvent> GetDomainEvents()
        {
            return _domainEvents.ToList();
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    }

}
